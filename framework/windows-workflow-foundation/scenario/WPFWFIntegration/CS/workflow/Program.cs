//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace Microsoft.Samples.WPFWFIntegration
{
	
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //loading PresentationFramework so Xaml can resolve its types
            Assembly.Load("PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL");

            RunUnderApplication((Activity)System.Windows.Markup.XamlReader.Load(File.OpenRead("ShowWindow.xaml")));
       
            Console.WriteLine("Press [ENTER] to quit");
            Console.ReadLine();
        }

        static void RunUnderApplication(Activity activity)
        {
            Application application = new System.Windows.Application() { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            application.Startup += delegate
            {
                WorkflowApplication wfApp = new WorkflowApplication(activity);
                wfApp.SynchronizationContext = SynchronizationContext.Current;
                wfApp.Completed = delegate(WorkflowApplicationCompletedEventArgs e)
                {
                    application.Shutdown();
                };
                wfApp.Run();
            };
            application.Run();
        }

    }
}
