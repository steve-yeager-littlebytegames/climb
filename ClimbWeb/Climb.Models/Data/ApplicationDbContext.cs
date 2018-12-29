using System.Linq;
using Climb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Climb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<LeagueUser> LeagueUsers { get; set; }
        public DbSet<SeasonLeagueUser> SeasonLeagueUsers { get; set; }
        public DbSet<Set> Sets { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<MatchCharacter> MatchCharacters { get; set; }
        public DbSet<RankSnapshot> RankSnapshots { get; set; }
        public DbSet<SetRequest> SetRequests { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationUser> OrganizationUsers { get; set; }

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach(var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            builder.Entity<OrganizationUser>().HasQueryFilter(ou => !ou.HasLeft);
            builder.Entity<LeagueUser>().HasQueryFilter(lu => !lu.HasLeft);
            builder.Entity<SeasonLeagueUser>().HasQueryFilter(slu => !slu.HasLeft);
            builder.Entity<MatchCharacter>().HasKey(m => new {m.MatchID, m.CharacterID, m.LeagueUserID});
            builder.Entity<SetRequest>().HasQueryFilter(lu => lu.IsOpen);
        }

        public void DetachEntries()
        {
            foreach(var entry in ChangeTracker.Entries().ToArray())
            {
                if(entry.Entity != null)
                {
                    entry.State = EntityState.Detached;
                }
            }
        }
    }
}