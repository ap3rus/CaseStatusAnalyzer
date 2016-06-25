using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CaseStatusAnalyzer
{
    public class Application
    {
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        private readonly II129CaseProvider _i129CaseProvider;
        private readonly ICaseWriter _caseWriter;

        public Application(IServiceCollection serviceCollection)
        {
            _configuration = CreateConfiguration();
            ConfigureServices(serviceCollection);

            _services = serviceCollection.BuildServiceProvider();

            _i129CaseProvider = _services.GetRequiredService<II129CaseProvider>();
            _caseWriter = _services.GetRequiredService<ICaseWriter>();
        }

        private IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddOptions();

            serviceCollection.Configure<CaseProviderOptions>(_configuration.GetSection("CaseProviderOptions"));
            serviceCollection.Configure<ReceiptHtmlProviderOptions>(_configuration.GetSection("ReceiptHtmlProviderOptions"));
            serviceCollection.Configure<CaseCsvWriterOptions>(_configuration.GetSection("CaseCsvWriterOptions"));
            serviceCollection.Configure<CaseParserOptions>(_configuration.GetSection("CaseParserOptions"));

            serviceCollection.AddSingleton<IReceiptHtmlProvider, WebReceiptHtmlProvider>();
            serviceCollection.AddSingleton<ICaseParser, CaseParser>();
            serviceCollection.AddSingleton<ICaseProvider, CaseProvider>();
            serviceCollection.AddSingleton<II129CaseProvider, I129CaseProvider >();
            serviceCollection.AddSingleton<ICaseWriter, CaseCsvWriter>();
        }

        public void ParseH1b()
        {
            var allCases = _i129CaseProvider.GetCasesForFiscalYear("WAC", 16);
            _caseWriter.Write(allCases);
        }
    }
}
