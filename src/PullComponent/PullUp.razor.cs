using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Reflection.Metadata;
using System.Xml;

namespace BcdLib.Components;

public partial class PullUp
{
    [Inject]
    public DocumentJsInterop DocumentJs { get; set; } = default!;

    #region Parameter

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Func<Task<bool>>? OnRefreshing { get; set; }

    [Parameter]
    public RenderFragment LoadingTip { get; set; } = builder =>
    {
        builder.AddContent(0, "加载中...");
    };

    [Parameter]
    public RenderFragment CompletedTip { get; set; } = builder =>
    {
        builder.AddContent(0, "加载完成");
    };

    [Parameter]
    public RenderFragment FailedTip { get; set; } = builder =>
    {
        builder.AddContent(0, "暂无更多数据");
    };

    [Parameter]
    public int MaxDistance { get; set; } = 10;
    
    #endregion

    private PullUpStatus pullStatus = PullUpStatus.Awaiting;

    private RenderFragment? GetTipHtml()
    {
        var renderFragment = pullStatus switch
        {
            PullUpStatus.Loading => LoadingTip,
            PullUpStatus.Completed => CompletedTip,
            PullUpStatus.Failed => FailedTip,
            _ => null,
        };
        return renderFragment;
    }

    private string GetTipStyle()
    {
        return pullStatus switch
        {
            PullUpStatus.Loading => "display: flex;",
            PullUpStatus.Completed => "display: flex;",
            PullUpStatus.Failed => "display: flex;",
            _ => "display: none;",
        };
    }


    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Window.OnScroll += Window_OnScroll;
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    private bool hasLoadAll = false;
    private Task _lastTask = Task.CompletedTask;
    private Task Window_OnScroll(ScrollEvent arg)
    {
        var task = new Task(async () =>
        {
            if (hasLoadAll)
            {
                Window.OnScroll -= Window_OnScroll;
                return;
            }
            Console.WriteLine(arg.DistToBottom);
            if (arg.DistToBottom <= MaxDistance)
            {
                SetPullStatus(PullUpStatus.Loading);
                StateHasChanged();
                if (OnRefreshing != null)
                {
                    if (await OnRefreshing())
                    {
                        SetPullStatus(PullUpStatus.Completed);
                        StateHasChanged();
                    }
                    else
                    {
                        hasLoadAll = true;
                        SetPullStatus(PullUpStatus.Failed);
                        StateHasChanged();
                        await Task.Delay(1500);
                    }
                }
                SetPullStatus(PullUpStatus.Awaiting);
                StateHasChanged();
            }
        });
        if (_lastTask.IsCompleted)
        {
            _lastTask = task;
            _lastTask.Start();
        }
        else
        {
            _lastTask = _lastTask.ContinueWith(async t =>
            {
                await task;
            });
        }
        return Task.CompletedTask;
    }

    private void SetPullStatus(PullUpStatus newPullStatus)
    {
        if (this.pullStatus != newPullStatus)
        {
            this.pullStatus = newPullStatus;
        }
    }
}
