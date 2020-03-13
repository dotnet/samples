//<snippet0>
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Principal;
using System.Text;
using System.IO;
using System.Threading;

namespace Examples.NegotiateStreamExample
{
    public class AsynchronousAuthenticatingTcpListener 
    {
        public static void Main() 
        {
            // Create an IPv4 TCP/IP socket. 
            TcpListener listener = new TcpListener(IPAddress.Any, 11000);
            // Listen for incoming connections.
            listener.Start();
            while (true)
            {
                TcpClient clientRequest;
                // Application blocks while waiting for an incoming connection.
                // Type CNTL-C to terminate the server.
                clientRequest = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");
                // A client has connected. 
                try
                {
                    AuthenticateClient(clientRequest);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

//<snippet1>
        public static void AuthenticateClient(TcpClient clientRequest)
        {
            NetworkStream stream = clientRequest.GetStream();
            // Create the NegotiateStream.
            NegotiateStream authStream = new NegotiateStream(stream, false);
            // Save the current client and NegotiateStream instance 
            // in a ClientState object.
            ClientState cState = new ClientState(authStream, clientRequest);
            // Listen for the client authentication request.
            Task authTask = authStream
                .AuthenticateAsServerAsync()
                .ContinueWith(task => { EndAuthenticateCallback(cState); });

            // Any exceptions that occurred during authentication are
            // thrown by the EndAuthenticateAsServer method.
            try
            {
                // This call blocks until the authentication is complete.
                authTask.Wait();
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Authentication failed - closing connection.");
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Closing connection.");
                return;
            }

            Task<int> readTask = authStream
                .ReadAsync(cState.Buffer, 0, cState.Buffer.Length);

            readTask
                .ContinueWith((task) => { EndReadCallback(cState, task.Result); })
                .Wait();
            // Finished with the current client.
            authStream.Close();
            clientRequest.Close();
        }
//</snippet1>    

//<snippet2>
        private static void EndAuthenticateCallback(ClientState cState)
        {
            // Get the saved data.
            NegotiateStream authStream = (NegotiateStream)cState.AuthenticatedStream;
            Console.WriteLine("Ending authentication.");

            // Display properties of the authenticated client.
            IIdentity id = authStream.RemoteIdentity;
            Console.WriteLine("{0} was authenticated using {1}.",
                id.Name,
                id.AuthenticationType
            );
        }
//</snippet2>

//<snippet3>
        private static void EndReadCallback(ClientState cState, int bytes)
        {
            NegotiateStream authStream = (NegotiateStream)cState.AuthenticatedStream;
            // Read the client message.
            try
            {
                cState.Message.Append(Encoding.UTF8.GetChars(cState.Buffer, 0, bytes));
                if (bytes != 0)
                {
                    Task<int> readTask = authStream.ReadAsync(cState.Buffer, 0, cState.Buffer.Length);
                    readTask
                        .ContinueWith(task => { EndReadCallback(cState, task.Result); })
                        .Wait();

                    return;
                }
            }
            catch (Exception e)
            {
                // A real application should do something
                // useful here, such as logging the failure.
                Console.WriteLine("Client message exception:");
                Console.WriteLine(e);
                return;
            }
            IIdentity id = authStream.RemoteIdentity;
            Console.WriteLine("{0} says {1}", id.Name, cState.Message.ToString());
        }
//</snippet3>
    }
    // ClientState is the AsyncState object.
    internal class ClientState
    {
        private StringBuilder _message = null;

        internal ClientState(AuthenticatedStream a, TcpClient theClient)
        {
            AuthenticatedStream = a;
            Client = theClient;
        }
        internal TcpClient Client { get; }

        internal AuthenticatedStream AuthenticatedStream { get; }

        internal byte[] Buffer { get; } = new byte[2048];

        internal StringBuilder Message
        {
            get { return _message ??= new StringBuilder(); }
        }
    }
}
//</snippet0>
