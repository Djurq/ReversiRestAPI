using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ReversiRestApi.Model
{
    public class Spel : ISpel
    {
        private const int BordOmvang = 8;

        private readonly int[,] _richting = new int[8, 2]
        {
            {0, 1}, // naar rechts
            {0, -1}, // naar links
            {1, 0}, // naar onder
            {-1, 0}, // naar boven
            {1, 1}, // naar rechtsonder
            {1, -1}, // naar linksonder
            {-1, 1}, // naar rechtsboven
            {-1, -1}
        }; // naar linksboven

        [Key]
        public int ID { get; set; }
        public string Omschrijving { get; set; }
        public string Token { get; set; }
        public string Speler1Token { get; set; }
        public string Speler2Token { get; set; }

        [NotMapped]
        private Kleur[,] _bord;

        [NotMapped]
        [JsonConverter(typeof(SpelBordConverter))]
        public Kleur[,] Bord
        { get => _bord; set => BordAsString = _convert(value); }

        [JsonIgnore]
        private string BordAsString {
            get => _convert(_bord);

            set => _bord = _convert(value);
        }

        private static string _convert(Kleur[,] bord) {
            
            string bordString = "";

            for (int rij = 0; rij < BordOmvang; rij++) {
                for (int kolom = 0; kolom < BordOmvang; kolom++) {

                    Kleur kleur = bord[kolom, rij];
                    int intKleur;
                    switch (kleur) {
                        default:
                        case Kleur.Geen:
                            intKleur = 0;
                            break;
                        case Kleur.Wit:
                            intKleur = 1;
                            break;
                        case Kleur.Zwart:
                            intKleur = 2;
                            break;
                    }

                    bordString += intKleur;
                }

                bordString += ":";
            }
            return bordString;
        }

        private static Kleur[,] _convert(string bordString) {
            Kleur[,] bord = new Kleur[BordOmvang, BordOmvang];

            int rij = 0;
            foreach (string line in bordString.Split(":")) {
                int kolom = 0;
                foreach (char c in line.ToCharArray()) {
                    Kleur kleur;
                    
                    switch (c) {
                        default:
                        case '0':
                            kleur = Kleur.Geen;
                            break;
                        case '1':
                            kleur = Kleur.Wit;
                            break;
                        case '2':
                            kleur = Kleur.Zwart;
                            break;
                    }

                    bord[kolom, rij] = kleur;
                    
                    rij++;
                }
                kolom++;
            }
            return bord;
        }



        public Kleur AandeBeurt { get; set; }

        public Spel()
        {
            Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            Token = Token.Replace("/", "q"); // slash mijden ivm het opvragen van een spel via een api obv het token
            Token = Token.Replace("+", "r"); // plus mijden ivm het opvragen van een spel via een api obv het token

            Bord = new Kleur[BordOmvang, BordOmvang];
            Bord[3, 3] = Kleur.Wit;
            Bord[4, 4] = Kleur.Wit;
            Bord[3, 4] = Kleur.Zwart;
            Bord[4, 3] = Kleur.Zwart;

            AandeBeurt = Kleur.Geen;
        }

        public void Pas()
        {
            // controleeer of er geen zet mogelijk is voor de speler die wil passen, alvorens van beurt te wisselen.
            if (IsErEenZetMogelijk(AandeBeurt))
                throw new Exception("Passen mag niet, er is nog een zet mogelijk");
            else
                WisselBeurt();
        }


        public bool Afgelopen() // return true als geen van de spelers een zet kan doen
        {
            if (!IsErEenZetMogelijk(Kleur.Wit) && !IsErEenZetMogelijk(Kleur.Zwart))
            {
                return true;
            }

            return false;
        }
        
        public void Opgeven()
        {
            for (int rijZet = 0; rijZet < BordOmvang; rijZet++)
            {
                for (int kolomZet = 0; kolomZet < BordOmvang; kolomZet++)
                {
                    _bord[rijZet, kolomZet] = GetKleurTegenstander(AandeBeurt);
                }
            }
        }

        public Kleur OverwegendeKleur()
        {
            int aantalWit = 0;
            int aantalZwart = 0;
            for (int rijZet = 0; rijZet < BordOmvang; rijZet++)
            {
                for (int kolomZet = 0; kolomZet < BordOmvang; kolomZet++)
                {
                    if (_bord[rijZet, kolomZet] == Kleur.Wit)
                        aantalWit++;
                    else if (_bord[rijZet, kolomZet] == Kleur.Zwart)
                        aantalZwart++;
                }
            }

            if (aantalWit > aantalZwart)
                return Kleur.Wit;
            if (aantalZwart > aantalWit)
                return Kleur.Zwart;
            return Kleur.Geen;
        }

        public bool ZetMogelijk(int rijZet, int kolomZet)
        {
            if (!PositieBinnenBordGrenzen(rijZet, kolomZet))
                throw new Exception($"Zet ({rijZet},{kolomZet}) ligt buiten het bord!");
            return ZetMogelijk(rijZet, kolomZet, AandeBeurt);
        }

        private static int[][] _richtingen =
        {
            new[] {1, 0}, new[] {0, 1}, new[] {-1, 0}, new[] {0, -1},
            new[] {1, 1}, new[] {-1, -1}, new[] {-1, 1}, new[] {1, -1}
        };

        public void DoeZet(int rijZet, int kolomZet)
        {
            if (!ZetMogelijk(rijZet, kolomZet))
            {
                throw new Exception($"Zet ({rijZet},{kolomZet}) is niet mogelijk!");
            }

            _bord[rijZet, kolomZet] = AandeBeurt;
            foreach (var directions in _richtingen)
            {
                DraaiStenenVanTegenstanderInOpgegevenRichtingOmIndienIngesloten(rijZet, kolomZet, AandeBeurt,
                    directions[0], directions[1]);
            }

          
            // todo: maak hierbij gebruik van de reeds in deze klassen opgenomen methoden!
        }

        private static Kleur GetKleurTegenstander(Kleur kleur)
        {
            return kleur switch
            {
                Kleur.Wit => Kleur.Zwart,
                Kleur.Zwart => Kleur.Wit,
                _ => Kleur.Geen
            };
        }

        private bool IsErEenZetMogelijk(Kleur kleur)
        {
            if (kleur == Kleur.Geen)
                throw new Exception("Kleur mag niet gelijk aan Geen zijn!");
            // controleeer of er een zet mogelijk is voor kleur
            for (int rijZet = 0; rijZet < BordOmvang; rijZet++)
            {
                for (int kolomZet = 0; kolomZet < BordOmvang; kolomZet++)
                {
                    if (ZetMogelijk(rijZet, kolomZet, kleur))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ZetMogelijk(int rijZet, int kolomZet, Kleur kleur)
        {
            // Check of er een richting is waarin een zet mogelijk is. Als dat zo is, return dan true.
            for (int i = 0; i < 8; i++)
            {
                {
                    if (StenenInTeSluitenInOpgegevenRichting(rijZet, kolomZet,
                            kleur,
                            _richting[i, 0], _richting[i, 1]))
                        return true;
                }
            }

            return false;
        }

        private void WisselBeurt()
        {
            if (AandeBeurt == Kleur.Wit)
                AandeBeurt = Kleur.Zwart;
            else
                AandeBeurt = Kleur.Wit;
        }

        private static bool PositieBinnenBordGrenzen(int rij, int kolom)
        {
            return (rij >= 0 && rij < BordOmvang &&
                    kolom >= 0 && kolom < BordOmvang);
        }

        private bool ZetOpBordEnNogVrij(int rijZet, int kolomZet)
        {
            // Als op het bord gezet wordt, en veld nog vrij, dan return true, anders false
            return (PositieBinnenBordGrenzen(rijZet, kolomZet) && Bord[rijZet, kolomZet] == Kleur.Geen);
        }

        private bool StenenInTeSluitenInOpgegevenRichting(int rijZet, int kolomZet,
            Kleur kleurZetter,
            int rijRichting, int kolomRichting)
        {
            int rij, kolom;
            Kleur kleurTegenstander = GetKleurTegenstander(kleurZetter);
            if (!ZetOpBordEnNogVrij(rijZet, kolomZet))
                return false;

            // Zet rij en kolom op de index voor het eerst vakje naast de zet.
            rij = rijZet + rijRichting;
            kolom = kolomZet + kolomRichting;

            int aantalNaastGelegenStenenVanTegenstander = 0;
            // Zolang Bord[rij,kolom] niet buiten de bordgrenzen ligt, en je in het volgende vakje 
            // steeds de kleur van de tegenstander treft, ga je nog een vakje verder kijken.
            // Bord[rij, kolom] ligt uiteindelijk buiten de bordgrenzen, of heeft niet meer de
            // de kleur van de tegenstander.
            // N.b.: deel achter && wordt alleen uitgevoerd als conditie daarvoor true is.
            while (PositieBinnenBordGrenzen(rij, kolom) && Bord[rij, kolom] == kleurTegenstander)
            {
                rij += rijRichting;
                kolom += kolomRichting;
                aantalNaastGelegenStenenVanTegenstander++;
            }

            // Nu kijk je hoe je geeindigt bent met bovenstaande loop. Alleen
            // als alle drie onderstaande condities waar zijn, zijn er in de
            // opgegeven richting stenen in te sluiten.
            return (PositieBinnenBordGrenzen(rij, kolom) &&
                    Bord[rij, kolom] == kleurZetter &&
                    aantalNaastGelegenStenenVanTegenstander > 0);
        }

        private bool DraaiStenenVanTegenstanderInOpgegevenRichtingOmIndienIngesloten(int rijZet, int kolomZet,
            Kleur kleurZetter,
            int rijRichting, int kolomRichting)
        {
            int rij, kolom;
            Kleur kleurTegenstander = GetKleurTegenstander(kleurZetter);
            bool stenenOmgedraaid = false;

            if (StenenInTeSluitenInOpgegevenRichting(rijZet, kolomZet, kleurZetter, rijRichting, kolomRichting))
            {
                rij = rijZet + rijRichting;
                kolom = kolomZet + kolomRichting;

                // N.b.: je weet zeker dat je niet buiten het bord belandt,
                // omdat de stenen van de tegenstander ingesloten zijn door
                // een steen van degene die de zet doet.
                while (Bord[rij, kolom] == kleurTegenstander)
                {
                    Bord[rij, kolom] = kleurZetter;
                    rij += rijRichting;
                    kolom += kolomRichting;
                }

                stenenOmgedraaid = true;
            }

            return stenenOmgedraaid;
        }
    }
}