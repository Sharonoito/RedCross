using NuGet.Common;
using System.Collections.Generic;

namespace RedCrossChat
{
    public class DashboardReportVM
    {
        public int TotalVisits { get; set; } = 0;
        public int HandledByAgents { get; set; } = 0;
        public int UncompletedVisits { get; set; } = 0;
        public int AverageBotRating { get; set; } = 0;

        public int UserHandledItem { get; set; } = 0;

        public int HandOverRequest { get; set; } = 0;

        public Dictionary<string, int> items { get; set; }
    }
}
