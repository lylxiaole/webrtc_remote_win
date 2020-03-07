using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispath.MoudleInterface
{
    public class MoudleCommon
    {
        static MoudleCommon()
        {
            getAllViews();
        }

        public static List<MoudleInfo> NormalViews { get; set; }
        public static List<MoudleInfo> WindowViews { get; set; }

        public static List<MoudleInfo> PageViews { get; set; }
        static void getAllViews()
        {
            NormalViews = new List<MoudleInfo>();
            WindowViews = new List<MoudleInfo>();
            PageViews = new List<MoudleInfo>();

            var views = ReflectionHelper.GetChildrenTypesInAssembly(typeof(INormalView), System.AppDomain.CurrentDomain.BaseDirectory);
            var windows = ReflectionHelper.GetChildrenTypesInAssembly(typeof(IWindowView), System.AppDomain.CurrentDomain.BaseDirectory);
            var pages = ReflectionHelper.GetChildrenTypesInAssembly(typeof(IPage), System.AppDomain.CurrentDomain.BaseDirectory);

            foreach (var v in views)
            {
                var att = v.GetCustomAttributes(typeof(ViewAttribute), true)?.Cast<ViewAttribute>().FirstOrDefault();
                if (att == null) { continue; }
                NormalViews.Add(new MoudleInfo
                {
                    MoudleImage = att.MoudleImage,
                    MoudleName = att.MoudleName,
                    MoudleType = v
                });
            }

            foreach (var v in windows)
            {
                var att = v.GetCustomAttributes(typeof(ViewAttribute), true)?.Cast<ViewAttribute>().FirstOrDefault();
                if (att == null) { continue; }
                WindowViews.Add(new MoudleInfo
                {
                    MoudleImage = att.MoudleImage,
                    MoudleName = att.MoudleName,
                    MoudleType = v
                });
            }

            foreach (var v in pages)
            {
                var att = v.GetCustomAttributes(typeof(ViewAttribute), true)?.Cast<ViewAttribute>().FirstOrDefault();
                if (att == null) { continue; }
                PageViews.Add(new MoudleInfo
                {
                    MoudleImage = att.MoudleImage,
                    MoudleName = att.MoudleName,
                    MoudleType = v
                });
            }
        }



    }
}
