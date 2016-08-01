using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class CaseParser : ICaseParser
    {
        private readonly IOptions<CaseParserOptions> _optionsAccessor;

        public CaseParser(IOptions<CaseParserOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
                throw new ArgumentNullException(nameof(optionsAccessor));

            _optionsAccessor = optionsAccessor;

            // this is to tell HtmlAgilityPack to keep hierarchy of contents of <form> tags exactly as it is in source html
            // for reference see issue http://htmlagilitypack.codeplex.com/workitem/23074
            HtmlAgilityPack.HtmlNode.ElementsFlags.Remove("form");
        }

        public int ExtractWorkDay(string receiptNum)
        {
            if (receiptNum == null)
                throw new ArgumentNullException(nameof(receiptNum));

            var match = Regex.Match(receiptNum, _optionsAccessor.Value.CaseReceiptNumRegex);
            if (!match.Success)
                throw new InvalidOperationException("Could not parse receipt number.");

            return int.Parse(match.Groups["WorkDay"].Value);
        }

        public bool TryParseOne(string inputHtml, out Case result)
        {
            if (inputHtml == null)
                throw new ArgumentNullException(nameof(inputHtml));

            var options = _optionsAccessor.Value;
            result = null;

            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            //htmlDoc.OptionFixNestedTags = true;
            htmlDoc.LoadHtml(inputHtml);
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Any() || htmlDoc.DocumentNode == null)
            {
                return false;
            }

            var caseDetailsNode = htmlDoc.DocumentNode.SelectSingleNode(options.CaseDetailsXPath);
            if (caseDetailsNode == null)
            {
                return false;
            }

            var caseStatus = caseDetailsNode.SelectSingleNode(options.CaseStatusXPath)?.InnerText;
            var caseText = caseDetailsNode.SelectSingleNode(options.CaseTextXPath)?.InnerText;
            if (string.IsNullOrWhiteSpace(caseStatus) || string.IsNullOrWhiteSpace(caseText))
            {
                return false;
            }

            DateTime lastUpdated;
            TryParseLastUpdated(caseText, out lastUpdated);

            result = new Case
            {
                Status = caseStatus,
                Text = caseText,
                LastUpdated = lastUpdated
            };
            return true;
        }

        private static bool TryParseLastUpdated(string caseText, out DateTime result)
        {
            int commaCounter = 0;
            for (var i = 0; i < caseText.Length; i++)
            {
                if (caseText[i] == ',')
                {
                    ++commaCounter;
                    if (commaCounter == 2)
                    {
                        return DateTime.TryParse(caseText.Substring(3, i - 3), out result);
                    }
                }
            }

            result = default(DateTime);
            return false;
        }
    }
}
