using System;
using Climb.Models;

namespace Climb.Responses.Sets
{
    public class SetRequestDto
    {
        public int ID { get; }
        public int LeagueID { get; }
        public int RequesterID { get; }
        public int ChallengedID { get; }
        public DateTime DateCreated { get; }
        public int? SetID { get; }
        public bool IsOpen { get; }
        public string Message { get; }

        public SetRequestDto(SetRequest setRequest)
        {
            ID = setRequest.ID;
            LeagueID = setRequest.LeagueID;
            RequesterID = setRequest.RequesterID;
            ChallengedID = setRequest.ChallengedID;
            DateCreated = setRequest.DateCreated;
            SetID = setRequest.SetID;
            IsOpen = setRequest.IsOpen;
            Message = setRequest.Message;
        }
    }
}