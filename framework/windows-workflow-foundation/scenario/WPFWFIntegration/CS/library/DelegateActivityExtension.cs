//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xaml;

namespace Microsoft.Samples.WPFWFIntegration
{

    [ContentProperty("Activity")]
    public class DelegateActivityExtension : MarkupExtension
    {
        Func<SymbolResolver> environmentProvider;
        Func<IXamlNameResolver> tableProvider;
        ParameterInfo[] parameters;
        Type returnType;

        public Activity Activity { get; set; }

        public Type DelegateType{ get; set; }

        void EnsureDelegateType(IServiceProvider serviceProvider)
        {
            if (DelegateType == null)
            {
                IProvideValueTarget valueTargetProvider = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
                if (valueTargetProvider != null)
                {
                    EventInfo eventInfo = valueTargetProvider.TargetProperty as EventInfo;
                    if (eventInfo != null)
                    {
                        DelegateType = eventInfo.EventHandlerType;
                    }
                    else
                    {
                        PropertyInfo propertyInfo = valueTargetProvider.TargetProperty as PropertyInfo;
                        if (propertyInfo != null)
                        {
                            DelegateType = propertyInfo.PropertyType;
                        }
                    }
                }

                if (DelegateType == null)
                {
                    throw new InvalidOperationException(
                        "No delegate type was specified and none could be inferred. You must specify a delegate type.");
                }
            }

        }

        object GenericInvoke(params object[] args)
        {
            if (args.Length != this.parameters.Length)
            {
                throw new ArgumentException("Wrong number of arguments");
            }

            SymbolResolver symbolResolver = environmentProvider();

            for (int i = 0; i < this.parameters.Length; i++)
            {
                ParameterInfo parameter = this.parameters[i];
                if (!parameter.IsOut) 
                {
                    symbolResolver.Add(parameter.Name, args[i]);
                }
            }

            if (this.returnType != typeof(void))
            {
                symbolResolver.Add(Argument.ResultValue, new ValueHolder<object>());
            }

            WorkflowApplication application = new WorkflowApplication(this.Activity, new Dictionary<string, object>());
            application.SynchronizationContext = SynchronizationContext.Current;
            application.Extensions.Add(symbolResolver);

            application.Completed = OnWorkflowCompleted;
            application.Aborted = OnWorkflowAborted;

            application.Run();

            return null;
        }

        void OnWorkflowCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            if (e.CompletionState == ActivityInstanceState.Faulted)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    () => { throw new DelegateActivityException("The handler for the DelegateActivityExtension was faulted. See inner exception.", e.TerminationException); });
            }
        }

        void OnWorkflowAborted(WorkflowApplicationAbortedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                () => { throw new DelegateActivityException("The handler for the DelegateActivityExtension was aborted. See inner exception", e.Reason); });
        }

        static Func<object> GetContextProvider(IServiceProvider serviceProvider)
        {
            // If we can get our hands on a FrameworkElement, then we will only consider its 
            // DataContext as a source for the environment. If we can't, we will consider the 
            // template context, to give non-WPF code a chance.
            //
            IProvideValueTarget valueTargetProvider = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (valueTargetProvider != null && valueTargetProvider.TargetObject != null)
            {
                object target = valueTargetProvider.TargetObject;
                if (DataContextHelper.HasDataContext(target))
                {
                    return () =>
                    {
                        object finalDataContext = DataContextHelper.GetCurrentItem(DataContextHelper.GetDataContext(target));
                        return finalDataContext;
                    };


                }
            }

            SymbolResolver symbolResolver = serviceProvider.GetService(typeof(SymbolResolver)) as SymbolResolver;

            if (symbolResolver != null)
            {
                return () => symbolResolver;
            }

            // No framework element
            //
            return () => null;
        }

        MethodInfo GetDelegateMethod()
        {
            if (this.parameters.Any(parameterInfo => parameterInfo.IsOut))
            {
                throw new NotImplementedException("Methods using out parameters not supported");
            }

            MethodInfo method;
            List<Type> typeArguments = new List<Type>();
            if (this.returnType == typeof(void))
            {
                method = this.GetType().GetMethod("InvokeWithoutReturn" + this.parameters.Length);
            }
            else
            {
                method = this.GetType().GetMethod("InvokeWithReturn" + this.parameters.Length);
            }

            if (method == null)
            {
                throw new NotImplementedException("This method signature is not supported"); ;
            }

            if (method.IsGenericMethodDefinition)
            {
                typeArguments.AddRange(from parameterInfo in this.parameters select parameterInfo.ParameterType);
                if (this.returnType != typeof(void))
                {
                    typeArguments.Add(this.returnType);
                }

                method = method.MakeGenericMethod(typeArguments.ToArray());
            }
            return method;
        }

        Func<SymbolResolver> GetEnvironmentProvider(
            Func<object> contextProvider)
        {
            return () => SymbolResolverExtensions.EnvironmentFromNameTable(
                this.tableProvider(),
                SymbolResolverExtensions.EnvironmentFromObject(contextProvider(), null));
        }

        void SetupNameTableProvider(IServiceProvider serviceProvider)
        {
            IXamlNameResolver serializerContext = (IXamlNameResolver)serviceProvider.GetService(typeof(IXamlNameResolver));

            if (serializerContext != null)
            {
                this.tableProvider = () => serializerContext;
            }
            else
            {
                this.tableProvider = () => null;
            }
        }

        public void InvokeWithoutReturn0()
        {
            GenericInvoke();
        }
        public void InvokeWithoutReturn1<TArg0>(TArg0 arg0)
        {
            GenericInvoke(arg0);
        }
        public void InvokeWithoutReturn2<TArg0, TArg1>(TArg0 arg0, TArg1 arg1)
        {
            GenericInvoke(arg0, arg1);
        }
        public void InvokeWithoutReturn3<TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            GenericInvoke(arg0, arg1, arg2);
        }
        public void InvokeWithoutReturn4<TArg0, TArg1, TArg2, TArg3>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            GenericInvoke(arg0, arg1, arg2, arg3);
        }

        public T InvokeWithReturn0<T>()
        {
            return (T)GenericInvoke();
        }
        public T InvokeWithReturn1<TArg0, T>(TArg0 arg0)
        {
            return (T)GenericInvoke(arg0);
        }
        public T InvokeWithReturn2<TArg0, TArg1, T>(TArg0 arg0, TArg1 arg1)
        {
            return (T)GenericInvoke(arg0, arg1);
        }
        public T InvokeWithReturn3<TArg0, TArg1, TArg2, T>(TArg0 arg0, TArg1 arg1, TArg2 arg2)
        {
            return (T)GenericInvoke(arg0, arg1, arg2);
        }
        public T InvokeWithReturn4<TArg0, TArg1, TArg2, TArg3, T>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return (T)GenericInvoke(arg0, arg1, arg2, arg3);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            EnsureDelegateType(serviceProvider);

            SetupNameTableProvider(serviceProvider);
            this.environmentProvider = GetEnvironmentProvider(
                GetContextProvider(serviceProvider));
            MethodInfo invokeMethod = DelegateType.GetMethod("Invoke");

            this.parameters = invokeMethod.GetParameters();
            this.returnType = invokeMethod.ReturnType;

            return Delegate.CreateDelegate(DelegateType, this, GetDelegateMethod());
        }
    }
}
