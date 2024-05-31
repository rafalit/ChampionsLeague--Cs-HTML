using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class TeamsController : Controller
    {
        private readonly IClubsService _clubsService;

        public TeamsController(IClubsService clubsService)
        {
            _clubsService = clubsService;
        }

        public IActionResult ChooseTeam()
        {
            // Pobierz linki do zdjęć z bazy danych
            var clubs = _clubsService.Get(); // Załóżmy, że serwis ClubsService ma metodę GetAll, która pobiera wszystkie kluby z bazy danych
            var teamImages = new List<string>();
            foreach (var club in clubs)
            {
                teamImages.Add(club.Photo); // Zakładając, że właściwość Photo w modelu Club zawiera linki do zdjęć
            }

            // Przekazujemy listę linków do zdjęć do widoku
            return View(teamImages);
        }

        [HttpGet("/api/teams")]
        public IActionResult GetTeams()
        {
            var teams = _clubsService.Get();
            return Json(teams);
        }
    }
}
