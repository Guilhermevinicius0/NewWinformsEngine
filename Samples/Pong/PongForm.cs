using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using WinformsEngine.Engine.Systems;
using WinformsEngine.Samples.Pong.Entities;
using WinformsEngine.Utils;

namespace WinformsEngine.Samples.Pong
{
    public partial class PongForm : Form
    {
        private Scene _scene { get; set; } = new Scene();
        private Score _score = new Score();

        public PongForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        private void Pong_Forms_Load(object sender, EventArgs e)
        {
            this.ConfigureForm(_scene);

            int textAreaHeight = 20;

            var ball = new Ball(
                0,
                ClientSize.Height - textAreaHeight,
                scene: _scene,
                size: new Vector2(16, 16),
                position: new Vector2(this.ClientSize.Width / 2, this.ClientSize.Height / 2),
                speed: 100,
                maxSpeed: 1000);

            var pad1 = new Paddle(
                0,
                ClientSize.Height - textAreaHeight,
                keyUp: Keys.W,
                keyDown: Keys.S,
                scene: _scene,
                size: new Vector2(16, 64),
                position: new Vector2(0, _scene.ScreenSize.Y / 2 - 16),
                speed: 150,
                maxSpeed: 150);

            var pad2 = new Paddle(
                0,
                ClientSize.Height - textAreaHeight,
                keyUp: Keys.Up,
                keyDown: Keys.Down,
                scene: _scene,
                size: new Vector2(16, 64),
                position: new Vector2(_scene.ScreenSize.X - 16, _scene.ScreenSize.Y / 2 - 16),
                speed: 150,
                maxSpeed: 150);

            var labelScore1 = new Engine.UI.Label(this.Font, Color.Black, _scene);
            labelScore1.Position = new Vector2(10, this.ClientSize.Height - 20);
            labelScore1.Content = "0";
            labelScore1.AddTag("left");

            var labelScore2 = new Engine.UI.Label(this.Font, Color.Black, _scene);
            labelScore2.Position = new Vector2(this.ClientSize.Width - 25, this.ClientSize.Height - 20);
            labelScore2.Content = "0";
            labelScore2.AddTag("right");

            var rand = new Random();
            ball.Direction = new Vector2(rand.Next(-1, 1) == 0 ? -1 : 1, rand.Next(-1, 1) == 0 ? -0.3f : 0.3f);

            ball.DefineDefaultPosition();
            pad1.DefineDefaultPosition();
            pad2.DefineDefaultPosition();

            ball.AddTag("ball");
            pad1.AddTag("pad");
            pad2.AddTag("pad");
            pad1.AddTag("left");
            pad2.AddTag("right");

            ball.Scored += OnBallScored;

            _scene.AddEntity(ball);
            _scene.AddEntity(pad1);
            _scene.AddEntity(pad2);

            _scene.AddUIElement(labelScore1);
            _scene.AddUIElement(labelScore2);

            this.StartGameLoop(_scene);
        }

        public void OnBallScored(Ball ball, string text)
        {
            if (text == "left")
                _score.Player2_Score++;
            else
                _score.Player1_Score++;

            var lbl = (_scene.GetUIElementsTaggedWith(text).FirstOrDefault() as Engine.UI.Label);
            lbl.Content = text == "left" ? _score.Player2_Score.ToString() : _score.Player1_Score.ToString();

            foreach (var entity in _scene.GetEntitiesTaggedWith("pad"))
            {
                if (entity is Paddle pad)
                    pad.Reset();
            }

            ball.Reset();
        }
    }
}