namespace Climb.Services
{
<<<<<<< HEAD
<<<<<<< master
<<<<<<< master
=======
>>>>>>> Send data to WPF app
=======
>>>>>>> data-analyzer
    public class PlayerData<T> : AnalyzerData
    {
        public T Player1Data { get; set; }
        public T Player2Data { get; set; }

        public PlayerData(string name)
            : base(name)
        {
        }
    }

<<<<<<< HEAD
<<<<<<< master
=======
>>>>>>> Setup class and start API tests
=======
>>>>>>> Send data to WPF app
=======
>>>>>>> data-analyzer
    public abstract class AnalyzerData
    {
        public string Name { get; }

<<<<<<< HEAD
<<<<<<< master
<<<<<<< master
=======
>>>>>>> Implementing AnalyzerService
=======
>>>>>>> data-analyzer
        public AnalyzerData()
        {
        }

<<<<<<< HEAD
<<<<<<< master
=======
>>>>>>> Setup class and start API tests
=======
>>>>>>> Implementing AnalyzerService
=======
>>>>>>> data-analyzer
        protected AnalyzerData(string name)
        {
            Name = name;
        }
    }
}