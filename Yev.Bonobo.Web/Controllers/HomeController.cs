using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Yev.Bonobo.Database;
using Yev.Bonobo.Database.Entities;
using Yev.Bonobo.Models;

namespace Yev.Bonobo.Controllers
{
    [Authorize(Policy = "Readers")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var dbCtx = this.HttpContext.RequestServices.GetRequiredService<GitDbContext>();
            var defRepo = await dbCtx.Repos.AsNoTracking()
                .Where(x => x.Name == "default")
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(this.HttpContext.RequestAborted)
                .ConfigureAwait(false);

            return this.View(defRepo);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
