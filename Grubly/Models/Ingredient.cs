using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Grubly.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Ingredient
    {
        public int ID { get; set; }
        
        [StringLength(50)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}