using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;
using Stripe.Model;
using SewNash.Data;
using SewNash.Models;
using Stripe.Tax;
namespace sewnash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private SewNashDbContext _dbContext;

        public StripeController(SewNashDbContext dbContext)
        {
           _dbContext = dbContext;
        }

    [HttpPost]
    public ActionResult Create([FromBody] PaymentIntentCreateRequest request)
    {
        
      // Create a Tax Calculation for the items being sold
       
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

      return new JsonResult(new 
      { 
        clientSecret = paymentIntent.ClientSecret,
    
        TotalAmount = AmountTotal / 100.0,
        paymentIntentId = paymentIntent.Id
       });
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

        
    }

    
}