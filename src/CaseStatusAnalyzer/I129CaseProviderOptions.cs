using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class I129CaseProviderOptions
    {
        public int StartDayIncl { get; set; }
        public int EndDayExcl { get; set; }
        public string ServiceCenter { get; set; }
        public int FiscalYear { get; set; }
    }
}
