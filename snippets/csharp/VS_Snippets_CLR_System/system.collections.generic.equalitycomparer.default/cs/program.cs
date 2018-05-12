using System;
using System.Collections.Generic;

static class Program
{
    static void Main()
    {
        Box redBox = new Box(8, 8, 4);
        Box blueBox = new Box(6, 8, 4);
        Box greenBox = new Box(4, 8, 8);

        var boxes = new CustomCollection<Box>(new[] { redBox, blueBox, greenBox });

        var foundByDimension = boxes.FindFirst(greenBox);

        Console.WriteLine($"Found box {foundByDimension} by dimension.");

        var foundByVolume = boxes.FindFirst(greenBox, new BoxEqVolume());

        Console.WriteLine($"Found box {foundByVolume} by volume.");
    }
}

public class CustomCollection<T>
{
    private IEnumerable<T> items;

    public CustomCollection(IEnumerable<T> items) => this.items = items;

    public T FindFirst(T itemToFind, IEqualityComparer<T> comparer = null)
    {
        comparer = comparer ?? EqualityComparer<T>.Default;

        foreach (var item in items)
        {
            if (comparer.Equals(item, itemToFind))
            {
                return item;
            }
        }

        throw new InvalidOperationException("No macthing item found.");
    }
}

public class BoxEqVolume : EqualityComparer<Box>
{
    public override bool Equals(Box b1, Box b2)
    {
        if (b1 == b2)
            return true;

        if (b1 == null || b2 == null)
            return false;

        return b1.Volume == b2.Volume;
    }

    public override int GetHashCode(Box box) => box.Volume.GetHashCode();
}

public class Box : IEquatable<Box>
{
    public Box(int height, int length, int width)
    {
        this.Height = height;
        this.Length = length;
        this.Width = width;
    }

    public int Height { get; }
    public int Length { get; }
    public int Width { get; }

    public int Volume => Height * Length * Width;

    public bool Equals(Box other)
    {
        if (other == null)
            return false;

        return this.Height == other.Height && this.Length == other.Length
            && this.Width == other.Width;
    }

    public override bool Equals(object obj) => Equals(obj as Box);
    public override int GetHashCode() => (Height, Length, Width).GetHashCode();

    public override string ToString() => $"{Height} x {Length} x {Width}";
}

/* This example produces the following output:
 * 
    Found box 4 x 8 x 8 by dimension.
    Found box 8 x 8 x 4 by volume.
 */