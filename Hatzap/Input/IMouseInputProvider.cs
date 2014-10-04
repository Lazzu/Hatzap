using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace Hatzap.Input
{
    public interface IMouseInputProvider
    {
        void Initialize(GameWindow gw);
        void FrameEnd();
        void Update();

        /// <summary>
        /// Returns true if the mouse button is held down.
        /// </summary>
        /// <param name="button">The mouse button</param>
        /// <returns></returns>
        bool IsButtonDown(MouseButton button);

        /// <summary>
        /// Returns true if the mouse button was pushed down this frame.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool IsButtonPushed(MouseButton button);

        /// <summary>
        /// Returns if the mouse button was clicked on this frame.
        /// </summary>
        /// <param name="button">The mouse button</param>
        /// <returns>True if the given mouse button was clicked on this frame.</returns>
        bool IsButtonClicked(MouseButton button);

        /// <summary>
        /// Returns if any mouse button was clicked on this frame.
        /// </summary>
        /// <returns>True if any mouse buttons were clicked on this frame.</returns>
        bool IsButtonClicked();

        /// <summary>
        /// Returns all buttons that were clicked on this frame.
        /// </summary>
        /// <returns>Array of buttons.</returns>
        MouseButton[] GetClickedButtons();

        /// <summary>
        /// Returns if the mouse cursor is inside the rectangle AABB
        /// </summary>
        /// <param name="min">The min coords</param>
        /// <param name="max">The max coords</param>
        /// <returns>True if the mouse cursor is inside the rectangle.</returns>
        bool IsInsideRect(Vector2 min, Vector2 max);

        int X { get; }
        int Y { get; }

        Vector2 Position { get; }

        float Wheel { get; }

        /// <summary>
        /// The time in seconds in between mouse down and mouse up to register it as a click event.
        /// </summary>
        double ClickInterval { get; set; }

        bool OnGameWindow { get; }
    }
}
