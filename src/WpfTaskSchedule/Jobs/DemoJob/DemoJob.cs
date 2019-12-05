using Quartz;
using System.Threading.Tasks;

namespace Wpf.TaskSchedule.Jobs
{
    /**+++++++++++++++++++++++++++
     * 说明：示例任务
     *
     * 编写：[yohuy@qq.com]
     * 创建：[2019-12-05 10:16]
     * 修改：[2019-12-05 10:16]
     *+++++++++++++++++++++++++++*/
    internal class DemoJob : BaseJob
    {
        public DemoJob()
        {
            m_jobName = this.GetType().Name;
        }
        public override async Task Do(params object[] param)
        {
            TaskBegin();
        }

        public override void Init(JobConfig config)
        {
            // define the job and tie it to our HelloJob class
            m_job = JobBuilder.Create<DemoJob>()
                .WithIdentity(m_jobName)
                .WithDescription(config.Desc)
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            m_trigger = TriggerBuilder.Create()
                .WithIdentity(m_jobName)
                .WithCronSchedule(config.CronExpression)
                //.StartNow()
                .Build();
        }
        private async void TaskBegin()
        {
            Print("TaskBegin");
        }
        public override async void End()
        {
            base.End();
            Print("Complated");
        }
    }
}
