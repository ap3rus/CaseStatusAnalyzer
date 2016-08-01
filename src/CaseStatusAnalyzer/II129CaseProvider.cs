using System.Collections.Generic;

namespace CaseStatusAnalyzer
{
    public interface II129CaseProvider
    {
        IEnumerable<Case> GetCasesForFiscalYear();
    }
}