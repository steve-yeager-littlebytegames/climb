using System;
using System.Collections.Generic;
using Climb.Models;

namespace Climb.Services
{
    public class BracketGenerator : IBracketGenerator
    {
        public class Game
        {
            public int ID { get; }
            public bool IsBye { get; }
            public Game NextWin { get; set; }
            public Game NextLoss { get; set; }

            public Game(int id, bool isBye)
            {
                ID = id;
                IsBye = isBye;
            }

            public override string ToString() => $"{ID} W={NextWin?.ID} L={NextLoss?.ID} B={IsBye}";
        }

        public class Round
        {
            public int Index { get; }
            public List<Game> Games { get; } = new List<Game>();

            public Round(int index)
            {
                Index = index;
            }
        }

        public class Bracket
        {
            public List<Round> Rounds { get; } = new List<Round>();
        }

        public class Tournament
        {
            public int GameCount { get; set; }
            public int RoundCount { get; set; }
            public List<int?> Competitors { get; }
            
            public Bracket Winners { get; } = new Bracket();
            public Bracket Losers { get; } = new Bracket();

            public Tournament(List<int?> competitors)
            {
                Competitors = competitors;
            }

            public Round AddRound(Bracket bracket)
            {
                ++RoundCount;
                var round = new Round(RoundCount);
                bracket.Rounds.Add(round);
                return round;
            }

            public Game AddGame(Round round, bool isBye = false)
            {
                ++GameCount;
                var game = new Game(GameCount, isBye);
                round.Games.Add(game);
                return game;
            }
        }

        public int MinCompetitors => 4;

        public List<SetSlot> Generate(int competitorCount)
        {
            if(competitorCount < MinCompetitors)
            {
                throw new ArgumentException($"Need at least {MinCompetitors} competitors to generate bracket.", nameof(competitorCount));
            }

            var competitors = GetCompetitors(competitorCount);

            var tournament = new Tournament(competitors);

            throw new NotImplementedException();
        }

        private static List<int?> GetCompetitors(int count)
        {
            var fullBracketCount = GetFullBracketCount(count);

            var competitors = new List<int?>(fullBracketCount);
            for(var i = 0; i < count; i++)
            {
                competitors.Add(i);
            }

            for(var i = count; i < fullBracketCount; i++)
            {
                competitors.Add(null);
            }

            return competitors;
        }

        private static List<int?> SortCompetitors(IReadOnlyList<int?> competitors)
        {
            var count = competitors.Count;
            var sortedCompetitors = new List<int?>(count);
            for(int i = 0; i < count/2; i++)
            {
                sortedCompetitors.Add(competitors[i]);
                sortedCompetitors.Add(competitors[count - i - 1]);
            }

            return sortedCompetitors;
        }

        // https://www.hackerearth.com/practice/notes/round-a-number-to-the-next-power-of-2/
        private static int GetFullBracketCount(int count)
        {
            var nextNumber = Math.Ceiling(Math.Log(count, 2));
            var result = Math.Pow(2, nextNumber);
            return (int)result;
        }

        public Tournament CreateTournament(int competitorCount)
        {
            if(competitorCount < MinCompetitors)
            {
                throw new ArgumentException($"Need at least {MinCompetitors} competitors to generate bracket.", nameof(competitorCount));
            }

            var competitors = GetCompetitors(competitorCount);
            competitors = SortCompetitors(competitors);

            var tournament = new Tournament(competitors);

            CreateFirstRounds(tournament, competitors);
            CreateMiddleRounds(tournament);
            CreateGrandFinals(tournament);

            return tournament;
        }

        private static void CreateFirstRounds(Tournament tournament, IReadOnlyList<int?> competitors)
        {
            var winners = tournament.AddRound(tournament.Winners);
            for(var i = 0; i < competitors.Count; i += 2)
            {
                var isBye = competitors[i] == null || competitors[i + 1] == null;
                tournament.AddGame(winners, isBye);
            }

            var losers = tournament.AddRound(tournament.Losers);
            for(var i = 0; i < winners.Games.Count; i += 2)
            {
                var isBye = winners.Games[i].IsBye || winners.Games[i + 1].IsBye;
                var game = tournament.AddGame(losers, isBye);
                winners.Games[i].NextLoss = game;
                winners.Games[i + 1].NextLoss = game;
            }
        }

        private static void CreateMiddleRounds(Tournament tournament)
        {
            var lastWinnersRound = tournament.Winners.Rounds[0];
            while(lastWinnersRound.Games.Count > 1)
            {
                lastWinnersRound = CreateRoundGroup(tournament, lastWinnersRound);
            }
        }

        private static Round CreateRoundGroup(Tournament tournament, Round lastWinnersRound)
        {
            var winners = tournament.AddRound(tournament.Winners);
            for(var i = 0; i < lastWinnersRound.Games.Count; i += 2)
            {
                var game = tournament.AddGame(winners);
                lastWinnersRound.Games[i].NextWin = game;
                lastWinnersRound.Games[i + 1].NextWin = game;
            }

            var lastLosersRound = tournament.Losers.Rounds[tournament.Losers.Rounds.Count - 1];

            var losers = tournament.AddRound(tournament.Losers);
            for(var i = 0; i < lastLosersRound.Games.Count; i++)
            {
                var game = tournament.AddGame(losers);
                lastLosersRound.Games[i].NextWin = game;
                winners.Games[i].NextLoss = game;
            }

            if(losers.Games.Count > 1)
            {
                var secondLosers = tournament.AddRound(tournament.Losers);
                for(var i = 0; i < losers.Games.Count; i += 2)
                {
                    var game = tournament.AddGame(secondLosers);
                    losers.Games[i].NextWin = game;
                    losers.Games[i + 1].NextWin = game;
                }
            }

            return winners;
        }

        private static void CreateGrandFinals(Tournament tournament)
        {
            var lastWinners = tournament.Winners.Rounds[tournament.Winners.Rounds.Count - 1];
            var lastLosers = tournament.Losers.Rounds[tournament.Losers.Rounds.Count - 1];

            var grandFinals = tournament.AddRound(tournament.Winners);
            var firstGame = tournament.AddGame(grandFinals);
            lastWinners.Games[0].NextWin = firstGame;
            lastLosers.Games[0].NextWin = firstGame;

            var secondGame = tournament.AddGame(grandFinals);
            firstGame.NextWin = secondGame;
            firstGame.NextLoss = secondGame;
        }
    }
}