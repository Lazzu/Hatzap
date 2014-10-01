using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Input;

namespace Hatzap.Input
{
    public class Keyboard : IKeyboardInputProvider
    {
        GameWindow gameWindow;
        KeyboardState state;
        
        public bool CaptureText { get; set; }

        public string CapturedText { get; set; }

        public bool AllowCaptureLineBreaks { get; set; }

        public void Initialize(OpenTK.GameWindow gw)
        {
            gameWindow = gw;

            // Keydown registers key repeating, keyup does not.
            gw.KeyDown += gw_KeyDown;
            gw.KeyPress += gw_KeyPress;
        }

        void gw_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (!CaptureText)
                return;

            switch(e.Key)
            {
                case Key.Enter:
                    CapturedText += Environment.NewLine;
                    break;
                case Key.Tab:
                    CapturedText += "\t";
                    break;
                case Key.BackSpace:
                    if(CapturedText.Length > 0)
                        CapturedText = CapturedText.Remove(CapturedText.Length - 1);
                    break;
            }
        }

        void gw_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CaptureText)
                return;

            CapturedText += e.KeyChar;
        }

        

        public void Update()
        {
            state = OpenTK.Input.Keyboard.GetState();
        }

        public void FrameEnd()
        {
            
        }

        public bool IsKeyDown(Key key)
        {
            return state.IsKeyDown(key);
        }
    }
}
