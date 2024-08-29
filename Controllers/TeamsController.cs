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

        [HttpGet("/api/Teams")]
        public IActionResult GetTeams()
        {
            var teams = _clubsService.Get();
            return Json(teams);
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

        public IActionResult DrawGroups()
        {
            // Pobieramy wybrane zespoły z sesji
            var selectedClubs = HttpContext.Session.GetObject<List<Club>>("SelectedClubs");
            if (selectedClubs == null || selectedClubs.Count == 0)
            {
                // Obsługa sytuacji, gdy nie ma wybranych zespołów w sesji
                return RedirectToAction("ChooseTeam");
            }

            var selectedTeamIds = HttpContext.Session.GetObject<List<string>>("SelectedTeamIds") ?? new List<string>(); // Pobieramy wybrane ID zespołów z sesji

            ViewData["SelectedTeamIds"] = selectedTeamIds; // Przekazujemy listę wybranych ID zespołów do widoku

            var groups = CreateGroups(selectedClubs);
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

                groups[groupIndex].Teams.Add(club);
            }

            return groups;
        }

        [HttpPost]
        public IActionResult ProceedToKnockout([FromBody] List<string> selectedClubIds)
        {
            var selectedClubs = _clubsService.GetClubsByIds(selectedClubIds);

            // Przygotuj pary drużyn dla fazy pucharowej (16 drużyn)
            var knockoutPairs = PrepareKnockoutPairs(selectedClubs);

            // Zapisz pary drużyn do sesji
            HttpContext.Session.SetObject("KnockoutPairs", knockoutPairs.Select(pair => new List<Club> { pair.Item1, pair.Item2 }).ToList());


            _logger.LogInformation($"Saved {knockoutPairs.Count} pairs to session.");

            foreach (var pair in knockoutPairs)
            {
                _logger.LogInformation($"{pair.Item1.Name} - {pair.Item2.Name}");
            }

            return RedirectToAction("KnockoutStage");
        }

        [HttpGet("Teams/KnockoutStage")]
        public IActionResult KnockoutStage()
        {
            // Pobierz pary drużyn dla fazy pucharowej z sesji
            var knockoutPairs = HttpContext.Session.GetObject<List<List<Club>>>("KnockoutPairs")
                                .Select(pair => (pair[0], pair[1]))
                                .ToList();

            _logger.LogInformation($"Retrieved {knockoutPairs.Count} pairs from session.");

            foreach (var pair in knockoutPairs)
            {
                _logger.LogInformation($"retrieved {pair.Item1.Name} - {pair.Item2.Name}");
            }

            return View(knockoutPairs);
        }

        // Metoda do przygotowania par drużyn dla fazy pucharowej (16 drużyn)
        private List<(Club, Club)> PrepareKnockoutPairs(List<Club> clubs)
        {
            var pairs = new List<(Club, Club)>();

            // Utwórz pary drużyn dla fazy pucharowej
            for (int i = 0; i < clubs.Count; i += 2)
            {
                pairs.Add((clubs[i], clubs[i + 1]));
            }

            return pairs;
        }
    }
}
