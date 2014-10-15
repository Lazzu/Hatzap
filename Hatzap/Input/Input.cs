using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hatzap.Utilities;
using OpenTK;

namespace Hatzap.Input
{
    public static class UserInput
    {
        static GameWindow gameWindow;

        static IMouseInputProvider mouse;
        static IKeyboardInputProvider keyboard;

        public static IMouseInputProvider Mouse { get { return mouse; } }
        public static IKeyboardInputProvider Keyboard { get { return keyboard; } }
        
        public static void Initialize(GameWindow gw, Type mouseProviderType, Type keyboardProviderType)
        {
            if (!typeof(IMouseInputProvider).IsAssignableFrom(mouseProviderType))
            {
                throw new ArgumentException("Mouse provider must implement IMouseInputProvider.");
            }

            if (!typeof(IKeyboardInputProvider).IsAssignableFrom(keyboardProviderType))
            {
                throw new ArgumentException("Keyboard provider must implement IKeyboardInputProvider.");
            }
            
            gameWindow = gw;
            
            mouse = (IMouseInputProvider)Activator.CreateInstance(mouseProviderType);
            mouse.Initialize(gw);

            keyboard = (IKeyboardInputProvider)Activator.CreateInstance(keyboardProviderType);
            keyboard.Initialize(gw);
        }

        public static void Update()
        {
            Time.StartTimer("UserInput.Update()", "Update");
            mouse.Update();
            keyboard.Update();
            Time.StopTimer("UserInput.Update()");
        }

        public static void FrameEnd()
        {
            Time.StartTimer("UserInput.FrameEnd()", "Render");
            mouse.FrameEnd();
            keyboard.FrameEnd();
            Time.StopTimer("UserInput.FrameEnd()");
        }
    }
}
