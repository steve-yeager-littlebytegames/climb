using System.Collections.Generic;
using System.IO;
using System.Linq;
using Climb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Climb.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext dbContext, IHostingEnvironment environment)
        {
            if(environment.IsProduction())
            {
                return;
            }

            if(!dbContext.Users.Any())
            {
                CreateTestData(dbContext);
            }
        }

        private static void CreateTestData(ApplicationDbContext dbContext)
        {
            var games = LoadFromFile(dbContext, dbContext.Games, "Games");
            LoadFromFile(dbContext, dbContext.Characters, "Characters");
            LoadFromFile(dbContext, dbContext.Stages, "Stages");

            var users = CreateTestUsers(dbContext, 8);
            var league = CreateTestLeague(dbContext, games[0], users[0]);
            CreateTestMembers(dbContext, league, users);
        }

        private static List<T> LoadFromFile<T>(DbContext context, DbSet<T> set, string filePath) where T : class
        {
            var data = File.ReadAllText($@".\Data\SeedData\{filePath}.json");
            var models = JsonConvert.DeserializeObject<List<T>>(data);
            set.AddRange(models);
            context.SaveChanges();
            return models;
        }

        private static League CreateTestLeague(ApplicationDbContext dbContext, Game game, ApplicationUser admin)
        {
            var league = new League(game.ID, "Fun Smash Friends", admin.Id);
            dbContext.Leagues.Add(league);
            dbContext.SaveChanges();
            return league;
        }

        private static List<ApplicationUser> CreateTestUsers(ApplicationDbContext dbContext, int count)
        {
            var users = new List<ApplicationUser>();
            for(var i = 0; i < count; i++)
            {
                users.Add(new ApplicationUser{UserName = $"User_{i}"});
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