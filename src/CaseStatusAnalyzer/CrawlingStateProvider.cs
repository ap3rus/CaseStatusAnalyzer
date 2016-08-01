using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class CrawlingStateProvider: ICrawlingStateProvider
    {
        private IOptions<CrawlingStateProviderOptions> _optionsAccessor;

        public CrawlingStateProvider(IOptions<CrawlingStateProviderOptions> optionsAccessor)
        {
            if (optionsAccessor == null)
                throw new ArgumentNullException(nameof(optionsAccessor));

            _optionsAccessor = optionsAccessor;
        }

        public CrawlingState Get()
        {
            var options = _optionsAccessor.Value;
            if (!File.Exists(options.StoreFileName))
            {
                return null;
            }

            var json = File.ReadAllText(options.StoreFileName);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CrawlingState>(json);
        }

        public void Set(CrawlingState state)
        {
            var options = _optionsAccessor.Value;
            var fileInfo = new FileInfo(options.StoreFileName);

            if (state == null && fileInfo.Exists)
            {
                File.Delete(options.StoreFileName);
                return;
            }

            if (!fileInfo.Directory.Exists)
                Directory.CreateDirectory(fileInfo.DirectoryName);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(state);
            File.WriteAllText(options.StoreFileName, json);
        }
    }
}
