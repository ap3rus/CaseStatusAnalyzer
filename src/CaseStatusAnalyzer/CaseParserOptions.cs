using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class CaseParserOptions
    {
        public string CaseDetailsXPath { get; set; }
        public string CaseStatusXPath { get; set; }
        public string CaseTextXPath { get; set; }
        public string CaseReceiptNumRegex { get; set; }
    }
}
