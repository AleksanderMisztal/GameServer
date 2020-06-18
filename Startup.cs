using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using GameServer.Networking;

namespace WebSocketServerAppTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

            app.UseWebSockets();

            app.Use(async (context, next) =>
            {
                Console.WriteLine("In request pipeline");
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
                    Console.WriteLine("Web socket connection accepted!");

                    await Server.ConnectNewClient(socket);
                }
                else
                {
                    await next();
                }
            });

            app.Run(async context =>
            {
                Console.WriteLine("Received a non websocket request.");
                await context.Response.WriteAsync("Hello world");
            });
        }
    }
}
