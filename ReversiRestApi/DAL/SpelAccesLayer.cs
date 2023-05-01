using System.Collections.Generic;
using System.Linq;
using ReversiRestApi.Model;

namespace ReversiRestApi.DAL
{
    public class SpelAccesLayer : ISpelRepository
    {
        private readonly ReversiContext _context;

        public SpelAccesLayer(ReversiContext context)
        {
            _context = context;
        }
        public void AddSpel(Spel spel) {
            _context.Spellen.Add(spel);
            _context.SaveChanges();
        }

        public List<Spel> GetSpellen() {
            return _context.Spellen.ToList();
        }

        public Spel GetSpel(string spelToken) {
            return _context.Spellen.First(spel => spel.Token == spelToken);
        }

        public void Delete(Spel spel) {
            _context.Spellen.Remove(spel);
        }

        public void Save() {
            _context.SaveChanges();
        }
    }
}