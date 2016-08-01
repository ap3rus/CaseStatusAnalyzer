using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CaseStatusAnalyzer
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                IServiceCollection serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                Application application = new Application(serviceCollection);
                application.ParseI129Cases();
            }
            catch(Exception ex)
            {
                Debugger.Break();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // init global services
        }
    }
}
