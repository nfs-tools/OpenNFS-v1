using System;
using System.Runtime.CompilerServices;
using LibOpenNFS.Core;

namespace LibOpenNFS.Utils
{
    public static class DebugUtil
    {
        public static void EnsureCondition(Predicate<object> condition, Func<string> exceptionMessage, [CallerMemberName] string callerName = "")
        {
            if (!condition.Invoke(new object())) // dirty hack
            {
                throw new NFSException($"[{callerName}]: {exceptionMessage()}");
            }
        }
    }
}