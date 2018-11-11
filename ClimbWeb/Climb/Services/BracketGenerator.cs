using System;
using System.Collections.Generic;
using System.Diagnostics;
using Climb.Models;

namespace Climb.Services
{
    public class BracketGenerator : IBracketGenerator
    {
        public class Game
        {
            public int ID { get; }
            public bool IsBye { get; }
            public int? P1 { get; private set; }
            public int? P2 { get; private set; }
            public Game NextWin { get; set; }
            public Game NextLoss { get; set; }
            public Game P1Game { get; set; }
            public Game P2Game { get; set; }
            public int? P1Score { get; set; }
            public int? P2Score { get; set; }

            public Game(int id, bool isBye, int? p1, int? p2)
            {
                ID = id;
                IsBye = isBye;
                P1 = p1;
                P2 = p2;
            }

            public override string ToString() => $"{ID} W={NextWin?.ID} L={NextLoss?.ID} B={IsBye}";

            public void AddPlayer(int? player)
            {
                Debug.Assert(P1 == null || P2 == null);

                if(P1 == null)
                {
                    P1 = player;
                }
                else
                {
                    P2 = player;
                }
            }
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
            public Round GrandFinals { get; set; }

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

            public Game AddGame(Round round, int? p1 = null, int? p2 = null, bool isBye = false)
            {
                ++GameCount;
                var game = new Game(GameCount, isBye, p1, p2);
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
                tournament.AddGame(winners, competitors[i], competitors[i + 1], isBye);
            }

            var losers = tournament.AddRound(tournament.Losers);
            for(var i = 0; i < winners.Games.Count; i += 2)
            {
                var isBye = winners.Games[i].IsBye || winners.Games[i + 1].IsBye;
                var game = tournament.AddGame(losers, null, null, isBye);
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
                game.P1Game = lastWinnersRound.Games[i];
                game.P2Game = lastWinnersRound.Games[i + 1];
                game.P1Game.NextWin = game;
                game.P2Game.NextWin = game;
            }

            var lastLosersRound = tournament.Losers.Rounds[tournament.Losers.Rounds.Count - 1];

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

        private static void CreateGrandFinals(Tournament tournament)
        {
            var lastWinners = tournament.Winners.Rounds[tournament.Winners.Rounds.Count - 1];
            var lastLosers = tournament.Losers.Rounds[tournament.Losers.Rounds.Count - 1];

            tournament.GrandFinals = new Round(++tournament.RoundCount);
            var firstGame = tournament.AddGame(tournament.GrandFinals);
            lastWinners.Games[0].NextWin = firstGame;
            lastLosers.Games[0].NextWin = firstGame;

            var secondGame = tournament.AddGame(tournament.GrandFinals);
            firstGame.NextWin = secondGame;
            firstGame.NextLoss = secondGame;
        }
    }
}