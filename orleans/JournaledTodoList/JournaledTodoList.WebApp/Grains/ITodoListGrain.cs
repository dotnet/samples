using System.Collections.Immutable;
using JournaledTodoList.WebApp.Grains.Events;

namespace JournaledTodoList.WebApp.Grains;

public interface ITodoListGrain : IGrainWithStringKey
{
    Task<TodoList> GetTodoListAsync();

    Task AddTodoItemAsync(string title);

    Task ToggleTodoItemAsync(int id);

    Task UpdateTodoItemAsync(int id, string title);

    Task RemoveTodoItemAsync(int id);

    Task<ImmutableArray<TodoListEvent>> GetHistoryAsync();
}
