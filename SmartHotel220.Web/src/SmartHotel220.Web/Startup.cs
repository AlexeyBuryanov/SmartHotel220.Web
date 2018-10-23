using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SmartHotel220.Web.Models.Settings;
using SmartHotel220.Web.Services;

namespace SmartHotel220.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // ���� ����� ���������� �� ����� ����������. ������������ ��� ���������� �������� � ��������� ������������.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // ������������ ��������� ������ � �����������
            services.Configure<LocalSettings>(Configuration);
            // ��������� ��������� � �������
            services.AddSingleton(sp => SettingsService.Load(sp.GetService<IOptions<LocalSettings>>().Value));
            // ���������� ApplicationInsights
            services.AddSingleton<ITelemetryProcessorFactory>(new SnapshotCollectorTelemetryProcessorFactory());

            // ��������� �������
            if (!string.IsNullOrEmpty(Configuration["USE_NULL_TESTIMONIALS_SERVICE"])) {
                services.AddSingleton<ICustomerTestimonialService>(new NullCustomerTestimonialService());
            } else {
                services.AddSingleton<ICustomerTestimonialService, PositiveTweetService>();
            }
        }

        // ���� ����� ���������� �� ����� ����������. ����������� ���� ����� ��� ��������� ��������� HTTP-��������.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            } else {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes => {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    "spa-fallback",
                    new { controller = "Home", action = "Index" });
            });
        }

        // ���������� ApplicationInsights
        private class SnapshotCollectorTelemetryProcessorFactory : ITelemetryProcessorFactory
        {
            public ITelemetryProcessor Create(ITelemetryProcessor next) =>
                new SnapshotCollectorTelemetryProcessor(next);
        }
    }
}
