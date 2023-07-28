namespace GadgetIsLanding.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int GameId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } //Usually 1

        //2 parents
        public Cart? Cart { get; set; }
        public Game? Game { get; set; }
    }
}