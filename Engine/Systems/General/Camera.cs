using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.Entities;

namespace WinformsEngine.Engine.Systems.General
{
    public class Camera : Transform
    {
        public Entity Target;
        public bool AllowFollow = false;
        public float Zoom = 1f;
        public float Smothness = 5f;
        public float Offset_X = 0f;
        public float Offset_Y = -100f;

        public Camera(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public Vector2 WorldToScreen(Vector2 worldPos)
        {
            return (worldPos - Position) * Zoom;
        }

        public Vector2 ScreenToWorld(Vector2 screenPos)
        {
            return screenPos / Zoom + Position;
        }

        public void Update(float delta)
        {
            if (AllowFollow && Target != null)
            {
                Vector2 targetPosition = Target.CenteredPosition;

                targetPosition.X += Offset_X;
                targetPosition.Y += Offset_Y;

                Position = Vector2.Lerp(Position, targetPosition - (Size / 2f) / Zoom, Smothness * delta);
            }
        }
    }
}
