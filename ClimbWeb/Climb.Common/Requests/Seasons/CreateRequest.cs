using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Climb.Requests.Seasons
{
    public class CreateRequest
    {
        [Required]
        [HiddenInput]
        public int LeagueID { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public CreateRequest()
        {
        }

        public CreateRequest(int leagueID, DateTime start, DateTime end)
        {
            LeagueID = leagueID;
            StartDate = start;
            EndDate = end;
        }
    }
}