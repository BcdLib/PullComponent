# PullComponent
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BcdLib.PullComponent)](https://www.nuget.org/packages/BcdLib.PullComponent/)

A blazor pull refresh component library for maui.

# 1. Usage

1. add service
   ```c#
   builder.Services.AddBcdLibPullComponent();
   ```

2. import namespace:

   ```c#
   using BcdLib.Components;
   ```

3.  using `PullDown` component

   ```html
   <PullDown OnRefreshing="OnRefresh">
   
       // ChildContent ....
   
   </PullDown>
   
   @code {
   	public async Task OnRefresh()
       {
           await Task.Delay(1000);
       }
   }
   ```

   Sample to see `sample\PullSample\Pages\PullDownIFrame.razor`.

# 2. Gif

![](https://github.com/BcdLib/PullComponent/.assets/README/PullDown.gif)

# 3. Design

## 3.1.Pull Status

PullStatus enum has 5 status:

| enum value | describe                                                     |
| ---------- | ------------------------------------------------------------ |
| Awaiting   | Pull down has not started yet.                               |
| Pulling    | Pull down has started, but it has not reached a certain height (see MaxDistance paramter in 3.2.Api). |
| Loosing    | Pull down has started and reached a certain height.          |
| Loading    | After reaching the Loosing state, and finger has been release from the screen. The `OnRefreshing` event will be invoked. |
| Completed  | refresh completed.                                           |

State transition of PullStatus:

- `Awaiting --> Pulling --> Loosing --> Loading --> Completed --> Awaiting `
- `Awaiting --> Pulling --> Awaiting `

## 3.2.Api

| paramter     | type           | default   | remark                                                       |
| ------------ | -------------- | --------- | ------------------------------------------------------------ |
| OnRefreshing | EventCallback  | --        | Pull down for callback when refreshing.                      |
| PullingTip   | RenderFragment | 下拉刷新  | Dom displayed when in PullStatus.Pulling                     |
| LoosingTip   | RenderFragment | 释放更新  | Dom displayed when in PullStatus.Loosing                     |
| LoadingTip   | RenderFragment | 更新中... | Dom displayed when in PullStatus.Loading                     |
| CompletedTip | RenderFragment | 更新完成  | Dom displayed when in PullStatus.Completed                   |
| MaxDistance  | int            | 50        | Unit px.<br />To modify this value, you need to define the css variable  `--pull-refresh-head-height` too |

# 4.Developer

zxyao

# 5.License

MIT