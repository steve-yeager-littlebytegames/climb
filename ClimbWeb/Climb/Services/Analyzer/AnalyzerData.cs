namespace Climb.Services
{
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