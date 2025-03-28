using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
        private const string JugadorDesqualificada = "Espanya";

        static async Task Main(string[] args)
        {
            var participantsValids = await ConseguirParticipants();

            var victories = await ProcessarPartides(participantsValids);

            Console.WriteLine("Resultats Finals:");
            foreach (var victory in victories)
            {
                Console.WriteLine($"{victory.Key}: {victory.Value} victòries");
            }
        }

        private static async Task<Dictionary<string, string>> ConseguirParticipants()
        {
            var participants = await client.GetFromJsonAsync<List<string>>("/jugadors");
            if (participants == null) return new Dictionary<string, string>();

            string pattern = @"participant (?<nom>[A-Za-z]+ [A-Za-z'-]+).*representa(nt)? (a|de) (?<pais>[A-za-z]+)";
            var participantsValids = new Dictionary<string, string>();
        
            foreach (var participant1 in participants)
            {
                Match match = Regex.Match(participant1, pattern);
                if (match.Success)
                {
                    string nom = match.Groups["nom"].Value;
                    string cognom = match.Groups["cognom"].Value;
                    string pais = match.Groups["pais"].Value;
                
                    if (pais != JugadorDesqualificada)
                    {
                        Console.WriteLine($"Nom: {nom} {cognom}, País: {pais}");

                    }
                }
            }

            return participantsValids;
        }

        private static async Task<Dictionary<string, int>> ProcessarPartides(Dictionary<string, string> participantsValids)
        {
            var partidesNumeros = await client.GetFromJsonAsync<List<int>>("/partides");

            Dictionary<string, int> victories = new();

            foreach (var numero in partidesNumeros)
            {
                var partida = await client.GetFromJsonAsync<Partida>($"partida{numero}");

                if (partida == null) continue;

                if (!participantsValids.ContainsKey(partida.Jugador1.ToLower()) || !participantsValids.ContainsKey(partida.Jugador2.ToLower()))
                    continue;

                string guanyador = SaberQuiHaGuanyat(partida.Tauler);

                if (!string.IsNullOrEmpty(guanyador))
                {
                    Victories(victories, guanyador);
                }
            }

            return victories;
        }

        private static string SaberQuiHaGuanyat(string tauler)
        {
            string[] liniesGuanyadores = { "012", "345", "678", "036", "147", "258", "048", "246" };

            foreach (var linia in liniesGuanyadores)
            {
                char a = tauler[linia[0] - '0'];
                char b = tauler[linia[1] - '0'];
                char c = tauler[linia[2] - '0'];

                if (a == b && b == c && a != '.')
                    return a == 'X' ? "Jugador1" : "Jugador2";
            }

            return string.Empty;
        }

        private static void Victories(Dictionary<string, int> victories, string jugador)
        {
            if (victories.ContainsKey(jugador))
            {
                victories[jugador]++;
            }
            else
            {
                victories.Add(jugador, 1);
            }
        }
    }
    
}
