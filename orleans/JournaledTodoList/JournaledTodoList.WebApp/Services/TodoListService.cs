using System.Collections.Immutable;
using JournaledTodoList.WebApp.Grains;
using JournaledTodoList.WebApp.Grains.Events;

namespace JournaledTodoList.WebApp.Services;

public class TodoListService(IGrainFactory grainFactory)
{
    public async Task<IDisposable> SubscribeAsync(ITodoListRegistryObserver observer)
    {
        var registryGrain = grainFactory.GetGrain<ITodoListRegistryGrain>("registry");
        var observerRef = grainFactory.CreateObjectReference<ITodoListRegistryObserver>(observer);
        await registryGrain.Subscribe(observerRef);
        return new Subscription(observerRef, registryGrain);
    }

    public Task<ImmutableArray<string>> GetAllTodoListsAsync()
    {
        var registryGrain = grainFactory.GetGrain<ITodoListRegistryGrain>("registry");
        return registryGrain.GetAllTodoListsAsync();
    }

    public async Task CreateTodoListAsync(string listId)
    {
        // Just accessing the grain is enough to have it register itself
        var grain = grainFactory.GetGrain<ITodoListGrain>(listId);
        await grain.GetTodoListAsync();
    }

    public Task<TodoList> GetTodoListAsync(string listId)
    {
        var grain = grainFactory.GetGrain<ITodoListGrain>(listId);
        return grain.GetTodoListAsync();
    }

    public Task<TodoList?> GetTodoListAtTimestampAsync(string listId, DateTimeOffset timestamp)
    {
        var grain = grainFactory.GetGrain<ITodoListGrain>(listId);
        return grain.GetTodoListAtTimestampAsync(timestamp);
    }

    public Task<ImmutableArray<TodoListEvent>> GetTodoListHistoryAsync(string listId)
    {
        var grain = grainFactory.GetGrain<ITodoListGrain>(listId);
        return grain.GetHistoryAsync();
    }

    public Task AddTodoItemAsync(string listId, string title)
    {
        var grain = grainFactory.GetGrain<ITodoListGrain>(listId);
        return grain.AddTodoItemAsync(title);
    }

    public Task UpdateTodoItemAsync(string listId, int itemId, string title)
    {
        var grain = grainFactory.GetGrain<ITodoListGrain>(listId);
        return grain.UpdateTodoItemAsync(itemId, title);
    }

    public Task ToggleTodoItemAsync(string listId, int itemId)
    {
        var grain = grainFactory.GetGrain<ITodoListGrain>(listId);
        return grain.ToggleTodoItemAsync(itemId);
    }

    public Task RemoveTodoItemAsync(string listId, int itemId)
    {
        var grain = grainFactory.GetGrain<ITodoListGrain>(listId);
        return grain.RemoveTodoItemAsync(itemId);
    }

    private sealed class Subscription(
        ITodoListRegistryObserver observerRef,
        ITodoListRegistryGrain registryGrain) : IDisposable
    {
        public void Dispose()
        {
            registryGrain.Unsubscribe(observerRef);
        }
    }
}
