using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace benchmark
{
    public readonly struct MyImmutableStruct
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        private readonly double a;
        private readonly double b;
        private readonly double c;
        private readonly double d;
        private readonly double e;
        private readonly double f;
        private readonly double g;
        private readonly double h;
        public MyImmutableStruct(double x, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
            a = 1;
            b = 2;
            c = 3;
            d = 4;
            e = 5;
            f = 6;
            g = 7;
            h = 8;
        }
    }

    public struct MyMutableStruct
    {
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public double Z { get => z; set => z = value; }

        private double a;
        private double b;
        private double c;
        private double d;
        private double e;
        private double f;
        private double g;
        private double h;
        private double z;
        private double y;
        private double x;

        public MyMutableStruct(double x, double y = 0, double z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.a = 1;
            this.b = 2;
            this.c = 3;
            this.d = 4;
            this.e = 5;
            this.f = 6;
            this.g = 7;
            this.h = 8;
        }
    }
    public struct MyMutableStructReadonly
    {
        public double X { readonly get => x; set => x = value; }
        public double Y { readonly get => y; set => y = value; }
        public double Z { readonly get => z; set => z = value; }

        private double a;
        private double b;
        private double c;
        private double d;
        private double e;
        private double f;
        private double g;
        private double h;
        private double z;
        private double y;
        private double x;

        public MyMutableStructReadonly(double x, double y = 0, double z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.a = 1;
            this.b = 2;
            this.c = 3;
            this.d = 4;
            this.e = 5;
            this.f = 6;
            this.g = 7;
            this.h = 8;
        }
    }

    public class Benchmarks
    {
        MyImmutableStruct sStruct = new MyImmutableStruct(1.1, 2.2);
        MyMutableStruct mStruct = new MyMutableStruct(1.1, 2.2);
        MyMutableStructReadonly mrStruct = new MyMutableStructReadonly(1.1, 2.2);

        [Benchmark]
        public double ImmutableAddByType()
        {
            return add_by_type(sStruct);
        }

        [Benchmark]
        public double ImmutableAddByRefType()
        {
            return add_by_reftype(in sStruct);
        }
        public double add_by_type(MyImmutableStruct s)
        {
            return s.X + s.Y;
        }
        public double add_by_reftype(in MyImmutableStruct s)
        {
            return s.X + s.Y;
        }

        [Benchmark]
        public double MutableAddByType()
        {
            return add_by_type(mStruct);
        }

        [Benchmark]
        public double MutableAddByRefType()
        {
            return add_by_reftype(in mStruct);
        }

        public double add_by_type(MyMutableStruct s)
        {
            return s.X + s.Y;
        }
        public double add_by_reftype(in MyMutableStruct s)
        {
            return s.X + s.Y;
        }

        [Benchmark]
        public double MutableReadOnlyAddByType()
        {
            return add_by_type(mrStruct);
        }

        [Benchmark]
        public double MutableReadonlyAddByRefType()
        {
            return add_by_reftype(in mrStruct);
        }

        public double add_by_type(MyMutableStructReadonly s)
        {
            return s.X + s.Y;
        }
        public double add_by_reftype(in MyMutableStructReadonly s)
        {
            return s.X + s.Y;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var Summary = BenchmarkRunner.Run<Benchmarks>();
        }
    }
}
