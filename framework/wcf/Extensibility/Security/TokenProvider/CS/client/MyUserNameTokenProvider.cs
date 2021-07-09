using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Text;
using System.ServiceModel;

//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace Microsoft.Samples.TokenProvider
{
    class MyUserNameTokenProvider : SecurityTokenProvider
    {
        static string GetUserName()
        {
            Console.WriteLine("Username authentication required.");
            Console.WriteLine("   Enter username:");
            string username = Console.ReadLine();
            return username;
        }

        static string GetPassword()
        {
            Console.WriteLine("   Enter password:");
            string password = "";
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    password += info.KeyChar;
                    info = Console.ReadKey(true);
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (password != "")
                    {
                        password = password.Substring(0, password.Length - 1);

                    }
                    info = Console.ReadKey(true);
                }
            }

            for (int i = 0; i < password.Length; i++)
                Console.Write("*");

            Console.WriteLine();

            return password;
        }

        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            // obtain username and password from the user using console window
            string username = GetUserName();
            string password = GetPassword();
            Console.WriteLine("username: {0}", username);

            // return new UserNameSecurityToken containing information obtained from user
            return new UserNameSecurityToken(username, password);
        }
    }
}

