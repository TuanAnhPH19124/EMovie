using EMovie.Data;
using EMovie.Data.Services;
using EMovie.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EMovie.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MoviesController : Controller
    {
        private readonly IMoviesService _service;

        public MoviesController(IMoviesService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync(m => m.Cinema);
            return View(data);
        }

        public async Task<IActionResult> Filter(string searchString)
        {
            var data = await _service.GetAllAsync(m => m.Cinema);

            if (!string.IsNullOrEmpty(searchString))
            {
                var searchMovies = data.Where(p => p.Name.ToLower().Contains(searchString.ToLower()) || p.Description.ToLower().Contains(searchString.ToLower())).ToList();
                return View(nameof(Index), searchMovies);
            }

            return View(nameof(Index),data);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var movieDetail = await _service.GetMovieByIdAsync(id);
            if (movieDetail == null) { return View(nameof(NotFound)); }
            return View(movieDetail);
        }

        public async Task<IActionResult> Create()
        {
            var movieDropdownData = await _service.GetNewMovieDropdownsValuesAsync();

            ViewBag.Cinemas = new SelectList(movieDropdownData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownData.Actors, "Id", "FullName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewMovieVM movie)
        {
            if (!ModelState.IsValid)
            {
                var movieDropdownData = await _service.GetNewMovieDropdownsValuesAsync();

                ViewBag.Cinemas = new SelectList(movieDropdownData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownData.Actors, "Id", "FullName");
                return View(movie);
            }

            await _service.AddNewMovieAsync(movie);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movieDetail = await _service.GetMovieByIdAsync(id);
            if (movieDetail == null) { return View("NotFound"); }

            var response = new NewMovieVM()
            {
                Id = movieDetail.Id,
                Name = movieDetail.Name,
                Description = movieDetail.Description,
                Price = movieDetail.Price,
                ImageURL = movieDetail.ImageURL,
                MovieCategory = movieDetail.MovieCategory,
                StartDate = movieDetail.StartDate,
                EndDate = movieDetail.EndDate,
                CinemaId = movieDetail.CinemaId,
                ProducerId = movieDetail.ProducerId,
                ActorIds = movieDetail.Actors_Movies.Select(a => a.ActorId).ToList()
            };

            var movieDropdownData = await _service.GetNewMovieDropdownsValuesAsync();

            ViewBag.Cinemas = new SelectList(movieDropdownData.Cinemas, "Id", "Name");
            ViewBag.Producers = new SelectList(movieDropdownData.Producers, "Id", "FullName");
            ViewBag.Actors = new SelectList(movieDropdownData.Actors, "Id", "FullName");

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewMovieVM movie)
        {
            if (id != movie.Id)
            {
                return View("NotFound");
            }

            if (!ModelState.IsValid)
            {
                var movieDropdownData = await _service.GetNewMovieDropdownsValuesAsync();

                ViewBag.Cinemas = new SelectList(movieDropdownData.Cinemas, "Id", "Name");
                ViewBag.Producers = new SelectList(movieDropdownData.Producers, "Id", "FullName");
                ViewBag.Actors = new SelectList(movieDropdownData.Actors, "Id", "FullName");
                return View(movie);
            }

            await _service.UpdateMovieAsync(movie);
            return RedirectToAction(nameof(Index));
        }
    }
}
