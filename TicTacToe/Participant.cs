using System.Text.Json.Serialization;

namespace TicTacToe;

public class Participant
{
    [JsonPropertyName("nom")]
    public string Nom { get; set; }
    
    [JsonPropertyName("pais")]
    public string Pais { get; set; }
}