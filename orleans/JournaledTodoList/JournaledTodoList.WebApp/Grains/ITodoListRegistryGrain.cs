using System.Collections.Immutable;

namespace JournaledTodoList.WebApp.Grains;

public interface ITodoListRegistryGrain : IGrainWithStringKey
{
    Task RegisterTodoListAsync(string todoListId);

    Task<ImmutableArray<string>> GetAllTodoListsAsync();

    Task Subscribe(ITodoListRegistryObserver observer);

    Task Unsubscribe(ITodoListRegistryObserver observer);
}
