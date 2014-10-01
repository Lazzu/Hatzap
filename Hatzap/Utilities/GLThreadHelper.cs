using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Utilities
{
    public static class GLThreadHelper
    {
        static GameWindow gameWindow = null;

        /// <summary>
        /// It is very important to lock() this when you are using the GL Context outside main thread.
        /// </summary>
        static object ContextLock = new object();

        static int currentContextThreadId;
        static bool locked = false;

        public static bool Locked {
            get
            {
                lock(ContextLock)
                {
                    return locked;
                }
            }
        }

        /// <summary>
        /// Initializes the GLThreadHelper class variables.
        /// </summary>
        /// <param name="gw">Your game window.</param>
        public static void Initialize(GameWindow gw)
        {
            gameWindow = gw;
            locked = false;
        }

        /// <summary>
        /// Make the GL Context current in the thread calling this function and set the Locked property to true.
        /// </summary>
        public static bool MakeGLContextCurrent()
        {
            if (gameWindow == null)
                throw new GraphicsException("You need to initialize GLThreadHelper before you can use it!");

            var threadID = Thread.CurrentThread.ManagedThreadId;

            if(threadID != currentContextThreadId)
            {
                if (Locked)
                    return false;

                gameWindow.MakeCurrent();
                currentContextThreadId = threadID;
                locked = true;
            }

            return true;
        }

        public static void Unlock()
        {
            lock(ContextLock)
            {
                if (!locked)
                    return;

                locked = false;
            }
        }
    }
}
