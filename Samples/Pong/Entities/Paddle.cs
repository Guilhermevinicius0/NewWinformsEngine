using WinformsEngine.Engine.Entities;
using WinformsEngine.Engine.Systems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformsEngine.Engine.Systems.Render;
using WinformsEngine.Engine.Systems.Inputs;

namespace WinformsEngine.Samples.Pong.Entities
{
    public class Paddle : Entity
    {
        public Keys Up_Key { get; set; }
        public Keys Down_Key { get; set; }

        private KeyboardMouseInput _input => _scene.Input;


        private Vector2 _defaultPosition = Vector2.Zero;
        private float _defaultSpeed;
        public override int ZIndex { get; set; } = 1;
        public override bool IsStatic { get; set; } = false;
        public override bool IsPausable { get; set; } = true;

        public bool BlockInputs = false;
        private readonly float _friction = 0f;

        public int MaxTop { get; set; }
        public int MaxBottom { get; set; }

        public Paddle(int maxTop, int maxBottom, Keys keyUp, Keys keyDown, Scene scene, Vector2 size, Vector2 position, float speed, float maxSpeed) : base(scene, size, position, speed, maxSpeed)
        {
            Up_Key = keyUp;
            Down_Key = keyDown;
            MaxTop = maxTop;
            MaxBottom = maxBottom;
            _defaultSpeed = speed;
            RenderSystem = new RawSquareRender(this, Color.LightGray);
        }

        public void DefineDefaultPosition() 
        {
            _defaultPosition = Position;
        }

        public void Reset() 
        {
            Speed = _defaultSpeed;
            Position = _defaultPosition;
        }

        public override void ProcessPhysiques()
        {
            Direction = BlockInputs ? Vector2.Zero : new Vector2(0, _input.GetAxis(Up_Key, Down_Key));

            if (Direction == Vector2.Zero)
                Velocity *= _friction;
            else
                Velocity = Direction * Speed;

            MoveAndSlide();
            ClampPaddleToScreen();
        }

        public void ClampPaddleToScreen()
        {
            if (Top < MaxTop)
                Top = MaxTop;

            if (Bottom > MaxBottom)
                Bottom = MaxBottom;
        }
    }
}