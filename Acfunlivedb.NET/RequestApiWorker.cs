using Acfunlivedb.NET.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acfunlivedb.NET
{
    internal class RequestApiWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public RequestApiWorker(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    GetLiveListService getLiveListService =
                        scope.ServiceProvider.GetRequiredService<GetLiveListService>();
                    await getLiveListService.AddNewLive();
                }
                await Task.Delay(_configuration.GetValue<int>("Queryinterval"), stoppingToken);
            }
        }
    }
}