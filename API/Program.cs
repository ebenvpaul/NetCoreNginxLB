using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

var rabbitHostName = Environment.GetEnvironmentVariable("Redis_HOSTNAME") ?? "localhost";

var builder = WebApplication.CreateBuilder(args);

// Add Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = rabbitHostName;
});

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var redis = ConnectionMultiplexer.Connect(rabbitHostName);

// Add data protection
builder.Services.AddDataProtection()
    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure forwarded headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting(); // Add UseRouting here

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    endpoints.MapGet("/", async context =>
    {
        context.Response.ContentType = "text/plain";

        // Host info
        var name = Dns.GetHostName(); // get container id
        var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
        Console.WriteLine($"Host Name: {Environment.MachineName} \t {name}\t {ip}");
        await context.Response.WriteAsync($"Host Name: {Environment.MachineName}{Environment.NewLine}");
        await context.Response.WriteAsync(Environment.NewLine);

        // Request method, scheme, and path
        await context.Response.WriteAsync($"Request Method: {context.Request.Method}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request URL: {context.Request.GetDisplayUrl()}{Environment.NewLine}");
        await context.Response.WriteAsync($"Request Path: {context.Request.Path}{Environment.NewLine}");

        // Headers
        await context.Response.WriteAsync($"Request Headers:{Environment.NewLine}");
        foreach (var (key, value) in context.Request.Headers)
        {
            await context.Response.WriteAsync($"\t {key}: {value}{Environment.NewLine}");
        }
        await context.Response.WriteAsync(Environment.NewLine);

        // Connection: RemoteIp
        await context.Response.WriteAsync($"Request Remote IP: {context.Connection.RemoteIpAddress}");
    });
});

app.Run();
