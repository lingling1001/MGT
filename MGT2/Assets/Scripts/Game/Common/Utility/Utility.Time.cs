using System;

namespace MFrameWork
{
    public static partial class Utility
    {
        public static string ToDayHourMinuteSecondString(this int seconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, seconds);
            if (ts.Days > 0)
            {
                return string.Format("{0:D}" + " {1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            }
            return string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
        }

    }
}