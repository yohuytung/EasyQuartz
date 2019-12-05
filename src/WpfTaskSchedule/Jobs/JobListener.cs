using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Wpf.TaskSchedule.Jobs
{
    /**+++++++++++++++++++++++++++
     * 说明：任务监听事件，主要是回写WPF界面任务状态信息
     *
     * 编写：[yohuy@qq.com]
     * 创建：[2019-12-05 10:13]
     * 修改：[2019-12-05 10:13]
     *+++++++++++++++++++++++++++*/

    public class JobListener : IJobListener
    {
        public event JobEndDelegate JobEndEvent;

        public string Name => "JobListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => { }, cancellationToken);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => { }, cancellationToken);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                JobDetail detail = new JobDetail
                {
                    JobKey = context.JobDetail.Key.Name
                };
                JobEndEvent?.Invoke(detail);
            }, cancellationToken);
        }
    }
}
