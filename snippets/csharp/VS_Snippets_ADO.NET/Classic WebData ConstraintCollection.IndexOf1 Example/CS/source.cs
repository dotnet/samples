using System;
using System.Data;
using System.Windows.Forms;

public class Form1: Form
{
    protected DataSet DataSet1;


    // <Snippet1>
    private void RemoveConstraint(
        ConstraintCollection constraints, Constraint constraint)
    {
        try
        {
            if (constraints.Contains(constraint.ConstraintName) && constraints.CanRemove(constraint)) 
            {
                constraints.RemoveAt(constraints.IndexOf(constraint));
            }
        }
        catch(Exception e) 
        {
            // Process exception and return.
            Console.WriteLine($"Exception of type {e.GetType()} occurred.");
        }
    }
    // </Snippet1>

}
