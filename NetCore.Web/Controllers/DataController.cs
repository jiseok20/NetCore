using Microsoft.AspNetCore.Mvc;

namespace NetCore.Web.Controllers
{
    public class DataController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
