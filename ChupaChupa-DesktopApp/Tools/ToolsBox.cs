using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chupachupa_DesktopApp.Tools
{
    class ToolsBox
    {
        public enum TabIndex
        {
            TAB_ACCOUNT,
            TAB_CATEGORY,
            TAB_CHANNEL,
            TAB_ITEMS,
            TAB_VIEWER
        };

        public static TabIndex TabOnApplicationInit = TabIndex.TAB_CATEGORY;

    }
}
