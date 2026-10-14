using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformsEngine.Engine.Systems.Enums;
using WinformsEngine.Engine.Systems.Scene;
using WinformsEngine.Samples.FlapBird.Entities;
using WinformsEngine.Utils;

namespace WinformsEngine.Samples.FlapBird
{
    public partial class FlapBirdForm : Form
    {
        private Scene _scene = new Scene();
        private PipeSpawner _pipeSpawner;
        private Bird bird;
        private Engine.UI.Label startHintLabel;
        private Engine.UI.Label pointsLabel;

        private int _points = 0;

        public FlapBirdForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        private void FlapBirdForm_Load(object sender, EventArgs e)
        {
            this.ConfigureForm(_scene);

            _scene.SceneStateChangedEvent += OnSceneStateChange;
            _scene.Input.KeyPressedEvent += OnKeyPressed;

            _pipeSpawner = new PipeSpawner(_scene);
            _pipeSpawner.PointEvent += OnPointAchieved;

            bird = new Bird(
                scene: _scene, 
                size: new Vector2(32, 32),
                position: new Vector2(_scene.ScreenCenter.X - 16, _scene.ScreenCenter.Y - 16),
                speed: 100, 
                maxSpeed: 1000);

            startHintLabel = new Engine.UI.Label(
                font: this.Font,
                textColor: Color.Black,
                scene: _scene);

            pointsLabel = new Engine.UI.Label(
                font: this.Font,
                textColor: Color.Black,
                scene: _scene);

            bird.AddTag("Bird");

            startHintLabel.Content = "Pressione Espaço para iniciar";
            pointsLabel.Content = "Points: 0";

            startHintLabel.Position = new Vector2(_scene.ScreenLeft + 70, _scene.ScreenTop + 70);
            pointsLabel.Position = new Vector2(10, _scene.ScreenBottom - 20);

            _scene.AddEntity(bird);
            _scene.AddUIElement(startHintLabel);
            _scene.AddUIElement(pointsLabel);

            _scene.ChangeSceneState(SceneStates.Menu);
            this.StartGameLoop(_scene);
        }

        private void OnKeyPressed(Keys key) 
        {
            if (_scene.CurrentState != SceneStates.Menu)
                return;

            if (key == Keys.Space) 
            {
                _scene.Input.KeyPressedEvent -= OnKeyPressed;
                bird.IsPaused = false;
                _scene.ChangeSceneState(SceneStates.Playing);
            }
        }

        private void OnPointAchieved() 
        {
            _points++;
            pointsLabel.Content = "Points: " + _points;
        }

        private void OnSceneStateChange(SceneStates state) 
        {
            if (bird == null)
                return;

            if (state == SceneStates.GameOver)
            {
                _points = 0;
                pointsLabel.Content = "Points: " + _points;
                _scene.ChangeSceneState(SceneStates.Menu);
                return;
            }

            if (state == SceneStates.Menu) 
            {
                startHintLabel.IsVisible = true;
                pointsLabel.IsVisible = false;

                this.BackColor = Color.LightGray;
                bird.DisableInputListener();
                bird.Velocity = Vector2.Zero;
                bird.Position = new Vector2(bird.Position.X, _scene.ScreenSize.Y + bird.Size.Y + 1);
                bird.IsPaused = true;
                _pipeSpawner.Stop();

                _scene.Input.KeyPressedEvent += OnKeyPressed;
            }
            else if (state == SceneStates.Playing)
            {
                startHintLabel.IsVisible = false;
                pointsLabel.IsVisible = true;

                this.BackColor = Color.White;
                bird.EnableInputListener();
                bird.Velocity = Vector2.Zero;
                bird.Position = new Vector2(_scene.ScreenSize.X / 2 - bird.Size.X / 2, _scene.ScreenSize.Y / 2 - bird.Size.Y / 2);
                bird.IsPaused = false;
                _pipeSpawner.Start();
            }
        }
    }
}   