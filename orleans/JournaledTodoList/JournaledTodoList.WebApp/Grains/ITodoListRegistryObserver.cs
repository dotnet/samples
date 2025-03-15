using System.Collections.Immutable;

namespace JournaledTodoList.WebApp.Grains;

public interface ITodoListRegistryObserver : IGrainObserver
{
    Task OnTodoListsChanged(ImmutableArray<string> todoLists);
}
