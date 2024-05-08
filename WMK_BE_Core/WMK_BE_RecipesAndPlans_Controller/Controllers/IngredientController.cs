using Microsoft.AspNetCore.Mvc;

namespace WMK_BE_RecipesAndPlans_Controller.Controllers
{
    public class IngredientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
