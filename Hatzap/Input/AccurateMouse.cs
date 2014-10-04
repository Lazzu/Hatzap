using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hatzap.Utilities;
using OpenTK;
using OpenTK.Input;

namespace Hatzap.Input
{
    public class AccurateMouse : IMouseInputProvider
    {
        GameWindow gameWindow;

        Dictionary<MouseButton, bool> downButtons = new Dictionary<MouseButton, bool>();
        Dictionary<MouseButton, bool> pushedButtons = new Dictionary<MouseButton, bool>();
        Dictionary<MouseButton, bool> clickedButtons = new Dictionary<MouseButton, bool>();
        Dictionary<MouseButton, double> clickTimers = new Dictionary<MouseButton, double>();

        public int X
        {
            get;
            protected set;
        }

        public int Y
        {
            get;
            protected set;
        }

        public OpenTK.Vector2 Position
        {
            get { return new Vector2(X, Y); }
        }

        public float Wheel
        {
            get;
            protected set;
        }

        public double ClickInterval
        {
            get;
            set;
        }

        public bool OnGameWindow
        {
            get;
            protected set;
        }

        MouseButton[] allButtons;

        public void Initialize(GameWindow gw)
        {
            ClickInterval = 0.15f;

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

            gw.MouseWheel += gw_MouseWheel;
            gw.MouseMove += gw_MouseMove;
            gw.MouseUp += gw_MouseUp;
            gw.MouseDown += gw_MouseDown;

            OnGameWindow = true;
        }

        void gw_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Wheel = e.ValuePrecise;
        }

        void gw_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("MouseDown");

            var button = e.Button;

            var previous = downButtons[button];

            if (!previous)
            {
                pushedButtons[button] = true;
            }

            downButtons[button] = true;
            clickTimers[button] = Time.UpdateTime;
        }

        void gw_MouseUp(object sender, OpenTK.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("MouseUp");

            var button = e.Button;

            downButtons[button] = false;

            var timestamp = Time.UpdateTime;
            var downTimeStamp = clickTimers[button];

            if (timestamp - downTimeStamp <= ClickInterval)
            {
                clickedButtons[button] = true;
            }
        }

        void gw_MouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            X = e.X;
            Y = e.Y;
        }

        public void FrameEnd()
        {
            foreach (var button in allButtons)
            {
                pushedButtons[button] = false;
                clickedButtons[button] = false;
            }
        }

        public void Update()
        {
            
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




        public bool IsInsideRect(Vector2 min, Vector2 max)
        {
            return min.X < X && min.Y < Y && max.X > X && max.Y > Y;
        }


        public bool IsButtonClicked()
        {
            foreach (var item in allButtons)
            {
                if (IsButtonClicked(item))
                    return true;
            }
            return false;
        }

        public MouseButton[] GetClickedButtons()
        {
            List<MouseButton> clicked = new List<MouseButton>();

            foreach (var item in allButtons)
            {
                if (clickedButtons[item])
                    clicked.Add(item);
            }

            return clicked.ToArray();
        }
    }
}
