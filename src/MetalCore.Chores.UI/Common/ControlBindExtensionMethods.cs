using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetalCore.Chores.UI.Common
{
    public static class ControlBindExtensionMethods
    {
        public static BindableProperty BindTitle<T>(this T _) where T : Page => Page.TitleProperty;

        public static BindableProperty BindText(this Button _) => Button.TextProperty;
        public static BindableProperty BindClick(this Button _) => Button.CommandProperty;
    }
}
