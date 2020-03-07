using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dispath
{//双检索
    public class TotalClassHelper<T> where T : class, new()
    {
        //这个对象引用一个单实例对象
        private static T s_value = null;
        //私有化，防止外部实例化
        private TotalClassHelper()
        {
            //构造函数里，千万不能有副作用的代码，也不希望能被外部类调用。。。所以声明为private，并且什么也不写
        }

        public static T GetSingleton()
        {
            //如果对象已经创建，就直接返回（这样速度很快）
            if (s_value != null) return s_value;
            //创建一个实例对象，并把它固定下来
            T temp = new T();
            Interlocked.CompareExchange(ref s_value, temp, null);
            //这个技术的缺点是，，在多个线程同时访问这个方法时，会实例化多个对象。。。
            //但是，这种情况几乎不会发生。。。。
            //就算发生， 多创建的实例，之后也会被释放掉
            return s_value;
        }
    }
}
