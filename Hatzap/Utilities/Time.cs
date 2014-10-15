using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            try
            {
                StopTimer("Frame Total");
            }
            catch
            { }

            StartTimer("Frame Total", "Loop");

            SaveHistory();
        }

        [Conditional("DEBUG")]
        private static void SaveHistory()
        {
            foreach (var item in timers)
            {
                var key = item.Key;
                var measure = measures[key];

                measures[key] = new List<double>();

                if (!History.ContainsKey(key))
                    History.Add(key, new Queue<List<double>>());

                var history = History[key];

                history.Enqueue(measure);

                if (history.Count > 10)
                    history.Dequeue();
            }
            
        }

        public static void Render(double delta)
        {
            renderTime += delta;
            renderDeltaTime = delta;
        }

        static Dictionary<string, Stopwatch> timers = new Dictionary<string, Stopwatch>();
        static Dictionary<string, List<double>> measures = new Dictionary<string, List<double>>();

        public static Dictionary<string, string> Groups = new Dictionary<string, string>();
        public static Dictionary<string, Queue<List<double>>> History = new Dictionary<string, Queue<List<double>>>();

        [Conditional("DEBUG")]
        public static void StartTimer(string timer, string group)
        {
            if(!timers.ContainsKey(timer))
            {
                timers.Add(timer, new Stopwatch());
                Groups.Add(timer, group);
                measures.Add(timer, new List<double>());
            }

            timers[timer].Restart();
        }

        [Conditional("DEBUG")]
        public static void StopTimer(string timer)
        {
            if(!timers.ContainsKey(timer))
            {
                throw new Exception("Tried to stop a timer that does not exist.");
            }

            var sw = timers[timer];

            sw.Stop();
            measures[timer].Add(sw.Elapsed.TotalMilliseconds);
        }

        public static double GetAverage(string timer)
        {
            double value = 0;
            int n = 0;
            foreach (var item in History[timer])
            {
                foreach (var item2 in item)
                {
                    value += item2;
                }
                n++;
            }
            return value / n;
        }
    }
}
