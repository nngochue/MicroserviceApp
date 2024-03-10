
using ApiGateway.Aggregators;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Ocelot.Values;

namespace ApiGateway
{

    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Config Ocelot
            var routes = "Routes";
            builder.Configuration.AddOcelotWithSwaggerSupport(options =>
            {
                options.Folder = routes;
            });
            builder.Services
                .AddOcelot(builder.Configuration)
                .AddSingletonDefinedAggregator<FakeDefinedAggregator>()
                .AddPolly();

            builder.Services.AddSwaggerForOcelot(builder.Configuration, (o) => { o.GenerateDocsForAggregates = true; });
            var environtment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
           var ocelotBuilder = builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddOcelot(routes, builder.Environment)
                .AddEnvironmentVariables() as IOcelotBuilder;

            

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(x => x.FullName.Replace("+", "."));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                //app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            

            app.UseAuthorization();

            app.UseSwaggerForOcelotUI(options =>
            {
                options.PathToSwaggerGenerator = "/swagger/docs";
                options.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;
            }).UseOcelot().Wait();

            app.MapControllers();

            app.Run();
        }
    }
}
