namespace Climb.Responses.Models
{
    public class StageDto
    {
        public StageDto(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public int ID { get; }
        public string Name { get; }
    }
}