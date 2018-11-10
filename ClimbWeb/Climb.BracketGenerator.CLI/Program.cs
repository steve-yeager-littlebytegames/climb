using System;

namespace Climb.BracketGenerator.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                Console.Write("Competitor Count: ");
                var command = Console.ReadLine();

                if(int.TryParse(command, out var competitorCount))
                {
                    var bracketGenerator = new Services.BracketGenerator();
                    var slots = bracketGenerator.Generate(competitorCount);
                    Console.WriteLine($"Competitor Count: {competitorCount}, Slot Count: {slots.Count}");
                }
            }
        }
    }
}