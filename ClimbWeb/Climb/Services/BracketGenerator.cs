using System;
using System.Collections.Generic;
using Climb.Models;

namespace Climb.Services
{
    public class BracketGenerator : IBracketGenerator
    {
        private class Game
        {
            public bool IsBye { get; }
            public Game NextWin { get; set; }
            public Game NextLoss { get; set; }

            public Game(bool isBye)
            {
                IsBye = isBye;
            }
        }

        private class Round
        {
            public int Index { get; }
            public List<Game> Games { get; } = new List<Game>();

            public Round(int index)
            {
                Index = index;
            }
        }

        private class Bracket
        {
            public bool IsLosers { get; }
            public List<Round> Rounds { get; } = new List<Round>();

            public Bracket(bool isLosers)
            {
                IsLosers = isLosers;
            }
        }

        private class Tournament
        {
            public Bracket Winners { get; } = new Bracket(false);
            public Bracket Losers { get; } = new Bracket(true);
        }

        public int MinCompetitors => 4;

        public List<SetSlot> Generate(int competitorCount)
        {
            if(competitorCount < MinCompetitors)
            {
                throw new ArgumentException($"Need at least {MinCompetitors} competitors to generate bracket.", nameof(competitorCount));
            }

            var competitors = GetCompetitors(competitorCount);

            var tournament = new Tournament();
            CreateWinners(competitors, tournament);
            CreateLosers(tournament);

            // grand finals
            // reset


            // make slots
            // remove null slots

            return new List<SetSlot>();
        }

        private void CreateWinners(IReadOnlyList<int?> competitors, Tournament tournament)
        {
            var round = new Round(0);
            tournament.Winners.Rounds.Add(round);
            for(int i = 0; i < competitors.Count; i+=2)
            {
                var isBye = competitors[i] == null || competitors[i + 1] == null;
                var game = new Game(isBye);
                round.Games.Add(game);
            }

            while(round.Games.Count > 1)
            {
                round = CreateRound(round);
                tournament.Winners.Rounds.Add(round);
            }
        }

        private void CreateLosers(Tournament tournament)
        {
            var round = new Round(0);
            tournament.Losers.Rounds.Add(round);
            for(int i = 0; i < tournament.Winners.Rounds[0].Games.Count; i+=2)
            {
                // TODO: There will be byes.
                var game = new Game(false);
                round.Games.Add(game);
                tournament.Winners.Rounds[0].Games[i].NextLoss = game;
                tournament.Winners.Rounds[0].Games[i + 1].NextLoss = game;
            }
        }

        private Round CreateRound(Round previousRound)
        {
            var round = new Round(previousRound.Index + 1);
            for(int i = 0; i < previousRound.Games.Count; i+=2)
            {
                var game = new Game(false);
                round.Games.Add(game);
                previousRound.Games[i].NextWin = game;
                previousRound.Games[i+1].NextWin = game;
            }

            return round;
        }

        private static IReadOnlyList<int?> GetCompetitors(int count)
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

        // https://www.hackerearth.com/practice/notes/round-a-number-to-the-next-power-of-2/
        private static int GetFullBracketCount(int count)
        {
            var nextNumber = Math.Ceiling(Math.Log(count, 2));
            var result = Math.Pow(2, nextNumber);
            return (int)result;
        }
    }
}