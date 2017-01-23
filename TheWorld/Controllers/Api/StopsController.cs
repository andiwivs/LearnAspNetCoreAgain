using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("/api/trips/{tripName}/stops")]
    public class StopsController : Controller
    {
        private GeoCoordsService _geoService;
        private ILogger<StopsController> _logger;
        private IWorldRepository _repository;

        public StopsController(IWorldRepository repository, ILogger<StopsController> logger, GeoCoordsService geoService)
        {
            _repository = repository;
            _logger = logger;
            _geoService = geoService;
        }

        [HttpGet]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetUserTripByName(User.Identity.Name, tripName);
                var stops = trip.Stops.OrderBy(s => s.Order).ToList();

                return Ok(Mapper.Map<IEnumerable<StopVM>>(stops));
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get stops for trip: {0}", ex);
            }

            return BadRequest("Failed to get stops");
        }

        [HttpPost]
        public async Task<IActionResult> Post(string tripName, [FromBody] StopVM stop)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(stop);

                    var result = await _geoService.GetCoordsAsync(newStop.Name);

                    if (!result.Success)
                    {
                        _logger.LogError(result.Message);
                    }
                    else
                    {

                        newStop.Longitude = result.Longitude;
                        newStop.Latitude = result.Latitude;

                        _repository.AddStop(tripName, newStop, User.Identity.Name);

                        if (await _repository.SaveChangesAsync())
                            return Created($"/api/trips/{tripName}/stops/{newStop.Name}", Mapper.Map<StopVM>(newStop));
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to save new stop for trip: {0}", ex);
            }

            return BadRequest("Failed to save new stop");
        }
    }
}
