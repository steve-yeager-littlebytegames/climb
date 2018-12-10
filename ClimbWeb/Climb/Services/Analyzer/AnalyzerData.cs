namespace Climb.Services
{
<<<<<<< master
<<<<<<< master
=======
>>>>>>> Send data to WPF app
    public class PlayerData<T> : AnalyzerData
    {
        public T Player1Data { get; set; }
        public T Player2Data { get; set; }

        public PlayerData(string name)
            : base(name)
        {
        }
    }

<<<<<<< master
=======
>>>>>>> Setup class and start API tests
=======
>>>>>>> Send data to WPF app
    public abstract class AnalyzerData
    {
        public string Name { get; }

<<<<<<< master
<<<<<<< master
=======
>>>>>>> Implementing AnalyzerService
        public AnalyzerData()
        {
        }

<<<<<<< master
=======
>>>>>>> Setup class and start API tests
=======
>>>>>>> Implementing AnalyzerService
        protected AnalyzerData(string name)
        {
            Name = name;
        }
    }
}