using Microsoft.AspNetCore.Mvc;
using pizzashop_service.Interface;

namespace pizzashop.Controllers;

[Route("GetData")]
public class GetDataController : Controller
{
     private readonly IRegionService _regionService;

        public GetDataController(IRegionService regionService)
        {
            _regionService = regionService;
        }

        [HttpGet("GetCountries")]
        public IActionResult GetCountries()
        {
            var countries = _regionService.GetCountries()
                .Select(c => new
                {
                    countryId = c.Id,
                    countryName = c.Countryname
                })
                .ToList();

            return Ok(countries);
        }

        [HttpGet("GetStates")]
        public IActionResult GetStates(int countryId)
        {
            var states = _regionService.GetStates(countryId)
                .Select(s => new
                {
                    stateId = s.Id,
                    stateName = s.Statename
                })
                .ToList();

            return Ok(states);
        }

        [HttpGet("GetCities")]
        public IActionResult GetCities(int stateId)
        {
            var cities = _regionService.GetCities(stateId)
                .Select(c => new
                {
                    cityId = c.Id,
                    cityName = c.Cityname
                })
                .ToList();

            return Ok(cities);
        }
}

