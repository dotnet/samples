using System;
using System.Data;

public class Sample
{

    // <Snippet1>
    public static void RemoveConstraint(
        ConstraintCollection constraints, Constraint constraint)
    {
        try
        {
            if (constraints.Contains(constraint.ConstraintName) && constraints.CanRemove(constraint))
            {
                constraints.Remove(constraint.ConstraintName);
            }
        }
        catch (Exception e) 
        {
            // Process exception and return.
            Console.WriteLine($"Exception of type {e.GetType()} occurred.");
        }
    }
    // </Snippet1>

}
