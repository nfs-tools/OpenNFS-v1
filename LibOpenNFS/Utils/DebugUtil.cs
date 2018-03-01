using System;
using System.Runtime.CompilerServices;

namespace LibOpenNFS.Utils
{
    public class DebugUtil
    {
        public static void EnsureCondition(Predicate<object> condition, Func<string> exceptionMessage, [CallerMemberName] string callerName = "")
        {
            if (!condition.Invoke(new object())) // dirty hack
            {
                throw new Exception($"[{callerName}]: {exceptionMessage()}");
            }
        }
    }
}