using EMovie.Data;
using EMovie.Data.Base;
using EMovie.Data.Services;
using EMovie.Data.Static;
using EMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace EMovie.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ActorsController : Controller
    {
        private readonly IActorsService _service;
       

        public ActorsController(IActorsService service)
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
        public async Task<IActionResult> Create([Bind("ProfilePictureURL, FullName, Bio")]Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            await _service.AddAsync(actor);
            return RedirectToAction(nameof(Index));
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var actorDetail = await _service.GetByIdAsync(id);

            if (actorDetail == null) { return View("NotFound"); }
            return View(actorDetail);   
        }

        public async Task<IActionResult> Edit(int id)
        {
            var actorDetail = await _service.GetByIdAsync(id);

            if (actorDetail == null) { return View("NotFound"); }
            return View(actorDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, ProfilePictureURL, FullName, Bio")] Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            await _service.UpdateAsync(id, actor);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var actorDetail = await _service.GetByIdAsync(id);

            if (actorDetail == null) { return View("NotFound"); }
            return View(actorDetail);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var actorDetail = await _service.GetByIdAsync(id);

            if (actorDetail == null) { return View("NotFound"); }
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
