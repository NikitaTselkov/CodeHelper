using CodeHelper.Core;
using CodeHelper.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace CodeHelper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SitemapGenerator _sitemapGenerator;

        public HomeController(ILogger<HomeController> logger, SitemapGenerator sitemapGenerator)
        {
            _logger = logger;
            _sitemapGenerator = sitemapGenerator;
        }

        public IActionResult Index()
        {
            TempData.Clear();

            return RedirectToAction("All", "Questions");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Cookie()
        {
            return View();
        }

        public IActionResult Sitemap()
        {
            var sitemapNodes = _sitemapGenerator.GetSitemapNodes();
            string xml = _sitemapGenerator.GetSitemapDocument(sitemapNodes);
            return Content(xml, "text/xml", Encoding.UTF8);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}