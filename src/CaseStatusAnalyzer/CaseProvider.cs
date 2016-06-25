using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class CaseProvider : ICaseProvider
    {
        private readonly ICaseParser _caseParser;
        private IOptions<CaseProviderOptions> _optionsAccessor;
        private readonly IReceiptHtmlProvider _receiptHtmlProvider;

        public CaseProvider(IReceiptHtmlProvider receiptHtmlProvider, ICaseParser caseParser, IOptions<CaseProviderOptions> optionsAccessor)
        {
            _receiptHtmlProvider = receiptHtmlProvider;
            _caseParser = caseParser;
            _optionsAccessor = optionsAccessor;
        }

        public IEnumerable<Case> GetCasesForDay(string serviceCenter, int fiscalYear, int workday)
        {
            var options = _optionsAccessor.Value;
            int caseNumber = 0;
            int failureStreak = 0;

            while (true)
            {
                ++caseNumber;

                var receiptNum = string.Format(options.ReceiptNumFormat, serviceCenter, fiscalYear, workday, caseNumber);
                var receiptHtml = _receiptHtmlProvider.GetReceiptHtml(receiptNum);

                Case currentCase;
                if (_caseParser.TryParseOne(receiptHtml, out currentCase))
                {
                    yield return currentCase;
                }
                else if (++failureStreak > options.MaximumFailureStreakForDay)
                {
                    break;
                }

                if (options.SleepAfterGetOneMilliseconds > 0)
                {
                    Thread.Sleep(options.SleepAfterGetOneMilliseconds);
                }
            }
        }
    }
}
