using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.Entities;
using WinformsEngine.Engine.Systems.Render;
using WinformsEngine.Engine.Systems.Scene;

namespace WinformsEngine.Samples.FlapBird.Entities
{
    public class Pipe : Entity
    {
        public override bool IsStatic { get; set; }
        public override bool IsPausable { get; set; }
        public override int ZIndex { get; set; }

        private bool _pastMidScreen = false;

        public event Action MidScreenEvent;
        public event Action LeftScreenEvent;

        public Pipe(Scene scene, Vector2 size, Vector2 position, float speed, float maxSpeed) : base(scene, size, position, speed, maxSpeed)
        {
            RenderSystem = new RawSquareRender(this, Color.Green);
        }

        public override void ProcessPhysiques()
        {
            Velocity = new Vector2(-1 * Speed, 0);
            MoveAndCollide();

            if (Right <= ScreenCenter.X && !_pastMidScreen) 
            {
                _pastMidScreen = true;
                MidScreenEvent?.Invoke();
            }

            if (Right < ScreenLeft)
                LeftScreenEvent?.Invoke();
        }
    }
}