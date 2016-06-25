using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class I129CaseProvider : II129CaseProvider
    {
        private readonly ICaseProvider _caseProvider;

        public I129CaseProvider (ICaseProvider caseProvider)
        {
            _caseProvider = caseProvider;
        }

        public IEnumerable<Case> GetCasesForFiscalYear(string serviceCenter, int fiscalYear)
        {
            var startDayIncl = 127;
            var endDayExcl = 148;
            for (var workday = startDayIncl; workday < endDayExcl; workday++)
            {
                var cases = _caseProvider.GetCasesForDay(serviceCenter, fiscalYear, workday);
                foreach (var potentialCase in cases)
                {
                    if (potentialCase.Text.Contains("I-129"))
                    {
                        yield return potentialCase;
                    }
                }
            }
        }
    }
}
