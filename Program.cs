using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll"); 


app.MapGet("/api/worldtime/{timezone}", (string timezone) =>
{
    var validTimezones = new[] { "japan", "usa", "india" };

    if (!Array.Exists(validTimezones, tz => tz.Equals(timezone, StringComparison.OrdinalIgnoreCase)))
        return Results.BadRequest(new { error = "Supported timezones: japan, usa, india" });

    try
    {
        TimeZoneInfo timeZoneInfo = timezone.ToLower() switch
        {
            "japan" => TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"),
            "usa" => TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"),
            "india" => TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"),
            _ => throw new TimeZoneNotFoundException()
        };

        var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

        return Results.Ok(new
        {
            time = currentTime.ToString("yyyy-MM-dd HH:mm:ss"),
            timezone
        });
    }
    catch (TimeZoneNotFoundException)
    {
        return Results.BadRequest(new { error = "Invalid timezone" });
    }
});

app.Run();
