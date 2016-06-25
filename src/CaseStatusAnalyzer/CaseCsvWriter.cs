using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class CaseCsvWriter: ICaseWriter
    {
        private readonly IOptions<CaseCsvWriterOptions> _optionsAccessor;

        public CaseCsvWriter(IOptions<CaseCsvWriterOptions> optionsAccessor)
        {
            _optionsAccessor = optionsAccessor;
        }

        public void Write(IEnumerable<Case> cases)
        {
            var options = _optionsAccessor.Value;
            var fileName = string.Format(options.OutputFileNameFormat, DateTime.UtcNow);
            if (File.Exists(fileName))
            {
                var backupFileName = $"{fileName}.{Guid.NewGuid()}.bak";
                File.Move(fileName, backupFileName);
            }

            using (var writer = File.CreateText(fileName))
            {
                writer.WriteLine("Date,ReceiptNum,Status");
                writer.Flush();
                foreach (var currentCase in cases)
                {
                    writer.WriteLine(string.Format("{0},\"{1}\",\"{2}\""));
                }
                writer.Flush();
            }
        }
    }
}
