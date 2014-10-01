using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace Hatzap.Gui.Events
{
    public delegate void MouseEnter();
    public delegate void MouseLeave();
    public delegate void MouseHover();
    public delegate void MouseClick(MouseButton button);
}
