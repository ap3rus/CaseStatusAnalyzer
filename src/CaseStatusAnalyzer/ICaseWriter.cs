using System.Collections.Generic;

namespace CaseStatusAnalyzer
{
    public interface ICaseWriter
    {
        void Write(IEnumerable<Case> cases);
    }
}