using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;
using System.Threading;
using GameServer;

namespace WebSocketServerAppTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
