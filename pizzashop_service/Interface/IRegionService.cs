using pizzashop_repository.Models;

namespace pizzashop_service.Interface;

public interface IRegionService
{
    List<Country> GetCountries();
    List<State> GetStates(int countryId);
    List<City> GetCities(int stateId);
}
