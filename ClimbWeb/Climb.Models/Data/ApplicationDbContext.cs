using System.Linq;
using System.Threading.Tasks;
using Climb.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Climb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        [UsedImplicitly]
        public DbSet<Character> Characters { get; private set; }
        [UsedImplicitly]
        public DbSet<Game> Games { get; private set; }
        [UsedImplicitly]
        public DbSet<League> Leagues { get;  private set; }
        [UsedImplicitly]
        public DbSet<LeagueUser> LeagueUsers { get;  private set; }
        [UsedImplicitly]
        public DbSet<Match> Matches { get;  private set; }
        [UsedImplicitly]
        public DbSet<MatchCharacter> MatchCharacters { get;  private set; }
        [UsedImplicitly]
        public DbSet<Organization> Organizations { get;  private set; }
        [UsedImplicitly]
        public DbSet<OrganizationUser> OrganizationUsers { get;  private set; }
        [UsedImplicitly]
        public DbSet<RankSnapshot> RankSnapshots { get;  private set; }
        [UsedImplicitly]
        public DbSet<Round> Rounds { get;  private set; }
        [UsedImplicitly]
        public DbSet<SeasonLeagueUser> SeasonLeagueUsers { get;  private set; }
        [UsedImplicitly]
        public DbSet<Season> Seasons { get;  private set; }
        [UsedImplicitly]
        public DbSet<SetRequest> SetRequests { get;  private set; }
        [UsedImplicitly]
        public DbSet<Set> Sets { get;  private set; }
        [UsedImplicitly]
        public DbSet<SetSlot> SetSlots { get;  private set; }
        [UsedImplicitly]
        public DbSet<Stage> Stages { get;  private set; }
        [UsedImplicitly]
        public DbSet<Tournament> Tournaments { get;  private set; }
        [UsedImplicitly]
        public DbSet<TournamentUser> TournamentUsers { get;  private set; }

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // TODO: Delete?
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

        public void Clean()
        {
            foreach(var entry in ChangeTracker.Entries().ToArray())
            {
                entry.State = EntityState.Detached;
            }
        }

        public async Task AddAndSaveAsync(object entity)
        {
            Add(entity);
            await SaveChangesAsync();
        }
    }
}