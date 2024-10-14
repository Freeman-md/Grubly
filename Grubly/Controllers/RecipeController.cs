using System.ComponentModel.DataAnnotations;
using Grubly.Interfaces.Services;
using Grubly.Models;
using Grubly.Services;
using Grubly.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

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

        public async Task<IActionResult> Show(int id)
        {
            Recipe? recipe = await _recipeService.GetRecipeWithAllDetails(id);

            if (recipe == null)
            {
                return View("NotFound");
            }

            return View(recipe);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IReadOnlyCollection<Ingredient> ingredients = await _ingredientService.GetAllIngredients();
            IReadOnlyCollection<Category> categories = await _categoryService.GetAllCategories();

            var viewModel = new RecipeFormViewModel
            {
                AvailableIngredients = ingredients.ToList(),
                AvailableCategories = categories.ToList(),
                SelectedIngredients = new bool[ingredients.Count],
                SelectedCategories = new bool[categories.Count]
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeFormViewModel viewModel)
        {
            await PopulateAvailableIngredientsAndCategories(viewModel);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            try
            {
                var recipe = new Recipe
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Instructions = viewModel.Instructions,
                    CuisineType = viewModel.CuisineType,
                    DifficultyLevel = viewModel.DifficultyLevel,
                    ImageUrl = viewModel.ImageUrl,
                    Ingredients = new List<Ingredient>(),
                    Categories = new List<Category>()
                };

                for (int i = 0; i < viewModel.SelectedIngredients.Length; i++)
                {
                    if (viewModel.SelectedIngredients[i])
                    {
                        recipe.Ingredients.Add(viewModel.AvailableIngredients[i]);
                    }
                }

                for (int i = 0; i < viewModel.SelectedCategories.Length; i++)
                {
                    if (viewModel.SelectedCategories[i])
                    {
                        recipe.Categories.Add(viewModel.AvailableCategories[i]);
                    }
                }

                await _recipeService.CreateRecipe(recipe);
                return RedirectToAction("Index");
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("", "One or more selected ingredients or categories are invalid.");
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Recipes_Title") == true)
            {
                ModelState.AddModelError("Title", "A recipe with this title already exists. Please choose a different title.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "A database error occurred. Please ensure your data is valid or try again later.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the recipe. Please try again.");

            }

            await PopulateAvailableIngredientsAndCategories(viewModel);
            return View(viewModel);
        }

        public Task<IActionResult> Edit(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<IActionResult> Update(Recipe recipe, int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _recipeService.DeleteRecipe(id);

                return RedirectToAction("Index");

            }
            catch (KeyNotFoundException ex)
            {
                return View("NotFound", ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        private async Task PopulateAvailableIngredientsAndCategories(RecipeFormViewModel viewModel)
        {
            viewModel.AvailableIngredients = (await _ingredientService.GetAllIngredients()).ToList();
            viewModel.AvailableCategories = (await _categoryService.GetAllCategories()).ToList();
        }

    }
}
