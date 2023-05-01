using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReversieISpelImplementatie.Model;
using ReversiRestApi.Model;

namespace ReversiRestApi.Controllers
{
    [Route("api/Spel")]
    [ApiController]
    public class SpelController : ControllerBase
    {
        private readonly ISpelRepository iRepository;

        public SpelController(ISpelRepository repository)
        {
            iRepository = repository;
        }

        // GET api/spel
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler()
        {
            return iRepository.GetSpellen().Where(s => s.Speler2Token == null).Select(s => s.Omschrijving).ToList();
        }
        
        [HttpPost]
        public ActionResult<Spel> AddSpel([FromForm] string spelerToken, [FromForm] string omschrijving) {
            var spel = new Spel();
            spel.Speler1Token = spelerToken;
            spel.Omschrijving = omschrijving;
            spel.Token = Guid.NewGuid().ToString();
            iRepository.AddSpel(spel);
            return Ok(spel);
        }
        
        [HttpGet("/api/spel/{id}")]
        public ActionResult<Spel> GetSpel(string id) {
            var spel = iRepository.GetSpel(id);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpGet("/api/spel/beurt")]
        public ActionResult<string> GetSpelBeurt([FromForm] string spelToken)
        {
            var spel = iRepository.GetSpel(spelToken);

            if (spel == null) return NotFound();
            var beurt = spel.AandeBeurt;
            return Ok(beurt);
        }

        [HttpPut("/api/spel/zet/{id}")]
        public ActionResult<string> SpelDoeZet(string id, [FromQuery]string spelToken,
            [FromQuery] int rijzet, [FromQuery] int kolomzet)
        {
            if (spelToken == null)
            {
                return Unauthorized();
            }

            var spel = iRepository.GetSpel(id);
            if (spel == null)
            {
                return NotFound();
            }

            if (spel.AandeBeurt == Kleur.Wit)
            {
                //TODO
            }
            spel.DoeZet(rijzet, kolomzet);

            return Ok();
        }




    }
}