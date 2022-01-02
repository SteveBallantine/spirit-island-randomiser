using System;
using System.Linq;
using System.Threading.Tasks;
using SiRandomizer.Data;

namespace SiRandomizer.Services
{
    public class ConfigurationService
    {
        public Task<OverallConfiguration> GetConfigurationAsync()
        {
            return Task.FromResult(new OverallConfiguration());
        }
    }
}
