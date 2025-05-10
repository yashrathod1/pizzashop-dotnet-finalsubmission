using pizzashop_repository.Database;
using pizzashop_repository.Interface;
using pizzashop_repository.Models;

namespace pizzashop_repository.Implementation;

public class RegionRepository : IRegionRepository
{
     private readonly PizzaShopDbContext _context;

        public RegionRepository(PizzaShopDbContext context)
        {
            _context = context;
        }

        public List<Country> GetCountries()
        {
            return _context.Countries.ToList();
        }

        public List<State> GetStates(int countryId)
        {
            return _context.States.Where(s => s.Countryid == countryId).ToList();
        }

        public List<City> GetCities(int stateId)
        {
            return _context.Cities.Where(c => c.Stateid == stateId).ToList();
        }
        
}

