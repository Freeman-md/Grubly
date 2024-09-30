using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Ingredient
    {
        public int ID { get; set; }
        
        public required string Name { get; set; }

        public string? Description { get; set; }

        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}