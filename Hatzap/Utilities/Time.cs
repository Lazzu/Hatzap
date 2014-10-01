using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Hatzap.Utilities
{
    public static class Time
    {
        static double updateTime, renderTime, updateDeltaTime, renderDeltaTime;

        /// <summary>
        /// Elapsed time since UpdateFrames started. Each UpdateFrame increases this by it's delta value.
        /// </summary>
        public static double UpdateTime { get { return updateTime; } }

        /// <summary>
        /// Elapsed time since RenderFrames started. Each RenderFrame increases this by it's delta value.
        /// </summary>
        public static double RenderTime { get { return renderTime; } }

        /// <summary>
        /// Current update frame's delta time
        /// </summary>
        public static double UpdateDeltaTime { get { return updateDeltaTime; } }

        /// <summary>
        /// Current update frame's delta time
        /// </summary>
        public static double RenderDeltaTime { get { return renderDeltaTime; } }

        public static void Initialize()
        {
            updateTime = 0;
            renderTime = 0;
            updateDeltaTime = 0;
            renderDeltaTime = 0;
        }

        public static void Update(double delta)
        {
            updateTime += delta;
            updateDeltaTime = delta;
        }

        public static void Render(double delta)
        {
            renderTime += delta;
            renderDeltaTime = delta;
        }
    }
}
