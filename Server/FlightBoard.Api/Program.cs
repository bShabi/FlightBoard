using Microsoft.AspNetCore.Builder;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.DependencyInjection; // Ensure this is included
using Microsoft.OpenApi.Models; // Ensure this is included
using Swashbuckle.AspNetCore.SwaggerUI;
using FlightBoard.Models.Interface;
using FlightBoard.Provider.Providers;
using FlightBoard.Api.Middleware;
using FlightBoard.Bl.Managers;
using FlightBoard.Api.hubs;
using System.Text.Json.Serialization;
using FlightBoard.Api.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddJsonConsole();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddScoped<IFlightProvider, FlightsProvider>();
builder.Services.AddScoped<IFlightManager, FlightManager>();
builder.Services.AddHostedService<FlightStatusUpdaterService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // כתובת ה-React שלך
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger";
});
app.UseRouting();
app.UseAuthorization();
app.UseRequestResponseLogging();
app.UseErrorHandlingMiddlewareExtensions();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<FlightsHub>("/flightsHub");

app.Run();
