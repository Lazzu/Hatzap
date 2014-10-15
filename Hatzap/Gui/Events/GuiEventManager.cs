using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Gui.Widgets;
using Hatzap.Utilities;

namespace Hatzap.Gui.Events
{
    internal class GuiEventManager
    {
        Dictionary<GuiEvent, Tuple<Widget, object>> Events = new Dictionary<GuiEvent, Tuple<Widget, object>>();
        /// <summary>
        /// Push event in the event queue. Replaces previous events so that only the latest event will be handled.
        /// </summary>
        /// <param name="e">Event type</param>
        /// <param name="widget">The widget calling the event.</param>
        public void RaiseEvent(GuiEvent e, Widget widget, object args = null)
        {
            var eventObj = Tuple.Create(widget, args);

            if(!Events.ContainsKey(e))
            {
                Events.Add(e, eventObj);
            }
            else
            {
                Events[e] = eventObj;
            }
            
        }

        public void HandleEvents()
        {
            Time.StartTimer("GuiEventManager.HandleEvents()", "Gui");

            foreach (var eKvp in Events)
            {
                var eventObj = eKvp.Value;
                var e = eKvp.Key;

                switch(e)
                {
                    case GuiEvent.KeyDown:
                        eventObj.Item1.OnKeyDown((OpenTK.Input.Key)eventObj.Item2);
                        break;
                    case GuiEvent.KeyUp:
                        eventObj.Item1.OnKeyUp((OpenTK.Input.Key)eventObj.Item2);
                        break;
                    case GuiEvent.KeyPress:
                        eventObj.Item1.OnKeyPress((OpenTK.Input.Key)eventObj.Item2);
                        break;
                    case GuiEvent.MouseClick:
                        eventObj.Item1.OnMouseClick((OpenTK.Input.MouseButton)eventObj.Item2);
                        break;
                    case GuiEvent.MouseDown:
                        eventObj.Item1.OnMouseDown((OpenTK.Input.MouseButton)eventObj.Item2);
                        break;
                    case GuiEvent.MouseUp:
                        eventObj.Item1.OnMouseUp((OpenTK.Input.MouseButton)eventObj.Item2);
                        break;
                    case GuiEvent.MouseEnter:
                        eventObj.Item1.OnMouseEnter();
                        break;
                    case GuiEvent.MouseHover:
                        eventObj.Item1.OnMouseHover();
                        break;
                    case GuiEvent.MouseLeave:
                        eventObj.Item1.OnMouseLeave();
                        break;
                }
            }
            Events.Clear();

            Time.StopTimer("GuiEventManager.HandleEvents()");
        }

        public static GuiEventManager Current = new GuiEventManager();
    }
}
