using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Wpf.TaskSchedule.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using Quartz.Impl.Matchers;
using System.Xml.Linq;

namespace Wpf.TaskSchedule
{
    /**+++++++++++++++++++++++++++
     * 说明：程序启动入口
     *
     * 编写：[yohuy@qq.com]
     * 创建：[2019-12-05 10:26]
     * 修改：[2019-12-05 10:26]
     *+++++++++++++++++++++++++++*/

    public delegate void JobEndDelegate(JobDetail job);

    public class Startup
    {
        protected static MainWindow m_MainWindow;
        public List<JobDetail> _jobDetails = new List<JobDetail>();
        private IScheduler _scheduler;

        /// <summary>
        /// 任务添加
        /// </summary>
        private void AddJobs()
        {
            // 获取所有的任务配置
            List<JobConfig> jobConfigs = GetJobConfig();

            new DemoJob().Schedule(_scheduler, jobConfigs);
        }

        public Startup(MainWindow window)
        {
            m_MainWindow = window;
            m_MainWindow.BindGridData(_jobDetails);
        }

        public void Start()
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            Run().GetAwaiter().GetResult();
        }

        public async void Stop()
        {
            await _scheduler.Shutdown();
        }

        private async Task Run()
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                _scheduler = await factory.GetScheduler();
                var jobListener = new JobListener();
                jobListener.JobEndEvent += RefreshJobDetail;
                _scheduler.ListenerManager.AddJobListener(jobListener, GroupMatcher<JobKey>.AnyGroup());

                AddJobs();
                GetJobInfo();
                // and start it off
                await _scheduler.Start();
            }
            catch (SchedulerException se)
            {
                m_MainWindow.Print(se.Message);
            }
        }


        private void GetJobInfo()
        {
            var allTriggerKeys = _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            foreach (var triggerKey in allTriggerKeys.Result)
            {
                var triggerdetails = _scheduler.GetTrigger(triggerKey).GetAwaiter().GetResult();
                var jobDetail = _scheduler.GetJobDetail(triggerdetails.JobKey);
                var simpleTrigger = triggerdetails as ICronTrigger;
                if (!_jobDetails.Exists(x => x.JobKey == triggerdetails.JobKey.Name))
                {
                    _jobDetails.Add(new JobDetail
                    {
                        TriggerKey = triggerdetails.Key.Name,
                        TriggerGroup = triggerdetails.Key.Group,
                        JobKey = triggerdetails.JobKey.Name,
                        JobGroup = triggerdetails.JobKey.Group,
                        StartTimeUtc = triggerdetails.StartTimeUtc.LocalDateTime,
                        FinalFireTime = triggerdetails.FinalFireTimeUtc?.LocalDateTime,
                        NextFireTime = triggerdetails.GetNextFireTimeUtc()?.LocalDateTime,
                        Status = jobDetail.Status.ToString(),
                        Description = jobDetail.Result.Description,
                        //RepeatInterval = simpleTrigger.RepeatInterval
                        CronExpressionString = simpleTrigger.CronExpressionString
                    });
                }
            }

            m_MainWindow.RefreshGrid();
        }


        private List<JobConfig> GetJobConfig()
        {
            List<JobConfig> jobConfigs = new List<JobConfig>();

            var filePath = AppDomain.CurrentDomain.BaseDirectory + @"/Config/job.config";

            if (File.Exists(filePath))
            {
                XDocument xdocument = XDocument.Load(filePath);

                var config = from job in xdocument.Element("Jobs").Elements()
                             select new JobConfig
                             {
                                 Name = job.Attribute("Name").Value,
                                 CronExpression = job.Attribute("CronExpression").Value,
                                 Desc = job.Attribute("Desc").Value
                             };
                jobConfigs = config.ToList();
            }

            return jobConfigs;
        }

        public void RefreshJobDetail(JobDetail job)
        {
            var origJob = _jobDetails.Find(x => x.JobKey == job.JobKey);
            if (origJob == null)
            {
                _jobDetails.Add(job);
            }
            else
            {
                var triggerdetails = _scheduler.GetTrigger(new TriggerKey(origJob.TriggerKey, origJob.TriggerGroup)).GetAwaiter().GetResult();
                //var simpleTrigger = triggerdetails as ISimpleTrigger;
                origJob.FinalFireTime = triggerdetails.GetPreviousFireTimeUtc()?.LocalDateTime;// DateTimeOffset.Now;
                origJob.NextFireTime = triggerdetails.GetNextFireTimeUtc()?.LocalDateTime;
                origJob.ExecutedCount++;// = simpleTrigger.RepeatCount;
            }
            m_MainWindow.RefreshGrid();
        }

        public bool ExecuteJob(string key, out string err)
        {
            err = null;
            bool success = false;

            if (_scheduler.IsStarted)
            {
                var origJob = _jobDetails.Find(x => x.JobKey == key);
                var jobKey = new JobKey(key, origJob.JobGroup);
                var jobDetail = _scheduler.GetJobDetail(jobKey);
                if (jobDetail.Status == TaskStatus.Running)
                {
                    err = "当前任务正在执行，请稍候再试！";
                }
                else
                {
                    _scheduler.TriggerJob(jobKey);
                    success = true;
                }
            }
            else
            {
                err = "调度器未启动";
            }
            return success;
        }

        // simple log provider to get something to the console
        private class ConsoleLogProvider : ILogProvider
        {
            public Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                {
                    if (level >= LogLevel.Info && func != null)
                    {
                        var info = string.Format("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                        m_MainWindow.Print(info);
                    }
                    return true;
                };
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, string value)
            {
                throw new NotImplementedException();
            }
        }
    }
}