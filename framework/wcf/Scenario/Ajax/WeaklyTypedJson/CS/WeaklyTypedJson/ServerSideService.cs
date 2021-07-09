//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Microsoft.Samples.WeaklyTypedJson
{
    [ServiceContract]
    public class ServerSideProfileService
    {
        // The service returns the MemberProfile complex type

        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        public MemberProfile GetMemberProfile()
        {
            MemberProfile member = new MemberProfile();
            member.personal = new PersonalInfo();

            member.personal.name = "Paul";
            member.personal.age = 23;
            member.personal.height = 1.7;
            member.personal.isSingle = true;
            member.personal.luckyNumbers = new int[3]{5, 17, 21};

            member.favoriteBands = new string[2]{"Band ABC", "Band XYZ"};

            return member;
        }
    }

    [DataContract]
    public class MemberProfile
    {
        [DataMember]
        public PersonalInfo personal;

        [DataMember]
        public string[] favoriteBands;
    }

    [DataContract]
    public class PersonalInfo
    {
        [DataMember]
        public string name;

        [DataMember]
        public int age;

        [DataMember]
        public double height;

        [DataMember]
        public bool isSingle;

        [DataMember]
        public int[] luckyNumbers;
    }
}
