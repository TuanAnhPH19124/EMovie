using EMovie.Data.Base;
using EMovie.Data.ViewData;
using EMovie.Models;
using System.Threading.Tasks;

namespace EMovie.Data.Services
{
    public interface IMoviesService : IEntityBaseRepository<Movie>
    {
        Task<Movie> GetMovieByIdAsync(int id);
        Task<NewMovieDropdownsVM> GetNewMovieDropdownsValuesAsync();
        Task AddNewMovieAsync(NewMovieVM data);
        Task UpdateMovieAsync(NewMovieVM data);

    }
}
