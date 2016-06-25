using System.Collections.Generic;

namespace CaseStatusAnalyzer
{
    public interface ICaseProvider
    {
        IEnumerable<Case> GetCasesForDay(string serviceCenter, int fiscalYear, int workday);
    }
}