using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dispath
{
    public class ReflectionHelper
    {
        /// <summary>
        /// 获取所有程序集
        /// </summary>
        /// <returns></returns>
        private static List<Assembly> GetAllAssemblies()
        {
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            return assemblys?.ToList();
        }
        /// <summary>
        /// 获取路径下所有的程序集
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static List<Assembly> GetAllAssemblies(string path)
        {
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            return assemblys?.ToList();
        }
        /// <summary>
        /// 获取程序集中所以的类型
        /// </summary>
        /// <param name="ass"></param>
        /// <returns></returns>
        private static List<Type> GetAllClass(Assembly ass)
        {
            return ass.GetTypes()?.ToList();
        }
        /// <summary>
        /// 获取程序及中所有的指定类型的类
        /// </summary>
        /// <param name="typeinfo"></param>
        /// <returns></returns>
        //public static List<Type> GetTypesInAssembly(Type typeinfo)
        //{
        //    List<Type> types = new List<Type>();
        //    var asses = GetAllAssemblies();
        //    if (asses == null) { return types; }
        //    foreach (var a in asses)
        //    {

        //        var currenttypes = GetAllClass(a);
        //        if (currenttypes == null) { return types; }
        //        types.AddRange(currenttypes.Where(o => o == typeinfo));
        //    }
        //    return types;
        //}

        /// <summary>
        /// 获取程序及中所有的指定类型的类
        /// </summary>
        /// <param name="typeinfo"></param>
        /// <returns></returns>
        public static List<Type> GetChildrenTypesInAssembly(Type typeinfo, string dictoryPath)
        {
            List<Type> types = new List<Type>();
            List<Assembly> asses = new List<Assembly>();
            DirectoryInfo info = new DirectoryInfo(dictoryPath);
            foreach (var f in info.GetFiles("*.dll"))
            {
                try
                {
                    asses.Add(Assembly.LoadFrom(f.FullName));
                }
                catch (Exception)
                {
                }
            }
            /////////////////////////
            foreach (var a in asses)
            {
                try
                {
                    var currenttypes = GetAllClass(a);
                    if (currenttypes == null) { continue; }
                    types.AddRange(currenttypes.Where(o => o.GetInterface(typeinfo.Name) != null));
                }
                catch (Exception)
                { 
                } 
            }
            return types;
        }


    }
}
