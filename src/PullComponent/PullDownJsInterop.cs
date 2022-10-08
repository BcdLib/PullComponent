using Microsoft.JSInterop;

namespace BcdLib.Components;


public class PullDownJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public PullDownJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "/_content/BcdLib.PullComponent/dist/PullRefresh.js").AsTask());
    }

    public async ValueTask<string> InitAsync(string elementSelector, DotNetObjectReference<PullDown> objRef)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("PullRefresh.init", elementSelector, objRef);
    }

    public async ValueTask<int> GetScrollTopAsync()
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<int>("PullRefresh.getScrollTop");
    }


    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
