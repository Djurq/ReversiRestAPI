using System.Collections.Generic;

namespace ReversiRestApi.Model
{
    public interface ISpelRepository
    {
        void AddSpel(Spel spel);

        void DoeZet(Spel spel);
        public List<Spel> GetSpellen();
        Spel GetSpel(string spelToken);

        Spel GetSpelWithSpelerToken(string spelerToken);
    }
}