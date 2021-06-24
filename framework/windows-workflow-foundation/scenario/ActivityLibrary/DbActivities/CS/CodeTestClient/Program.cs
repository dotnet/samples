//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Microsoft.Samples.Activities.Data
{

    public class Program
    {
        static string connectionString = @"Data Source=.\SQLExpress;Initial Catalog=DbActivitiesSample;Integrated Security=True";
        static string providerInvariantName = "System.Data.SqlClient";

        static void Main(string[] args)
        {
            Console.WriteLine("Select all Roles (using DbQuery and Func for mapping)");
            GetAllRoles();

            Console.WriteLine("");
            Console.WriteLine("Insert one record in Roles table");
            InsertArchRole();

            Console.WriteLine("");
            Console.WriteLine("Select Count from Roles table");
            GetRolesCount();

            Console.WriteLine("");
            Console.WriteLine("Select GetDate ");
            GetCurrentDate();

            Console.WriteLine("");
            Console.WriteLine("Select all Roles (using DbQuery and ActivityFunc for mapping)");
            GetAllRolesUsingActivityFuncMapping();

            Console.WriteLine("");
            Console.WriteLine("Select all Roles (using FindRolesActivity)");
            GetAllRolesUsingFindRoles();

            Console.WriteLine("");
            Console.WriteLine("Delete new added Role");
            DeleteRole();

            Console.WriteLine("");
            Console.WriteLine("Select Count from Roles table");
            GetRolesCount();

            Console.WriteLine("");
            Console.WriteLine("Select all Roles (using DbQueryDataSet)");
            GetAllRolesDataSet();          

            Console.WriteLine("");
            Console.WriteLine("Press Enter key to exit...");
            Console.ReadLine();
        }

        // Insert a new record using DbUpdate
        static void InsertArchRole()
        {
            Activity dbUpdate = new DbUpdate()
            {
                ProviderName = providerInvariantName,
                ConnectionString = connectionString,
                Sql = "INSERT INTO Roles (Code,Name) VALUES (@code, @name)",
                Parameters = 
                        {
                            { "@code", new InArgument<string>("ARCH") },
                            { "@name", new InArgument<string>("Architect") }
                        }
            };

            IDictionary<string, object> results = WorkflowInvoker.Invoke(dbUpdate);

            Console.WriteLine("Affected records: {0}", results["AffectedRecords"].ToString());
        }

        // Get the count of elements in the Roles table using DbQueryScalar        
        static void GetRolesCount()
        {
            Activity dbSelectCount = new DbQueryScalar<int>()
            {                
                ConfigName = "DbActivitiesSample",
                Sql = "SELECT COUNT(*) FROM Roles"
            };

            IDictionary<string, object> results = WorkflowInvoker.Invoke(dbSelectCount);
            Console.WriteLine("Result: {0}", results["Result"].ToString());
        }

        // Retrieve all roles from the database. Uses a Func<DataReader, Role> to map the results
        static void GetAllRoles()
        {
            Activity dbQuery = new DbQuery<Role>()
            {
                ProviderName = providerInvariantName,
                ConnectionString = connectionString,                 
                Sql = "SELECT * FROM Roles",
                Mapper = (dataReader) =>
                {
                    Role role = new Role();
                    role.Code = dataReader["code"].ToString();
                    role.Name = dataReader["name"].ToString();
                    return role;
                }
            };

            IDictionary<string, object> results = WorkflowInvoker.Invoke(dbQuery);
            IList<Role> roles = (IList<Role>)results["Result"];

            foreach (Role role in roles)
            {
                Console.WriteLine(role.ToString());
            }
        }

        // Retrieve all roles from the database. Uses an ActivityFunc<DataReader, Role> to map the results
        // Performance decreases (since the mapping is done in multiple pulses) but mapping can be serialized
        // to Xaml and authored declaratively in the the designer.
        static void GetAllRolesUsingActivityFuncMapping()
        {
            DelegateInArgument<DbDataReader> reader = new DelegateInArgument<DbDataReader>() { Name = "readerInArgument" };
            DelegateOutArgument<Role> roleOutArg = new DelegateOutArgument<Role>() { Name = "roleOutArgument" };

            Activity dbQuery = new DbQuery<Role>()
            {
                ConfigName = "DbActivitiesSample",
                Sql = "SELECT * FROM Roles",
                MapperFunc = new ActivityFunc<System.Data.Common.DbDataReader,Role>
                {
                    Argument = reader,
                    Handler = new Sequence
                    {
                        Activities = 
                        {
                            new Assign<Role> { To = roleOutArg, Value = new InArgument<Role>(c => new Role()) },                            
                            new Assign<string> 
                            { 
                                To = new OutArgument<string>(c => roleOutArg.Get(c).Code), 
                                Value = new InArgument<string>(c => reader.Get(c)["code"].ToString())
                            },
                            new Assign<string> 
                            { 
                                To = new OutArgument<string>(c => roleOutArg.Get(c).Name), 
                                Value = new InArgument<string>(c => reader.Get(c)["name"].ToString())
                            }            
                        }                        
                    },
                    Result = roleOutArg
                }
            };

            IDictionary<string, object> results = WorkflowInvoker.Invoke(dbQuery);
            IList<Role> roles = (IList<Role>)results["Result"];

            foreach (Role role in roles)
            {
                Console.WriteLine(role.ToString());
            }
        }

        // Retrieve all roles from the database using the FindAllRoles activity (see FindAllRoles.cs)
        static void GetAllRolesUsingFindRoles()
        {
            Activity dbQueryAllRoles = new FindAllRoles();

            IDictionary<string, object> results = WorkflowInvoker.Invoke(dbQueryAllRoles);
            IList<Role> roles = (IList<Role>)results["Result"];

            foreach (Role role in roles)
            {
                Console.WriteLine(role.ToString());
            }
        }

        // Deletes a role from the database
        static void DeleteRole()
        {
            Activity dbUpdate = new DbUpdate()
            {
                ProviderName = providerInvariantName,
                ConnectionString = connectionString,
                Sql = "DELETE FROM Roles WHERE code = @code",
                Parameters = 
                        {
                            { "@code", new InArgument<string>("ARCH") }
                        }
            };

            IDictionary<string, object> results = WorkflowInvoker.Invoke(dbUpdate);
            Console.WriteLine("Affected records: {0}", results["AffectedRecords"].ToString());
        }

        // Retrieves the current date using a DbQueryScalar
        static void GetCurrentDate()
        {
            Activity dbSelectCount = new DbQueryScalar<DateTime>()
            {
                ProviderName = providerInvariantName,
                ConnectionString = connectionString,
                Sql = "SELECT GetDate()"
            };

            IDictionary<string, object> results = WorkflowInvoker.Invoke(dbSelectCount);
            Console.WriteLine("Result: {0}", results["Result"].ToString());
        }

        // Retrieves all the roles as a DataSet using DbQueryDataSet
        static void GetAllRolesDataSet()
        {
            Activity dbQuery = new DbQueryDataSet()
            {
                ConfigName = "DbActivitiesSample",
                Sql = "SELECT * FROM Roles",
            };

            IDictionary<string, object> results = WorkflowInvoker.Invoke(dbQuery);
            DataSet roles = (DataSet)results["Result"];
            foreach (DataRow row in roles.Tables[0].Rows)
            {
                Console.WriteLine(string.Format("{0} - {1}", row["code"].ToString(), row["name"].ToString()));
            }
        }
    }
}
