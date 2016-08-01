using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class Case
    {
        public string ReceiptNum { get; set; }
        public string Status { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public int Workday { get; set; }
    }
}
