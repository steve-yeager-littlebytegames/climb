namespace Climb.Requests.Games
{
    public class CharacterAddRequest
    {
        public CharacterAddRequest()
        {
        }

        public CharacterAddRequest(int gameID, int? characterID)
        {
            GameID = gameID;
            CharacterID = characterID;
        }

        public int GameID { get; set; }
        public int? CharacterID { get; set; }
    }
}