

namespace Wpf.TaskSchedule.Jobs
{
    /**+++++++++++++++++++++++++++
     * 说明：任务配置
     *
     * 编写：[yohuy@qq.com]
     * 创建：[2019-12-05 10:14]
     * 修改：[2019-12-05 10:14]
     *+++++++++++++++++++++++++++*/

    public class JobConfig
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 任务执行周期表达式
        /// </summary>
        public string CronExpression { get; set; }
        /// <summary>
        /// 任务说明
        /// </summary>
        public string Desc { get; set; }
    }
}
