using System;
using System.Collections.Generic;

namespace Climb.Services
{
    public partial class BracketGenerator : IBracketGenerator
    {
        public int MinCompetitors => 4;

        public TournamentData Generate(int competitorCount)
        {
            if(competitorCount < MinCompetitors)
            {
                throw new ArgumentException($"Need at least {MinCompetitors} competitors to generate bracket.", nameof(competitorCount));
            }

            var competitors = GetCompetitors(competitorCount);
            competitors = SortCompetitors(competitors);

            var tournament = new TournamentData();

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

        private static void CreateFirstRounds(TournamentData tournament, IReadOnlyList<int?> competitors)
        {
            var winners = tournament.AddRound(tournament.Winners);
            for(var i = 0; i < competitors.Count; i += 2)
            {
                var isBye = competitors[i] == null || competitors[i + 1] == null;
                tournament.AddGame(winners, competitors[i], competitors[i + 1], isBye);
            }

            var losers = tournament.AddRound(tournament.Losers);
            for(var i = 0; i < winners.Games.Count; i += 2)
            {
                var isBye = winners.Games[i].IsBye || winners.Games[i + 1].IsBye;
                var game = tournament.AddGame(losers, null, null, isBye);
                game.P1Game = winners.Games[i];
                game.P2Game = winners.Games[i + 1];
                game.P1Game.NextLoss = game;
                game.P2Game.NextLoss = game;
            }
        }

        private static void CreateMiddleRounds(TournamentData tournament)
        {
            var lastWinnersRound = tournament.Winners[0];
            while(lastWinnersRound.Games.Count > 1)
            {
                lastWinnersRound = CreateRoundGroup(tournament, lastWinnersRound);
            }
        }

        private static RoundData CreateRoundGroup(TournamentData tournament, RoundData lastWinnersRound)
        {
            var winners = tournament.AddRound(tournament.Winners);
            for(var i = 0; i < lastWinnersRound.Games.Count; i += 2)
            {
                var game = tournament.AddGame(winners);
                game.P1Game = lastWinnersRound.Games[i];
                game.P2Game = lastWinnersRound.Games[i + 1];
                game.P1Game.NextWin = game;
                game.P2Game.NextWin = game;
            }

            var lastLosersRound = tournament.Losers[tournament.Losers.Count - 1];

            var losers = tournament.AddRound(tournament.Losers);
            for(var i = 0; i < lastLosersRound.Games.Count; i++)
            {
                var game = tournament.AddGame(losers);
                game.P1Game = lastLosersRound.Games[i];
                game.P2Game = winners.Games[i];
                game.P1Game.NextWin = game;
                game.P2Game.NextLoss = game;
            }

            if(losers.Games.Count > 1)
            {
                var secondLosers = tournament.AddRound(tournament.Losers);
                for(var i = 0; i < losers.Games.Count; i++)
                {
                    var game = tournament.AddGame(secondLosers);
                    game.P1Game = losers.Games[i];
                    game.P2Game = losers.Games[++i];
                    game.P1Game.NextWin = game;
                    game.P2Game.NextWin = game;
                }
            }

            return winners;
        }

        private static void CreateGrandFinals(TournamentData tournament)
        {
            var lastWinners = tournament.Winners[tournament.Winners.Count - 1];
            var lastLosers = tournament.Losers[tournament.Losers.Count - 1];

            tournament.GrandFinals = new List<RoundData>(2) {new RoundData(++tournament.RoundCount), new RoundData(++tournament.RoundCount)};

            var firstGame = tournament.AddGame(tournament.GrandFinals[0]);
            lastWinners.Games[0].NextWin = firstGame;
            lastLosers.Games[0].NextWin = firstGame;

            var secondGame = tournament.AddGame(tournament.GrandFinals[1]);
            secondGame.P1Game = firstGame;
            secondGame.P2Game = firstGame;
            firstGame.NextWin = secondGame;
            firstGame.NextLoss = secondGame;
        }

        private static void NameRounds(TournamentData tournament)
        {
            tournament.GrandFinals[0].Name = "Grand Finals";
            tournament.GrandFinals[1].Name = "Grand Finals Reset";

            var winnersCount = tournament.Winners.Count;
            var namedRounds = 0;

            tournament.Winners[winnersCount - 1].Name = "Winners Finals";
            ++namedRounds;

            if(winnersCount > 3)
            {
                tournament.Winners[winnersCount - 2].Name = "Winners Semi-Finals";
                ++namedRounds;
            }

            for(var i = winnersCount - namedRounds - 1; i >= 0; i--)
            {
                tournament.Winners[i].Name = $"Winners {i + 1}";
            }

            var losersCount = tournament.Losers.Count;
            namedRounds = 0;

            tournament.Losers[losersCount - 1].Name = "Losers Finals";
            ++namedRounds;

            if(losersCount > 3)
            {
                tournament.Losers[losersCount - 2].Name = "Losers Semi-Finals";
                ++namedRounds;
            }

            for(var i = losersCount - namedRounds - 1; i >= 0; i--)
            {
                tournament.Losers[i].Name = $"Losers {i + 1}";
            }
        }
    }
}