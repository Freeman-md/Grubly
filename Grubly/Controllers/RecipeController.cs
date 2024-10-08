using Grubly.Interfaces.Services;
using Grubly.Models;
using Grubly.Services;
using Grubly.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Grubly.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IIngredientService _ingredientService;
        private readonly ICategoryService _categoryService;

        public RecipeController(IRecipeService recipeService, IIngredientService ingredientService, ICategoryService categoryService)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            IReadOnlyCollection<Recipe> recipes = await _recipeService.GetAllRecipes();
            IReadOnlyCollection<Category> categories = await _categoryService.GetAllCategories();
            IReadOnlyCollection<Ingredient> ingredients = await _ingredientService.GetAllIngredients();

            IReadOnlyCollection<CuisineType> cuisineTypes = Enum.GetValues(typeof(CuisineType)).Cast<CuisineType>().ToList();
            IReadOnlyCollection<DifficultyLevel> difficultyLevels = Enum.GetValues(typeof(DifficultyLevel)).Cast<DifficultyLevel>().ToList();

            RecipeIndexViewModel recipeIndexViewModel = new RecipeIndexViewModel(
                recipes, 
                categories,
                ingredients,
                cuisineTypes, 
                difficultyLevels
            );

            return View(recipeIndexViewModel);
        }

        public Task<IActionResult> Show(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> Create()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<IActionResult> Create(Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> Edit(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<IActionResult> Edit(Recipe recipe, int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<IActionResult> Delete(int id) {
            throw new NotImplementedException();
        }

    }
}
