using Orleans;

namespace HelloWorld;

public interface IHelloGrain : IGrainWithStringKey
{
    Task<string> SayHello(string greeting);
}
