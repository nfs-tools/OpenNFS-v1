using System;
using System.Runtime.CompilerServices;
using LibOpenNFS.Core;

namespace LibOpenNFS.Utils
{
    public static class DebugUtil
    {
        public static bool IsContainerChunk(uint id)
        {
            return (id & 0x80000000) == 0x80000000;
        }
        
        public static void EnsureCondition(bool condition, Func<string> exceptionMessage, [CallerMemberName] string callerName = "")
        {
            if (!condition) // dirty hack
            {
                throw new NFSException($"[{callerName}]: {exceptionMessage()}");
            }
        }
        public static void EnsureCondition(Predicate<object> condition, Func<string> exceptionMessage, [CallerMemberName] string callerName = "")
        {
            EnsureCondition(condition.Invoke(new object()), exceptionMessage);
        }
    }
}