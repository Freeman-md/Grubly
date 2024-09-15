using Grubly.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Grubly.Data
{
    public static class DbInitializer
    {
        public static void Initialize(GrublyContext context)
        {
            // Ensure the database is created.
            context.Database.EnsureCreated();

            // Look for any Categories.
            if (context.Categories.Any())
            {
                return;   // DB has been seeded
            }

            // Seed Categories
            var categories = new Category[]
            {
                new Category { Name = "Breakfast" },
                new Category { Name = "Lunch" },
                new Category { Name = "Dinner" },
                new Category { Name = "Dessert" },
                new Category { Name = "Beverage" }
            };
            foreach (Category c in categories)
            {
                context.Categories.Add(c);
            }
            context.SaveChanges();

            // Seed Ingredients
            var ingredients = new Ingredient[]
            {
                new Ingredient { Name = "Tomato", Description = "Fresh red tomatoes" },
                new Ingredient { Name = "Chicken Breast", Description = "Boneless chicken breast" },
                new Ingredient { Name = "Salt", Description = "Sea salt" },
                new Ingredient { Name = "Olive Oil", Description = "Extra virgin olive oil" },
                new Ingredient { Name = "Garlic", Description = "Fresh garlic cloves" }
            };
            foreach (Ingredient i in ingredients)
            {
                context.Ingredients.Add(i);
            }
            context.SaveChanges();

            // Seed Recipes
            var recipes = new Recipe[]
            {
                new Recipe
                {
                    Title = "Tomato Omelette",
                    Description = "A simple and delicious tomato omelette.",
                    Instructions = "1. Beat the eggs.\n2. Chop tomatoes and garlic.\n3. Heat olive oil in a pan.\n4. Sauté garlic and tomatoes.\n5. Pour eggs and cook until done.",
                    CuisineType = CuisineType.Italian,
                    DifficultyLevel = DifficultyLevel.Easy,
                    ImageUrl = "https://example.com/tomato_omelette.jpg",
                    Ingredients = new List<Ingredient> { ingredients[0], ingredients[4] }, // Tomato, Garlic
                    Categories = new List<Category> { categories[0] } // Breakfast
                },
                new Recipe
                {
                    Title = "Grilled Chicken Salad",
                    Description = "Healthy grilled chicken salad with fresh vegetables.",
                    Instructions = "1. Season the chicken with salt and olive oil.\n2. Grill the chicken until cooked.\n3. Chop tomatoes and other vegetables.\n4. Mix all ingredients in a bowl.\n5. Serve fresh.",
                    CuisineType = CuisineType.Mexican,
                    DifficultyLevel = DifficultyLevel.Medium,
                    ImageUrl = "https://example.com/grilled_chicken_salad.jpg",
                    Ingredients = new List<Ingredient> { ingredients[0], ingredients[1], ingredients[2], ingredients[3] }, // Tomato, Chicken Breast, Salt, Olive Oil
                    Categories = new List<Category> { categories[1], categories[2] } // Lunch, Dinner
                },
                new Recipe
                {
                    Title = "Garlic Butter Chicken",
                    Description = "Succulent chicken cooked in garlic butter sauce.",
                    Instructions = "1. Season the chicken with salt.\n2. Heat olive oil and butter in a pan.\n3. Add garlic and sauté until fragrant.\n4. Cook the chicken until golden brown.\n5. Serve hot with your choice of sides.",
                    CuisineType = CuisineType.Chinese,
                    DifficultyLevel = DifficultyLevel.Hard,
                    ImageUrl = "https://example.com/garlic_butter_chicken.jpg",
                    Ingredients = new List<Ingredient> { ingredients[1], ingredients[2], ingredients[3], ingredients[4] }, // Chicken Breast, Salt, Olive Oil, Garlic
                    Categories = new List<Category> { categories[2] } // Dinner
                },
                new Recipe
                {
                    Title = "Tomato Basil Soup",
                    Description = "Creamy tomato basil soup perfect for any season.",
                    Instructions = "1. Sauté garlic in olive oil.\n2. Add chopped tomatoes and cook until soft.\n3. Add vegetable broth and simmer.\n4. Blend the mixture until smooth.\n5. Add fresh basil and season with salt.",
                    CuisineType = CuisineType.Italian,
                    DifficultyLevel = DifficultyLevel.Medium,
                    ImageUrl = "https://example.com/tomato_basil_soup.jpg",
                    Ingredients = new List<Ingredient> { ingredients[0], ingredients[2], ingredients[3], ingredients[4] }, // Tomato, Salt, Olive Oil, Garlic
                    Categories = new List<Category> { categories[2] } // Dinner
                },
                new Recipe
                {
                    Title = "Spicy Tomato Salsa",
                    Description = "Fresh and spicy tomato salsa to complement your meals.",
                    Instructions = "1. Chop tomatoes, garlic, and onions.\n2. Mix all ingredients in a bowl.\n3. Add olive oil and salt to taste.\n4. Let it sit for flavors to meld.\n5. Serve with chips or as a topping.",
                    CuisineType = CuisineType.Mexican,
                    DifficultyLevel = DifficultyLevel.Easy,
                    ImageUrl = "https://example.com/spicy_tomato_salsa.jpg",
                    Ingredients = new List<Ingredient> { ingredients[0], ingredients[2], ingredients[3], ingredients[4] }, // Tomato, Salt, Olive Oil, Garlic
                    Categories = new List<Category> { categories[3], categories[4] } // Dessert (optional), Beverage (optional)
                }
            };
            foreach (Recipe r in recipes)
            {
                context.Recipes.Add(r);
            }
            context.SaveChanges();
        }
    }
}
