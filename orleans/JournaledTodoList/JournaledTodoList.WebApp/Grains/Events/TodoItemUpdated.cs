namespace JournaledTodoList.WebApp.Grains.Events;

[GenerateSerializer, Immutable]
public record class TodoItemUpdated(int ItemId, DateTimeOffset Timestamp, string Title) : TodoListEvent(ItemId, Timestamp)
{
    public override string GetDescription() => $"Updated item {ItemId}: {Title}";
}
