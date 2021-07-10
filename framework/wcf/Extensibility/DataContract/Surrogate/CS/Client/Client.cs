
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;

namespace Microsoft.Samples.DCSurrogate
{
    //The service contract is defined in generatedProxy.cs, generated from the service by the svcutil tool.

    //Client implementation code.
    class Client
    {
        static void Main()
        {
            // Create a proxy with given client endpoint configuration
            PersonnelDataServiceClient client = new PersonnelDataServiceClient();

            // Call the AddEmployee service operation.
            Employee newEmployee = new Employee();
            newEmployee.dateHired = new DateTime(2004, 6, 10);
            newEmployee.salary = 100;
            newEmployee.person = new Person();
            newEmployee.person.FirstName = "Foo";
            newEmployee.person.LastName = "Barman";
            newEmployee.person.Age = 21;
            client.AddEmployee(newEmployee);

            // Verify if employee was added by doing a search by first name.
            Employee employee = client.GetEmployee("Foo");
            Console.WriteLine("GetEmployee(Foo) : {0}", (employee == null ? "not found" : "found"));

            client.Close();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate client.");
            Console.ReadLine();
        }
    }
}
