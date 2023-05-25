using Metarankings.Data;
using Metarankings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Metarankings.Controllers
{
    public class CriticsController : Controller
    {
        private readonly MetarankingsContext _metarankingsContext;

        public CriticsController(MetarankingsContext metarankingsContext)
        {
            _metarankingsContext = metarankingsContext;
        }

        public async Task<IActionResult> GetAll()
        {
            return View(await _metarankingsContext.Critics.ToListAsync());
        }

        public async Task<IActionResult> Get(long id)
        {
            Critic critic = await _metarankingsContext.Critics.FirstOrDefaultAsync(c => c.Id == id);
            if (critic == null)
            {
                return NotFound();
            }
            else
            {
                return View(critic);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Critic critic)
        {
            if (ModelState.IsValid)
            {
                _metarankingsContext.Critics.Add(critic);
                await _metarankingsContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(critic);
        }

        public async Task<IActionResult> Update(long id)
        {
            Critic critic = await _metarankingsContext.Critics.FirstOrDefaultAsync(c => c.Id == id);
            if (critic == null)
            {
                return NotFound();
            }
            return View(critic);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Critic critic)
        {
            if (ModelState.IsValid)
            {
                _metarankingsContext.Critics.Update(critic);
                await _metarankingsContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(critic);
        }

        public async Task<IActionResult> Delete(long id)
        {
            Critic critic = await _metarankingsContext.Critics.FirstOrDefaultAsync(c => c.Id == id);
            if (critic == null)
            {
                return NotFound();
            }
            return View(critic);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Critic critic)
        {
            _metarankingsContext.Critics.Remove(critic);
            await _metarankingsContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
