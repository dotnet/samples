//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Activities;
using System.Collections.Generic;
using System.Activities.Expressions;
using System.Net.Mail;

namespace Microsoft.Samples.Activities.Statements
{
    class Program
    {
        static void Main(string[] args)
        {

            // Call the declarative workflow.
            WorkflowInvoker.Invoke(new Sequence1());

            // Instantiate the activity programmtically, and call it.
            Activity act = new SendMail
            {
                From = new LambdaValue<MailAddress>(ctx => new MailAddress("john.doe@contoso.com")),
                Subject = "Test email",
                Body = "This is a test email. The current date is @date",
                Host = "localhost",
                Port = 25,
            };

            WorkflowInvoker.Invoke(act, new Dictionary<string, object> {
                {"To", new MailAddressCollection() { new MailAddress("someone@contoso.com") } } } );
            
            Console.WriteLine("Press <return> to exit...");
            Console.ReadLine();
        }
    }
}
