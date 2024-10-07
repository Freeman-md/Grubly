using Grubly.Interfaces.Services;
using Grubly.Models;
using Grubly.Services;
using Microsoft.AspNetCore.Mvc;

namespace Grubly.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly IIngredientService _ingredientService;
        private readonly ICategoryService _categoryService;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Show(int id)
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Recipe recipe)
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit(Recipe recipe, int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(int id) {
            return RedirectToAction("Index");
        }

    }
}
