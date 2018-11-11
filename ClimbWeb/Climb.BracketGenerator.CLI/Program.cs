using System;

namespace Climb.BracketGenerator.CLI
{
    public static class Program
    {
        private static void Main()
        {
            while(true)
            {
                Console.Write("Competitor Count: ");
                var command = Console.ReadLine();

                if(int.TryParse(command, out var competitorCount))
                {
                    var bracketGenerator = new Services.BracketGenerator();
                    var tournament = bracketGenerator.CreateTournament(competitorCount);
                    PrintTournament(tournament);
                }
            }
        }

        private static void PrintTournament(Services.BracketGenerator.Tournament tournament)
        {
            Console.WriteLine("---Winners---");
            foreach(var round in tournament.Winners.Rounds)
            {
                PrintRound(round);
            }

            Console.WriteLine("---Losers---");
            foreach(var round in tournament.Losers.Rounds)
            {
                PrintRound(round);
            }

            void PrintRound(Services.BracketGenerator.Round round) => Console.WriteLine($"Round {round.Index}\n{string.Join("\n", round.Games)}");
        }
    }
}