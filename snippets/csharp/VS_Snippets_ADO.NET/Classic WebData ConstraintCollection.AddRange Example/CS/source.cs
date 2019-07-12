using System;
using System.Data;

public class Sample
{

    // <Snippet1>
    public static void ConstraintAddRange(DataSet dataSet)
    {
        try
        {
            // Reference the tables from the DataSet.
            DataTable customersTable = dataSet.Tables["Customers"];
            DataTable ordersTable = dataSet.Tables["Orders"];

            // Create unique and foreign key constraints.
            var uniqueConstraint = new UniqueConstraint(customersTable.Columns["CustomerID"]);
            var fkConstraint = new ForeignKeyConstraint("CustOrdersConstraint",
                customersTable.Columns["CustomerID"],
                ordersTable.Columns["CustomerID"]);

            // Add the constraints.
            customersTable.Constraints.AddRange(new Constraint[] 
                {uniqueConstraint, fkConstraint});
        }
        catch(Exception ex)
        {
            // Process exception and return.
            Console.WriteLine($"Exception of type {ex.GetType()} occurred.");
        }
    }
    // </Snippet1>

}
