using System.Collections.Generic;

namespace Climb.Services
{
    public class AnalyzerData
    {
        public string Title { get; }
        public List<string> Player1Data { get; set; } = new List<string>();
        public List<string> Player2Data { get; set; } = new List<string>();

        public AnalyzerData(string title)
        {
            Title = title;
        }
    }
}