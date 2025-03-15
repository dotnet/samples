using System.Collections.Immutable;
using Orleans.Utilities;

namespace JournaledTodoList.WebApp.Grains;

public sealed class TodoListRegistryGrain(
    [PersistentState("todoListRegistry", "Default")]
    IPersistentState<TodoListRegistryGrain.TodoListRegistry> state,
    ILogger<TodoListRegistryGrain> logger) : Grain, ITodoListRegistryGrain
{
    private readonly ObserverManager<ITodoListRegistryObserver> observers = new(
        TimeSpan.FromMinutes(5),
        logger);

    public async Task RegisterTodoListAsync(string todoListId)
    {
        if (state.State.TodoListIds.Contains(todoListId))
        {
            return;
        }

        state.State.TodoListIds.Add(todoListId);
        await state.WriteStateAsync();

        // Notify observers
        NotifyObservers();
    }

    public Task<ImmutableArray<string>> GetAllTodoListsAsync()
    {
        return Task.FromResult(state.State.TodoListIds.ToImmutableArray());
    }

    public Task Subscribe(ITodoListRegistryObserver observer)
    {
        observers.Subscribe(observer, observer);

        // Send current state to new observer
        observer.OnTodoListsChanged(state.State.TodoListIds.ToImmutableArray());

        return Task.CompletedTask;
    }

    public Task Unsubscribe(ITodoListRegistryObserver observer)
    {
        observers.Unsubscribe(observer);
        return Task.CompletedTask;
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        observers.Clear();
        return base.OnDeactivateAsync(reason, cancellationToken);
    }

    private void NotifyObservers()
    {
        var todoLists = state.State.TodoListIds.ToImmutableArray();
        observers.Notify(observer => observer.OnTodoListsChanged(todoLists));
    }

    public sealed class TodoListRegistry
    {
        public HashSet<string> TodoListIds { get; set; } = [];
    }
}
