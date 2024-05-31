using WebApplication1.Models;
using MongoDB.Driver;

namespace WebApplication1.Services
{
    public class ClubsService : IClubsService
    {
        private readonly IMongoCollection<Club> _clubs;

        public ClubsService(IClubsStoreDatabaseSettings settings, IMongoClient mongoClient)
        {
            var databse = mongoClient.GetDatabase(settings.DatabaseName);
            _clubs = databse.GetCollection<Club>(settings.ClubsCoursesCollectionName);
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

        public void Remove(string id)
        {
            _clubs.DeleteOne(club => club.Id == id);
        }

        public void Update(string id, Club club)
        {
            _clubs.ReplaceOne(club => club.Id == id, club);
        }
    }
}
