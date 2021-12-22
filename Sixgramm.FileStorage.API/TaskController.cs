using Microsoft.AspNetCore.Mvc;

namespace Sixgramm.FileStorage.API
{
    public class TaskController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}