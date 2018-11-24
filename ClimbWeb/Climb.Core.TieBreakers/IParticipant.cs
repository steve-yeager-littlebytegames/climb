namespace Climb.Core.TieBreakers
{
    public interface IParticipant
    {
        int ID { get; }
        int TieBreakerPoints { get; set; }
    }
}