using System.Collections.Immutable;
using JournaledTodoList.WebApp.Grains;
using JournaledTodoList.WebApp.Grains.Events;
using JournaledTodoList.WebApp.Services;
using Microsoft.AspNetCore.Components;

namespace JournaledTodoList.WebApp.Components.Pages;

public partial class TodoListPage(TodoListService todoService)
{
    private string newItemTitle = "";
    private TodoList? todoList;
    private ImmutableArray<TodoListEvent> history;

    [Parameter, EditorRequired]
    public required string ListId { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (todoList?.Name != ListId)
        {
            todoList = null;
            await LoadTodoList();
        }
    }

    private async Task LoadTodoList()
    {
        todoList = await todoService.GetTodoListAsync(ListId);
        history = await todoService.GetTodoListHistoryAsync(ListId);
    }

    private async Task AddItem()
    {
        if (string.IsNullOrWhiteSpace(newItemTitle))
        {
            return;
        }

        await todoService.AddTodoItemAsync(ListId, newItemTitle);
        newItemTitle = "";
        await LoadTodoList();
    }

    private async Task UpdateItem(int itemId, string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        await todoService.UpdateTodoItemAsync(ListId, itemId, title);
        await LoadTodoList();
    }

    private async Task ToggleItem(int itemId)
    {
        await todoService.ToggleTodoItemAsync(ListId, itemId);
        await LoadTodoList();
    }

    private async Task RemoveItem(int itemId)
    {
        await todoService.RemoveTodoItemAsync(ListId, itemId);
        await LoadTodoList();
    }
}
