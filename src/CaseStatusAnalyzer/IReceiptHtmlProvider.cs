using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public interface IReceiptHtmlProvider
    {
        string GetReceiptHtml(string receiptNum);
    }
}
