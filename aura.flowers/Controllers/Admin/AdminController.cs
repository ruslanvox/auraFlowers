using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace aura.flowers.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProducts()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPortfolio()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePortfolio()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTestimonials()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTestimonials()
        {
            return View();
        }
    }
}