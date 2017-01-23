using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public class WorldRepository : IWorldRepository
    {
        private WorldContext _context;
        private ILogger<WorldRepository> _logger;

        public WorldRepository(WorldContext context, ILogger<WorldRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddStop(string tripName, Stop stop, string username)
        {
            var trip = GetUserTripByName(username, tripName);

            if (trip != null)
            {
                trip.Stops.Add(stop); // need to add explicitly to ensure FK relationship is maintained
                _context.Stops.Add(stop);
            }
        }

        public void AddTrip(Trip trip)
        {
            _context.Add(trip);
        }

        public IEnumerable<Trip> GetAllTrips()
        {

            _logger.LogInformation("Getting all trips from the db");

            return _context
                        .Trips
                        .ToList();
        }

        public IEnumerable<Trip> GetTripsByUsername(string username)
        {
            
            _logger.LogInformation("Getting trips for user {0} the db", username);

            return _context
                        .Trips
                        .Include(t => t.Stops)
                        .Where(t => t.OwnerUsername == username)
                        .ToList();
        }

        public Trip GetTripByName(string tripName)
        {
            return _context
                        .Trips
                        .Include(t => t.Stops)
                        .Where(t => t.Name.Equals(tripName, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();
        }

        public Trip GetUserTripByName(string username, string tripName)
        {
            return _context
                        .Trips
                        .Include(t => t.Stops)
                        .Where(t => t.Name.Equals(tripName, StringComparison.OrdinalIgnoreCase) && t.OwnerUsername == username)
                        .FirstOrDefault();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
