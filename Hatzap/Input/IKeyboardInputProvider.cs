using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace Hatzap.Input
{
    public interface IKeyboardInputProvider
    {
        bool CaptureText { get; set; }
        string CapturedText { get; set; }
        bool AllowCaptureLineBreaks { get; set; }

        void Initialize(GameWindow gw);
        void FrameEnd();
        void Update();

        /// <summary>
        /// Returns true if the keyboard key is held down.
        /// </summary>
        /// <param name="button">The keyboard key</param>
        /// <returns>True if the key is currently down</returns>
        bool IsKeyDown(Key key);
    }
}
