using System;
using System.Collections.Generic;

namespace Climb.Services
{
    public partial class BracketGenerator : IBracketGenerator
    {
        public int MinCompetitors => 4;

        public BracketData Generate(int competitorCount)
        {
            if(competitorCount < MinCompetitors)
            {
                throw new ArgumentException($"Need at least {MinCompetitors} competitors to generate bracket.", nameof(competitorCount));
            }

            var competitors = GetCompetitors(competitorCount);
            competitors = SortCompetitors(competitors);

            var tournament = new BracketData();

            CreateFirstRounds(tournament, competitors);
            CreateMiddleRounds(tournament);
            CreateGrandFinals(tournament);
            NameRounds(tournament);

            return tournament;
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
            for(var i = 0; i < count / 2; i++)
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

        private static void CreateFirstRounds(BracketData bracket, IReadOnlyList<int?> competitors)
        {
            var winners = bracket.AddRound(bracket.Winners);
            for(var i = 0; i < competitors.Count; i += 2)
            {
                var isBye = competitors[i] == null || competitors[i + 1] == null;
                bracket.AddGame(winners, competitors[i], competitors[i + 1], isBye);
            }

            var losers = bracket.AddRound(bracket.Losers);
            for(var i = 0; i < winners.Games.Count; i += 2)
            {
                var isBye = winners.Games[i].IsBye || winners.Games[i + 1].IsBye;
                var game = bracket.AddGame(losers, null, null, isBye);
                game.P1Game = winners.Games[i];
                game.P2Game = winners.Games[i + 1];
                game.P1Game.NextLoss = game;
                game.P2Game.NextLoss = game;
            }
        }

        private static void CreateMiddleRounds(BracketData bracket)
        {
            var lastWinnersRound = bracket.Winners[0];
            while(lastWinnersRound.Games.Count > 1)
            {
                lastWinnersRound = CreateRoundGroup(bracket, lastWinnersRound);
            }
        }

        private static RoundData CreateRoundGroup(BracketData bracket, RoundData lastWinnersRound)
        {
            var winners = bracket.AddRound(bracket.Winners);
            for(var i = 0; i < lastWinnersRound.Games.Count; i += 2)
            {
                var game = bracket.AddGame(winners);
                game.P1Game = lastWinnersRound.Games[i];
                game.P2Game = lastWinnersRound.Games[i + 1];
                game.P1Game.NextWin = game;
                game.P2Game.NextWin = game;
            }

            var lastLosersRound = bracket.Losers[bracket.Losers.Count - 1];

            var losers = bracket.AddRound(bracket.Losers);
            for(var i = 0; i < lastLosersRound.Games.Count; i++)
            {
                var game = bracket.AddGame(losers);
                game.P1Game = lastLosersRound.Games[i];
                game.P2Game = winners.Games[i];
                game.P1Game.NextWin = game;
                game.P2Game.NextLoss = game;
            }

            if(losers.Games.Count > 1)
            {
                var secondLosers = bracket.AddRound(bracket.Losers);
                for(var i = 0; i < losers.Games.Count; i++)
                {
                    var game = bracket.AddGame(secondLosers);
                    game.P1Game = losers.Games[i];
                    game.P2Game = losers.Games[++i];
                    game.P1Game.NextWin = game;
                    game.P2Game.NextWin = game;
                }
            }

            return winners;
        }

        private static void CreateGrandFinals(BracketData bracket)
        {
            var lastWinners = bracket.Winners[bracket.Winners.Count - 1];
            var lastLosers = bracket.Losers[bracket.Losers.Count - 1];

            bracket.GrandFinals = new List<RoundData>(2) {new RoundData(++bracket.RoundCount), new RoundData(++bracket.RoundCount)};

            var firstGame = bracket.AddGame(bracket.GrandFinals[0]);
            lastWinners.Games[0].NextWin = firstGame;
            lastLosers.Games[0].NextWin = firstGame;

            var secondGame = bracket.AddGame(bracket.GrandFinals[1]);
            secondGame.P1Game = firstGame;
            secondGame.P2Game = firstGame;
            firstGame.NextWin = secondGame;
            firstGame.NextLoss = secondGame;
        }

        private static void NameRounds(BracketData bracket)
        {
            bracket.GrandFinals[0].Name = "Grand Finals";
            bracket.GrandFinals[1].Name = "Grand Finals Reset";

            var winnersCount = bracket.Winners.Count;
            var namedRounds = 0;

            bracket.Winners[winnersCount - 1].Name = "Winners Finals";
            ++namedRounds;

            if(winnersCount > 3)
            {
                bracket.Winners[winnersCount - 2].Name = "Winners Semi-Finals";
                ++namedRounds;
            }

            for(var i = winnersCount - namedRounds - 1; i >= 0; i--)
            {
                bracket.Winners[i].Name = $"Winners {i + 1}";
            }

            var losersCount = bracket.Losers.Count;
            namedRounds = 0;

            bracket.Losers[losersCount - 1].Name = "Losers Finals";
            ++namedRounds;

            if(losersCount > 3)
            {
                bracket.Losers[losersCount - 2].Name = "Losers Semi-Finals";
                ++namedRounds;
            }

            for(var i = losersCount - namedRounds - 1; i >= 0; i--)
            {
                bracket.Losers[i].Name = $"Losers {i + 1}";
            }
        }
    }
}