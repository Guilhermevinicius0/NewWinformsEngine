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

        private Vector2 _size { get; set; }
        private Vector2 _position { get; set; }
        
        public virtual Vector2 Size 
        {
            get => _size;
            set 
            {
                if (_size == value) return;
                var previews = _size;
                _size = value;
                SizeChangedEvent?.Invoke(previews);
            }
        }

        public virtual Vector2 Position
        {
            get => _position;
            set 
            {
                if (_position == value) return;
                var previews = _position;
                _position = value;
                PositionChangedEvent?.Invoke(previews);
            }
        }

        public virtual float Left { get => Position.X; set => Position = new Vector2(value, Position.Y); }
        public virtual float Right { get => Position.X + Size.X; set => Position = new Vector2(value - Size.X, Position.Y); }
        public virtual float Top { get => Position.Y; set => Position = new Vector2(Position.X, value); }
        public virtual float Bottom { get => Position.Y + Size.Y; set => Position = new Vector2(Position.X, value - Size.Y); }

        public virtual float CenteredX { get => (Left + Right) / 2; set => Position = new Vector2(value - Size.X / 2, Position.Y); }
        public virtual float CenteredY { get => (Top + Bottom) / 2; set => Position = new Vector2(Position.X, value - Size.Y / 2); }

        public virtual Vector2 CenteredPosition { get => new Vector2(CenteredX, CenteredY); set => Position = new Vector2(value.X - Size.X / 2, value.Y - Size.Y / 2); }

        protected event Action<Vector2> PositionChangedEvent;
        protected event Action<Vector2> SizeChangedEvent;
    }
}
