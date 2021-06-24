//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;
using System.Collections;
using System.ComponentModel;
using System.Windows.Markup;

namespace Microsoft.Samples.Activities.Statements
{
    [Designer(typeof(Microsoft.Samples.Activities.Statements.Presentation.ForEachDesigner))]
    [ContentProperty("Body")]
    public sealed class ForEach : NativeActivity
    {
        Variable<IEnumerator> valueEnumerator;
        CompletionCallback onChildComplete;

        public ForEach()
            : base()
        {
            this.valueEnumerator = new Variable<IEnumerator>();
        }

        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<IEnumerable> Values
        {
            get;
            set;
        }

        [Browsable(false)]
        [DefaultValue(null)]
        [DependsOn("Values")]
        public ActivityAction<object> Body
        {
            get;
            set;
        }

        CompletionCallback OnChildComplete
        {
            get
            {
                if (this.onChildComplete == null)
                {
                    this.onChildComplete = new CompletionCallback(GetStateAndExecute);
                }

                return this.onChildComplete;
            }
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            RuntimeArgument valuesArgument = new RuntimeArgument("Values", typeof(IEnumerable), ArgumentDirection.In, true);
            metadata.Bind(this.Values, valuesArgument);

            metadata.AddArgument(valuesArgument);
            metadata.AddDelegate(this.Body);
            metadata.AddImplementationVariable(this.valueEnumerator);
        }

        protected override void Execute(NativeActivityContext context)
        {
            IEnumerable values = this.Values.Get(context);
            if (values == null)
            {
                throw new InvalidOperationException(string.Format("ForEach requires a non null Values argument ({0})", this.DisplayName));                
            }

            IEnumerator valueEnumerator = values.GetEnumerator();
            this.valueEnumerator.Set(context, valueEnumerator);

            if (this.Body == null || this.Body.Handler == null)
            {
                while (valueEnumerator.MoveNext())
                {
                    // do nothing
                };
                OnForEachComplete(valueEnumerator);
                return;
            }
            InternalExecute(context, valueEnumerator);
        }

        void GetStateAndExecute(NativeActivityContext context, ActivityInstance completedInstance)
        {
            IEnumerator valueEnumerator = this.valueEnumerator.Get(context);            
            InternalExecute(context, valueEnumerator);
        }

        void InternalExecute(NativeActivityContext context, IEnumerator valueEnumerator)
        {        
            if (!valueEnumerator.MoveNext())
            {
                OnForEachComplete(valueEnumerator);
                return;
            }

            // After making sure there is another value, let's check for cancelation
            if (context.IsCancellationRequested)
            {
                context.MarkCanceled();
                OnForEachComplete(valueEnumerator);
                return;
            }

            context.ScheduleAction(this.Body, valueEnumerator.Current, this.OnChildComplete);
        }

        void OnForEachComplete(IEnumerator valueEnumerator)
        {
            IDisposable disposable = (valueEnumerator as IDisposable);
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
