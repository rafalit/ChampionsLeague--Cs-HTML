namespace WebApplication1.Models
{
    public class ClubsStoreDatabaseSettings : IClubsStoreDatabaseSettings
    {
        public string ClubsCoursesCollectionName { get; set; } = "Clubs";
        public string ConnectionString { get; set; } = String.Empty;

        public string DatabaseName { get; set; } = String.Empty;


    }
}
