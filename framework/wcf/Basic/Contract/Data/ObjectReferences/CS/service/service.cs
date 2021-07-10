
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Microsoft.Samples.ObjectReferences
{
    // Define a service contract.
    [ServiceContract(Namespace = "Microsoft.Samples.ObjectReferences")]
    public interface ISocialNetwork
    {
        [OperationContract]
        List<Person> GetPeopleInNetwork(Person p);
        [OperationContract]
        List<Person> GetMutualFriends(Person p);
        [OperationContract]
        List<Person> GetCommonFriends(List<Person> p);
    }

    [DataContract(IsReference=true)]
    public class Person
    {
        string name;
        string location;
        string gender;
        int age;
        List<Person> friends;
        [DataMember()]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [DataMember()]
        public string Location
        {
            get { return location; }
            set { location = value; }
        }
        [DataMember()]
        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        [DataMember()]
        public int Age
        {
            get { return age; }
            set { age = value; }
        }
        [DataMember()]
        public List<Person> Friends
        {
            get { if (friends == null) friends = new List<Person>();  return friends; }
            set { friends = value; }
        }

        public Person()
        {
        }
    }

    // Service class which implements the service contract.
    public class SocialNetworkService : ISocialNetwork
    {
        public List<Person> GetPeopleInNetwork(Person p)
        {
            List<Person> people = new List<Person>();
            ListPeopleInNetwork(p, people);
            return people;
            
        }
        public List<Person> GetMutualFriends(Person p)
        {
            List<Person> mutual = new List<Person>();
            foreach (Person friend in p.Friends)
            {
                if (friend.Friends.Contains(p))
                    mutual.Add(friend);
            }
            return mutual;
        }
        public List<Person> GetCommonFriends(List<Person> people)
        {
            List<Person> common = new List<Person>();
            foreach (Person friend in people[0].Friends)
                if (people[1].Friends.Contains(friend))
                    common.Add(friend);
            return common;
        }
        
        void ListPeopleInNetwork(Person p, List<Person> lst)
        {
            if (!lst.Contains(p))
            {
                lst.Add(p);
                foreach (Person friend in p.Friends)
                    ListPeopleInNetwork(friend, lst);
            }
        }
    }

}
