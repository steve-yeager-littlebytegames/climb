using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Climb.Models;
using ClimbV1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Character = Climb.Models.Character;
using Game = Climb.Models.Game;
using League = Climb.Models.League;
using LeagueUser = Climb.Models.LeagueUser;
using Match = Climb.Models.Match;
using MatchCharacter = Climb.Models.MatchCharacter;
using RankSnapshot = Climb.Models.RankSnapshot;
using Season = Climb.Models.Season;
using Set = Climb.Models.Set;
using Stage = Climb.Models.Stage;

namespace Climb.Data
{
    public static class DataMigrator
    {
        private const int SetsTillNotNewcomer = 4;
        private static readonly Dictionary<string, (string id, string name)> applicationUserIDs = new Dictionary<string, (string id, string name)>();
        private static readonly Dictionary<int, int> gameIDs = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> characterIDs = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> leagueIDs = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> leagueUserIDs = new Dictionary<int, int>();
        private static readonly Dictionary<string, int> seasonLeagueUserIDs = new Dictionary<string, int>();
        private static readonly Dictionary<int, int> seasonIDs = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> setIDs = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> matchIDs = new Dictionary<int, int>();

        public static async Task MigrateV1(ApplicationDbContext context, UserManager<ApplicationUser> userManager, string connectionString)
        {
            await ResetDatabase(context);

            var v1Context = CreateDB(connectionString);

            await MigrateUsers(v1Context, context, userManager);
            await MigrateGames(v1Context, context);
            await MigrateCharacters(v1Context, context);
            await MigrateStages(v1Context, context);
            await MigrateLeagues(v1Context, context);
            await MigrateSeasons(v1Context, context);
            await MigrateLeagueUsers(v1Context, context);
            await MigrateSeasonLeagueUsers(v1Context, context);
            await MigrateSets(v1Context, context);
            await MigrateMatches(v1Context, context);
            await MigrateMatchCharacters(v1Context, context);
            await MigrateRankSnapshots(v1Context, context);
        }

        private static async Task ResetDatabase(ApplicationDbContext context)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();
        }

        private static ClimbV1Context CreateDB(string connectionString)
        {
            var options = new DbContextOptionsBuilder<ClimbV1Context>()
                .UseSqlServer(connectionString)
                .Options;
            var context = new ClimbV1Context(options);
            return context;
        }

        private static async Task MigrateUsers(ClimbV1Context v1Context, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            var v1Users = await v1Context.User
                .Include(u => u.ApplicationUser).AsNoTracking()
                .ToArrayAsync();
            var users = new ApplicationUser[v1Users.Length];

            for(var i = 0; i < v1Users.Length; i++)
            {
                var oldUser = v1Users[i];
                users[i] = new ApplicationUser
                {
                    Id = oldUser.ApplicationUser.Id,
                    Email = oldUser.ApplicationUser.Email,
                    NormalizedEmail = oldUser.ApplicationUser.NormalizedEmail,
                    UserName = oldUser.Username,
                    NormalizedUserName = userManager.KeyNormalizer.Normalize(oldUser.Username),
                    PasswordHash = oldUser.ApplicationUser.PasswordHash,
                    ConcurrencyStamp = oldUser.ApplicationUser.ConcurrencyStamp,
                    SecurityStamp = oldUser.ApplicationUser.SecurityStamp,
                };
            }

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            for(var i = 0; i < users.Length; i++)
            {
                applicationUserIDs[v1Users[i].ApplicationUser.Id] = (users[i].Id, users[i].UserName);
            }
        }

        private static async Task MigrateGames(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldGames = await v1Context.Game.ToArrayAsync();
            var games = new Game[oldGames.Length];

            for(var i = 0; i < oldGames.Length; i++)
            {
                var v1Game = oldGames[i];
                games[i] = new Game
                {
                    Name = v1Game.Name,
                    CharactersPerMatch = v1Game.CharactersPerMatch,
                    MaxMatchPoints = v1Game.MaxMatchPoints,
                    DateAdded = DateTime.Today,
                    HasStages = v1Game.RequireStage,
                };
            }

            context.Games.AddRange(games);
            await context.SaveChangesAsync();

            for(var i = 0; i < oldGames.Length; i++)
            {
                gameIDs[oldGames[i].ID] = games[i].ID;
            }
        }

        private static async Task MigrateCharacters(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldCharacters = await v1Context.Character.ToArrayAsync();
            var characters = new Character[oldCharacters.Length];

            for(var i = 0; i < oldCharacters.Length; i++)
            {
                var v1Character = oldCharacters[i];
                characters[i] = new Character
                {
                    Name = v1Character.Name,
                    GameID = gameIDs[v1Character.GameID],
                };
            }

            context.Characters.AddRange(characters);
            await context.SaveChangesAsync();

            for(var i = 0; i < oldCharacters.Length; i++)
            {
                characterIDs[oldCharacters[i].ID] = characters[i].ID;
            }
        }

        private static async Task MigrateStages(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldStages = await v1Context.Stage.ToArrayAsync();
            var stages = new Stage[oldStages.Length];

            for(var i = 0; i < oldStages.Length; i++)
            {
                var v1Stage = oldStages[i];
                stages[i] = new Stage
                {
                    Name = v1Stage.Name,
                    GameID = gameIDs[v1Stage.GameID],
                };
            }

            context.Stages.AddRange(stages);
            await context.SaveChangesAsync();
        }

        private static async Task MigrateLeagues(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldLeagues = await v1Context.League
                .Include(l => l.Admin).ThenInclude(u => u.ApplicationUser).AsNoTracking()
                .ToArrayAsync();
            var leagues = new League[oldLeagues.Length];

            for(var i = 0; i < oldLeagues.Length; i++)
            {
                var oldLeague = oldLeagues[i];
                leagues[i] = new League
                {
                    GameID = gameIDs[oldLeague.GameID],
                    AdminID = oldLeague.Admin.ApplicationUser.Id,
                    Name = oldLeague.Name,
                    DateCreated = DateTime.Today,
                    SetsTillRank = SetsTillNotNewcomer,
                };
            }

            context.Leagues.AddRange(leagues);
            await context.SaveChangesAsync();

            for(var i = 0; i < oldLeagues.Length; i++)
            {
                leagueIDs[oldLeagues[i].ID] = leagues[i].ID;
            }
        }

        private static async Task MigrateSeasons(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldSeasons = await v1Context.Season.ToArrayAsync();
            var seasons = new Season[oldSeasons.Length];

            for(var i = 0; i < oldSeasons.Length; i++)
            {
                var oldSeason = oldSeasons[i];
                seasons[i] = new Season
                {
                    LeagueID = leagueIDs[oldSeason.LeagueID],
                    StartDate = oldSeason.StartDate,
                    EndDate = oldSeason.StartDate + TimeSpan.FromDays(30),
                    Index = oldSeason.Index,
                    IsActive = !oldSeason.IsComplete,
                };
            }

            context.Seasons.AddRange(seasons);
            await context.SaveChangesAsync();

            for(var i = 0; i < oldSeasons.Length; i++)
            {
                seasonIDs[oldSeasons[i].ID] = seasons[i].ID;
            }
        }

        private static async Task MigrateLeagueUsers(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldLeagueUsers = await v1Context.LeagueUser
                .Include(lu => lu.User).ThenInclude(u => u.ApplicationUser).AsNoTracking()
                .ToArrayAsync();
            var leagueUsers = new LeagueUser[oldLeagueUsers.Length];

            for(var i = 0; i < oldLeagueUsers.Length; i++)
            {
                var oldLeagueUser = oldLeagueUsers[i];
                var (oldUserID, oldUsername) = applicationUserIDs[oldLeagueUser.User.ApplicationUser.Id];

                leagueUsers[i] = new LeagueUser
                {
                    LeagueID = leagueIDs[oldLeagueUser.LeagueID],
                    UserID = oldUserID,
                    DisplayName = oldUsername,
                    HasLeft = oldLeagueUser.HasLeft,
                    Rank = oldLeagueUser.Rank,
                    Points = oldLeagueUser.Points,
                    SetCount = oldLeagueUser.SetsPlayed,
                    IsNewcomer = oldLeagueUser.SetsPlayed < SetsTillNotNewcomer, 
                };
            }

            context.LeagueUsers.AddRange(leagueUsers);
            await context.SaveChangesAsync();

            for(var i = 0; i < oldLeagueUsers.Length; i++)
            {
                leagueUserIDs[oldLeagueUsers[i].ID] = leagueUsers[i].ID;
            }
        }

        private static async Task MigrateSeasonLeagueUsers(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldLeagueUserSeasons = await v1Context.LeagueUserSeason.ToArrayAsync();
            var seasonLeagueUsers = new SeasonLeagueUser[oldLeagueUserSeasons.Length];

            for(var i = 0; i < oldLeagueUserSeasons.Length; i++)
            {
                var oldLeagueUserSeason = oldLeagueUserSeasons[i];
                seasonLeagueUsers[i] = new SeasonLeagueUser
                {
                    LeagueUserID = leagueUserIDs[oldLeagueUserSeason.LeagueUserID],
                    SeasonID = seasonIDs[oldLeagueUserSeason.SeasonID],
                    Standing = oldLeagueUserSeason.Standing,
                    Points = oldLeagueUserSeason.Points,
                };
            }

            context.SeasonLeagueUsers.AddRange(seasonLeagueUsers);
            await context.SaveChangesAsync();

            for(var i = 0; i < oldLeagueUserSeasons.Length; i++)
            {
                var compositeID = $"{oldLeagueUserSeasons[i].LeagueUserID}-{oldLeagueUserSeasons[i].SeasonID}";
                seasonLeagueUserIDs[compositeID] = seasonLeagueUsers[i].ID;
            }
        }

        private static async Task MigrateSets(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldSets = await v1Context.Set.ToArrayAsync();
            var sets = new List<Set>(oldSets.Length);
            var setIDsDict = new Dictionary<Set, int>();

            foreach(var oldSet in oldSets)
            {
                if(oldSet.Player1Score == null && oldSet.Player2Score == null)
                {
                    continue;
                }

                Debug.Assert(oldSet.Player1ID != null, "oldSet.Player1ID != null");
                Debug.Assert(oldSet.Player2ID != null, "oldSet.Player2ID != null");
                var isComplete = oldSet.Player1Score != null;
                var player1Won = oldSet.Player1Score > oldSet.Player2Score;
                var player2Won = oldSet.Player2Score > oldSet.Player1Score;

                var newSet = new Set
                {
                    LeagueID = leagueIDs[oldSet.LeagueID],
                    DueDate = oldSet.DueDate,
                    UpdatedDate = oldSet.UpdatedDate,
                    IsLocked = oldSet.IsLocked,
                    Player1ID = leagueUserIDs[oldSet.Player1ID.Value],
                    Player2ID = leagueUserIDs[oldSet.Player2ID.Value],
                    Player1Score = oldSet.Player1Score,
                    Player2Score = oldSet.Player2Score,
                    IsComplete = isComplete,
                    Player1SeasonPoints = isComplete ? player1Won ? 2 : 1 : 0,
                    Player2SeasonPoints = isComplete ? player2Won ? 2 : 1 : 0,
                };

                if(oldSet.SeasonID != null)
                {
                    newSet.SeasonID = oldSet.SeasonID != null ? seasonIDs[oldSet.SeasonID.Value] : (int?)null;
                    newSet.SeasonPlayer1ID = seasonLeagueUserIDs[$"{oldSet.Player1ID.Value}-{oldSet.SeasonID}"];
                    newSet.SeasonPlayer2ID = seasonLeagueUserIDs[$"{oldSet.Player2ID.Value}-{oldSet.SeasonID}"];
                }

                setIDsDict.Add(newSet, oldSet.ID);
                sets.Add(newSet);
            }

            context.Sets.AddRange(sets);
            await context.SaveChangesAsync();

            foreach(var set in setIDsDict)
            {
                setIDs[set.Value] = set.Key.ID;
            }
        }

        private static async Task MigrateMatches(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldMatches = await v1Context.Match
                .Include(m => m.Set).AsNoTracking()
                .ToArrayAsync();
            var matches = new List<Match>(oldMatches.Length);
            var matchIDsDict = new Dictionary<Match, int>();

            foreach(var oldMatch in oldMatches)
            {
                if(oldMatch.Player1Score == 0 && oldMatch.Player2Score == 0)
                {
                    continue;
                }

                var newMatch = new Match
                {
                    Index = oldMatch.Index,
                    SetID = setIDs[oldMatch.Set.ID],
                    Player1Score = oldMatch.Player1Score,
                    Player2Score = oldMatch.Player2Score,
                    StageID = oldMatch.StageID,
                };

                matches.Add(newMatch);
                matchIDsDict.Add(newMatch, oldMatch.ID);
            }

            context.Matches.AddRange(matches);
            await context.SaveChangesAsync();

            foreach(var match in matchIDsDict)
            {
                matchIDs[match.Value] = match.Key.ID;
            }
        }

        private static async Task MigrateMatchCharacters(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldMatchCharacters = await v1Context.MatchCharacters.ToArrayAsync();
            var matchCharacters = new List<MatchCharacter>(oldMatchCharacters.Length);

            for(var i = 0; i < oldMatchCharacters.Length; i++)
            {
                var oldMatchCharacter = oldMatchCharacters[i];
                if(!matchIDs.ContainsKey(oldMatchCharacter.MatchID))
                {
                    continue;
                }

                matchCharacters.Add(new MatchCharacter
                {
                    MatchID = matchIDs[oldMatchCharacter.MatchID],
                    LeagueUserID = leagueUserIDs[oldMatchCharacter.LeagueUserID],
                    CharacterID = characterIDs[oldMatchCharacter.CharacterID],
                });
            }

            context.MatchCharacters.AddRange(matchCharacters);
            await context.SaveChangesAsync();
        }

        private static async Task MigrateRankSnapshots(ClimbV1Context v1Context, ApplicationDbContext context)
        {
            var oldRankSnapshots = await v1Context.RankSnapshot.ToArrayAsync();
            var rankSnapshots = new RankSnapshot[oldRankSnapshots.Length];

            for(var i = 0; i < oldRankSnapshots.Length; i++)
            {
                var oldRankSnapshot = oldRankSnapshots[i];
                rankSnapshots[i] = new RankSnapshot
                {
                    CreatedDate = oldRankSnapshot.CreatedDate,
                    LeagueUserID = leagueUserIDs[oldRankSnapshot.LeagueUserID],
                    Points = oldRankSnapshot.Points,
                    Rank = oldRankSnapshot.Rank,
                    DeltaPoints = oldRankSnapshot.DeltaPoints,
                    DeltaRank = oldRankSnapshot.DeltaRank,
                };
            }

            context.RankSnapshots.AddRange(rankSnapshots);
            await context.SaveChangesAsync();
        }
    }
}