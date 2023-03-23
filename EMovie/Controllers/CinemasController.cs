using EMovie.Data;
using EMovie.Data.Services;
using EMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EMovie.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CinemasController : Controller
    {
        private readonly ICinemasService _service;

        public CinemasController(ICinemasService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Logo, Name, Description")]Cinema cinema)
        {
            if (!ModelState.IsValid) { return View(cinema); }
            await _service.AddAsync(cinema);
            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var cinemaDetails = await _service.GetByIdAsync(id);
            if (cinemaDetails == null) return View("NotFound");
            return View(cinemaDetails);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cinemaDetail = await _service.GetByIdAsync(id);
            if (cinemaDetail == null) return View("NotFound");
            return View(cinemaDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Logo, Name, Description")]Cinema cinema)
        {
            if (!ModelState.IsValid) { return View(cinema); }
            
            if (id == cinema.Id)
            {
                await _service.UpdateAsync(id, cinema);
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cinemaDetail = await _service.GetByIdAsync(id);
            if (cinemaDetail == null) return View("NotFound");
            return View(cinemaDetail);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));

        }

    }
}
