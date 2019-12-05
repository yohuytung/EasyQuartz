using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.TaskSchedule
{
    /**+++++++++++++++++++++++++++
     * 说明：主窗体代理
     *
     * 编写：[yohuy@qq.com]
     * 创建：[2019-12-05 10:27]
     * 修改：[2019-12-05 10:27]
     *+++++++++++++++++++++++++++*/

    public class ApplicationProxy
    {
        protected static ApplicationProxy m_ApplicationProxy;
        protected static MainWindow m_MainWindow;
        private static object _locker = new object();

        protected ApplicationProxy()
        {
        }
        
        public void Print(string info)
        {
            m_MainWindow.Print(info);
        }

        public static ApplicationProxy Singleton()
        {
            if (m_MainWindow == null)
            {
                throw new Exception("MainWindow 未初始化！");
            }
            lock (_locker)
            {
                if (m_ApplicationProxy == null)
                {
                    m_ApplicationProxy = new ApplicationProxy();
                }
                return m_ApplicationProxy;
            }
        }
    }
}
