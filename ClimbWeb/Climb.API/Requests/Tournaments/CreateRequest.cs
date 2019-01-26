namespace Climb.Requests.Tournaments
{
    public class CreateRequest
    {
        public int LeagueID { get; set; }
        public int? SeasonID { get; set; }
        public string Name { get; set; }

        public CreateRequest()
        {
        }

        public CreateRequest(int leagueID, int? seasonID, string name)
        {
            LeagueID = leagueID;
            SeasonID = seasonID;
            Name = name;
        }
    }
}