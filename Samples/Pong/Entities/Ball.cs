using System;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;
using WinformsEngine.Engine.Entities;
using WinformsEngine.Engine.Systems;
using WinformsEngine.Engine.Systems.Render;
using WinformsEngine.Engine.Systems.Sounds;

namespace WinformsEngine.Samples.Pong.Entities
{
    public class Ball : Entity
    {
        private Entity _lastTarget = null;
        public event Action<Ball, string> Scored;
        public override int ZIndex { get; set; } = 1;
        public override bool IsStatic { get; set; } = false;
        public override bool IsPausable { get; set; } = true;

        private Vector2 _defaultPosition;
        private float _defaultSpeed;
        private bool _isResetting;
        private float _resetTimer;

        public int MaxTop { get; set; }
        public int MaxBottom { get; set; }

        public Ball(int maxTop, int maxBottom, Scene scene, Vector2 size, Vector2 position, float speed, float maxSpeed) : base(scene, size, position, speed, maxSpeed)
        {
            MaxTop = maxTop;
            MaxBottom = maxBottom;
            _defaultSpeed = speed;
            RenderSystem = new RawSquareRender(this, Color.LightGray);
            SoundSystem = new BeepSoundPlayer(90, 200);
        }

        public override void ProcessPhysiques()
        {
            if (_isResetting)
            {
                _resetTimer -= GetCurrentDelta();
                if (_resetTimer <= 0)
                {
                    _isResetting = false;
                    Velocity = Direction * Speed;
                }
                return;
            }

            Velocity = Direction * Speed;
            MoveAndSlide();
            BounceOnYBorders();
            CheckScoreConditions();
        }

        public void DefineDefaultPosition()
        {
            _defaultPosition = Position;
        }

        public void Reset()
        {
            Velocity = Vector2.Zero;
            Speed = _defaultSpeed;
            Position = _defaultPosition;
        }

        public void BounceOnYBorders()
        {
            if (Top < MaxTop)
            {
                Direction = new Vector2(Direction.X, Direction.Y * -1);
                Top = MaxTop;
            }
            else if (Bottom > MaxBottom)
            {
                Direction = new Vector2(Direction.X, Direction.Y * -1);
                Bottom = MaxBottom;
            }
        }
        private void CheckScoreConditions()
        {
            if (CenteredX < 0)
            {
                StartReset("right");
            }
            else if (CenteredX > _scene.ScreenSize.X)
            {
                StartReset("left");
            }
        }

        private void StartReset(string player)
        {
            _isResetting = true;
            _resetTimer = 1f;

            Scored?.Invoke(this, player);
            Reset();
        }

        protected override void OnColide(Entity entity)
        {
            if (!entity.IsTaggedWith("pad")) return;

            Console.WriteLine("Colidiu");
            SoundSystem?.PlaySound();

            float hitOffset = (CenteredY - entity.CenteredY) / (entity.Size.Y / 2f);
            Direction = Vector2.Normalize(new Vector2(entity.IsTaggedWith("left") ? 1 : -1, hitOffset));

            if (Math.Abs(Direction.X) < 0.1f)
                Direction = new Vector2(Direction.X < 0 ? -0.2f : 0.2f, Direction.Y);

            if (entity == _lastTarget) return;
            Speed = Math.Min(Speed * 1.05f, MaxSpeed);
            _lastTarget = entity;
        }
    }
}