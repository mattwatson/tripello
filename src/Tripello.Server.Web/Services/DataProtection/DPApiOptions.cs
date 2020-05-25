using System;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tripello.Server.Web.Services.DataProtection
{
    public class DPApiOptions
    {
        public bool Enabled { get; set; }
        public string FileLocation { get;set; }
    }

    public static class DataProtectionExtenisions
    {
        public static IServiceCollection AddDataProtectionWithDPApiOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var dpApiOptions = configuration.GetSection("DPAPI").Get<DPApiOptions>();
            if (dpApiOptions != null && dpApiOptions.Enabled && !String.IsNullOrWhiteSpace(dpApiOptions.FileLocation))
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(dpApiOptions.FileLocation));
            }
            
            return services;
        }
    }
}
