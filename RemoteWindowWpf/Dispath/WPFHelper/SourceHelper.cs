using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Dispath
{
    public class SourceHelper
    {
        /// <summary>
        ///根据资源字典路径和资源的key，找到资源
        ///  //source实例
        //_resourceDictionary.Source = new Uri(@"pack://application:,,,/MEFA.Common.Audit;component/Communication/Controls/TreeViewItemToggleButtonStyle.xaml");
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T FindSourceByKey<T>(string sourcePath, string key) where T : class
        {
            ResourceDictionary _resourceDictionary = new ResourceDictionary();
            _resourceDictionary.Source = new Uri(sourcePath);
            var list = _resourceDictionary.Cast<DictionaryEntry>().ToList();
            if (list == null || list.Count < 1)
            {
                return null;
            }
            return (T)list.Where(o => o.Key.ToString() == key).First().Value;
        }

        public static T GetChildObject<T>(DependencyObject obj) where T : class
        {
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                var o = child as T;
                if (o != null)
                {
                    return o;
                }
                // 在下一级中没有找到指定名字的子控件，就再往下一级找
                var grandChild = GetChildObject<T>(child);
                if (grandChild != null)
                    return grandChild;
            }
            return null;
        }

        public static T GetChildObject<T>(DependencyObject obj, string name) where T : class
        {
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                var o = child as T;
                if (o != null && (o as FrameworkElement).Name == name | string.IsNullOrEmpty(name))
                {
                    return o;
                }
                // 在下一级中没有找到指定名字的子控件，就再往下一级找
                var grandChild = GetChildObject<T>(child, name);
                if (grandChild != null)
                    return grandChild;
            }
            return null;
        }


        public static List<T> GetChildrenObject<T>(DependencyObject obj) where T : class
        {
            List<T> list = new List<T>();
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                var o = child as T;
                if (o != null)
                {
                    list.Add(o);
                }
                // 在下一级中没有找到指定名字的子控件，就再往下一级找
                var grandChild = GetChildrenObject<T>(child);
                if (grandChild != null)
                    list.AddRange(grandChild);
            }
            return list;
        }

        /// <summary>
        /// 查找父控件
        /// </summary>
        /// <typeparam name="T">父控件的类型</typeparam>
        /// <param name="obj">要找的是obj的父控件</param>
        /// <param name="name">想找的父控件的Name属性</param>
        /// <returns>目标父控件</returns>
        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                var o = parent as T;
                if (o != null && (o.Name == name | string.IsNullOrEmpty(name)))
                {
                    return o;
                }

                // 在上一级父控件中没有找到指定名字的控件，就再往上一级找
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

    }
}
