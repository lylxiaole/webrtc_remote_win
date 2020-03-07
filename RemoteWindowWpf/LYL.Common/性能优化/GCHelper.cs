using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dispath
{
    public static class GCHelper
    {
        #region GC
        private static readonly ConditionalWeakTable<object, NotifyWhenGCd<string>> sw_t = new ConditionalWeakTable<object, NotifyWhenGCd<string>>();
        internal class NotifyWhenGCd<T>
        {
            private readonly T m_value;
            internal NotifyWhenGCd(T value)
            {
                m_value = value;
            }
            public override string ToString()
            {
                return m_value.ToString();
            }
            ~NotifyWhenGCd()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("DC" + m_value);
            }
        }
        public static T GCWatch<T>(this T @sender, string tag) where T : class
        {
            sw_t.Add(@sender, new NotifyWhenGCd<string>(tag));
            return @sender;
        }
        #endregion

        #region weakReffence
        private static readonly List<WeakReference> References = new List<WeakReference>();
        private static readonly object obj1 = new object();
        public static T SetWeak<T>(this T obj) where T : class
        {
            Monitor.Enter(obj1);
            try
            {
                if (obj == null) { return null; }
                References.RemoveAll(o => o.IsAlive == false);
                var reff = References.FirstOrDefault(o => o.Target == obj);
                if (reff != null) { return reff.Target as T; }
                var weakobj = new WeakReference(obj);
                References.Add(weakobj);
                return weakobj.Target as T;
            }
            finally
            {
                Monitor.Exit(obj1);
            }
        }
        #endregion

        #region  memory&&CPU
        static long size;
        static Stopwatch watch;
        static object obj2 = new object();
        public static void StartWatch()
        {
            Monitor.Enter(obj2); 
            size = GC.GetTotalMemory(true);
            watch = new Stopwatch();
            watch.Start();
            Monitor.Exit(obj2);
        }

        public static void StopWatch()
        {
            var lastSize = GC.GetTotalMemory(true);
            lastSize = lastSize - size;
            watch.Stop();
            TimeSpan times = watch.Elapsed;
            Console.WriteLine($"***********分配内存:{(lastSize/1024)}KB********代码运行时间:{times.Milliseconds}毫秒***************************************");
        }
        #endregion

    }


}
