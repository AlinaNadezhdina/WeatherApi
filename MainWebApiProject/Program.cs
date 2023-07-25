using System.Reflection;
using Microsoft.OpenApi.Models;
using WeatherRequests;
using WeatherRequests.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.Configure<ServiceSettings>(builder.Configuration);
builder.Services.Add(new ServiceDescriptor(typeof(WeatherClient), typeof(WeatherClient), ServiceLifetime.Singleton));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Weather forecast query service",
        Description = "An ASP.NET Core Web API",
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}";
	options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{xmlFilename}.xml"));
	options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "WeatherRequests.xml"));

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();//  для Rider 

	// ==========  для vs code ============
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
	// ====================================se
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
