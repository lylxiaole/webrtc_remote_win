using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Controls
{

    //所以重写treeview的控件必须继承这个类，。这样能限定 控件的节点数据类型
    public class TreeViewBase : TreeView
    {
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (((IEnumerable<ItemDataInterface>)newValue) == null)
            {
                throw new Exception("数据类型错误");
            }
        }
    }
}
