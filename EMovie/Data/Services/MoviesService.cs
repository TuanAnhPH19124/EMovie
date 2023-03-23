using EMovie.Data.Base;
using EMovie.Data.ViewData;
using EMovie.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMovie.Data.Services
{
    public class MoviesService : EntityBaseRepository<Movie>, IMoviesService
    {
        private readonly AppDbContext _context;
        public MoviesService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddNewMovieAsync(NewMovieVM data)
        {
            var movie = new Movie()
            {
                Name = data.Name,
                Description = data.Description,
                Price = data.Price,
                ImageURL = data.ImageURL,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                MovieCategory = data.MovieCategory,
                CinemaId = data.CinemaId,
                ProducerId = data.ProducerId
            };
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();

            foreach (var actorId in data.ActorIds)
            {
                var actor_Movie = new Actor_Movie()
                {
                    MovieId = movie.Id,
                    ActorId = actorId
                };
                await _context.Actors_Movies.AddAsync(actor_Movie);
            }
            await _context.SaveChangesAsync();

        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            var movieDetail = await _context.Movies
                .Include(c => c.Cinema)
                .Include(p => p.Producer)
                .Include(ma => ma.Actors_Movies).ThenInclude(a => a.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
           

            return movieDetail;
        }

        public async Task<NewMovieDropdownsVM> GetNewMovieDropdownsValuesAsync()
        {
            var movieDropdownData = new NewMovieDropdownsVM()
            {
                Producers = await _context.Producers.OrderBy(p => p.FullName).ToListAsync(),
                Cinemas = await _context.Cinemas.OrderBy(c => c.Name).ToListAsync(),
                Actors = await _context.Actors.OrderBy(a => a.FullName).ToListAsync()
            };

            return movieDropdownData;
        }

        public async Task UpdateMovieAsync(NewMovieVM data)
        {
            var dbMovie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == data.Id);
            if (dbMovie != null)
            {
                dbMovie.Name = data.Name;
                dbMovie.Description = data.Description;
                dbMovie.Price = data.Price;
                dbMovie.ImageURL = data.ImageURL;
                dbMovie.StartDate = data.StartDate;
                dbMovie.EndDate = data.EndDate;
                dbMovie.MovieCategory = data.MovieCategory;
                dbMovie.CinemaId = data.CinemaId;
                dbMovie.ProducerId = data.ProducerId;
                await _context.SaveChangesAsync();
            }

            // remove existing actors
            var existingActorsDb = await _context.Actors_Movies.Where(p => p.MovieId == data.Id).ToListAsync();
            _context.Actors_Movies.RemoveRange(existingActorsDb);
            await _context.SaveChangesAsync();

            // add actors movie

            foreach (var actorid in data.ActorIds)
            {
                var actor_Movie = new Actor_Movie()
                {
                    MovieId = data.Id,
                    ActorId = actorid
                };
                await _context.Actors_Movies.AddAsync(actor_Movie);
            }
            await _context.SaveChangesAsync();

        }
    }
}
