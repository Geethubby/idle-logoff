﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lithnet.idlelogoff;
using Timer = System.Windows.Forms.Timer;

namespace lithnet.idlelogoff
{
    public partial class LogoffWarning : Form
    {
        public DateTime LogoffDateTime
        {
            get => this.logoffDateTime;
            set
            {
                this.logoffDateTime = value;
                this.UpdateLabelText();
            }
        }

        private Timer timer;
        private DateTime logoffDateTime;

        public LogoffWarning()
        {
            this.InitializeComponent();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
            {
                this.timer = new Timer();
                this.timer.Tick += this.Timer_Tick;
                this.timer.Interval = 1000;
                this.timer.Start();
            }
            else
            {
                this.timer?.Stop();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.UpdateLabelText();
        }

        private string GetActionDescription()
        {
            switch (Settings.Action)
            {
                case IdleTimeoutAction.Logoff:
                    return "logged off";
                case IdleTimeoutAction.Reboot:
                    return "rebooted";
                case IdleTimeoutAction.Shutdown:
                    return "shut down";
                case IdleTimeoutAction.TurnOffMonitor:
                    return "have your monitor(s) turned off";
                case IdleTimeoutAction.Screensaver:
                    return "have the screensaver activated";
                default:
                    return "logged off";
            }
        }

        private void UpdateLabelText()
        {
            TimeSpan remaining = this.LogoffDateTime.Subtract(DateTime.Now);

            if (remaining.Ticks > 0)
            {
                string message = Settings.WarningMessage;

                // If using default message, customize it based on action
                if (message == "Your session has been idle for too long, and you will be logged out in {0}")
                {
                    message = $"Your session has been idle for too long, and you will {GetActionDescription()} in {{0}}";
                }

                if (message.Contains("{0}"))
                {
                    message = string.Format(message, $"{(int)remaining.TotalMinutes}{CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator}{remaining.Seconds:00}");
                }

                this.lbWarning.Text = message;
            }
            else
            {
                this.lbWarning.Text = string.Empty;
            }
        }

        private void LogoffWarning_KeyPress(object sender, KeyPressEventArgs e)
        {
            Trace.WriteLine("Hiding warning window on key press");
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Trace.WriteLine("User pressed cancel button");
            this.Hide();
        }
    }
}
