using Backuper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SBWPF
{
    public class BackupWorker
    {
        public DispatcherTimer Timer { get; set; }
        public Guid Guid { get; set; }
        private MainWindow m_mainWindow;
        private BackupPlan m_backupPlan;
        
        public BackupWorker(MainWindow mainWindow, BackupPlan plan) 
        {
            m_mainWindow = mainWindow;
            m_backupPlan = plan;
            Guid = Guid.NewGuid();
        }

        public void StartTimer(int intervall)
        {
            m_mainWindow.ShowPlanProgress(m_backupPlan);
            Timer = new DispatcherTimer();
            Timer.Tick += Timer_Tick;
            Timer.Interval = TimeSpan.FromMicroseconds(intervall);
            Timer.Start();
        }

        public void StopTimer()
        {
            Timer.Stop();
            m_mainWindow.HidePlanProgress(m_backupPlan);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            m_mainWindow.Dispatcher.InvokeAsync(() =>
            {
                m_mainWindow.ShowPlanProgress(m_backupPlan);
            });
        }
    }
}
