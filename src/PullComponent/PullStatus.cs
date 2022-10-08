using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BcdLib.Components
{
    public enum PullStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        Awaiting = 0,
        /// <summary>
        /// 已经下拉，未达到高度
        /// </summary>
        Pulling = 1,
        /// <summary>
        /// 达到高度，等待松手
        /// </summary>
        Loosing = 2,
        /// <summary>
        /// 已经松手，正在刷新
        /// </summary>
        Loading = 3, 
        /// <summary>
        /// 刷新完成
        /// </summary>
        Completed = 4, 
    }
}
