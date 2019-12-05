using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.TaskSchedule.Jobs
{
    /**+++++++++++++++++++++++++++
     * 说明：任务执行公共接口
     *
     * 编写：[yohuy@qq.com]
     * 创建：[2019-12-05 10:15]
     * 修改：[2019-12-05 10:15]
     *+++++++++++++++++++++++++++*/
    internal interface IBaseJob
    {
        /// <summary>
        /// 任务初始化
        /// </summary>
        void Init(JobConfig config);
        /// <summary>
        /// 任务布置
        /// </summary>
        /// <param name="scheduler"></param>
        void Schedule(IScheduler scheduler, List<JobConfig> config);
        /// <summary>
        /// 任务开始执行
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task Do(params object[] param);
    }
}
