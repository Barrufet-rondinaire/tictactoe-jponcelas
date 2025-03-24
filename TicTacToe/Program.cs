namespace TicTacToe;

class Program
{
    private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
    
    static void Main(string[] args)
    {
        
    }
}