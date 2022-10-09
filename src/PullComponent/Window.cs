using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcdLib.Components
{
    public class Window
    {
        public static event Func<ScrollEvent, Task>? OnScroll;

        [JSInvokable]
        public static async Task Scroll(ScrollEvent e)
        {
            if(OnScroll != null)
            {
                await OnScroll(e);
            }
        }
    }
}
