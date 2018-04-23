using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraScheduler;

namespace WindowsFormsApplication1
{
    public class MyTimeScaleFixedInterval : TimeScaleFixedInterval
    {
        TimeSpan start;
        TimeSpan end;

        public MyTimeScaleFixedInterval(TimeSpan span, TimeSpan spanStart, TimeSpan spanEnd) : base(span)
        {
            this.Width = 40;
            start = spanStart;
            end = spanEnd;
        }

        public override DateTime Floor(DateTime date)
        {
            if (date.TimeOfDay < start)
                return date.Date.AddDays(-1) + end - Value;
            if (date.TimeOfDay == start)
                return date;
            if (date.TimeOfDay == end)
                return date;
            DateTime result = base.Floor(date);
            if (result.TimeOfDay > end)
                return date.Date + end;
            if (result.TimeOfDay < start)
                return result.Date + start;
            return result;
        }

        public override DateTime GetNextDate(DateTime date)
        {
            if (date.TimeOfDay == start)
                date = base.Floor(date);
            DateTime result = base.GetNextDate(date);
            if (result.TimeOfDay >= end)
                return result.Date.AddDays(1) + start;
            if (result.TimeOfDay <= start)
                return result + start;
            return result;
        }

        public override string FormatCaption(DateTime start, DateTime end)
        {
            if (base.Value == TimeSpan.FromDays(1)) return start.ToString("d ddd");
            return start.ToString("HH:mm");
        }
    }
}
