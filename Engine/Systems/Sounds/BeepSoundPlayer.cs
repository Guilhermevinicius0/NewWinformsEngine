using System;
using System.Threading.Tasks;

namespace WinformsEngine.Engine.Systems.Sounds
{
    public class BeepSoundPlayer : ISoundPlayer
    {
        private const int _frequencyMin = 32;
        private const int _durationMin = 100;
        
        private const int _frequencyMax = 32767;
        private const int _durationMax = 5000;

        private int _frequency = _frequencyMin;
        private int _duration = _durationMin;
        public int Frequency
        {
            get => _frequency;
            set
            {
                if (value < _frequencyMin)
                {
                    _frequency = _frequencyMin;
                    return;
                }

                if (value > _frequencyMax) 
                {
                    _frequency = _frequencyMax;
                    return;
                }

                _frequency = value;
            }
        }

        public int Duration
        {
            get => _duration;
            set 
            {
                if (value < _durationMin)
                {
                    _duration = _durationMin;
                    return;
                }

                if (value > _durationMax)
                {
                    _duration = _durationMax;
                    return;
                }

                _duration = value;
            }
        }

        private static volatile bool isPlaying = false;

        public BeepSoundPlayer(int frequency, int duration)
        {
            Frequency = frequency;
            Duration = duration;
        }

        public void PlaySound()
        {
            if (isPlaying) return;

            isPlaying = true;
            Task.Run(() =>
            {
                Console.Beep(Frequency, Duration);
                isPlaying = false;
            });
        }
    }
}
