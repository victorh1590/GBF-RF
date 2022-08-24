using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RaidFinder.Data;
using Microsoft.AspNetCore.ResponseCompression;
using RaidFinder.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<StreamConsumerService>();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();

app.MapHub<FinderHub>("/FinderHub");

app.MapFallbackToPage("/_Host");

var context = app.Services.GetService<StreamConsumerService>();

if(context != null) app.Lifetime.ApplicationStarted.Register(async () => await context.Watch());

app.Run();
