//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Samples.WSStreamedHttpBinding
{
    class Client
    {
        static void Main(string[] args)
        {
            // WARNING: This code is only needed for test certificates such as those created by makecert. It is 
            // not recommended for production code.
            PermissiveCertificatePolicy.Enact("CN=ServiceModelSamples-HTTPS-Server");

            StreamedEchoServiceClient client = new StreamedEchoServiceClient();

            Console.Write("Enter the filename of the file you want to duplicate: ");
            string filename = Console.ReadLine();

            FileStream readStream = new FileStream(filename, FileMode.Open);
            Stream data = client.Echo(readStream);

            FileStream writeStream = new FileStream("Copy of " + filename, FileMode.Create);

            byte[] byteArray = new byte[8192];
            int bytesRead = data.Read(byteArray, 0, 8192);
            while (bytesRead > 0)
            {
                writeStream.Write(byteArray, 0, bytesRead);
                bytesRead = data.Read(byteArray, 0, 8192);
            }

            readStream.Close();
            writeStream.Close();
            data.Close();

            client.Close();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }

    // This code is only needed for test certificates such as those created by makecert

    // WARNING: This code is only needed for test certificates such as those created by makecert. It is 
    // not recommended for production code.
    class PermissiveCertificatePolicy
    {
        string subjectName;
        static PermissiveCertificatePolicy currentPolicy;
        PermissiveCertificatePolicy(string subjectName)
        {
            this.subjectName = subjectName;
            ServicePointManager.ServerCertificateValidationCallback +=
                new System.Net.Security.RemoteCertificateValidationCallback(RemoteCertValidate);
        }

        public static void Enact(string subjectName)
        {
            currentPolicy = new PermissiveCertificatePolicy(subjectName);
        }

        bool RemoteCertValidate(object sender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            if (cert.Subject == subjectName)
            {
                return true;
            }

            return false;
        }
    }
}
