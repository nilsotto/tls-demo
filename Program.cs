internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        var summaries = new[]
        {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<Program>();

        app.MapGet("/healthz", async (context) =>
        {
           var content = "Alt OK";
           await context.Response.WriteAsync(content);
        })
        .WithName("/healthz")
        .WithOpenApi();

        
        app.MapGet("/", async (context) =>
        {
            //check if the value of environment variable NEXT_URL is set, call Url if it is 
            if (Environment.GetEnvironmentVariable("NEXT_URL") != null)
            {
                Console.WriteLine("fetching forecast from: " + Environment.GetEnvironmentVariable("NEXT_URL"));
                var nextUrl = Environment.GetEnvironmentVariable("NEXT_URL");
                var client = new HttpClient();
                var response = await client.GetAsync(nextUrl);
                var content = await response.Content.ReadAsStringAsync();
                await context.Response.WriteAsync(content);
            }
            else
            {
                // return http 500 if NEXT_URL is not set
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("NEXT_URL environment variable is not set");
                context.Response.StatusCode = 500;
            }
        })
        .WithName("/")
        .WithOpenApi();

        app.MapGet("/weatherforecast", async (context) =>
        {
            Console.WriteLine("Returning weather forecast");
            var forecast = Enumerable.Range(1, 1).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            await context.Response.WriteAsJsonAsync(forecast);
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
