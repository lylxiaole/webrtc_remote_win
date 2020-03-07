using Dispath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controls
{
    public class ItemDataInterface: ViewModelBase
    {
        //是否展开
        public virtual Boolean IsExpand { get; set; }
        //是否选择
        public virtual Boolean IsSelected { get; set; }
        //展现在treeviewitem上的值（键）
        public virtual string Content { get; set; }
        //代表treeviewitem的值（值）,这个值是item选中后，右侧的panel中显示该值，该值一般是一个element
        public virtual object ItemValue { get; set; }
        //该节点的子节点
        public virtual List<ItemDataInterface> Children { get; set; }



    }
}
