using Grubly.Models;
using Microsoft.AspNetCore.Mvc;

namespace Grubly.Controllers
{
    public class RecipeController : Controller
    {
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
