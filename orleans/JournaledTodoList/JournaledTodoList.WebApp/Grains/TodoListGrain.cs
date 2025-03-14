using System.Collections.Immutable;
using JournaledTodoList.WebApp.Grains.Events;
using Orleans.EventSourcing;

namespace JournaledTodoList.WebApp.Grains;

public sealed class TodoListGrain : JournaledGrain<TodoListGrain.TodoListProjection, TodoListEvent>, ITodoListGrain
{
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var registry = GrainFactory.GetGrain<ITodoListRegistryGrain>("registry");
        await registry.RegisterTodoListAsync(this.GetPrimaryKeyString());
    }

    public Task<TodoList> GetTodoListAsync()
    {
        var list = new TodoList(
            Name: this.GetPrimaryKeyString(),
            Items: State.Items.Values.ToImmutableArray(),
            Timestamp: State.Timestamp);

        return Task.FromResult(list);
    }

    public async Task AddTodoItemAsync(string title)
    {
        var evt = new TodoItemAdded(
            Version,
            DateTimeOffset.UtcNow,
            title);

        RaiseEvent(evt);
        await ConfirmEvents();
    }

    public async Task UpdateTodoItemAsync(int id, string title)
    {
        var evt = new TodoItemUpdated(id, DateTimeOffset.UtcNow, title);
        RaiseEvent(evt);
        await ConfirmEvents();
    }

    public async Task ToggleTodoItemAsync(int id)
    {
        var evt = new TodoItemToggled(id, DateTimeOffset.UtcNow);
        RaiseEvent(evt);
        await ConfirmEvents();
    }

    public async Task RemoveTodoItemAsync(int id)
    {
        var evt = new TodoItemRemoved(id, DateTimeOffset.UtcNow);
        RaiseEvent(evt);
        await ConfirmEvents();
    }

    public async Task<ImmutableArray<TodoListEvent>> GetHistoryAsync()
    {
        var events = await RetrieveConfirmedEvents(0, Version);
        return events.ToImmutableArray();
    }

    /// <summary>
    /// The state container for <see cref="TodoListGrain"/>.
    /// NOTE: this has to be a mutable object.
    /// </summary>
    public sealed class TodoListProjection
    {
        public Dictionary<int, TodoItem> Items { get; set; } = [];

        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.MinValue;

        public void Apply(TodoItemAdded added)
        {
            Items.Add(added.ItemId, new TodoItem(added.ItemId, added.Title, false));
            Timestamp = added.Timestamp;
        }

        public void Apply(TodoItemUpdated updated)
        {
            if (Items.TryGetValue(updated.ItemId, out var item))
            {
                Items[updated.ItemId] = item with { Title = updated.Title };
            }
            Timestamp = updated.Timestamp;
        }

        public void Apply(TodoItemToggled toggled)
        {
            if (Items.TryGetValue(toggled.ItemId, out var item))
            {
                Items[toggled.ItemId] = item with { IsCompleted = !item.IsCompleted };
            }
            Timestamp = toggled.Timestamp;
        }

        public void Apply(TodoItemRemoved removed)
        {
            Items.Remove(removed.ItemId);
            Timestamp = removed.Timestamp;
        }
    }
}
