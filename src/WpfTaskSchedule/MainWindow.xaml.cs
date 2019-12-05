using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Text;
using Wpf.TaskSchedule.Jobs;

namespace Wpf.TaskSchedule
{
    public delegate void Display(string info);

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;
        private int _maxInfoCount = 500;
        private List<string> _infoList;
        private static object _locker = new object();
        private Startup _taskScheduleApp;
        public MainWindow()
        {
            InitializeComponent();
            InitBus();
            InitNotifyIcon();
        }
        private void InitBus()
        {
            new MainWindowProxy(this);
            _taskScheduleApp = new Startup(this);
            _infoList = new List<string>(_maxInfoCount);
            btn_stop.IsEnabled = false;
            btn_singleExecute.IsEnabled = false;
        }

        private void InitNotifyIcon()
        {
            this._notifyIcon = new NotifyIcon();
            this._notifyIcon.BalloonTipText = "简易任务管理器 v1.0 by yohuy";
            this._notifyIcon.ShowBalloonTip(2000);
            this._notifyIcon.Text = "简易任务管理器 v1.0 by yohuy";
            this._notifyIcon.Icon = new System.Drawing.Icon(@"logo.ico");
            //this._notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
            this._notifyIcon.Visible = true;
            //打开菜单项
            System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("显示窗体");
            open.Click += new EventHandler(Show);
            //退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(Close);
            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
            _notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            this._notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == MouseButtons.Left) this.Show(o, e);
            });
        }

        public void BindGridData(List<JobDetail> jobDetails)
        {
            grid_task.ItemsSource = jobDetails;
        }
        public void RefreshGrid()
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    grid_task.Items.Refresh();
                });
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
        public void Print(string info)
        {
            if (!string.IsNullOrEmpty(info))
            {
                lock (_locker)
                {
                    var log = $"{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss 〉")}{info}";
                    _infoList.Add(log);
                    if (_infoList.Count > _maxInfoCount)
                    {
                        _infoList.RemoveAt(0);
                    }

                    this.Dispatcher.Invoke(() =>
                    {
                        txt_info.Text = string.Join("\n", _infoList);
                        if (txt_info.IsFocused)
                        {
                            txt_status.Focus();
                        }
                        txt_info.ScrollToEnd();
                    });
                }
            }
        }

        private void Show(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        private void Hide(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Close(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            _taskScheduleApp.Start();
            txt_status.Text = "运行中";
            btn_stop.IsEnabled = true;
            btn_singleExecute.IsEnabled = true;
            btn_start.IsEnabled = false;
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            var msgResult = System.Windows.MessageBox.Show("确认停止任务？", "停止", MessageBoxButton.OKCancel);
            if (msgResult == MessageBoxResult.OK)
            {
                _taskScheduleApp.Stop();
                txt_status.Text = "已停止";
                btn_stop.IsEnabled = false;
                btn_singleExecute.IsEnabled = false;
                btn_start.IsEnabled = true;
            }
        }

        public void JobsPannel()
        {
            //grid_task.DataConte
        }
        private class MainWindowProxy : ApplicationProxy
        {
            public MainWindowProxy(MainWindow window)
            {
                if (m_ApplicationProxy == null)
                {
                    m_ApplicationProxy = this;
                    m_MainWindow = window;
                }
            }
        }

        private void btn_singleExecute_Click(object sender, RoutedEventArgs e)
        {
            var select = grid_task.SelectedValue as JobDetail;
            if (select == null)
            {
                System.Windows.MessageBox.Show("请选择需要立即执行的任务", "任务执行");
            }
            else
            {
                var msgResult = System.Windows.MessageBox.Show($"确认立即执行任务：{select.JobKey}？", "任务执行", MessageBoxButton.OKCancel);
                if (msgResult == MessageBoxResult.OK)
                {
                    string err;
                    var success = _taskScheduleApp.ExecuteJob(select.JobKey, out err);
                    System.Windows.MessageBox.Show(success ? $"任务执行成功！" : $"任务执行失败！错误：{err}", "任务执行");
                }
            }
        }
    }
}
