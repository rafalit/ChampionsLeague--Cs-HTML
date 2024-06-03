using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class TeamsController : Controller
    {
        private readonly IClubsService _clubsService;
        private readonly ILogger<TeamsController> _logger;

        public TeamsController(IClubsService clubsService, ILogger<TeamsController> logger)
        {
            _clubsService = clubsService;
            _logger = logger;
        }

        public IActionResult ChooseTeam()
        {
            var clubs = _clubsService.Get();
            return View(clubs);
        }

        [HttpPost]
        public IActionResult SelectTeam(string[] selectedTeamIds)
        {
            foreach (var teamId in selectedTeamIds)
            {
                _clubsService.AddSelectedClub(teamId);
                _logger.LogInformation($"Team selected: {teamId}");
            }

            // Zapisujemy wybrane zespoły w sesji
            HttpContext.Session.SetObject("SelectedClubs", _clubsService.GetSelectedClubs());

            if (_clubsService.SelectedClubsCount() == 32)
            {
                _logger.LogInformation("32 teams selected. Redirecting to DrawGroups.");
                return RedirectToAction("DrawGroups");
            }
            _logger.LogInformation("Less than 32 teams selected. Redirecting to ChooseTeam.");
            return RedirectToAction("ChooseTeam");
        }

        [HttpGet("/api/Teams")]
        public IActionResult GetTeams()
        {
            var teams = _clubsService.Get();
            return Json(teams);
        }

        [HttpGet("DrawGroups")]
        public IActionResult DrawGroups()
        {
            // Pobieramy wybrane zespoły z sesji
            var selectedClubs = HttpContext.Session.GetObject<List<Club>>("SelectedClubs");
            if (selectedClubs == null || selectedClubs.Count == 0)
            {
                // Obsługa sytuacji, gdy nie ma wybranych zespołów w sesji
                // Możesz przekierować użytkownika z powrotem do strony wyboru zespołów lub wyświetlić odpowiedni komunikat
                return RedirectToAction("ChooseTeam");
            }

            _logger.LogInformation($"Number of selected clubs: {selectedClubs.Count}");
            var groups = CreateGroups(selectedClubs);
            _logger.LogInformation($"Number of groups created: {groups.Count}");
            return View(groups);
        }

        private List<Group> CreateGroups(List<Club> clubs)
        {
            var groups = new List<Group>();
            for (int i = 0; i < 8; i++)
            {
                groups.Add(new Group { Name = $"Group {i + 1}", Teams = new List<Club>() });
            }

            var random = new Random();
            foreach (var club in clubs)
            {
                int groupIndex;
                do
                {
                    groupIndex = random.Next(0, 8);
                } while (groups[groupIndex].Teams.Count >= 4); // Ensure each group has only 4 teams

                _logger.LogInformation($"Adding team {club.Name} to group {groupIndex + 1}");
                groups[groupIndex].Teams.Add(club);
            }

            return groups;
        }
    }
}
