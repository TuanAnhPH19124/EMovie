using EMovie.Data.Base;
using EMovie.Models;

namespace EMovie.Data.Services
{
    public class ProducersService : EntityBaseRepository<Producer>, IProducersService
    {
        public ProducersService(AppDbContext context) : base(context) { }
        
    }
}
