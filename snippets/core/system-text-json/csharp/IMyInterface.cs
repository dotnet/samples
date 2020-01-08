namespace SystemTextJsonSamples
{
    public interface IMyInterface
    {
        public int Count { get; set; }
    }

    public class MyImplementation : IMyInterface
    {
        public int Count { get; set; }
        public string Name { get; set; }
    }

    public class PolymorphicTestInterface
    {
        public IMyInterface MyInterface { get; set; }
        public object MyObject { get; set; }
    }
}
