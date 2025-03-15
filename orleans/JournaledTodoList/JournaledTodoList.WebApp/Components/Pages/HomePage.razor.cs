using System.Collections.Immutable;
using JournaledTodoList.WebApp.Services;

namespace JournaledTodoList.WebApp.Components.Pages;

public partial class HomePage(TodoListService todoListService)
{
    private string newListName = "";
    private ImmutableArray<string> todoLists = [];

    protected override async Task OnInitializedAsync()
    {
        todoLists = await todoListService.GetAllTodoListsAsync();
        await CreateNewList("Default");
    }

    private async Task CreateNewList(string listName)
    {
        var normalizedName = NormalizeListName(listName);
        if (string.IsNullOrWhiteSpace(normalizedName) || todoLists.Contains(normalizedName))
        {
            return;
        }

        await todoListService.CreateTodoListAsync(normalizedName);
        todoLists = await todoListService.GetAllTodoListsAsync();
        newListName = "";
    }

    private static string NormalizeListName(string name)
    {
        // Replace spaces and special characters to ensure valid URL
        return name.Trim()
                  .Replace(" ", "-")
                  .Replace("/", "-")
                  .Replace("\\", "-");
    }
}
