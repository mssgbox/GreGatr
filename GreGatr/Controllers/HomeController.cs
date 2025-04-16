using System.Diagnostics;
using System.Net.NetworkInformation;
using GreGatr.Models;
using GreGatr.Domain;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing;
using GreGatr.Domain.Services;

namespace GreGatr.Controllers
{
    

    public class UserController : Controller
    {

        public IActionResult GetUsers()
        {
            return Content("List of all users.");
        }

        public IActionResult GetUserById(string userId)
        {
            return Content("User with id: " + userId);
        }
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();


        }
        public async Task <IActionResult> Click()
        {
            

            await ModifyNetflixFeed();
            

            return Redirect("/"+Settings.OutputFileName);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task ModifyNetflixFeed()
        {
                var aggregator = new Aggregator();
                await aggregator.AggregateMoviesAsync();
       }
    }
}
