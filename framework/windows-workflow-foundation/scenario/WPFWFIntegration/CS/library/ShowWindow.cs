//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Microsoft.Samples.WPFWFIntegration
{

    [ContentProperty("Window")]
    public sealed class ShowWindow : AsyncCodeActivity
    {
        Func<Window> windowTemplate;

        //need to defer loading here because you want a window to be created for each
        //activity execution - not when the activity is deserialized.
        [XamlDeferLoad(typeof(FuncFactoryDeferringLoader), typeof(Window))]
        public Func<Window> Window
        {
            get { return this.windowTemplate; }
            set { this.windowTemplate = value; }
        }

        void Show(object state)        {          

            Window window = this.windowTemplate();
            window.DataContext = state;
            window.ShowDialog();
        }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext context, AsyncCallback callback, object state)
        {
            if (Application.Current == null)
            {
                throw new InvalidOperationException("Must have an application");
            }

            DispatcherSynchronizationContext syncContext = new DispatcherSynchronizationContext(Application.Current.Dispatcher);
            object dataContext = context.DataContext;
            Action showDelegate = () => syncContext.Send(Show, dataContext);
            context.UserState = showDelegate;

            return showDelegate.BeginInvoke(callback, state);
        }

        protected override void EndExecute(AsyncCodeActivityContext context, IAsyncResult result)
        {
            Action showDelegate = (Action)context.UserState;
            showDelegate.EndInvoke(result);
        }
    }
}
