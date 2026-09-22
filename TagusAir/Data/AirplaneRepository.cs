using TagusAir.Data.Entities;

namespace TagusAir.Data
{
    public class AirplaneRepository : GenericRepository<Airplane>, IAirplaneRepository
    {

        public AirplaneRepository(DataContext context) : base (context)
        {
            
        }
    }
}
