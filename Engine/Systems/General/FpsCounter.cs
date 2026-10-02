using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformsEngine.Engine.Systems.General
{
    public class FpsCounter
    {
        private readonly Queue<double> _frameTimes = new Queue<double>();
        private const int _windowSize = 60;

        public int Fps { get; private set; }
        public double FrameTimeMs { get; private set; }

        public void Tick(double delta)
        {
            _frameTimes.Enqueue(delta);
            if (_frameTimes.Count > _windowSize)
                _frameTimes.Dequeue();

            double total = 0;
            foreach (var t in _frameTimes) total += t;

            if (total > 0)
            {
                Fps = (int)(_frameTimes.Count / total);
                FrameTimeMs = (total / _frameTimes.Count) * 1000.0;
            }
        }
    }
}