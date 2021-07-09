
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.UsingUsing
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    // The "using" keyword in C# enables deterministic cleanup without writing a lot of code.  It
    // encourages correct cleanup because you do not have to write a lot of try/catch/finally code
    // to clean up resources correctly.  This works well when Dispose and Close rarely throw.  For
    // example, writing to a file rarely fails, and it may be acceptable for an application to fail
    // under such circumstances.
    //
    // Unfortunately, network connections are much more prone to failure than most disposable
    // resources.  Because Dispose and Close on a client can throw based on factors outside the
    // application's control, applications typically need to be hardened to handle these
    // Exceptions.  Handling these Exceptions correctly while using the C# "using" statement results
    // in more complicated code than just writing the try/catch blocks.
    //
    // This sample illustrates some of the problems that can happen if you use the C# "using"
    // statement with a client.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            DemonstrateProblemUsingCanThrow();
            DemonstrateProblemUsingCanThrowAndMask();
            DemonstrateCleanupWithExceptions();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }

        // This method shows one problem with the "using" statement.
        //
        // The service Aborts the channel to indicate that the session failed.  When that
        // happens, the client gets an Exception from Close.  Because Dispose is the same as
        // Close, the client gets an Exception from Dispose as well.  The close of the "using"
        // block results in a call to client.Dispose.  Typically developers use "using" to avoid
        // having to write try/catch/finally code.  However, because the closing brace can throw,
        // the try/catch is necessary.
        static void DemonstrateProblemUsingCanThrow()
        {
            Console.WriteLine("=");
            Console.WriteLine("= Demonstrating problem:  closing brace of using statement can throw.");
            Console.WriteLine("=");

            try
            {
                // Create a new client.
                using (CalculatorClient client = new CalculatorClient())
                {
                    // Call Divide and catch the associated Exception.  This throws because the
                    // server aborts the channel before returning a reply.
                    try
                    {
                        client.Divide(0.0, 0.0);
                    }
                    catch (CommunicationException e)
                    {
                        Console.WriteLine("Got {0} from Divide.", e.GetType());
                    }
                }

                // The previous line calls Dispose on the client.  Dispose and Close are the
                // same thing, and the Close is not successful because the server Aborted the
                // channel.  This means that the code after the using statement does not run.
                Console.WriteLine("Hope this code wasn't important, because it might not happen.");
            }
            catch (CommunicationException e)
            {
                // The closing brace of the "using" block throws, so we end up here.  If you
                // want to use using, you must surround it with a try/catch
                Console.WriteLine("Got {0}", e.GetType());
            }
        }

        // This method shows another problem with the "using" statement.
        //
        // The service Aborts the channel to indicate that the session failed.  When that
        // happens, the client gets an Exception from Close.  Because Dispose is the same as
        // Close, the client gets an Exception from Dispose as well.  The close of the "using"
        // block results in a call to client.Dispose.  Because the closing brace executes
        // regardless of whether an Exception occurred, Exceptions from Close can mask more
        // important Exceptions from inside the "using" block.
        static void DemonstrateProblemUsingCanThrowAndMask()
        {
            Console.WriteLine("=");
            Console.WriteLine("= Demonstrating problem:  closing brace of using statement can mask other Exceptions.");
            Console.WriteLine("=");

            try
            {
                // Create a new client.
                using (CalculatorClient client = new CalculatorClient())
                {
                    // Call Divide and catch the associated Exception.  This throws because the
                    // server aborts the channel before returning a reply.
                    try
                    {
                        client.Divide(0.0, 0.0);
                    }
                    catch (CommunicationException e)
                    {
                        Console.WriteLine("Got {0} from Divide.", e.GetType());
                    }
                    throw new ObjectDisposedException("Hope this Exception wasn't important, because "+
                                                      "it might be masked by the Close Exception.");

                    // The following line calls Dispose on the client.  Dispose and Close are the
                    // same thing, and the Close is not successful because the server Aborted the
                    // channel.  This masks the ObjectDisposedException above so nobody outside the
                    // "using" block sees it.
                }
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("We do not come here because the ObjectDisposedException is masked.");
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("Got {0}", e.GetType());
            }
        }

        // This method shows the correct way to clean up a client, including catching the
        // approprate Exceptions.
        static void DemonstrateCleanupWithExceptions()
        {
            Console.WriteLine("=");
            Console.WriteLine("= Demonstrating cleanup with Exceptions.");
            Console.WriteLine("=");

            // Create a client
            CalculatorClient client = new CalculatorClient();
            try
            {
                // Demonstrate a successful client call.
                Console.WriteLine("Calling client.Add(0.0, 0.0);");
                double addValue = client.Add(0.0, 0.0);
                Console.WriteLine("        client.Add(0.0, 0.0); returned {0}", addValue);

                // Demonstrate a failed client call.
                Console.WriteLine("Calling client.Divide(0.0, 0.0);");
                double divideValue = client.Divide(0.0, 0.0);
                Console.WriteLine("        client.Divide(0.0, 0.0); returned {0}", divideValue);

                // Do a clean shutdown if everything works.  In this sample we do not end up
                // here, but correct code should Close the client if everything was successful.
                Console.WriteLine("Closing the client");
                client.Close();
            }
            catch (CommunicationException e)
            {
                // Because the server suffered an internal server error, it rudely terminated
                // our connection, so we get a CommunicationException.
                Console.WriteLine("Got {0} from Divide.", e.GetType());
                client.Abort();
            }
            catch (TimeoutException e)
            {
                // In this sample we do not end up here, but correct code should catch
                // TimeoutException when calling a client.
                Console.WriteLine("Got {0} from Divide.", e.GetType());
                client.Abort();
            }
            catch (Exception e)
            {
                // In this sample we do not end up here.  It is best practice to clean up the
                // client if some unexpected Exception occurs.
                Console.WriteLine("Got unexpected {0} from Divide, rethrowing.", e.GetType());
                client.Abort();
                throw;
            }
        }
    }
}
