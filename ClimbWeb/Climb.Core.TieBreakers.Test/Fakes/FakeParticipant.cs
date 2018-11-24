namespace Climb.Core.TieBreakers.Test
{
    public class FakeParticipant : IParticipant
    {
        public int ID { get; }
        public int TieBreakerPoints { get; set; }

        public FakeParticipant(int id)
        {
            ID = id;
        }
    }
}