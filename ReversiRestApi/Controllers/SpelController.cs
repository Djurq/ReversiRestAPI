using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReversiRestApi.Model;
using ReversiRestApi.Repositories;

namespace ReversiRestApi.Controllers
{
    [Route("api/Spel")]
    [ApiController]
    public class SpelController : ControllerBase
    {
        private readonly ISpelRepository _iRepository;

        public SpelController(ISpelRepository repository)
        {
            _iRepository = repository;
        }

        // GET api/spel
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler()
        {
            return _iRepository.GetSpellen().Where(s => s.Speler2Token == null).Select(s => s.Omschrijving).ToList();
        }
        
        [HttpPost]
        public ActionResult<Spel> AddSpel([FromForm] string spelerToken, [FromForm] string omschrijving) {
            var spel = new Spel();
            spel.Speler1Token = spelerToken;
            spel.Omschrijving = omschrijving;
            spel.Token = Guid.NewGuid().ToString();
            _iRepository.AddSpel(spel);
            return Ok(spel);
        }
        
        [HttpGet("/api/spel/{id}")]
        public ActionResult<Spel> GetSpel(string id) {
            var spel = _iRepository.GetSpel(id);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpGet("/api/spel/beurt/")]
        public ActionResult<string> GetSpelBeurt([FromForm] string spelToken)
        {
            var spel = _iRepository.GetSpel(spelToken);

            if (spel == null) return NotFound();
            var beurt = spel.AandeBeurt;
            return Ok(beurt);
        }

        [HttpPut("/api/spel/zet")]
        public ActionResult<string> SpelDoeZet([FromBody] string spelToken, [FromBody] string spelerToken, 
            [FromBody] int rijzet, [FromBody] int kolomzet)
        {
            var spel = _iRepository.GetSpel(spelToken);

            if (spel == null) return NotFound();
            spel.DoeZet(rijzet, kolomzet);
            _iRepository.DoeZet(spel);
            return Ok(spel);
        }
        
         [HttpPut("/api/spel/opgeven")]
                public ActionResult<string> SpelDoeZet([FromBody] string spelToken, [FromBody] string spelerToken)
                {
                    var spel = _iRepository.GetSpel(spelToken);
                    
                    if (spel == null) return NotFound();
                    spel.Opgeven();
                    _iRepository.DoeZet(spel);
                    return Ok(spel.Afgelopen());
                }
        
        




    }
}