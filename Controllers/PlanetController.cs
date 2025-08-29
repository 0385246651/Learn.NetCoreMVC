using App.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    public class PlanetController : Controller
    {
        private readonly PlanetService _planetService;
        private readonly ILogger<PlanetController> _logger;

        public PlanetController(PlanetService planetService, ILogger<PlanetController> logger)
        {
            _planetService = planetService;
            _logger = logger;
        }

        // GET: Planet
        // [Route("Planet")]
        public ActionResult Index()
        {
            return View();
        }

        // route action
        [BindProperty(SupportsGet = true, Name = "action")]
        public string Name { get; set; } //Action ~ PlanetModel
        public ActionResult Mercury()
        {
            var planet = _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail", planet);
        }

        public ActionResult Venus()
        {
            var planet = _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail", planet);
        }

        public ActionResult Earth()
        {
            var planet = _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail", planet);
        }

        public ActionResult Mars()
        {
            var planet = _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail", planet);
        }

        // [Route("Planet/[action]", Order =1) ] // /Planet/Jupiter dùng token
        // Order là độ ưu tiên của route, số nhỏ ưu tiên cao
        // [Controller]/[Action]/[Area] /Planet/Jupiter
        public ActionResult Jupiter()
        {
            var planet = _planetService.Where(p => p.Name == Name).FirstOrDefault();
            return View("Detail", planet);
        }

        // /Planet/PlanetInfo/1
        // [Route("Planet/PlanetInfo/{id:int}")]
        public IActionResult PlanetInfo(int id)
        {
            var planet = _planetService.Where(p => p.Id == id).FirstOrDefault();
            return View("Detail", planet);
        }
    }
}
