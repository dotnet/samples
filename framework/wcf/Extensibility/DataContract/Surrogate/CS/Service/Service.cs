
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Microsoft.Samples.DCSurrogate
{
    // Define a service contract.
    [ServiceContract(Namespace = "http://Microsoft.Samples.DCSurrogate")]
    [AllowNonSerializableTypes]
    public interface IPersonnelDataService
    {
        [OperationContract]
        void AddEmployee(Employee employee);

        [OperationContract]
        Employee GetEmployee(string name);
    }

    // This is the Employee (outer) type used in the sample.
    [DataContract(Namespace = "http://Microsoft.Samples.DCSurrogate")]
    public class Employee
    {
        [DataMember]
        public DateTime dateHired;

        [DataMember]
        public Decimal salary;

        [DataMember]
        public Person person;
    }

    // This is the Person (inner) type used in the sample.
    // Consider this a legacy type without DataContract or Serializable, so it cannot be serialized as is.
    public class Person
    {
        public string firstName;

        public string lastName;

        public int age;

        public Person() { }
    }

    // Service class which implements the service contract.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PersonnelDataService : IPersonnelDataService
    {
        Dictionary<string, Employee> employees = new Dictionary<string,Employee>();

        public void AddEmployee(Employee employee)
        {
            employees.Add(employee.person.firstName, employee);
        }

        public Employee GetEmployee(string name)
        {
            Employee employee;
            if (employees.TryGetValue(name, out employee))
                return employee;
            else
                return null;
        }

    }

}
