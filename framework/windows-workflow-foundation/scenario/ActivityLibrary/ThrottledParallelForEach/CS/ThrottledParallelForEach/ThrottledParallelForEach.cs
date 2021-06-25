//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Markup;

namespace Microsoft.Samples.Activities.Statements
{       
    [Designer(typeof(Microsoft.Samples.Activities.Statements.Presentation.ParallelForEachDesigner))]
    [ContentProperty("Body")]
    public sealed class ThrottledParallelForEach<T> : NativeActivity
    {
        Variable<bool> hasCompleted;        
        Variable<IEnumerator<T>> valueEnumerator;
        CompletionCallback<bool> onConditionComplete;
        CompletionCallback onBodyComplete;

        public ThrottledParallelForEach()
            : base()
        {                        
        }
       
        [Browsable(false)]
        [DefaultValue(null)]
        [DependsOn("CompletionCondition")]
        public ActivityAction<T> Body { get; set; }

        [DefaultValue(null)]
        [DependsOn("MaxConcurrentInstances ")]
        public Activity<bool> CompletionCondition { get; set; }

        [DependsOn("Values")]
        public InArgument<int> MaxConcurrentBranches { get; set; }

        [RequiredArgument]
        [DefaultValue(null)]
        public InArgument<IEnumerable<T>> Values { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // add the arguments to the argument collection
            metadata.AddArgument(new RuntimeArgument("Values", typeof(IEnumerable<T>), ArgumentDirection.In, true));
            metadata.AddArgument(new RuntimeArgument("MaxConcurrentBranches", typeof(int), ArgumentDirection.In));

            // declare the CompletionCondition as a child
            if (this.CompletionCondition != null)
            {
                metadata.SetChildrenCollection(new Collection<Activity> { this.CompletionCondition });
            }
                        
            // initialize the hasCompleted and valueEnumerator and add it to the list of private variables            
            this.hasCompleted = new Variable<bool>();
            metadata.AddImplementationVariable(this.hasCompleted);

            this.valueEnumerator = new Variable<IEnumerator<T>>();
            metadata.AddImplementationVariable(this.valueEnumerator);

            // add the body to the delegates collection
            metadata.AddDelegate(this.Body);
        }        

        protected override void Execute(NativeActivityContext context)
        {            
            // get the list of value to iterate through
            IEnumerable<T> values = this.Values.Get(context);
            if (values == null)
            {
                throw new ApplicationException("ParallelForEach requires a non null Values collection");
            }

            // get the enumerator            
            this.valueEnumerator.Set(context, values.GetEnumerator());     
                   
            // initialize the values for creating the execution window (max and runningCount)
            int max = this.MaxConcurrentBranches.Get(context);
            if (max < 1) max = int.MaxValue;
            int runningCount = 0;

            // initialize the value of the completion variable
            this.hasCompleted.Set(context, false);

            // cache the completion callback
            onBodyComplete = new CompletionCallback(OnBodyComplete);
            
            // iterate while there are items available and we didn't exceed the throttle factor
            while (runningCount < max && valueEnumerator.Get(context).MoveNext())
            {
                // increase the running instances counter
                runningCount++;

                if (this.Body != null)
                {
                    context.ScheduleAction(this.Body, valueEnumerator.Get(context).Current, onBodyComplete);
                }
            }            
        }
        
        void OnBodyComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {                        
            // for the completion condition, we handle cancelation ourselves
            if (this.CompletionCondition != null && !this.hasCompleted.Get(context))
            {
                if (completedInstance.State != ActivityInstanceState.Closed && context.IsCancellationRequested)
                {
                    // If we hadn't completed before getting canceled
                    // or one of our iteration of body cancels then we'll consider
                    // ourself canceled.
                    context.MarkCanceled();
                    this.hasCompleted.Set(context, true);
                }
                else
                {
                    if (this.CompletionCondition != null)
                    {
                        if (this.onConditionComplete == null)
                        {
                            this.onConditionComplete = new CompletionCallback<bool>(OnConditionComplete);
                        }
                        context.ScheduleActivity(CompletionCondition, this.onConditionComplete);
                    }
                }
            }
            else
            {
                if (!this.hasCompleted.Get(context))
                {
                    // get the next child and schedule it!
                    IEnumerator<T> enumerator = this.valueEnumerator.Get(context);
                    if (this.valueEnumerator.Get(context).MoveNext())
                    {
                        context.ScheduleAction(this.Body, this.valueEnumerator.Get(context).Current, onBodyComplete);
                    }
                }
            }
        }

        void OnConditionComplete(NativeActivityContext context, ActivityInstance completedInstance, bool result)
        {
            if (result)
            {                
                context.CancelChildren();
                this.hasCompleted.Set(context, true);
            }
            else
            {
                // get the next child and schedule it!
                IEnumerator<T> enumerator = this.valueEnumerator.Get(context);
                if (this.valueEnumerator.Get(context).MoveNext())
                {
                    context.ScheduleAction(this.Body, this.valueEnumerator.Get(context).Current, onBodyComplete);
                }
            }
        }
    }
}
