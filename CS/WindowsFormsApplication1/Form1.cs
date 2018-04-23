using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraScheduler;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TimeSpan minTime = new TimeSpan(8, 0, 0);
        TimeSpan maxTime = new TimeSpan(19, 0, 0);

        private void Form1_Load(object sender, EventArgs e)
        {
            TimeScaleCollection scales = schedulerControl1.TimelineView.Scales;
            schedulerControl1.TimelineView.Scales.BeginUpdate();
            try
            {
                scales.Clear();
                scales.Add(new MyTimeScaleFixedInterval(TimeSpan.FromDays(1), minTime, maxTime));
                scales.Add(new MyTimeScaleFixedInterval(TimeSpan.FromHours(1), minTime, maxTime));
                scales.Add(new MyTimeScaleFixedInterval(TimeSpan.FromMinutes(30), minTime, maxTime));
            }
            finally
            {
                schedulerControl1.TimelineView.Scales.EndUpdate();
            }

            Appointment apt = schedulerStorage1.CreateAppointment(AppointmentType.Normal);
            apt.Start = DateTime.Today.AddHours(10);
            apt.End = DateTime.Today.AddHours(14);
            apt.Subject = "Test appointment";
            schedulerStorage1.Appointments.Add(apt);
        }

        void CorrectAppointmentParameters(AppointmentDragEventArgs e)
        {
            TimeSpan customScaleDuration = (maxTime - minTime).Add(TimeSpan.FromHours(2));
            DateTime initialStart = e.EditedAppointment.Start;

            if (e.EditedAppointment.Start.TimeOfDay < minTime || e.EditedAppointment.Start.TimeOfDay >= maxTime)
            {
                bool durationMustBeIncreased = e.EditedAppointment.Duration > customScaleDuration;
                e.EditedAppointment.Start = durationMustBeIncreased ? e.EditedAppointment.Start + customScaleDuration : e.EditedAppointment.Start - customScaleDuration;

                TimeInterval appInterval = new TimeInterval(e.EditedAppointment.Start, e.EditedAppointment.Start + e.EditedAppointment.Duration + customScaleDuration);
                if (!appInterval.Contains(e.HitInterval))
                {
                    durationMustBeIncreased = !durationMustBeIncreased;
                    e.EditedAppointment.Start = durationMustBeIncreased ? initialStart + customScaleDuration : initialStart - customScaleDuration;
                }
                e.Handled = true;
            }

            if (e.EditedAppointment.End.TimeOfDay >= maxTime || e.EditedAppointment.End.TimeOfDay < minTime)
            {
                e.EditedAppointment.End = e.EditedAppointment.Duration > customScaleDuration ? e.EditedAppointment.End - customScaleDuration : e.EditedAppointment.End + customScaleDuration;
                e.Handled = true;
            }         
        }

        private void schedulerControl1_AppointmentDrag(object sender, AppointmentDragEventArgs e)
        {
            CorrectAppointmentParameters(e);
        }

        private void schedulerControl1_AppointmentDrop(object sender, AppointmentDragEventArgs e)
        {
            CorrectAppointmentParameters(e);
        }
    }
}
