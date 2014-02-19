using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aditi.Scheduler.Models
{
    public static class CommonCronExpressions
    {
        public const string EveryMinute = "0 0/1 * 1/1 * ? *";
        public const string EveryFiveMinutes = "0 0/5 * 1/1 * ? *";
        public const string EveryHour = "0 0 0/1 1/1 * ? *";
        public const string EveryTwelveHours = "0 0 0/12 1/1 * ? *";
        public const string EveryAlternateHour = "0 0 0/2 1/1 * ? *";
        public const string EveryDayAtMidnight = "0 0 0 * * ?";
        public const string EveryOtherDayAtNoon = "0 0 12 1/2 * ? *";
        public const string EverySundayAtNoon = "0 0 12 ? * SUN";
        public const string FirstOfEveryMonth = "0 0 0 1 * ?";
        public const string FirstSundayOfEveryMonth = "0 0 0 ? * 1#1";
    }
}
