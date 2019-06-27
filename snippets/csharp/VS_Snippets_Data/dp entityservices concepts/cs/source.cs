//<snippetNamespaces>
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.Common;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
//</snippetNamespaces>
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;


namespace Microsoft.Samples.Entity
{
    class Source
    {
        static public void PolymorphicQuery()
        {
            //<snippetPolymorphicQuery>
            using (EntityConnection conn = new EntityConnection("name=SchoolEntities"))
            {
                conn.Open();
                // Create a query that specifies to 
                // get a collection of only OnsiteCourses.

                string esqlQuery = @"SELECT VAlUE onsiteCourse FROM 
                    OFTYPE(SchoolEntities.Courses, SchoolModel.OnsiteCourse) 
                    AS onsiteCourse";
                using (EntityCommand cmd = new EntityCommand(esqlQuery, conn))
                {
                    // Execute the command.
                    using (DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        // Start reading.
                        while (rdr.Read())
                        {
                            // Display OnsiteCourse's location.
                            Console.WriteLine("CourseID: {0} ", rdr["CourseID"]);
                            Console.WriteLine("Location: {0} ", rdr["Location"]);
                        }
                    }
                }
            }
            //</snippetPolymorphicQuery>
        }

        static public void ComplexTypeWithEntityCommand()
        {
            //<snippetComplexTypeWithEntityCommand>
            using (EntityConnection conn =
                new EntityConnection("name=AdventureWorksEntities"))
            {
                conn.Open();

                string esqlQuery = @"SELECT VALUE contacts FROM
                        AdventureWorksEntities.Contacts AS contacts 
                        WHERE contacts.ContactID == @id";

                // Create an EntityCommand.
                using (EntityCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = esqlQuery;
                    EntityParameter param = new EntityParameter();
                    param.ParameterName = "id";
                    param.Value = 3;
                    cmd.Parameters.Add(param);

                    // Execute the command.
                    using (EntityDataReader rdr =
                        cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        // The result returned by this query contains 
                        // Address complex Types.
                        while (rdr.Read())
                        {
                            // Display CustomerID
                            Console.WriteLine("Contact ID: {0}",
                                rdr["ContactID"]);
                            // Display Address information.
                            DbDataRecord nestedRecord =
                                rdr["EmailPhoneComplexProperty"] as DbDataRecord;
                            Console.WriteLine("Email and Phone Info:");
                            for (int i = 0; i < nestedRecord.FieldCount; i++)
                            {
                                Console.WriteLine("  " + nestedRecord.GetName(i) +
                                    ": " + nestedRecord.GetValue(i));
                            }
                        }
                    }
                }
                conn.Close();
            }
            //</snippetComplexTypeWithEntityCommand>
        }

        static public void StoredProcWithEntityCommand()
        {
            //<snippetStoredProcWithEntityCommand>
            using (EntityConnection conn =
                new EntityConnection("name=SchoolEntities"))
            {
                conn.Open();
                // Create an EntityCommand.
                using (EntityCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SchoolEntities.GetStudentGrades";
                    cmd.CommandType = CommandType.StoredProcedure;
                    EntityParameter param = new EntityParameter();
                    param.Value = 2;
                    param.ParameterName = "StudentID";
                    cmd.Parameters.Add(param);

                    // Execute the command.
                    using (EntityDataReader rdr =
                        cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        // Read the results returned by the stored procedure.
                        while (rdr.Read())
                        {
                            Console.WriteLine("ID: {0} Grade: {1}", rdr["StudentID"], rdr["Grade"]);
                        }
                    }
                }
                conn.Close();
            }
            //</snippetStoredProcWithEntityCommand>
        }

        static public void BuildingConnectionStringWithEntityCommand()
        {
            //<snippetBuildingConnectionStringWithEntityCommand>

            // Specify the provider name, server and database.
            string providerName = "System.Data.SqlClient";
            string serverName = ".";
            string databaseName = "AdventureWorks";

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder =
                new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.IntegratedSecurity = true;

            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
                new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata = @"res://*/AdventureWorksModel.csdl|
                                        res://*/AdventureWorksModel.ssdl|
                                        res://*/AdventureWorksModel.msl";
            Console.WriteLine(entityBuilder.ToString());

            using (EntityConnection conn =
                new EntityConnection(entityBuilder.ToString()))
            {
                conn.Open();
                Console.WriteLine("Just testing the connection.");
                conn.Close();
            }
            //</snippetBuildingConnectionStringWithEntityCommand>
        }

        static public void ParameterizedQueryWithEntityCommand()
        {
            //<snippetParameterizedQueryWithEntityCommand>
            using (EntityConnection conn =
                new EntityConnection("name=AdventureWorksEntities"))
            {
                conn.Open();
                // Create a query that takes two parameters.
                string esqlQuery =
                    @"SELECT VALUE Contact FROM AdventureWorksEntities.Contacts 
                                AS Contact WHERE Contact.LastName = @ln AND
                                Contact.FirstName = @fn";

                using (EntityCommand cmd = new EntityCommand(esqlQuery, conn))
                {
                    // Create two parameters and add them to 
                    // the EntityCommand's Parameters collection 
                    EntityParameter param1 = new EntityParameter();
                    param1.ParameterName = "ln";
                    param1.Value = "Adams";
                    EntityParameter param2 = new EntityParameter();
                    param2.ParameterName = "fn";
                    param2.Value = "Frances";

                    cmd.Parameters.Add(param1);
                    cmd.Parameters.Add(param2);

                    using (DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        // Iterate through the collection of Contact items.
                        while (rdr.Read())
                        {
                            Console.WriteLine(rdr["FirstName"]);
                            Console.WriteLine(rdr["LastName"]);
                        }
                    }
                }
                conn.Close();
            }
            //</snippetParameterizedQueryWithEntityCommand>
        }

        static public void NavigateWithNavOperatorWithEntityCommand()
        {
            //<snippetNavigateWithNavOperatorWithEntityCommand>
            using (EntityConnection conn =
                new EntityConnection("name=AdventureWorksEntities"))
            {
                conn.Open();
                // Create an EntityCommand.
                using (EntityCommand cmd = conn.CreateCommand())
                {
                    // Create an Entity SQL query.
                    string esqlQuery =
                        @"SELECT address.AddressID, (SELECT VALUE DEREF(soh) FROM 
                      NAVIGATE(address, AdventureWorksModel.FK_SalesOrderHeader_Address_BillToAddressID) 
                      AS soh) FROM AdventureWorksEntities.Addresses AS address";

                    
                    cmd.CommandText = esqlQuery;

                    // Execute the command.
                    using (DbDataReader rdr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        // Start reading.
                        while (rdr.Read())
                        {
                            Console.WriteLine(rdr["AddressID"]);
                        }
                    }
                }
                conn.Close();
            }
            //</snippetNavigateWithNavOperatorWithEntityCommand>
        }

        static public void ReturnNestedCollectionWithEntityCommand()
        {
            //<snippetReturnNestedCollectionWithEntityCommand>
            using (EntityConnection conn =
                new EntityConnection("name=AdventureWorksEntities"))
            {
                conn.Open();
                // Create an EntityCommand.
                using (EntityCommand cmd = conn.CreateCommand())
                {
                    // Create a nested query.
                    string esqlQuery =
                        @"Select c.ContactID, c.SalesOrderHeaders
                    From AdventureWorksEntities.Contacts as c";

                    cmd.CommandText = esqlQuery;
                    // Execute the command.
                    using (EntityDataReader rdr =
                        cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        // The result returned by this query contains 
                        // ContactID and a nested collection of SalesOrderHeader items.
                        // associated with this Contact.
                        while (rdr.Read())
                        {
                            // the first column contains Contact ID.
                            Console.WriteLine("Contact ID: {0}", rdr["ContactID"]);

                            // The second column contains a collection of SalesOrderHeader 
                            // items associated with the Contact.
                            DbDataReader nestedReader = rdr.GetDataReader(1);
                            while (nestedReader.Read())
                            {
                                Console.WriteLine("   SalesOrderID: {0} ", nestedReader["SalesOrderID"]);
                                Console.WriteLine("   OrderDate: {0} ", nestedReader["OrderDate"]);
                            }
                        }
                    }
                }
                conn.Close();
            }
            //</snippetReturnNestedCollectionWithEntityCommand>
        }

        //string esqlQuery = @"SELECT VALUE Product FROM AdventureWorksEntities.Products AS Product";
        //<snippeteSQLStructuralTypes>
        static void ExecuteStructuralTypeQuery(string esqlQuery)
        {
            if (esqlQuery.Length == 0)
            {
                Console.WriteLine("The query string is empty.");
                return;
            }

            using (EntityConnection conn =
                new EntityConnection("name=AdventureWorksEntities"))
            {
                conn.Open();

                // Create an EntityCommand.
                using (EntityCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = esqlQuery;
                    // Execute the command.
                    using (EntityDataReader rdr =
                        cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        // Start reading results.
                        while (rdr.Read())
                        {
                            StructuralTypeVisitRecord(rdr as IExtendedDataRecord);
                        }
                    }
                }
                conn.Close();
            }
        }

        static void StructuralTypeVisitRecord(IExtendedDataRecord record)
        {
            int fieldCount = record.DataRecordInfo.FieldMetadata.Count;
            for (int fieldIndex = 0; fieldIndex < fieldCount; fieldIndex++)
            {
                Console.Write(record.GetName(fieldIndex) + ": ");

                // If the field is flagged as DbNull, the shape of the value is undetermined.
                // An attempt to get such a value may trigger an exception.
                if (record.IsDBNull(fieldIndex) == false)
                {
                    BuiltInTypeKind fieldTypeKind = record.DataRecordInfo.FieldMetadata[fieldIndex].
                        FieldType.TypeUsage.EdmType.BuiltInTypeKind;
                    // The EntityType, ComplexType and RowType are structural types
                    // that have members. 
                    // Read only the PrimitiveType members of this structural type.
                    if (fieldTypeKind == BuiltInTypeKind.PrimitiveType)
                    {
                        // Primitive types are surfaced as plain objects.
                        Console.WriteLine(record.GetValue(fieldIndex).ToString());
                    }
                }
            }
        }
        //</snippeteSQLStructuralTypes>


        //string esqlQuery = @"SELECT REF(p) FROM AdventureWorksEntities.Products as p";
        //<snippeteSQLRefTypes>
        static public void ExecuteRefTypeQuery(string esqlQuery)
        {
            if (esqlQuery.Length == 0)
            {
                Console.WriteLine("The query string is empty.");
                return;
            }

            using (EntityConnection conn =
                new EntityConnection("name=AdventureWorksEntities"))
            {
                conn.Open();

                // Create an EntityCommand.
                using (EntityCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = esqlQuery;
                    // Execute the command.
                    using (EntityDataReader rdr =
                        cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        // Start reading results.
                        while (rdr.Read())
                        {
                            RefTypeVisitRecord(rdr as IExtendedDataRecord);
                        }
                    }
                }
                conn.Close();
            }
        }

        static void RefTypeVisitRecord(IExtendedDataRecord record)
        {
            // For RefType the record contains exactly one field.
            int fieldIndex = 0;

            // If the field is flagged as DbNull, the shape of the value is undetermined.
            // An attempt to get such a value may trigger an exception.
            if (record.IsDBNull(fieldIndex) == false)
            {
                BuiltInTypeKind fieldTypeKind = record.DataRecordInfo.FieldMetadata[fieldIndex].
                    FieldType.TypeUsage.EdmType.BuiltInTypeKind;
                //read only fields that contain PrimitiveType
                if (fieldTypeKind == BuiltInTypeKind.RefType)
                {
                    // Ref types are surfaced as EntityKey instances. 
                    // The containing record sees them as atomic.
                    EntityKey key = record.GetValue(fieldIndex) as EntityKey;
                    // Get the EntitySet name.
                    Console.WriteLine("EntitySetName " + key.EntitySetName);
                    // Get the Name and the Value information of the EntityKey.
                    foreach (EntityKeyMember keyMember in key.EntityKeyValues)
                    {
                        Console.WriteLine("   Key Name: " + keyMember.Key);
                        Console.WriteLine("   Key Value: " + keyMember.Value);
                    }
                }
            }
        }
        //</snippeteSQLRefTypes>


        //string esqlQuery = @"SELECT VALUE AVG(p.ListPrice) FROM AdventureWorksEntities.Products as p";
        //<snippeteSQLPrimitiveTypes>
        static void ExecutePrimitiveTypeQuery(string esqlQuery)
        {
            if (esqlQuery.Length == 0)
            {
                Console.WriteLine("The query string is empty.");
                return;
            }

            using (EntityConnection conn =
                new EntityConnection("name=AdventureWorksEntities"))
            {
                conn.Open();

                // Create an EntityCommand.
                using (EntityCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = esqlQuery;
                    // Execute the command.
                    using (EntityDataReader rdr =
                        cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        // Start reading results.
                        while (rdr.Read())
                        {
                            IExtendedDataRecord record = rdr as IExtendedDataRecord;
                            // For PrimitiveType 
                            // the record contains exactly one field.
                            int fieldIndex = 0;
                            Console.WriteLine("Value: " + record.GetValue(fieldIndex));
                        }
                    }
                }
                conn.Close();
            }
        }
        //</snippeteSQLPrimitiveTypes>

        public static void TestESQL()
        {
            EntitySQL.TestEntitySQL();
        }
    }
}
