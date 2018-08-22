﻿using System.ComponentModel.DataAnnotations;
using Climb.Models;
using Climb.Services;

namespace Climb.Responses.Models
{
    public class LeagueUserDto
    {
        public int ID { get; }
        public int LeagueID { get; }
        [Required]
        public string UserID { get; }
        public bool HasLeft { get; }
        [Required]
        public string Username { get; }
        public int Points { get; }
        public int Rank { get; }
        public string ProfilePicture { get; }

        public LeagueUserDto(LeagueUser leagueUser, ICdnService cdnService)
        {
            ID = leagueUser.ID;
            LeagueID = leagueUser.ID;
            UserID = leagueUser.UserID;
            HasLeft = leagueUser.HasLeft;
            Username = leagueUser.User.UserName;
            Points = leagueUser.Points;
            Rank = leagueUser.Rank;

            ProfilePicture = cdnService.GetUserProfilePicUrl(leagueUser.UserID, leagueUser.User.ProfilePicKey, ClimbImageRules.ProfilePic);
        }
    }
}