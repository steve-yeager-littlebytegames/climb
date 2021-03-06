﻿using System.Collections.Generic;
using Climb.Models;

namespace Climb.ViewModels.Site
{
    public class SearchViewModel : BaseViewModel
    {
        public string Search { get; }
        public IReadOnlyList<Game> GameResults { get; }
        public IReadOnlyList<League> LeagueResults { get; }
        public IReadOnlyList<ApplicationUser> UserResults { get; }

        public bool EmptySearch => string.IsNullOrWhiteSpace(Search);
        public int ResultCount => GameResults.Count + LeagueResults.Count + UserResults.Count;
        public bool NoResults => ResultCount == 0;

        public SearchViewModel(ApplicationUser user)
            : base(user)
        {
            Search = string.Empty;
            GameResults = null;
            LeagueResults = null;
            UserResults = null;
        }

        public SearchViewModel(ApplicationUser user, string search, IReadOnlyList<Game> gameResults, IReadOnlyList<League> leagueResults, IReadOnlyList<ApplicationUser> userResults)
            : base(user)
        {
            Search = search;
            GameResults = gameResults;
            LeagueResults = leagueResults;
            UserResults = userResults;
        }
    }
}