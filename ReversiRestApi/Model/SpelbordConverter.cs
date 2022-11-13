using System;
using System.Text.Json;

namespace ReversiRestApi.Model
{
    public class SpelBordConverter : System.Text.Json.Serialization.JsonConverter<Kleur[,]>
    {
        public override Kleur[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Kleur[,] bord, JsonSerializerOptions options) {
            writer.WriteStartObject();
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    Kleur kleur = bord[y, x];
                    writer.WriteNumber(x + "," + y, kleur.GetHashCode());
                }
            }

            writer.WriteEndObject();
        }
    }
}
