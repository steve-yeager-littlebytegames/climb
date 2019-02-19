﻿// <auto-generated />
using System;
using Climb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Climb.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190218174331_Rename_Set_SetSlotID")]
    partial class Rename_Set_SetSlotID
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Climb.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("ProfilePicKey");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Climb.Models.Character", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("GameID");

                    b.Property<string>("ImageKey");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.HasIndex("GameID");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Climb.Models.Game", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CharactersPerMatch");

                    b.Property<DateTime>("DateAdded");

                    b.Property<string>("LogoImageKey");

                    b.Property<string>("MatchName");

                    b.Property<int>("MaxMatchPoints");

                    b.Property<string>("Name");

                    b.Property<string>("ScoreName");

                    b.HasKey("ID");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Climb.Models.League", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ActiveSeasonID");

                    b.Property<string>("AdminID");

                    b.Property<DateTime>("DateCreated");

                    b.Property<int>("GameID");

                    b.Property<DateTime>("LastRankUpdate");

                    b.Property<string>("Name");

                    b.Property<int?>("OrganizationID");

                    b.Property<int>("SetsTillRank");

                    b.HasKey("ID");

                    b.HasIndex("ActiveSeasonID");

                    b.HasIndex("AdminID");

                    b.HasIndex("GameID");

                    b.HasIndex("OrganizationID");

                    b.ToTable("Leagues");
                });

            modelBuilder.Entity("Climb.Models.LeagueUser", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DisplayName");

                    b.Property<bool>("HasLeft");

                    b.Property<bool>("IsNewcomer");

                    b.Property<DateTime>("JoinDate");

                    b.Property<int>("LeagueID");

                    b.Property<int>("Points");

                    b.Property<int>("Rank");

                    b.Property<int>("RankTrend");

                    b.Property<int>("SetCount");

                    b.Property<int?>("TournamentID");

                    b.Property<string>("UserID")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.HasIndex("TournamentID");

                    b.HasIndex("UserID");

                    b.ToTable("LeagueUsers");
                });

            modelBuilder.Entity("Climb.Models.Match", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Index");

                    b.Property<int>("Player1Score");

                    b.Property<int>("Player2Score");

                    b.Property<int>("SetID");

                    b.Property<int?>("StageID");

                    b.HasKey("ID");

                    b.HasIndex("SetID");

                    b.HasIndex("StageID");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("Climb.Models.MatchCharacter", b =>
                {
                    b.Property<int>("MatchID");

                    b.Property<int>("CharacterID");

                    b.Property<int>("LeagueUserID");

                    b.Property<DateTime>("CreatedDate");

                    b.HasKey("MatchID", "CharacterID", "LeagueUserID");

                    b.HasIndex("CharacterID");

                    b.HasIndex("LeagueUserID");

                    b.ToTable("MatchCharacters");
                });

            modelBuilder.Entity("Climb.Models.Organization", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Name");

                    b.HasKey("ID");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("Climb.Models.OrganizationUser", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("HasLeft");

                    b.Property<bool>("IsOwner");

                    b.Property<DateTime>("JoinDate");

                    b.Property<int>("OrganizationID");

                    b.Property<string>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("OrganizationID");

                    b.HasIndex("UserID");

                    b.ToTable("OrganizationUsers");
                });

            modelBuilder.Entity("Climb.Models.RankSnapshot", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("DeltaPoints");

                    b.Property<int>("DeltaRank");

                    b.Property<int>("LeagueUserID");

                    b.Property<int>("Points");

                    b.Property<int>("Rank");

                    b.HasKey("ID");

                    b.HasIndex("LeagueUserID");

                    b.ToTable("RankSnapshots");
                });

            modelBuilder.Entity("Climb.Models.Round", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Bracket");

                    b.Property<int>("Index");

                    b.Property<string>("Name");

                    b.Property<int>("TournamentID");

                    b.HasKey("ID");

                    b.HasIndex("TournamentID");

                    b.ToTable("Rounds");
                });

            modelBuilder.Entity("Climb.Models.Season", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("EndDate");

                    b.Property<int>("Index");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsComplete");

                    b.Property<int>("LeagueID");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.ToTable("Seasons");
                });

            modelBuilder.Entity("Climb.Models.SeasonLeagueUser", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("HasLeft");

                    b.Property<int>("LeagueUserID");

                    b.Property<int>("Points");

                    b.Property<int>("SeasonID");

                    b.Property<int>("Standing");

                    b.Property<int>("TieBreakerPoints");

                    b.Property<string>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("LeagueUserID");

                    b.HasIndex("SeasonID");

                    b.HasIndex("UserID");

                    b.ToTable("SeasonLeagueUsers");
                });

            modelBuilder.Entity("Climb.Models.Set", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DueDate");

                    b.Property<bool>("IsComplete");

                    b.Property<bool>("IsForfeit");

                    b.Property<bool>("IsLocked");

                    b.Property<int>("LeagueID");

                    b.Property<int>("Player1ID");

                    b.Property<int?>("Player1Score");

                    b.Property<int>("Player1SeasonPoints");

                    b.Property<int>("Player2ID");

                    b.Property<int?>("Player2Score");

                    b.Property<int>("Player2SeasonPoints");

                    b.Property<int?>("SeasonID");

                    b.Property<int?>("SeasonPlayer1ID");

                    b.Property<int?>("SeasonPlayer2ID");

                    b.Property<int?>("SetSlotID");

                    b.Property<int?>("TournamentID");

                    b.Property<int?>("TournamentPlayer1ID");

                    b.Property<int?>("TournamentPlayer2ID");

                    b.Property<int?>("TournamentUser1ID");

                    b.Property<int?>("TournamentUser2ID");

                    b.Property<DateTime?>("UpdatedDate");

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.HasIndex("Player1ID");

                    b.HasIndex("Player2ID");

                    b.HasIndex("SeasonID");

                    b.HasIndex("SeasonPlayer1ID");

                    b.HasIndex("SeasonPlayer2ID");

                    b.HasIndex("SetSlotID")
                        .IsUnique()
                        .HasFilter("[SetSlotID] IS NOT NULL");

                    b.HasIndex("TournamentID");

                    b.HasIndex("TournamentPlayer1ID");

                    b.HasIndex("TournamentPlayer2ID");

                    b.ToTable("Sets");
                });

            modelBuilder.Entity("Climb.Models.SetRequest", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ChallengedID");

                    b.Property<DateTime>("DateCreated");

                    b.Property<bool>("IsOpen");

                    b.Property<int>("LeagueID");

                    b.Property<string>("Message");

                    b.Property<int>("RequesterID");

                    b.Property<int?>("SetID");

                    b.HasKey("ID");

                    b.HasIndex("ChallengedID");

                    b.HasIndex("LeagueID");

                    b.HasIndex("RequesterID");

                    b.HasIndex("SetID");

                    b.ToTable("SetRequests");
                });

            modelBuilder.Entity("Climb.Models.SetSlot", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Identifier");

                    b.Property<int?>("LoseSlotIdentifier");

                    b.Property<int?>("P1Game");

                    b.Property<int?>("P2Game");

                    b.Property<int>("RoundID");

                    b.Property<int>("TournamentID");

                    b.Property<int?>("User1ID");

                    b.Property<int?>("User2ID");

                    b.Property<int?>("WinSlotIdentifier");

                    b.HasKey("ID");

                    b.HasIndex("RoundID");

                    b.HasIndex("TournamentID");

                    b.HasIndex("User1ID");

                    b.HasIndex("User2ID");

                    b.ToTable("SetSlots");
                });

            modelBuilder.Entity("Climb.Models.Stage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("GameID");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("ID");

                    b.HasIndex("GameID");

                    b.ToTable("Stages");
                });

            modelBuilder.Entity("Climb.Models.Tournament", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreateDate");

                    b.Property<DateTime?>("EndDate");

                    b.Property<int>("LeagueID");

                    b.Property<string>("Name");

                    b.Property<int?>("SeasonID");

                    b.Property<DateTime?>("StartDate");

                    b.Property<int>("State");

                    b.HasKey("ID");

                    b.HasIndex("LeagueID");

                    b.HasIndex("SeasonID");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("Climb.Models.TournamentUser", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LeagueUserID");

                    b.Property<int?>("SeasonLeagueUserID");

                    b.Property<int>("Seed");

                    b.Property<int>("TournamentID");

                    b.Property<string>("UserID");

                    b.HasKey("ID");

                    b.HasIndex("LeagueUserID");

                    b.HasIndex("SeasonLeagueUserID");

                    b.HasIndex("TournamentID");

                    b.HasIndex("UserID");

                    b.ToTable("TournamentUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Climb.Models.Character", b =>
                {
                    b.HasOne("Climb.Models.Game", "Game")
                        .WithMany("Characters")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.League", b =>
                {
                    b.HasOne("Climb.Models.Season", "ActiveSeason")
                        .WithMany()
                        .HasForeignKey("ActiveSeasonID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.ApplicationUser", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Game", "Game")
                        .WithMany("Leagues")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Organization", "Organization")
                        .WithMany("Leagues")
                        .HasForeignKey("OrganizationID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.LeagueUser", b =>
                {
                    b.HasOne("Climb.Models.League", "League")
                        .WithMany("Members")
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Tournament")
                        .WithMany("LeagueUsers")
                        .HasForeignKey("TournamentID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.ApplicationUser", "User")
                        .WithMany("LeagueUsers")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Match", b =>
                {
                    b.HasOne("Climb.Models.Set", "Set")
                        .WithMany("Matches")
                        .HasForeignKey("SetID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Stage", "Stage")
                        .WithMany()
                        .HasForeignKey("StageID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.MatchCharacter", b =>
                {
                    b.HasOne("Climb.Models.Character", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.LeagueUser", "LeagueUser")
                        .WithMany()
                        .HasForeignKey("LeagueUserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Match", "Match")
                        .WithMany("MatchCharacters")
                        .HasForeignKey("MatchID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.OrganizationUser", b =>
                {
                    b.HasOne("Climb.Models.Organization", "Organization")
                        .WithMany("Members")
                        .HasForeignKey("OrganizationID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.ApplicationUser", "User")
                        .WithMany("Organizations")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.RankSnapshot", b =>
                {
                    b.HasOne("Climb.Models.LeagueUser", "LeagueUser")
                        .WithMany("RankSnapshots")
                        .HasForeignKey("LeagueUserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Round", b =>
                {
                    b.HasOne("Climb.Models.Tournament", "Tournament")
                        .WithMany("Rounds")
                        .HasForeignKey("TournamentID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Season", b =>
                {
                    b.HasOne("Climb.Models.League", "League")
                        .WithMany("Seasons")
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.SeasonLeagueUser", b =>
                {
                    b.HasOne("Climb.Models.LeagueUser", "LeagueUser")
                        .WithMany("Seasons")
                        .HasForeignKey("LeagueUserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Season", "Season")
                        .WithMany("Participants")
                        .HasForeignKey("SeasonID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.ApplicationUser", "User")
                        .WithMany("Seasons")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Set", b =>
                {
                    b.HasOne("Climb.Models.League", "League")
                        .WithMany("Sets")
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.LeagueUser", "Player1")
                        .WithMany("P1Sets")
                        .HasForeignKey("Player1ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.LeagueUser", "Player2")
                        .WithMany("P2Sets")
                        .HasForeignKey("Player2ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Season", "Season")
                        .WithMany("Sets")
                        .HasForeignKey("SeasonID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.SeasonLeagueUser", "SeasonPlayer1")
                        .WithMany("P1Sets")
                        .HasForeignKey("SeasonPlayer1ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.SeasonLeagueUser", "SeasonPlayer2")
                        .WithMany("P2Sets")
                        .HasForeignKey("SeasonPlayer2ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.SetSlot", "SetSlot")
                        .WithOne("Set")
                        .HasForeignKey("Climb.Models.Set", "SetSlotID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Tournament", "Tournament")
                        .WithMany("Sets")
                        .HasForeignKey("TournamentID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.TournamentUser", "TournamentPlayer1")
                        .WithMany()
                        .HasForeignKey("TournamentPlayer1ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.TournamentUser", "TournamentPlayer2")
                        .WithMany()
                        .HasForeignKey("TournamentPlayer2ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.SetRequest", b =>
                {
                    b.HasOne("Climb.Models.LeagueUser", "Challenged")
                        .WithMany()
                        .HasForeignKey("ChallengedID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.League", "League")
                        .WithMany()
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.LeagueUser", "Requester")
                        .WithMany()
                        .HasForeignKey("RequesterID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Set", "Set")
                        .WithMany()
                        .HasForeignKey("SetID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.SetSlot", b =>
                {
                    b.HasOne("Climb.Models.Round", "Round")
                        .WithMany("SetSlots")
                        .HasForeignKey("RoundID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Tournament", "Tournament")
                        .WithMany("SetSlots")
                        .HasForeignKey("TournamentID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.TournamentUser", "User1")
                        .WithMany()
                        .HasForeignKey("User1ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.TournamentUser", "User2")
                        .WithMany()
                        .HasForeignKey("User2ID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Stage", b =>
                {
                    b.HasOne("Climb.Models.Game", "Game")
                        .WithMany("Stages")
                        .HasForeignKey("GameID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.Tournament", b =>
                {
                    b.HasOne("Climb.Models.League", "League")
                        .WithMany()
                        .HasForeignKey("LeagueID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Season", "Season")
                        .WithMany()
                        .HasForeignKey("SeasonID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Climb.Models.TournamentUser", b =>
                {
                    b.HasOne("Climb.Models.LeagueUser", "LeagueUser")
                        .WithMany()
                        .HasForeignKey("LeagueUserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.SeasonLeagueUser", "SeasonLeagueUser")
                        .WithMany()
                        .HasForeignKey("SeasonLeagueUserID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.Tournament", "Tournament")
                        .WithMany("TournamentUsers")
                        .HasForeignKey("TournamentID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Climb.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Climb.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Climb.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Climb.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
