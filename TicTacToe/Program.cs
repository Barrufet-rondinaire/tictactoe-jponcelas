using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TicTacToe;

class Program
{
    private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
    private const string JugadorDesqualificada = "Espanya";

    static async Task Main(string[] args)
    {
        await ConseguirParticipants();
    }

    private static async Task ConseguirParticipants()
    {
        var participants = await client.GetFromJsonAsync<List<string>>("/jugadors");
        if (participants == null) return;

        string pattern = @"participant (?<nom>[A-Za-z]+ [A-Za-z'-]+).*representa(nt)? (a|de) (?<pais>[A-za-z]+)";

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
    }

    private static string DeterminarGuanyador(string tauler)
    {
        string[] liniesGuanyadores = { "012", "345", "678", "036", "147", "258", "048", "246" };

        foreach (var line in liniesGuanyadores)
        {
            
        }
    }
    
    
    
    
}