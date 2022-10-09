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
    public DocumentJsInterop DocumentJs { get; set; } = default!;

    #region Parameter

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EventCallback OnRefreshing { get; set; }

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

    private string GetWrapperStyle()
    {
        return pullStatus switch
        {
            PullStatus.Awaiting => "",
            _ => wrapperStyle,
        };
    }

    private double startY = 0;
    private int moveDistance = 0;
    private string wrapperStyle = "";
    
    private void OnTouchStart(TouchEventArgs e)
    {
        if (this.pullStatus == PullStatus.Awaiting
            || this.pullStatus == PullStatus.Completed
        )
        {
            this.SetPullStatus(PullStatus.Pulling);
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
            // If document is a scroll bar, touch sliding is a simple way to scroll up and down the page
            var distToTop = await DocumentJs.GetScrollTopAsync();
            if (distToTop > 0)
            {
                return;
            }

            var move = e.TargetTouches[0].ClientY - this.startY;
            // Only a positive number means that the user has pulled down.
            if (move > 0)
            {
                this.SetDistance(CalcMoveDistance(move));
            }
        }
    }

    private async Task OnTouchEnd(TouchEventArgs e)
    {
        if (this.pullStatus == PullStatus.Loosing)
        {
            this.SetPullStatus(PullStatus.Loading);

            #region This part cannot be placed in SetPullStatus, otherwise async will lead to state confusion

            if (OnRefreshing.HasDelegate)
            {
                try
                {
                    await OnRefreshing.InvokeAsync();
                }
                catch (Exception)
                {
                    this.SetDistance(-1);
                    throw;
                }
            }
#if DEBUG
            else
            {
                await Task.Delay(1000);
            }
#endif
            #endregion


            this.SetPullStatus(PullStatus.Completed);
            await Task.Delay(800);
            this.SetDistance(-1);
        }
        else if (this.pullStatus == PullStatus.Awaiting || this.pullStatus == PullStatus.Pulling)
        {
            this.SetDistance(-1);
        }
    }

    private int CalcMoveDistance(double moveDist)
    {
        // Simulated resistance
        return (int) Math.Pow(moveDist, 0.8);
    }

    private void SetDistance(int moveDist)
    {
        if (moveDist < 0)
        {
            this.SetPullStatus(PullStatus.Awaiting);
            this.moveDistance = 0;
            wrapperStyle = "";
            StateHasChanged();
        }
        else
        {
            if (moveDist < MaxDistance)
            {
                this.SetPullStatus(PullStatus.Pulling);
            }
            else
            {
                this.SetPullStatus(PullStatus.Loosing);
                moveDist = MaxDistance;
            }
            if (this.moveDistance != moveDist)
            {
                this.moveDistance = moveDist;
                wrapperStyle = $"transform: translate3d(0, {moveDist}px, 0)";
                StateHasChanged();
            }
        }
    }

    private void SetPullStatus(PullStatus newPullStatus)
    {
        if (this.pullStatus != newPullStatus)
        {
            this.pullStatus = newPullStatus;
        }
    }
}
