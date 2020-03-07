using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseKeyPlayback
{
    public class MouseEvent
    {
		public CursorPoint Location { get; set; }
        public MouseHook.MouseEvents Action { get; set; }
        public int Value { get; set; }
    }
}
