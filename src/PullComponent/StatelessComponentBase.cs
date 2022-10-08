using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;

namespace BcdLib.Components;

public class StatelessComponentBase : IComponent
{
    private RenderHandle _renderHandle;
    private RenderFragment renderFragment;

    public StatelessComponentBase()
    {
        // 设置组件DOM树（的创建方式）
        renderFragment = BuildRenderTree;
    }

    public void Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
    }

    public Task SetParametersAsync(ParameterView parameters)
    {
        // 绑定props参数到具体的组件（为[Parameter]设置值）
        parameters.SetParameterProperties(this);

        // 渲染组件
        _renderHandle.Render(renderFragment);
        return Task.CompletedTask;
    }

    protected virtual void BuildRenderTree(RenderTreeBuilder builder)
    {
    }
}
