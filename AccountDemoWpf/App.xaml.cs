using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using NLog.Fluent;
using ReactiveDomain.EventStore;
using ReactiveDomain.Tests.Logging;

namespace AccountDemoWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Bootstrap _bootstrap;
        public string AssemblyName { get; set; }

        public App()
        {
            var fullName = Assembly.GetExecutingAssembly().FullName;
            //Log.Info($"{fullName} Loaded.");
            AssemblyName = fullName.Split(',')[0];
            _bootstrap = new Bootstrap();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _bootstrap.Run(e);
            base.OnStartup(e);
        }
    }
}
