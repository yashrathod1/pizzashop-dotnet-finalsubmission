using pizzashop_repository.Interface;
using pizzashop_repository.Models;
using pizzashop_service.Interface;

namespace pizzashop_service.Implementation;

public class RegionService : IRegionService
{
    private readonly IRegionRepository _regionRepository;

        public RegionService(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public List<Country> GetCountries()
        {
            return _regionRepository.GetCountries();
        }

        public List<State> GetStates(int countryId)
        {
            return _regionRepository.GetStates(countryId);
        }

        public List<City> GetCities(int stateId)
        {
            return _regionRepository.GetCities(stateId);
        }
}

