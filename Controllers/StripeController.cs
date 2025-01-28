using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;
using Stripe.Model;
using SewNash.Data;
using SewNash.Models;
using Stripe.Tax;
using StackExchange.Redis;
namespace sewnash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private SewNashDbContext _dbContext;
        private readonly ILogger<StripeController> _logger;
        private readonly IConnectionMultiplexer _redis;

        public StripeController(SewNashDbContext dbContext, ILogger<StripeController> logger, IConnectionMultiplexer redis)
        {
            _dbContext = dbContext;
            _logger = logger;
            _redis = redis;
        }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] PaymentIntentCreateRequest request)
    {
        
      // Create a Tax Calculation for the items being sold
       var db = _redis.GetDatabase();
        var existingCacheKey = await db.StringGetAsync($"SessionId:{request.SessionId}");
        if (existingCacheKey.HasValue)
        {
           int totalOccupancy = await CalculateTotalOccupancy(db, request.SessionId.ToString());
           if (totalOccupancy + request.Items.Sum(i => i.Quantity) > request.MaxPeople)
           {
               return BadRequest(new { message = "The session is fully booked" });
           }else{
               await db.StringSetAsync($"SessionId:{request.SessionId}", "true");
           }

        }
       
        // Create a Tax Calculation for the items being sold
        var taxCalculation = CalculateTax(request.Items, "usd");

        var AmountTotal = taxCalculation.AmountTotal;

      var paymentIntentService = new PaymentIntentService();
      var paymentIntent = paymentIntentService.Create(new PaymentIntentCreateOptions
      {
        Amount = AmountTotal,
        Currency = "usd",
        // In the latest version of the API, specifying the `automatic_payment_methods` parameter is optional because Stripe enables its functionality by default.
        AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
        {
          Enabled = true,
        },
        Metadata = new Dictionary<string, string>
        {
          { "tax_calculation", taxCalculation.Id },
        },
      });

        _logger.LogInformation($"PaymentIntent created: {paymentIntent}");
        var cacheKey = $"paymentIntent:{paymentIntent.Id}";
      return new JsonResult(new 
      { 
        clientSecret = paymentIntent.ClientSecret,
    
        TotalAmount = AmountTotal / 100.0,
        paymentIntentId = paymentIntent.Id
       });
    }
    private async Task<int> CalculateTotalOccupancy(IDatabase db, string sessionId)

        {
            var server = _redis.GetServer(_redis.GetEndPoints()[0]);
            var keys = server.Keys(pattern: $"SessionId:{sessionId}");
            var bookings = _dbContext.Bookings.Where(b => b.SessionId == int.Parse(sessionId));
            var totalBookedOccupancy = bookings.Sum(b => b.Occupancy);


            int totalOccupancy = 0;
            foreach (var key in keys)
            {
                var occupancyValue = await db.HashGetAsync(key, "occupancy");
                if (occupancyValue.HasValue && int.TryParse(occupancyValue, out int occupancy))
                {
                    totalOccupancy += occupancy;
                }
            }
            return totalOccupancy + totalBookedOccupancy;
        }

    // Securely calculate the order amount, including tax
    [NonAction]
    public long CalculateOrderAmount(Calculation taxCalculation)
    {
        // Calculate the order total with any exclusive taxes on the server to prevent
        // people from directly manipulating the amount on the client
        return taxCalculation.AmountTotal;
    }

    [NonAction]
    public Calculation CalculateTax(Item[] items, string currency)
    {
        var lineItems = items.Select(item => BuildLineItem(item)).ToList();
        foreach (var lineItem in lineItems)
{
    Console.WriteLine($"Line Item: Amount = {lineItem.Amount}, Reference = {lineItem.Reference}");
}
        var calculationCreateOptions = new CalculationCreateOptions
        {
            Currency = currency,
            CustomerDetails = new CalculationCustomerDetailsOptions
            {
                Address = new AddressOptions
                {
                    Line1 = "635 W Iris Drive",
                    City = "Nashville",
                    State = "TN",
                    PostalCode = "37204",
                    Country = "US",
                },
                AddressSource = "shipping",
            },
            LineItems = lineItems,
        };
        Console.WriteLine($"Currency: {calculationCreateOptions.Currency}");
Console.WriteLine($"Customer Address: {calculationCreateOptions.CustomerDetails.Address.Line1}, {calculationCreateOptions.CustomerDetails.Address.City}");
Console.WriteLine($"Line Items Count: {calculationCreateOptions.LineItems.Count}");

        var calculationService = new CalculationService();
        var calculation = calculationService.Create(calculationCreateOptions);

        return calculation;
    }

    [NonAction]
    public CalculationLineItemOptions BuildLineItem(Item item)
    {
        return new CalculationLineItemOptions
        {   
            Amount = item.Amount * item.Quantity, // Amount in cents
            Reference = item.Id, // Unique reference for the item in the scope of the calculation
        };
    }


    // Invoke this method in your webhook handler when `payment_intent.succeeded` webhook is received
    [NonAction]
    public void HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
    {
        // Create a Tax Transaction for the successful payment
        var transactionCreateOptions = new TransactionCreateFromCalculationOptions
        {
            Calculation = paymentIntent.Metadata["tax_calculation"],
            Reference = "myOrder_123", // Replace with a unique reference from your checkout/order system
        };
        var transactionService = new TransactionService();
        transactionService.CreateFromCalculation(transactionCreateOptions);
    }
    [HttpPost("update-payment-intent")]
        public ActionResult UpdatePaymentIntent([FromBody] UpdatePaymentIntentRequest request)
        {
            // Calculate the new total amount
            var taxCalculation = CalculateTax(request.Items, "usd");
            var amountTotal = taxCalculation.AmountTotal;

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = paymentIntentService.Update(request.PaymentIntentId, new PaymentIntentUpdateOptions
            {
                Amount = amountTotal,
                Metadata = new Dictionary<string, string>
                {
                    { "tax_calculation", taxCalculation.Id },
                },
            });

            return new JsonResult(new
            {
                clientSecret = paymentIntent.ClientSecret,
                totalAmount = amountTotal / 100.0,
                paymentIntentId = paymentIntent.Id
            });
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentConfirmation request)
        {
            

            var service = new PaymentIntentService();
            try
            {
                var paymentIntent = await service.ConfirmAsync(request.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    // Handle booking logic here
                    

                    // Save bookingForm to your database

                    return Ok(new { message = "Booking complete!" });
                }
                else
                {
                    return BadRequest(new { message = "Payment not successful" });
                }
            }
            catch (StripeException e)
            {
                _logger.LogError(e.Message, "Error confirming payment");
                return StatusCode(500, new { message = e.Message });
            }
        }

        
    }

    
}