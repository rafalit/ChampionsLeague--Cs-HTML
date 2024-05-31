namespace WebApplication1.Models
{
    public interface IClubsStoreDatabaseSettings
    {
        string ClubsCoursesCollectionName { get; set; }
        string ConnectionString { get; set; }

        string DatabaseName { get; set; }
    }
}
