using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ClubsService : IClubsService
    {
        private readonly IMongoCollection<Club> _clubs;
        private List<Club> _selectedClubs;
        private List<(Club, Club)> _knockoutPairs = new List<(Club, Club)>();

        public ClubsService(IClubsStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.DatabaseName);
            _clubs = database.GetCollection<Club>(settings.ClubsCoursesCollectionName);
            _selectedClubs = new List<Club>();
        }

        public Club Create(Club club)
        {
            _clubs.InsertOne(club);
            return club;
        }

        public List<Club> Get()
        {
            return _clubs.Find(club => true).ToList();
        }

        public Club Get(string id)
        {
            return _clubs.Find(club => club.Id == id).FirstOrDefault();
        }

        public List<Club> GetClubsByIds(List<string> clubIds)
        {
            return _clubs.Find(club => clubIds.Contains(club.Id)).ToList();
        }

        public void Remove(string id)
        {
            _clubs.DeleteOne(club => club.Id == id);
        }

        public void Update(string id, Club club)
        {
            _clubs.ReplaceOne(club => club.Id == id, club);
        }

        public void AddSelectedClub(string clubId)
        {
            var selectedClub = _clubs.Find(club => club.Id == clubId).FirstOrDefault();
            if (selectedClub != null && !_selectedClubs.Contains(selectedClub))
            {
                _selectedClubs.Add(selectedClub);
            }
        }

        public int SelectedClubsCount()
        {
            return _selectedClubs.Count();
        }

        public List<Club> GetSelectedClubs()
        {
            return _selectedClubs;
        }

        public void SetKnockoutPairs(List<(Club, Club)> pairs)
        {
            _knockoutPairs = pairs;
        }

        public List<(Club, Club)> GetKnockoutPairs()
        {
            return _knockoutPairs;
        }
    }
}
