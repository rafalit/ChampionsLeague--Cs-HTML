using WebApplication1.Models;
using System.Collections.Generic;

namespace WebApplication1.Services
{
    public interface IClubsService
    {
        List<Club> Get();
        Club Get(string id);
        Club Create(Club club);
        void Update(string id, Club club);
        void Remove(string id);
        void AddSelectedClub(string clubId);
        int SelectedClubsCount();
        List<Club> GetSelectedClubs();
    }
}
