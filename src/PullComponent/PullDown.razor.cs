using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Reflection.Metadata;
using System.Xml;

namespace BcdLib.Components;

public partial class PullDown
{
    [Inject]
    public PullDownJsInterop PullRefreshJs { get; set; } = default!;

    #region Parameter

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback OnLoading { get; set; }

    [Parameter]
    public RenderFragment PullingTip { get; set; } = builder =>
    {
        builder.AddContent(0, "下拉刷新");
    };

    [Parameter]
    public RenderFragment LoosingTip { get; set; } = builder =>
    {
        builder.AddContent(0, "释放更新");
    };

    [Parameter]
    public RenderFragment LoadingTip { get; set; } = builder =>
    {
        builder.AddContent(0, "更新中...");
    };

    [Parameter]
    public RenderFragment CompletedTip { get; set; } = builder =>
    {
        builder.AddContent(0, "更新完成");
    };

    [Parameter]
    public int MaxDistance { get; set; } = 50;
    
    #endregion

    private PullStatus pullStatus = PullStatus.Awaiting;

    #region PullRefreshJs

    [JSInvokable]
    public async Task ChangeStatusAsync(PullStatus pullStatus, string wrapperStyle)
    {
        Console.WriteLine(pullStatus);
        this.pullStatus = pullStatus;
        if(pullStatus == PullStatus.Loading && OnLoading.HasDelegate)
        {
            try
            {
                await OnLoading.InvokeAsync();
            }
                catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
#if DEBUG
        else
        {
            await Task.Delay(1000);
        }
#endif
        this.wrapperStyle = wrapperStyle;
        StateHasChanged();
    }
    #endregion

    private RenderFragment GetTipHtml()
    {
        var renderFragment = pullStatus switch
        {
            PullStatus.Loosing => LoosingTip,
            PullStatus.Loading => LoadingTip,
            PullStatus.Completed => CompletedTip,
            _ => PullingTip,
        };
        return renderFragment;
    }

    private double startY = 0;
    private double moveDistance = 0;
    private string wrapperStyle = "";
    
    private async Task OnTouchStart(TouchEventArgs e)
    {
        Console.WriteLine("OnTouchStart");
        if (this.pullStatus == PullStatus.Awaiting
            || this.pullStatus == PullStatus.Completed
        )
        {
            await this.SetPullStatus(PullStatus.Pulling);
            // 获取初始y轴位置
            this.startY = e.TargetTouches[0].ClientY;
            // 触摸开始时，动画时间，移动距离归0
            this.moveDistance = 0;
        }
    }

    private async Task OnTouchMove(TouchEventArgs e)
    {
        if (this.pullStatus == PullStatus.Pulling || this.pullStatus == PullStatus.Loosing)
        {
            // 首先计算当前页面是否有滚动条，有滚动条，那么触摸滑动就是页面简单的上下滚动
            var scrollTop = await PullRefreshJs.GetScrollTopAsync();
            if (scrollTop > 0)
                return;
            var move = e.TargetTouches[0].ClientY - this.startY;
            // 判断手指滑动的距离，只有为正数才代表用户下拉了。
            if (move > 0)
            {
                await this.SetDistance(move);
            }
        }
    }

    private async Task OnTouchEnd(TouchEventArgs e)
    {
        if (this.pullStatus == PullStatus.Loosing)
        {
            await this.SetDistance(this.MaxDistance, true);
            await Task.Delay(500);
            await this.SetDistance(-1, false);
        }
        else if (this.pullStatus == PullStatus.Awaiting || this.pullStatus == PullStatus.Pulling)
        {
            await this.SetDistance(-1, false);
        }
        Console.WriteLine("OnTouchEnd");
    }

    private async ValueTask SetDistance(double moveDist, bool isLoading = false)
    {
        if (isLoading && this.pullStatus != PullStatus.Loading && this.pullStatus != PullStatus.Completed)
        {
            await this.SetPullStatus(PullStatus.Loading);
            await this.SetPullStatus(PullStatus.Completed);
        }
        else if (moveDist <= 0)
        {
            this.moveDistance = 0;
            wrapperStyle = "";
            StateHasChanged();
            await this.SetPullStatus(PullStatus.Awaiting);
        }
        else
        {
            var moveDistance = Math.Pow(moveDist, 0.8);
            if (moveDistance < MaxDistance)
            {
                await this.SetPullStatus(PullStatus.Pulling);
            }
            else
            {
                moveDistance = MaxDistance;
                await this.SetPullStatus(PullStatus.Loosing);
            }
            if (this.moveDistance != moveDistance)
            {
                this.moveDistance = moveDistance;
                wrapperStyle = $"transform: translate3d(0, {moveDistance}px, 0)";
            }
        }
    }

    private async ValueTask SetPullStatus(PullStatus newPullStatus)
    {
        if (this.pullStatus != newPullStatus)
        {
            this.pullStatus = newPullStatus;
            if (pullStatus == PullStatus.Loading && OnLoading.HasDelegate)
            {
                try
                {
                    await OnLoading.InvokeAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
#if DEBUG
            else
            {
                await Task.Delay(1000);
            }
#endif
        }
        StateHasChanged();
    }
}
