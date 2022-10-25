using System.Collections.Generic;

namespace ReversiRestApi.Model
{
    public class SpelRepository : ISpelRepository
    {
        public List<Spel> Spellen { get; set; }
        
        public SpelRepository()
        {
            Spel spel1 = new Spel();
            Spel spel2 = new Spel();
            Spel spel3 = new Spel();
            spel1.Speler1Token = "abcdef";
            spel1.Omschrijving = "Potje snel reveri, dus niet lang nadenken";
            spel2.Speler1Token = "ghijkl";
            spel2.Speler2Token = "mnopqr";
            spel2.Omschrijving = "Ik zoek een gevorderde tegenspeler!";
            spel3.Speler1Token = "stuvwx";
            spel3.Omschrijving = "Na dit spel wil ik er nog een paar spelen tegen zelfde tegenstander";
            Spellen = new List<Spel> {spel1, spel2, spel3};
        }


        public void AddSpel(Spel spel)
        {
            Spellen.Add(spel);
        }

        public List<Spel> GetSpellen()
        {
            return Spellen;
        }

        public Spel GetSpel(string spelToken)
        {
            return Spellen.Find(s => s.Token == spelToken);
        }

        public Spel GetSpelWithSpelerToken(string id)
        {
            return Spellen.Find(s => s.Speler1Token == id || s.Speler2Token == id);
        }

        public void DoeZet(Spel spel)
        {
            var foundspel = Spellen.Find(s => s.Token == spel.Token);
            foundspel = spel;
        }

        public void Delete(Spel spel)
        {
            Spellen.Remove(spel);
        }
        
        public void Save(){}
    }
}