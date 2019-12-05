using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.TaskSchedule.Jobs
{
    /**+++++++++++++++++++++++++++
     * 说明：任务执行抽象类
     *
     * 编写：[yohuy@qq.com]
     * 创建：[2019-12-05 10:15]
     * 修改：[2019-12-05 10:15]
     *+++++++++++++++++++++++++++*/
    internal abstract class BaseJob : IJob, IBaseJob
    {
        protected IJobDetail m_job;
        protected ITrigger m_trigger;

        protected string m_jobName;
        public async Task Execute(IJobExecutionContext context)
        {
            var task = Do();
            task.GetAwaiter().OnCompleted(End);
        }

        /// <summary>
        /// 任务信息打印
        /// </summary>
        /// <param name="info"></param>
        protected void Print(string info)
        {
            if (!string.IsNullOrEmpty(info))
            {
                ApplicationProxy.Singleton().Print($"[{m_jobName}] {info}");
            }
        }

        /// <summary>
        /// 任务初始化
        /// </summary>
        public abstract void Init(JobConfig config);


        /// <summary>
        /// 任务布置
        /// </summary>
        /// <param name="scheduler"></param>
        public async void Schedule(IScheduler scheduler, List<JobConfig> configs)
        {
            var config = configs.Find(x => x.Name == m_jobName);
            if (config != null)
            {
                Init(config);
                Print("任务已添加");
                await scheduler.ScheduleJob(m_job, m_trigger);
            }
            else
            {
                Print("任务未配置，添加失败！");
            }
        }
        /// <summary>
        /// 任务开始执行
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public abstract Task Do(params object[] param);

        /// <summary>
        /// 任务完成后
        /// </summary>
        public virtual void End()
        {

        }
    }
}
