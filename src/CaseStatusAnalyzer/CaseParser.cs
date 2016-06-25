using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class CaseParser : ICaseParser
    {
        private readonly IOptions<CaseParserOptions> _optionsAccessor;

        public CaseParser(IOptions<CaseParserOptions> optionsAccessor)
        {
            _optionsAccessor = optionsAccessor;

            // this is to tell HtmlAgilityPack to keep hierarchy of contents of <form> tags exactly as it is in source html
            // for reference see issue http://htmlagilitypack.codeplex.com/workitem/23074
            HtmlAgilityPack.HtmlNode.ElementsFlags.Remove("form");
        }

        public bool TryParseOne(string inputHtml, out Case result)
        {
            var options = _optionsAccessor.Value;
            result = null;

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
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

            result = new Case
            {
                Status = caseStatus,
                Text = caseText
            };
            return true;
        }
    }
}
