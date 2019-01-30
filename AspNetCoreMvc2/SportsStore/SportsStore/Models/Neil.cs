using SportsStore.Infrastructure;

namespace SportsStore.Models
{
    public class Neil
    {
        private readonly UptimeService _uptimeService;

        // with  services.AddTransient<Neil>();
        // and  public NeilController(Neil neilModel)
        // testing ASP.Net services 
        public Neil(Infrastructure.UptimeService uptimeService)
        {
            _uptimeService = uptimeService;
        }

        public string Uptime => $"up for {_uptimeService.Uptime.ToString()} milliseconds";
    }
}
