namespace Grubly.Models
{
    public class Rating
    {
        public int ID { get; set; }
        public int RecipeID { get; set; }
        public int Score { get; set; }
        public string? Review { get; set; }

        public required Recipe Recipe { get; set; }
    }
}