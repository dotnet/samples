//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Threading;

namespace Microsoft.Samples.LocalChannel
{
    struct TimeoutHelper
    {
        DateTime deadline;
        bool deadlineSet;
        TimeSpan originalTimeout;
        public static readonly TimeSpan MaxWait = TimeSpan.FromMilliseconds(Int32.MaxValue);

        public TimeoutHelper(TimeSpan timeout)
        {
            this.originalTimeout = timeout;
            this.deadline = DateTime.MaxValue;
            this.deadlineSet = (timeout == TimeSpan.MaxValue);
        }

        public TimeSpan OriginalTimeout
        {
            get { return this.originalTimeout; }
        }

        public static bool IsTooLarge(TimeSpan timeout)
        {
            return (timeout > TimeoutHelper.MaxWait) && (timeout != TimeSpan.MaxValue);
        }

        public static TimeSpan FromMilliseconds(int milliseconds)
        {
            if (milliseconds == Timeout.Infinite)
            {
                return TimeSpan.MaxValue;
            }
            else
            {
                return TimeSpan.FromMilliseconds(milliseconds);
            }
        }

        public static int ToMilliseconds(TimeSpan timeout)
        {
            if (timeout == TimeSpan.MaxValue)
            {
                return Timeout.Infinite;
            }
            else
            {
                long ticks = Ticks.FromTimeSpan(timeout);
                if (ticks / TimeSpan.TicksPerMillisecond > int.MaxValue)
                {
                    return int.MaxValue;
                }
                return Ticks.ToMilliseconds(ticks);
            }
        }

        public static TimeSpan Add(TimeSpan timeout1, TimeSpan timeout2)
        {
            return Ticks.ToTimeSpan(Ticks.Add(Ticks.FromTimeSpan(timeout1), Ticks.FromTimeSpan(timeout2)));
        }

        public static DateTime Add(DateTime time, TimeSpan timeout)
        {
            if (timeout >= TimeSpan.Zero && DateTime.MaxValue - time <= timeout)
            {
                return DateTime.MaxValue;
            }
            if (timeout <= TimeSpan.Zero && DateTime.MinValue - time >= timeout)
            {
                return DateTime.MinValue;
            }
            return time + timeout;
        }

        public static DateTime Subtract(DateTime time, TimeSpan timeout)
        {
            return Add(time, TimeSpan.Zero - timeout);
        }

        public static TimeSpan Divide(TimeSpan timeout, int factor)
        {
            if (timeout == TimeSpan.MaxValue)
            {
                return TimeSpan.MaxValue;
            }

            return Ticks.ToTimeSpan((Ticks.FromTimeSpan(timeout) / factor) + 1);
        }

        public TimeSpan RemainingTime()
        {
            if (!this.deadlineSet)
            {
                this.SetDeadline();
                return this.originalTimeout;
            }
            else if (this.deadline == DateTime.MaxValue)
            {
                return TimeSpan.MaxValue;
            }
            else
            {
                TimeSpan remaining = this.deadline - DateTime.UtcNow;
                if (remaining <= TimeSpan.Zero)
                {
                    return TimeSpan.Zero;
                }
                else
                {
                    return remaining;
                }
            }
        }

        public TimeSpan ElapsedTime()
        {
            return this.originalTimeout - this.RemainingTime();
        }

        void SetDeadline()
        {
            this.deadline = DateTime.UtcNow + this.originalTimeout;
            this.deadlineSet = true;
        }

        public static void ThrowIfNegativeArgument(TimeSpan timeout)
        {
            ThrowIfNegativeArgument(timeout, "timeout");
        }

        public static void ThrowIfNegativeArgument(TimeSpan timeout, string argumentName)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
        }

        public static void ThrowIfNonPositiveArgument(TimeSpan timeout)
        {
            ThrowIfNonPositiveArgument(timeout, "timeout");
        }

        public static void ThrowIfNonPositiveArgument(TimeSpan timeout, string argumentName)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
        }

        public static bool WaitOne(WaitHandle waitHandle, TimeSpan timeout)
        {
            ThrowIfNegativeArgument(timeout);
            if (timeout == TimeSpan.MaxValue)
            {
                waitHandle.WaitOne();
                return true;
            }
            else
            {
                return waitHandle.WaitOne(timeout, false);
            }
        }

        static class Ticks
        {
            public static long FromMilliseconds(int milliseconds)
            {
                return (long)milliseconds * TimeSpan.TicksPerMillisecond;
            }

            public static int ToMilliseconds(long ticks)
            {
                return checked((int)(ticks / TimeSpan.TicksPerMillisecond));
            }

            public static long FromTimeSpan(TimeSpan duration)
            {
                return duration.Ticks;
            }

            public static TimeSpan ToTimeSpan(long ticks)
            {
                return new TimeSpan(ticks);
            }

            public static long Add(long firstTicks, long secondTicks)
            {
                if (firstTicks == long.MaxValue || firstTicks == long.MinValue)
                {
                    return firstTicks;
                }
                if (secondTicks == long.MaxValue || secondTicks == long.MinValue)
                {
                    return secondTicks;
                }
                if (firstTicks >= 0 && long.MaxValue - firstTicks <= secondTicks)
                {
                    return long.MaxValue - 1;
                }
                if (firstTicks <= 0 && long.MinValue - firstTicks >= secondTicks)
                {
                    return long.MinValue + 1;
                }
                return checked(firstTicks + secondTicks);
            }
        }
    }
}
