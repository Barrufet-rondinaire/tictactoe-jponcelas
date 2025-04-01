using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
        private const string JugadorDesqualificat = "Espanya";

        static async Task Main(string[] args)
        {
            var participantsValids = await ConseguirParticipants();
            var victorias = await ProcesarPartidasAsync(participantsValids);
            EnseyarResultatJugadors(victorias, participantsValids);
        }

        private static async Task<Dictionary<string, string>> ConseguirParticipants()
        {
            Console.WriteLine("Recuperant participants...");
            var participants = await client.GetFromJsonAsync<List<string>>("/jugadors");
            if (participants == null) return new Dictionary<string, string>();

            string pattern = @"participant (?<nom>[A-Za-z]+ [A-Za-z'-]+).*representa(nt)? (a|de) (?<pais>[A-Za-z]+)";
            var participantsValids = new Dictionary<string, string>();

            foreach (var participant in participants)
            {
                Match match = Regex.Match(participant, pattern);
                if (match.Success)
                {
                    string nom = match.Groups["nom"].Value;
                    string pais = match.Groups["pais"].Value;

                    if (pais != JugadorDesqualificat)
                    {
                        participantsValids[nom] = pais;
                        Console.WriteLine($"Nom: {nom}, País: {pais}");
                    }
                }
            }
            return participantsValids;
        }

        private static async Task<Dictionary<string, int>> ProcesarPartidasAsync(Dictionary<string, string> participantsValids)
        {
            Dictionary<string, int> victorias = new();

            for (int i = 1; i <= 10000; i++)
            {
                try
                {
                    var partida = await client.GetFromJsonAsync<Partida>($"/partida/{i}");
                    
                    string ganador = SaberQuiHaGuanyat(partida.Tauler);
                    if (ganador == "O")
                    {
                        if (!victorias.ContainsKey(partida.Jugador1))
                        {
                            victorias[partida.Jugador1] = 0;
                        }
                        victorias[partida.Jugador1]++;
                        Console.WriteLine($"Partida {i}: Guanyador - Jugador 1 ({partida.Jugador1})");
                    }
                    else if (ganador == "X")
                    {
                        if (!victorias.ContainsKey(partida.Jugador2))
                        {
                            victorias[partida.Jugador2] = 0;
                        }
                        victorias[partida.Jugador2]++;
                        Console.WriteLine($"Partida {i}: Guanyador - Jugador 2 ({partida.Jugador2})");
                    }
                    else
                    {
                        Console.WriteLine($"Partida {i}: Sense guanyador.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processant partida {i}: {ex.Message}");
                }
            }

            return victorias;
        }

        private static string SaberQuiHaGuanyat(List<string> tauler)
        {
            foreach (var fila in tauler)
            {
                if (fila == "XXX") return "X";
                if (fila == "OOO") return "O";
            }

            for (int columna = 0; columna < 3; columna++)
            {
                if (tauler[0][columna] == tauler[1][columna] && tauler[1][columna] == tauler[2][columna] && tauler[0][columna] != '.')
                {
                    return tauler[0][columna].ToString(); 
                }
            }

            if (tauler[0][0] == tauler[1][1] && tauler[1][1] == tauler[2][2] && tauler[0][0] != '.')
                return tauler[0][0].ToString();

            if (tauler[0][2] == tauler[1][1] && tauler[1][1] == tauler[2][0] && tauler[0][2] != '.')
                return tauler[0][2].ToString();

            return "";
        }


        private static void EnseyarResultatJugadors(Dictionary<string, int> victorias, Dictionary<string, string> participantsValids)
        {
            Console.WriteLine("\nResultats Finals:");
            foreach (var jugador in victorias.OrderByDescending(v => v.Value))
            {
                Console.WriteLine($"{jugador.Key}: {jugador.Value} victòries");
            }

            if (victorias.Count > 0)
            {
                var guanyador = victorias.OrderByDescending(v => v.Value).First();
                Console.WriteLine($"\nGuanyador: {guanyador.Key}, Victòries: {guanyador.Value}");
                if (participantsValids.TryGetValue(guanyador.Key, out string pais))
                {
                    Console.WriteLine($"País del guanyador: {pais}");
                }
            }
            else
            {
                Console.WriteLine("\nNo hi ha cap guanyador.");
            }
        }
    }
}
