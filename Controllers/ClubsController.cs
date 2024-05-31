using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClubsController : ControllerBase
    {
        private readonly IClubsService _clubService;

        public ClubsController(IClubsService clubService)
        {
            _clubService = clubService;
        }

        [HttpGet]
        public ActionResult<List<Club>> Get()
        {
            return _clubService.Get();
        }

        [HttpGet("{id}")]
        public ActionResult<Club> Get(string id)
        {
            var club = _clubService.Get(id);

            if (club == null)
            {
                return NotFound($"Club with id = {id} not found");
            }

            return club;
        }

        [HttpPost]
        public ActionResult<Club> Post([FromBody] Club club)
        {
            _clubService.Create(club);
            return CreatedAtAction(nameof(Get), new { id = club.Id }, club);
        }

        [HttpPut("{id}")]
        public ActionResult Put(string id, [FromBody] Club club)
        {
            var existingClub = _clubService.Get(id);

            if (existingClub == null)
            {
                return NotFound($"Club with id = {id} not found");
            }

            _clubService.Update(id, club);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string id)
        {
            var club = _clubService.Get(id);

            if (club == null)
            {
                return NotFound($"Club with id = {id} not found");
            }

            _clubService.Remove(club.Id);
            return Ok($"Club with id = {id} deleted");
        }
    }
}
