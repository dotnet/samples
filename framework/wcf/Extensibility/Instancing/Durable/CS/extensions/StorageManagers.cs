using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Microsoft.ServiceModel.Samples
{
    public interface IStorageManager
    {
        object GetInstance(string contextId, Type type);
        void SaveInstance(string contextId, object state);
    }

    //This class contains the implementation of IStorageManager 
    //which uses the SQL server as its back-end persistence storage 
    //medium.
    class SqlServerStorageManager : IStorageManager
    {
        public SqlServerStorageManager()
        {
        }

        /// <summary>
        /// Reads the instance from the SQL server. This method 
        /// returns null if an instance is not available for the 
        /// given context id.
        /// </summary>
        public object GetInstance(string contextId, Type type)
        {
            if (contextId == null)
                throw new ArgumentNullException("contextId");
            if (type == null)
                throw new ArgumentNullException("type");

            object data;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                string select = "SELECT Instance FROM Instances WHERE ContextId = @contextId";
                using (SqlCommand command = new SqlCommand(select, connection))
                {
                    command.Parameters.Add("@contextId", SqlDbType.VarChar, 256).Value = contextId;
                    data = command.ExecuteScalar();
                }
            }

            if (data != null)
            {
                XmlSerializer serializer = new XmlSerializer(type);
                using (StringReader reader = new StringReader((string)data))
                {
                    object instance = serializer.Deserialize(reader);
                    return instance;
                }
            }
            return null;
        }

        public void SaveInstance(string contextId, object state)
        {
            if (contextId == null)
                throw new ArgumentNullException("contextId");
            if (state == null)
                throw new ArgumentNullException("state");

            XmlSerializer serializer = new XmlSerializer(state.GetType());
            string data;

            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                serializer.Serialize(writer, state);
                data = writer.ToString();
            }

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                string update = @"UPDATE Instances SET Instance = @instance WHERE ContextId = @contextId";

                using (SqlCommand command = new SqlCommand(update, connection))
                {
                    command.Parameters.Add("@instance", SqlDbType.VarChar, 2147483647).Value = data;
                    command.Parameters.Add("@contextId", SqlDbType.VarChar, 256).Value = contextId;

                    int rows = command.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        string insert = @"INSERT INTO Instances(ContextId, Instance) VALUES(@contextId, @instance)";
                        command.CommandText = insert;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        string GetConnectionString()
        {
            string connectionString =
                ConfigurationManager.AppSettings[DurableInstanceContextUtility.ConnectionString];

            if (connectionString == null)
            {
                throw new InvalidOperationException(
                    ResourceHelper.GetString("ExConnectionStringNotFound"));
            }

            return connectionString;
        }
    }

    static class StorageManagerFactory
    {
        public static IStorageManager GetStorageManager(Type storageManagerType)
        {
            IStorageManager storageManager = null;

            if (storageManagerType == null)
            {
                return new SqlServerStorageManager();
            }
            else
            {
                object obj = Activator.CreateInstance(storageManagerType);

                // Throw if the specified storage manager type does not
                // implement IStorageManager.
                if (obj is IStorageManager)
                {
                    storageManager = (IStorageManager)obj;
                }
                else
                {
                    throw new InvalidOperationException(
                        ResourceHelper.GetString("ExInvalidStorageManager"));
                }

                return storageManager;
            }
        }
    }
}

