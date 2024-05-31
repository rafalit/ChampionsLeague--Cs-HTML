using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Collections.Generic;

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
            var clubs = _clubsService.Get();
            return View(clubs);
        }

        [HttpPost]
        public IActionResult SelectTeam(string teamId)
        {
            _clubsService.AddSelectedClub(teamId);
            if (_clubsService.SelectedClubsCount() == 32)
            {
                return RedirectToAction("DrawGroups");
            }
            return RedirectToAction("ChooseTeam");
        }

        [HttpGet("/api/teams")]
        public IActionResult GetTeams()
        {
            var teams = _clubsService.Get();
            return Json(teams);
        }

        [HttpGet("DrawGroups")]
        public IActionResult DrawGroups()
        {
            var selectedClubs = _clubsService.GetSelectedClubs();
            var groups = DrawGroups(selectedClubs);
            return View(groups);
        }

        private List<Group> DrawGroups(List<Club> clubs)
        {
            var groups = new List<Group>();
            for (int i = 0; i < 8; i++)
            {
                groups.Add(new Group { Name = $"Group {i + 1}", Teams = new List<Club>() });
            }

            var random = new Random();
            foreach (var club in clubs)
            {
                var groupIndex = random.Next(0, 8);
                groups[groupIndex].Teams.Add(club);
            }

            return groups;
        }
    }
}