using Metarankings.Models;
using Metarankings.Repositories.Interfaces.Derived;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Metarankings.Controllers
{
    public class GamesController : Controller
    {
        public readonly IGamesRepository _gamesRepository;
        public GamesController(IGamesRepository gamesResository)
        {
            _gamesRepository = gamesResository;
        }

        public async Task<IActionResult> Index() 
        {
            return View(await _gamesRepository.GetAllAsync());
        }
    }
}