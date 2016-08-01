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
            if (optionsAccessor == null)
                throw new ArgumentNullException(nameof(optionsAccessor));

            _optionsAccessor = optionsAccessor;
        }

        public void Write(IEnumerable<Case> cases)
        {
            if (cases == null)
                throw new ArgumentNullException(nameof(cases));

            var options = _optionsAccessor.Value;
            var fileName = string.Format(options.OutputFileNameFormat, DateTime.UtcNow);
            var fileInfo = new FileInfo(fileName);

            if (!fileInfo.Directory.Exists)
                Directory.CreateDirectory(fileInfo.DirectoryName);

            if (fileInfo.Exists)
            {
                var backupFileName = $"{fileName}.{Guid.NewGuid()}.bak";
                File.Move(fileName, backupFileName);
            }

            using (var writer = File.CreateText(fileName))
            {
                writer.WriteLine("Workday,Date,ReceiptNum,Status");
                writer.Flush();
                foreach (var currentCase in cases)
                {
                    writer.WriteLine($"{currentCase.Workday},{currentCase.LastUpdated},\"{currentCase.ReceiptNum}\",\"{currentCase.Status}\"");
                    writer.Flush(); // todo move outside of loop after debugging
                }
            }
        }
    }
}
