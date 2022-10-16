# PullComponent
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BcdLib.PullComponent)](https://www.nuget.org/packages/BcdLib.PullComponent/)

A blazor pull refresh component library for maui. It consists of two components:  `PullDown`  and  `PullUp`.

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
   
![](https://raw.githubusercontent.com/BcdLib/PullComponent/main/assets/README/PullDown.gif)
   
   
   
4. using `PullUp` component
   
   ```html
   <PullUp OnRefreshing="OnRefresh">
   
       <div style="user-select: none; display:flex; flex-direction:column; align-items: flex-end">
           pull up Refresh times @count
   
           @for (int i = 0; i < count; i++)
           {
               <h1 @key="i">item @i</h1>
           }
       </div>
   
   </PullUp>
   
   
   @code {
       int count = 20;
       public async Task<bool> OnRefresh()
       {
           if (count < 60)
           {
               await Task.Delay(1000);
               count += 20;
               return true;
           }
           return false;
       }
   }
   ```
   
   Sample to see `sample\PullSample\Pages\PullUpIFrame.razor`.
   
![](https://raw.githubusercontent.com/BcdLib/PullComponent/main/assets/README/PullUp.gif)

# 2. Design

## 2.1.PullDown Component

### 2.1.1.Pull Down Status

PullDownStatus enum has 5 status:

| enum value | describe                                                     |
| ---------- | ------------------------------------------------------------ |
| Awaiting   | Pull down has not started yet.                               |
| Pulling    | Pull down has started, but it has not reached a certain height (see MaxDistance paramter in 3.2.Api). |
| Loosing    | Pull down has started and reached a certain height.          |
| Loading    | After reaching the Loosing state, and finger has been release from the screen. The `OnRefreshing` event will be invoked. |
| Completed  | refresh completed.                                           |

State transition of PullDownStatus:

- `Awaiting --> Pulling --> Loosing --> Loading --> Completed --> Awaiting `
- `Awaiting --> Pulling --> Awaiting `

### 2.1.2.Api

| paramter     | type           | default   | remark                                                       |
| ------------ | -------------- | --------- | ------------------------------------------------------------ |
| OnRefreshing | EventCallback  | --        | Pull down for callback when refreshing.                      |
| PullingTip   | RenderFragment | 下拉刷新  | Dom displayed when in PullDownStatus.Pulling                 |
| LoosingTip   | RenderFragment | 释放更新  | Dom displayed when in PullDownStatus.Loosing                 |
| LoadingTip   | RenderFragment | 更新中... | Dom displayed when in PullDownStatus.Loading                 |
| CompletedTip | RenderFragment | 更新完成  | Dom displayed when in PullDownStatus.Completed               |
| MaxDistance  | int            | 50        | Unit px.<br />To modify this value, you need to define the css variable  `--pull-refresh-head-height` too |

## 2.2.PullUp Component

### 2.1.1.Pull Up Status

PullUpStatus enum has 6 status:

| enum value | describe                                                     |
| ---------- | ------------------------------------------------------------ |
| Awaiting   | Pull down has not started yet.                               |
| Pulling    | Pull down has started, but it has not reached a certain height (see MaxDistance paramter in 3.2.Api). |
| Loosing    | Pull down has started and reached a certain height.          |
| Loading    | After reaching the Loosing state, and finger has been release from the screen. The `OnRefreshing` event will be invoked. |
| Completed  | refresh completed.                                           |
| NoData     | refresh completed, but no more data is loaded(it is indicated by the return value of OnRefreshing) |

State transition of PullDownStatus:

- `Awaiting --> Pulling --> Loosing --> Loading --> Completed/NoData --> Awaiting `
- `Awaiting --> Pulling --> Awaiting `

### 2.1.2.Api

| paramter     | type              | default      | remark                                                       |
| ------------ | ----------------- | ------------ | ------------------------------------------------------------ |
| OnRefreshing | Func<Task<bool>>? | null         | returns whether there is more data                           |
| PullingTip   | RenderFragment    | 下拉刷新     | Dom displayed when in PullDownStatus.Pulling                 |
| LoosingTip   | RenderFragment    | 释放更新     | Dom displayed when in PullDownStatus.Loosing                 |
| LoadingTip   | RenderFragment    | 更新中...    | Dom displayed when in PullDownStatus.Loading                 |
| CompletedTip | RenderFragment    | 更新完成     | Dom displayed when in PullDownStatus.Completed               |
| NoDataTip    | RenderFragment    | 暂无更多数据 | Dom displayed when in PullDownStatus.NoData                  |
| MaxDistance  | int               | 50           | Unit px.<br />To modify this value, you need to define the css variable  `--pull-refresh-head-height` too |

# 3.Developer

zxyao

# 4.License

MIT
