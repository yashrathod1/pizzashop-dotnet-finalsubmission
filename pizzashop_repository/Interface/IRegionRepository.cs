using pizzashop_repository.Models;

namespace pizzashop_repository.Interface;

public interface IRegionRepository
{
    List<Country> GetCountries();
    List<State> GetStates(int countryId);
    List<City> GetCities(int stateId);

}
