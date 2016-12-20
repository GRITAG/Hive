using HiveSuite.Core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Log;
using Unosquare.Labs.EmbedIO.Modules;

namespace HiveSuite.Queen.Web
{
    public class WebHandler
    {
        WebServer Server { get; set; }
        Task ServerTask { get; set; }
        CancellationTokenSource token { get; set; }

        public WebHandler(ISettings settings)
        {
            Server = new WebServer("http://localhost:8080/", new NullLog(), RoutingStrategy.Regex);
            if(!Directory.Exists(Directory.GetCurrentDirectory() + "\\html"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\html");
            }
            Server.WithStaticFolderAt(Directory.GetCurrentDirectory() + "\\html");
            Server.RegisterModule(new WebApiModule());
            //Server.Module<WebApiModule>().RegisterController<null>();

            token = new CancellationTokenSource();
        }

        public void RegisterAPIController(Type classToRegister)
        {
            Server.Module<WebApiModule>().RegisterController(classToRegister);
        }

        public void Start()
        {
            ServerTask = Server.RunAsync(token.Token);
        }

        public void Stop()
        {
            token.Cancel();
        }
    }
}
