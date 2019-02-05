using System.Collections.Generic;

namespace Climb.Requests.Game
{
    public class AddCharacters
    {
        public int GameID { get; }
        public List<string> Names { get; }

        public AddCharacters(int gameID, List<string> names)
        {
            GameID = gameID;
            Names = names;
        }
    }
}