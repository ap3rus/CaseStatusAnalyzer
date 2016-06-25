using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class CaseProviderOptions
    {
        public string ReceiptNumFormat { get; set; }
        public int MaximumFailureStreakForDay { get; set; }
        public int SleepAfterGetOneMilliseconds { get; set; }
    }
}
