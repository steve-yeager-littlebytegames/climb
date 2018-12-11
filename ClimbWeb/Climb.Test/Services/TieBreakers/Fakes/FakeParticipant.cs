namespace Climb.Core.TieBreakers.Test
{
    public class FakeParticipant : IParticipant
    {
        public int ID { get; }
        public int Points { get; }
        public int TieBreakerPoints { get; set; }

        public FakeParticipant()
        {
            ID = 0;
            Points = 0;
        }

        public FakeParticipant(int id, int points)
        {
            ID = id;
            Points = points;
        }
    }
}