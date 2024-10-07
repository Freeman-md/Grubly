using Grubly.Interfaces.Services;
using Grubly.Models;
using Grubly.Services;
using Microsoft.AspNetCore.Mvc;

namespace Grubly.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeService? _recipeService;
        private readonly IIngredientService? _ingredientService;
        private readonly ICategoryService? _categoryService;

        public RecipeController(IRecipeService recipeService) {
            _recipeService = recipeService;
        }

        public RecipeController(IRecipeService recipeService, IIngredientService ingredientService, ICategoryService categoryService)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;
            _categoryService = categoryService;
        }

        public Task<IActionResult> Index()
        {
            throw new NotImplementedException();
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
