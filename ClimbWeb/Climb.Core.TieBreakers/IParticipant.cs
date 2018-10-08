namespace Climb.Core.TieBreakers
{
    public interface IParticipant
    {
        int ID { get; }
        int Points { get; }
        int TieBreakerPoints { get; set; }
    }
}