using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CaseStatusAnalyzer;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CaseStatusJson.Controllers
{
    [Route("api/[controller]")]
    public class MyCaseStatusController : Controller
    {
        private readonly IReceiptHtmlProvider _receiptHtmlProvider;
        private readonly ICaseParser _caseParser;

        public MyCaseStatusController(ICaseParser caseParser, IReceiptHtmlProvider receiptHtmlProvider)
        {
            _caseParser = caseParser;
            _receiptHtmlProvider = receiptHtmlProvider;
        }

        [HttpGet("{receiptNum}")]
        public Case Get(string receiptNum)
        {
            string receiptHtml;
            try
            {
                receiptHtml = _receiptHtmlProvider.GetReceiptHtml(receiptNum);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error getting receipt HTML for receipt `{receiptNum}`, see inner exception for details.", ex);
            }

            Case currentCase;
            if (!_caseParser.TryParseOne(receiptHtml, out currentCase))
            {
                throw new InvalidOperationException($"Could not parse receipt HTML for receipt `{receiptNum}`.");
            }

            currentCase.ReceiptNum = receiptNum;
            currentCase.Workday = _caseParser.ExtractWorkDay(receiptNum);
            return currentCase;
        }
    }
}
