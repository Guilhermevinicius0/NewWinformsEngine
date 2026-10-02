using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WinformsEngine.Engine.Systems.General
{
    public abstract class Transform
    {
        public Vector2 Size { get; set; }
        public Vector2 Position { get; set; }
        public float Left { get => Position.X; set => Position = new Vector2(value, Position.Y); }
        public float Right { get => Position.X + Size.X; set => Position = new Vector2(value - Size.X, Position.Y); }
        public float Top { get => Position.Y; set => Position = new Vector2(Position.X, value); }
        public float Bottom { get => Position.Y + Size.Y; set => Position = new Vector2(Position.X, value - Size.Y); }

        public float CenteredX { get => (Left + Right) / 2; set => Position = new Vector2(value - Size.X / 2, Position.Y); }
        public float CenteredY { get => (Top + Bottom) / 2; set => Position = new Vector2(Position.X, value - Size.Y / 2); }

        public Vector2 CenteredPosition { get => new Vector2(CenteredX, CenteredY); set => Position = new Vector2(value.X - Size.X / 2, value.Y - Size.Y / 2); }
    }
}
