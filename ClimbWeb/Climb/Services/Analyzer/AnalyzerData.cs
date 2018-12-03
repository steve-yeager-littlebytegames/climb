namespace Climb.Services
{
    public abstract class AnalyzerData
    {
        public string Name { get; }

        protected AnalyzerData(string name)
        {
            Name = name;
        }
    }
}