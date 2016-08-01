using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            if (receiptHtmlProvider == null)
                throw new ArgumentNullException(nameof(receiptHtmlProvider));

            if (caseParser == null)
                throw new ArgumentNullException(nameof(caseParser));

            if (optionsAccessor == null)
                throw new ArgumentNullException(nameof(optionsAccessor));

            _receiptHtmlProvider = receiptHtmlProvider;
            _caseParser = caseParser;
            _optionsAccessor = optionsAccessor;
        }

        public IEnumerable<Case> GetCasesForDay(string serviceCenter, int fiscalYear, int workday)
        {
            if (serviceCenter == null)
                throw new ArgumentNullException(nameof(serviceCenter));

            if (fiscalYear > 99 || fiscalYear < 0)
                throw new ArgumentOutOfRangeException(nameof(fiscalYear), fiscalYear, "Fiscal year must be an integer in range from 0 to 99 inclusive on both ends.");

            if (workday < 1 || workday > 262)
                throw new ArgumentOutOfRangeException(nameof(workday), workday, "Work day must be an integer in range from 1 to 261 inclusive on both ends.");

            var options = _optionsAccessor.Value;
            int caseNumber = 0;
            int failureStreak = 0;

            while (true)
            {
                ++caseNumber;

                var receiptNum = string.Format(options.ReceiptNumFormat, serviceCenter, fiscalYear, workday, caseNumber);
                string receiptHtml;
                try
                {
                    receiptHtml = _receiptHtmlProvider.GetReceiptHtml(receiptNum);
                }
                catch(Exception ex)
                {
                    Debugger.Break();
                    throw;
                }

                Case currentCase;
                if (_caseParser.TryParseOne(receiptHtml, out currentCase))
                {
                    currentCase.ReceiptNum = receiptNum;
                    currentCase.Workday = workday;
                    yield return currentCase;
                }
                else 
                {
                    Console.WriteLine($"Could not parse case {receiptNum}");
                    if (++failureStreak > options.MaximumFailureStreakForDay)
                    {
                        break;
                    }
                }

                if (options.SleepAfterGetOneMilliseconds > 0)
                {
                    Thread.Sleep(options.SleepAfterGetOneMilliseconds);
                }
            }
        }
    }
}
