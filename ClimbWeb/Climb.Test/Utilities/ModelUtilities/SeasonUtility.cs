using System;
using System.Collections.Generic;
using Climb.Data;
using Climb.Models;

namespace Climb.Test.Utilities
{
    public static class SeasonUtility
    {
        public static (Season season, List<LeagueUser> members) CreateSeason(ApplicationDbContext dbContext, int participants, Action<Season> preprocess = null)
        {
            var league = LeagueUtility.CreateLeague(dbContext);
            var members = LeagueUtility.AddUsersToLeague(league, participants, dbContext);

            preprocess += s => s.LeagueID = league.ID;
            var season = DbContextUtility.AddNew(dbContext, preprocess);
            DbContextUtility.AddNewRange<SeasonLeagueUser>(dbContext, participants, (slu, i) =>
            {
                slu.LeagueUserID = members[i].ID;
                slu.SeasonID = season.ID;
                slu.UserID = members[i].UserID;
            });

            return (season, members);
        }

        public static List<Set> CreateSets(ApplicationDbContext dbContext, Season season)
        {
            var sets = new List<Set>();
            var participants = season.Participants.ToArray();
            for(var i = 1; i < participants.Length; i++)
            {
                var set = SetUtility.Create(dbContext, participants[0], participants[i], season.LeagueID);
                sets.Add(set);
            }

            return sets;
        }

        public static List<SeasonLeagueUser> AddParticipants(ApplicationDbContext dbContext, Season season, params LeagueUser[] members)
        {
            return DbContextUtility.AddNewRange<SeasonLeagueUser>(dbContext, members.Length, (slu, i) =>
            {
                slu.LeagueUserID = members[i].ID;
                slu.SeasonID = season.ID;
                slu.UserID = members[i].UserID;
            });
        }
    }
}