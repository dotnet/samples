
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;

namespace Microsoft.Samples.DCSurrogate
{
    // This is the surrogated version of the Person type
    // that will be used for its serialization/deserialization.
    [DataContract(Name="Person", Namespace = "http://Microsoft.Samples.DCSurrogate")]
    public class PersonSurrogated
    {
        [DataMember]
        public string FirstName;

        [DataMember]
        public string LastName;

        [DataMember]
        public int Age;
    }

    //This is the surrogate that substitutes PersonSurrogated for Person
    class AllowNonSerializableTypesSurrogate : IDataContractSurrogate
    {
        #region IDataContractSurrogate Members

        public Type GetDataContractType(Type type)
        {
            // "Person" will be serialized as "PersonSurrogated"
            // This method is called during serialization and schema export
            if (typeof(Person).IsAssignableFrom(type))
            {
                return typeof(PersonSurrogated);
            }
            return type;
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            //This method is called on serialization.
            //If we're serializing Person,...
            if (obj is Person)
            {
                Person person = (Person)obj;
                PersonSurrogated personSurrogated = new PersonSurrogated();
                personSurrogated.FirstName = person.firstName;
                personSurrogated.LastName = person.lastName;
                personSurrogated.Age = person.age;
                return personSurrogated;
            }
            return obj;
        }

        public object GetDeserializedObject(object obj, Type targetType)
        {
            //This method is called on deserialization.
            //If we're deserializing PersonSurrogated,...
            if (obj is PersonSurrogated)
            {
                PersonSurrogated personSurrogated = (PersonSurrogated)obj;
                Person person = new Person();
                person.firstName = personSurrogated.FirstName;
                person.lastName = personSurrogated.LastName;
                person.age = personSurrogated.Age;
                return person;
            }
            return obj;
        }

        public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            // This method is called on schema import.

            // What we say here is that if we see a PersonSurrogated data contract in the specified namespace,
            // we should not create a new type for it since we already have an existing type, "Person".
            if (typeNamespace.Equals("http://schemas.datacontract.org/2004/07/DCSurrogateSample"))
            {
                if (typeName.Equals("PersonSurrogated"))
                {
                    return typeof(Person);
                }
            }
            return null;
        }

        public System.CodeDom.CodeTypeDeclaration ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            // Not used in this sample.
            // We could use this method to construct an entirely new CLR type when a certain type is imported.
            return typeDeclaration;
        }

        public object GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            // Not used in this sample
            return null;
        }

        public object GetCustomDataToExport(System.Reflection.MemberInfo memberInfo, Type dataContractType)
        {
            // Not used in this sample
            return null;
        }

        public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
        {
            // Not used in this sample
        }

        #endregion
    }

}
