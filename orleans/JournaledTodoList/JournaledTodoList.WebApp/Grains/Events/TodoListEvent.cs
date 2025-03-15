namespace JournaledTodoList.WebApp.Grains.Events;

[GenerateSerializer, Immutable]
public abstract record class TodoListEvent(int ItemId, DateTimeOffset Timestamp)
{
    public abstract string GetDescription();
}
