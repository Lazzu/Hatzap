using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Input;

namespace Hatzap.Input
{
    public class RealTimeMouse : IMouseInputProvider
    {
        GameWindow gameWindow;

        int x, y;
        float wheel, wheelDelta;

        bool lastHWUpdate = false;
        public bool HWUpdate { get; set; }

        Dictionary<MouseButton, bool> downButtons = new Dictionary<MouseButton,bool>();
        Dictionary<MouseButton, bool> pushedButtons = new Dictionary<MouseButton, bool>();
        Dictionary<MouseButton, bool> clickedButtons = new Dictionary<MouseButton, bool>();
        Dictionary<MouseButton, double> clickTimers = new Dictionary<MouseButton, double>();

        public int X { get { return x; } }
        public int Y { get { return y; } }

        public Vector2 Position { get { return new Vector2(x, y); } }

        public float Wheel { get { return wheel; } }

        /// <summary>
        /// The time in seconds in between mouse down and mouse up to register it as a click event.
        /// </summary>
        public double ClickInterval { get; set; }

        public bool OnGameWindow { get; protected set; }

        MouseButton[] allButtons;

        MouseState state;

        public void Initialize(GameWindow gw)
        {
            gameWindow = gw;

            allButtons = (MouseButton[])Enum.GetValues(typeof(MouseButton));

            // Add all button states to defaults.
            foreach (MouseButton button in allButtons)
            {
                downButtons.Add(button, false);
                pushedButtons.Add(button, false);
                clickedButtons.Add(button, false);
                clickTimers.Add(button, 0);
            }

            gw.MouseLeave += gw_MouseLeave;
            gw.MouseEnter += gw_MouseEnter;

            OnGameWindow = true;

            ClickInterval = 0.25;
        }

        int offsetx, offsety;
        
        public void Update()
        {
            state = OpenTK.Input.Mouse.GetState();
            
            if(HWUpdate)
            {
                var p = gameWindow.PointToClient(new System.Drawing.Point(state.X, state.Y));

                if(!lastHWUpdate)
                {
                    var lastx = x;
                    var lasty = y;

                    offsetx = x - p.X;
                    offsety = y - p.Y;
                }

                x = p.X + offsetx;
                y = p.Y + offsety;
            }
            else
            {
                x = gameWindow.Mouse.X;
                y = gameWindow.Mouse.Y;
            }

            lastHWUpdate = HWUpdate;

            var wheelValue = state.WheelPrecise;
            wheel = state.WheelPrecise;

            for(int i = 0; i < allButtons.Length; i++)
            {
                MouseButton button = allButtons[i];

                if (state.IsButtonDown(button))
                {
                    MouseDown(button);
                }
                else if(downButtons[button])
                {
                    MouseUp(button);
                }
            }

        }

        public void FrameEnd()
        {
            foreach (var button in allButtons)
            {
                pushedButtons[button] = false;
                clickedButtons[button] = false;
            }
        }

        void gw_MouseLeave(object sender, EventArgs e)
        {
            OnGameWindow = false;
        }

        void gw_MouseEnter(object sender, EventArgs e)
        {
            OnGameWindow = true;
        }

        void MouseDown(MouseButton button)
        {
            var previous = downButtons[button];

            if(!previous)
            {
                pushedButtons[button] = true;
            }

            downButtons[button] = true;
            clickTimers[button] = Time.UpdateTime;
        }

        void MouseUp(MouseButton button)
        {
            downButtons[button] = false;

            var timestamp = Time.UpdateTime;
            var downTimeStamp = clickTimers[button];

            if(timestamp - downTimeStamp <= ClickInterval)
            {
                clickedButtons[button] = true;
            }

        }

        /// <summary>
        /// Returns true if the mouse button is held down.
        /// </summary>
        /// <param name="button">The mouse button</param>
        /// <returns></returns>
        public bool IsButtonDown(MouseButton button)
        {
            return downButtons[button];
        }

        /// <summary>
        /// Returns true if the mouse button was pushed down this frame.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool IsButtonPushed(MouseButton button)
        {
            return pushedButtons[button];
        }

        /// <summary>
        /// Returns if the mouse button was clicked on this frame.
        /// </summary>
        /// <param name="button">The mouse button</param>
        /// <returns></returns>
        public bool IsButtonClicked(MouseButton button)
        {
            return clickedButtons[button];
        }

    }
}
