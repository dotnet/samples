
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using Client.ServiceReference;

namespace Microsoft.Samples.ObjectReferences
{
    //The service contract is defined in generatedClient.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a client
            SocialNetworkClient sn = new SocialNetworkClient();

            // Create my social network
            Person helena = new Person() { Name = "Helena", Age = 35, Location = "Chicago" };
            Person andrew = new Person(){Name = "Andrew", Age=25, Location="Seattle"};
            Person paul = new Person() { Name = "Paul", Age = 28, Location = "Bostan" };
            Person sarah = new Person() { Name = "Sarah", Age = 27, Location = "Seattle" };
            Person richard = new Person() { Name = "Richard", Age = 40, Location = "New York" };

            helena.Friends = new Person[] {andrew, sarah, richard, paul};
            andrew.Friends = new Person[] { helena, richard, paul };
            paul.Friends = new Person[] { sarah, richard };
            sarah.Friends = new Person[] { andrew};
            richard.Friends = new Person[] { helena };

            // Call the GetPeopleInNetwork to find people in Andrew's network
            Person[] network = sn.GetPeopleInNetwork(andrew);
            Console.WriteLine("Andrew's Network: ");
            foreach (Person p in network)
                Console.WriteLine(" "+p.Name);

            // Call GetMutualFriends to find Helena's mutual friends
            Person[] mutual = sn.GetMutualFriends(helena);
            Console.WriteLine("Helena's Mutual Friends: ");
            foreach (Person p in mutual)
                Console.WriteLine(" " + p.Name);

            // Call GetcommonFriends to find common friends of Helena and Andrew
            Person[] common = sn.GetCommonFriends(new Person[] {helena, andrew});
            Console.WriteLine("Helena and Andew's Common Friends: ");
            foreach (Person p in common)
                Console.WriteLine(" " + p.Name);

            Console.ReadLine();

        }
    }
}
