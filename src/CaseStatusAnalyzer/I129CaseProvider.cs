using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class I129CaseProvider : II129CaseProvider
    {
        private readonly IOptions<I129CaseProviderOptions> _optionsAccessor;
        private readonly ICaseProvider _caseProvider;
        private readonly ICrawlingStateProvider _crawlingStateProvider;

        public I129CaseProvider (ICaseProvider caseProvider, ICrawlingStateProvider crawlingStateProvider, IOptions<I129CaseProviderOptions> optionsAccessor)
        {
            if (caseProvider == null)
                throw new ArgumentNullException(nameof(caseProvider));

            if (crawlingStateProvider == null)
                throw new ArgumentNullException(nameof(crawlingStateProvider));

            if (optionsAccessor == null)
                throw new ArgumentNullException(nameof(optionsAccessor));

            _caseProvider = caseProvider;
            _crawlingStateProvider = crawlingStateProvider;
            _optionsAccessor = optionsAccessor;
        }

        public IEnumerable<Case> GetCasesForFiscalYear()
        {
            var options = _optionsAccessor.Value;
            var serviceCenter = options.ServiceCenter;
            var fiscalYear = options.FiscalYear;
            var startDayIncl = options.StartDayIncl; // default -> 127
            var endDayExcl = options.EndDayExcl; // default -> 148

            Console.WriteLine($"Starting processing service center {serviceCenter}, fiscal year 20{fiscalYear}");
            for (var workday = startDayIncl; workday < endDayExcl; workday++)
            {
                Console.WriteLine($"Starting processing day {workday}");
                int counter = 0;
                var cases = _caseProvider.GetCasesForDay(serviceCenter, fiscalYear, workday);
                foreach (var potentialCase in cases)
                {
                    if (potentialCase.Text.Contains("I-129"))
                    {
                        Console.WriteLine($"Found {++counter}-th case for I-129");
                        yield return potentialCase;
                    }
                    else
                    {
                        Console.WriteLine("Skipping non-I-129 case");
                    }
                }
            }
        }
    }
}
