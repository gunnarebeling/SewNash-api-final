using Newtonsoft.Json;


namespace Stripe.Model;

public class Item
    {
      [JsonProperty("id")]
      public string Id { get; set; }
      [JsonProperty("Amount")]
      public long Amount { get; set; }
      public int Quantity { get; set; }
    }

    public class PaymentIntentCreateRequest
    {
      [JsonProperty("items")]
      public Item[] Items { get; set; }
    }

   
      public class UpdatePaymentIntentRequest
    {
        public string PaymentIntentId { get; set; }
        public Item[] Items { get; set; }
        
    }
    