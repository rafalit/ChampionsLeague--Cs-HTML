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

            _logger.LogInformation($"Number of selected clubs: {selectedClubs.Count}");
            var groups = CreateGroups(selectedClubs);
            _logger.LogInformation($"Number of groups created: {groups.Count}");
            return View(groups);
        }

        [HttpPost]
        public IActionResult ProceedToKnockout([FromBody] Dictionary<string, List<string>> selectedTeams)
        {
            var selectedTeamIds = selectedTeams["selectedTeamIds"];
            HttpContext.Session.SetObject("SelectedTeamIds", selectedTeamIds);
            return Ok();
        }

        [HttpGet("KnockoutStage")]
        public IActionResult KnockoutStage()
        {
            var selectedTeamIds = HttpContext.Session.GetObject<List<string>>("KnockoutTeamIds");
            if (selectedTeamIds == null || selectedTeamIds.Count != 32) // Sprawdź czy jest odpowiednia liczba drużyn
            {
                return RedirectToAction("DrawGroups");
            }

            var selectedClubs = _clubsService.GetSelectedClubs().Where(club => selectedTeamIds.Contains(club.Id)).ToList();
            var groups = CreateGroups(selectedClubs);
            var pairs = CreateKnockoutPairs(groups);
            return View(pairs);
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

        private List<(Club, Club)> CreateKnockoutPairs(List<Group> groups)
        {
            var pairs = new List<(Club, Club)>();
            var random = new Random();

            foreach (var group in groups)
            {
                // Sprawdź, czy grupa ma co najmniej dwie drużyny
                if (group.Teams.Count >= 2)
                {
                    var selectedTeams = group.Teams.OrderBy(t => random.Next()).Take(2).ToList();
                    pairs.Add((selectedTeams[0], selectedTeams[1]));
                }
                else
                {
                    _logger.LogWarning($"Group {group.Name} has less than 2 teams and will be skipped in knockout pairs generation.");
                }
            }

            return pairs;
        }

    }
}
