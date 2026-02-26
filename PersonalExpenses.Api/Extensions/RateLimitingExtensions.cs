using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace PersonalExpenses.Api.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // ✅ Rate Limit para Login: 5 tentativas por 15 minutos
                options.AddFixedWindowLimiter("login", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 5;
                    limiterOptions.Window = TimeSpan.FromMinutes(15);
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                // ✅ Rate Limit para API geral: 100 requisições por minuto
                options.AddFixedWindowLimiter("general", limiterOptions =>
                {
                    limiterOptions.PermitLimit = 100;
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }
    }
}
