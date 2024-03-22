using Microsoft.AspNetCore.Mvc;

namespace AddressBook.Controllers
{
    public class ErrorController : Controller
    {
        public ErrorController()
        {
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View("Error");
        }
    }
}