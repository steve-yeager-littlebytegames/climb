namespace Climb.Services
{
    public class PlayerData<T> : AnalyzerData
    {
        public T Player1Data { get; set; }
        public T Player2Data { get; set; }

        public PlayerData(string name)
            : base(name)
        {
        }
    }

    public abstract class AnalyzerData
    {
        public string Name { get; }

        public AnalyzerData()
        {
        }

        protected AnalyzerData(string name)
        {
            Name = name;
        }
    }
}