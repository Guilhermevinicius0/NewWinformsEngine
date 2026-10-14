using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using WinformsEngine.Engine.Entities;
using WinformsEngine.Engine.Systems.Enums;
using WinformsEngine.Engine.Systems.Render;
using WinformsEngine.Engine.Systems.Scene;

namespace WinformsEngine.Samples.FlapBird.Entities
{
    public class Bird : Entity
    {
        public override bool IsStatic { get; set; } = false;
        public override bool IsPausable { get; set; } = true;
        public override int ZIndex { get; set; } = 1;

        private float _jumpForce = 250;
        private float _gravityForce = 800;

        public Bird(Scene scene, Vector2 size, Vector2 position, float speed, float maxSpeed) : base(scene, size, position, speed, maxSpeed)
        {
            RenderSystem = new RawSquareRender(this, Color.Yellow);
        }

        public override void ProcessPhysiques()
        {
            Velocity += Vector2.UnitY * GetCurrentDelta() * _gravityForce;
            MoveAndCollide();

            ClampHeight();
            CheckDeathByPit();
        }

        protected override void OnKeyPressed(Keys key)
        {
            if (key != Keys.W && key != Keys.Up && key != Keys.Space)
                return;

            Velocity = new Vector2(Velocity.X, -_jumpForce);

        }

        protected override void OnColide(Entity entity)
        {
            if (entity is Pipe)
                ChangeSceneState(SceneStates.GameOver);
        }

        private void ClampHeight()
        {
            if (Top < ScreenTop)
                Top = ScreenTop;
        }

        private void CheckDeathByPit()
        {
            if (Top > ScreenBottom)
                ChangeSceneState(SceneStates.GameOver);
        }
    }
}
