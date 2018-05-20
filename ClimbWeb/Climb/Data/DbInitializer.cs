﻿using System.Collections.Generic;
using System.Linq;
using Climb.Models;

namespace Climb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext dbContext)
        {
            if(!dbContext.Users.Any())
            {
                CreateTestData(dbContext);
            }
        }

        private static void CreateTestData(ApplicationDbContext dbContext)
        {
            var game = CreateTestGame(dbContext);
            var league = CreateTestLeague(dbContext, game);
            var users = CreateTestUsers(dbContext, 8);
            CreateTestMembers(dbContext, league, users);
        }

        private static Game CreateTestGame(ApplicationDbContext dbContext)
        {
            var game = new Game("Smash Bros");
            dbContext.Games.Add(game);
            dbContext.SaveChanges();
            return game;
        }

        private static League CreateTestLeague(ApplicationDbContext dbContext, Game game)
        {
            var league = new League(game.ID, "Fun Smash Friends");
            dbContext.Leagues.Add(league);
            dbContext.SaveChanges();
            return league;
        }

        private static ICollection<ApplicationUser> CreateTestUsers(ApplicationDbContext dbContext, int count)
        {
            var users = new List<ApplicationUser>();
            for(var i = 0; i < count; i++)
            {
                users.Add(new ApplicationUser());
            }

            dbContext.Users.AddRange(users);
            dbContext.SaveChanges();
            return users;
        }

        private static void CreateTestMembers(ApplicationDbContext dbContext, League league, ICollection<ApplicationUser> users)
        {
            var leagueUsers = new List<LeagueUser>();
            foreach(var user in users)
            {
                leagueUsers.Add(new LeagueUser(league.ID, user.Id));
            }

            dbContext.LeagueUsers.AddRange(leagueUsers);
            dbContext.SaveChanges();
        }
    }
}