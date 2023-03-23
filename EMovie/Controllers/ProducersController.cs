using EMovie.Data;
using EMovie.Data.Services;
using EMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EMovie.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProducersController : Controller
    {
        private readonly IProducersService _service;

        public ProducersController(IProducersService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var producerDetail = await _service.GetByIdAsync(id);

            if (producerDetail == null) { return View("NotFound"); }
            return View(producerDetail);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ProfilePictureURL, FullName, Bio")]Producer producer)
        {
            if (!ModelState.IsValid)
            {
                return View(producer);
            }
            await _service.AddAsync(producer);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var producerDetail = await _service.GetByIdAsync(id);
            if (producerDetail == null) return View(nameof(NotFound));

            return View(producerDetail);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditProducer(int id, [Bind()]Producer producer)
        {
            if (!ModelState.IsValid) return View(producer);
            if (id == producer.Id)
            {
                await _service.UpdateAsync(id, producer);
                return RedirectToAction(nameof(Index));
            }
            return View(producer);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var producerDetail = await _service.GetByIdAsync(id);
            if (producerDetail == null) return View(nameof(NotFound));
            return View(producerDetail);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfrim(int id)
        {
            var producerDetail = await _service.GetByIdAsync(id);
            if (producerDetail == null) return View(nameof(NotFound));
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
