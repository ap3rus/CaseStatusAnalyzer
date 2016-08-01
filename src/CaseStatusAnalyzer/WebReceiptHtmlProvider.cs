using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class WebReceiptHtmlProvider : IReceiptHtmlProvider
    {
        private IOptions<ReceiptHtmlProviderOptions> _optionsAccessor;

        public WebReceiptHtmlProvider(IOptions<ReceiptHtmlProviderOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
                throw new ArgumentNullException(nameof(optionsAccessor));

            _optionsAccessor = optionsAccessor;
        }

        public string GetReceiptHtml(string receiptNum)
        {
            if (receiptNum == null)
                throw new ArgumentNullException(nameof(receiptNum));

            var options = _optionsAccessor.Value;
            var url = string.Format(options.ReceiptUrlFormat, receiptNum);

            using (var client = new HttpClient())
            {
                var getStringTask = Task.Run(async () =>
                {
                    return await client.GetStringAsync(url);
                });

                return getStringTask.Result;
            }
        }
    }
}
