namespace JournaledTodoList.WebApp.Grains.Events;

[GenerateSerializer, Immutable]
public record class TodoItemRemoved(int ItemId, DateTimeOffset Timestamp) : TodoListEvent(ItemId, Timestamp)
{
    public override string GetDescription() => $"Removed item {ItemId}";
}
