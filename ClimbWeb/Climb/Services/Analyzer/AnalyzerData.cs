namespace Climb.Services
{
<<<<<<< master
    public class PlayerData<T> : AnalyzerData
    {
        public T Player1Data { get; set; }
        public T Player2Data { get; set; }

        public PlayerData(string name)
            : base(name)
        {
        }
    }

=======
>>>>>>> Setup class and start API tests
    public abstract class AnalyzerData
    {
        public string Name { get; }

<<<<<<< master
        public AnalyzerData()
        {
        }

=======
>>>>>>> Setup class and start API tests
        protected AnalyzerData(string name)
        {
            Name = name;
        }
    }
}