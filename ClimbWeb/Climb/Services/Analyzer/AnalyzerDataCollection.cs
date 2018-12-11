using System;
using System.Collections.Generic;

namespace Climb.Services
{
    public class AnalyzerDataCollection
    {
        public int Player1ID { get; }
        public int Player2ID { get; }
        public DateTime CreatedDate { get; }
        public List<AnalyzerData> Data { get; } = new List<AnalyzerData>();

        public AnalyzerDataCollection(int player1ID, int player2ID, DateTime createdDate)
        {
            Player1ID = player1ID;
            Player2ID = player2ID;
            CreatedDate = createdDate;
        }
    }
}