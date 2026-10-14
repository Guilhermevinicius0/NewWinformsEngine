using System;

namespace WinformsEngine.Engine.Systems.General
{

    public class CountDownTimer
    {
        private Scene.Scene _scene;
        private float _currentSeconds;
        public float Seconds;
        public bool AutoRestart = false;
        public bool IsPaused { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsFinished { get; private set; }

        private bool _subscribed = false;

        public event Action StartedEvent;
        public event Action StoppedEvent;
        public event Action PausedEvent;
        public event Action UnpausedEvent;
        public event Action ElapsedEvent;

        private readonly Action _action;

        public CountDownTimer(Scene.Scene scene, float seconds, Action action = null, bool autoRestart = false)
        {
            _scene = scene;
            Seconds = seconds;
            _action = action;
            AutoRestart = autoRestart;
        }

        private void Update(float delta)
        {
            if (IsPaused || IsFinished)
                return;

            _currentSeconds -= delta;

            if (_currentSeconds <= 0)
            {
                IsFinished = true;
                IsRunning = false;
                _scene.Timer.Tick -= Update;

                if (_action != null)
                    _action();

                ElapsedEvent?.Invoke();

                if (AutoRestart)
                    Start();
            }
        }

        public void Pause()
        {
            if (IsPaused)
                return;

            IsPaused = true;
            IsRunning = false;

            if (_subscribed)
            {
                _scene.Timer.Tick -= Update;
                _subscribed = false;
            }

            PausedEvent?.Invoke();
        }

        public void Resume()
        {
            if (!IsPaused)
                return;

            IsPaused = false;
            IsRunning = true;

            if (!_subscribed)
            {
                _scene.Timer.Tick += Update;
                _subscribed = true;
            }

            UnpausedEvent?.Invoke();
        }

        public void Start()
        {
            IsFinished = false;
            _currentSeconds = Seconds;
            IsPaused = false;
            IsRunning = true;

            if (!_subscribed)
            {
                _scene.Timer.Tick += Update;
                _subscribed = true;
            }

            StartedEvent?.Invoke();
        }

        public void Stop()
        {
            bool wasRunning = IsPaused || IsRunning;

            if (_subscribed)
            {
                _scene.Timer.Tick -= Update;
                _subscribed = false;
            }

            _currentSeconds = Seconds;
            IsPaused = false;
            IsFinished = false;
            IsRunning = false;

            if (wasRunning)
                StoppedEvent?.Invoke();
        }
    }
}