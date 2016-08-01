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
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            _configuration = CreateConfiguration();
            ConfigureServices(serviceCollection);

            _services = serviceCollection.BuildServiceProvider();

            _i129CaseProvider = _services.GetRequiredService<II129CaseProvider>();
            _caseWriter = _services.GetRequiredService<ICaseWriter>();
        }

        private static IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<I129CaseProviderOptions>(_configuration.GetSection("I129CaseProviderOptions"));
            services.Configure<CaseProviderOptions>(_configuration.GetSection("CaseProviderOptions"));
            services.Configure<ReceiptHtmlProviderOptions>(_configuration.GetSection("ReceiptHtmlProviderOptions"));
            services.Configure<CaseCsvWriterOptions>(_configuration.GetSection("CaseCsvWriterOptions"));
            services.Configure<CaseParserOptions>(_configuration.GetSection("CaseParserOptions"));
            services.Configure<CrawlingStateProviderOptions>(_configuration.GetSection("CrawlingStateProviderOptions"));

            services.AddSingleton<IReceiptHtmlProvider, WebReceiptHtmlProvider>();
            services.AddSingleton<ICaseParser, CaseParser>();
            services.AddSingleton<ICaseProvider, CaseProvider>();
            services.AddSingleton<II129CaseProvider, I129CaseProvider >();
            services.AddSingleton<ICaseWriter, CaseCsvWriter>();
            services.AddSingleton<ICrawlingStateProvider, CrawlingStateProvider>();
        }

        public void ParseI129Cases()
        {
            var allCases = _i129CaseProvider.GetCasesForFiscalYear();
            _caseWriter.Write(allCases);
        }
    }
}
