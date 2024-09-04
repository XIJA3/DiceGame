using Microsoft.Extensions.Configuration;

namespace Web.Helpers
{
    public static class GlobalConfiguration
    {
        private static IConfiguration? _configuration;

        public static void Configure(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static IConfiguration Configuration
        {
            get
            {
                return _configuration ?? throw new ArgumentNullException("Configuration is not provided");
            }
        }
    }
}
