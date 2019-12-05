using System;

namespace Wpf.TaskSchedule.Jobs
{
    /**+++++++++++++++++++++++++++
     * 说明：任务详情
     *
     * 编写：[yohuy@qq.com]
     * 创建：[2019-12-05 10:12]
     * 修改：[2019-12-05 10:12]
     *+++++++++++++++++++++++++++*/
    /// <summary>
    /// 任务详情
    /// </summary>
    public class JobDetail
    {
        /// <summary>
        /// 任务标识
        /// </summary>
        public string JobKey { get; set; }
        public string TriggerKey { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTimeUtc { get; set; }
        /// <summary>
        /// 最新执行时间
        /// </summary>
        public DateTime? FinalFireTime { get; set; }
        /// <summary>
        /// 下次执行时间
        /// </summary>
        public DateTime? NextFireTime { get; set; }
        /// <summary>
        /// 执行总次数
        /// </summary>
        public int ExecutedCount { get; set; }
        //public TimeSpan RepeatInterval { get; set; }
        ///// <summary>
        ///// 执行间隔
        ///// </summary>
        //public string ExecuteInterval { get; set; }
        /// <summary>
        /// 执行周期
        /// </summary>
        public string CronExpressionString { get; set; }
        /// <summary>
        /// 任务说明
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 任务当前状态
        /// </summary>
        public string Status { get; set; }

        public string JobGroup { get; set; }
        public string TriggerGroup { get; set; }

    }
}
