using System.Collections.Immutable;
using JournaledTodoList.WebApp.Grains;
using JournaledTodoList.WebApp.Services;

namespace JournaledTodoList.WebApp.Components.Layout;

public partial class NavBar(TodoListService todoListService) : ITodoListRegistryObserver, IDisposable
{
    private IDisposable? subscription;

    private ImmutableArray<string> TodoLists { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        TodoLists = await todoListService.GetAllTodoListsAsync();
        subscription = await todoListService.SubscribeAsync(this);
    }

    public void Dispose()
    {
        subscription?.Dispose();
    }

    async Task ITodoListRegistryObserver.OnTodoListsChanged(ImmutableArray<string> todoLists) => await InvokeAsync(() =>
    {
        TodoLists = todoLists;
        StateHasChanged();
    });
}
