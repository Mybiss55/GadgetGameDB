namespace GadgetIsLanding.Models

{

    public enum PaymentMethod
    {
        VISA,
        MasterCard,
        InteracDebit,
        PayPal,
        Stripe
    }
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CartId { get; set; }
        public decimal Total { get; set; }
        public string ShippingAddress { get; set; }
        public bool PaymentReceived { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public User? User { get; set; }
        public Cart? Cart { get; set; }
    }
}
