using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();

        IEnumerable<Trip> GetTripsByUsername(string username);

        Trip GetTripByName(string tripName);

        Trip GetUserTripByName(string username, string tripName);

        void AddTrip(Trip trip);

        void AddStop(string tripName, Stop stop, string username);

        Task<bool> SaveChangesAsync();
    }
}