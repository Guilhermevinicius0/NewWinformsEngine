using WinformsEngine.Engine.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using WinformsEngine.Engine.Systems.Render;
using WinformsEngine.Engine.Systems.Inputs;
using WinformsEngine.Engine.UI;
using WinformsEngine.Engine.Systems.General;
using System.Windows.Forms;
using WinformsEngine.Engine.Systems.Enums;

namespace WinformsEngine.Engine.Systems.Scene
{
    public partial class Scene
    {
        #region Fields

        public Camera MainCamera = new Camera(Vector2.Zero, new Vector2(200, 200));
        private FpsCounter _fpsCounter = new FpsCounter();
        public IdleTimer Timer = new IdleTimer();
        public KeyboardMouseInput Input = new KeyboardMouseInput();
        private readonly SpatialGrid _grid = new SpatialGrid();
        private readonly RenderManager _renderManager = new RenderManager();

        public SceneStates CurrentState { get; private set; } = SceneStates.Menu;
       
        public Vector2 ScreenSize = Vector2.One;
        public Vector2 ScreenCenter => ScreenSize / 2;
        public float ScreenRight => ScreenSize.X;
        public float ScreenBottom => ScreenSize.Y;
        public float ScreenLeft => 0;
        public float ScreenTop => 0;

        private float _currentDelta = 0;


        public bool FreezePhysics = false;

        public bool IsPaused { get => _isPaused; }
        private bool _isPaused;

        private const int MAX_QUEUE_INTERACTIONS = 100;

        #endregion

        #region Events

        public event Action<SceneStates> SceneStateChangedEvent;

        #endregion

        #region Methods

        public void ChangeSceneState(SceneStates state)
        {
            if (CurrentState != SceneStates.Pause && state == SceneStates.Pause)
                PauseScene();
            else if (CurrentState == SceneStates.Pause && state != SceneStates.Pause)
                UnpauseScene();

            CurrentState = state;
            SceneStateChangedEvent?.Invoke(state);
        }

        public void SetMainCameraViewPort(Vector2 position, Vector2 size)
        {
            MainCamera.Position = position;
            MainCamera.Size = size;
        }

        public void PauseScene()
        {
            foreach (var entity in DynamicEntities)
                entity.IsPaused = true;

            _isPaused = true;
        }

        public void UnpauseScene()
        {
            foreach (var entity in DynamicEntities)
                entity.IsPaused = false;

            _isPaused = false;
        }

        internal float GetCurrentDelta() => _currentDelta;

        public void ProcessPhysics(float delta)
        {
            _currentDelta = delta;

            foreach (var entity in DynamicEntities)
            {
                if (entity.IsPaused)
                    continue;

                entity.ProcessPhysiques();
            }

            foreach (var entity in DynamicEntities)
                ProcessCollisions(entity);

            foreach (var entity in DynamicEntities)
                SolveOverLapping(entity);


            foreach (var (A, B) in _collisionsSet)
            {
                A.NotifyCollision(B);
                B.NotifyCollision(A);
            }

            _collisionsSet.Clear();
        }

        public void ProcessWorldTick(float delta)
        {
            ProcessEntityQueues();
            ProcessUIQueues();

            if (!FreezePhysics)
                ProcessPhysics(delta);

            Input.UpdateInputs(delta);
            MainCamera.Update(delta);

            _fpsCounter.Tick(delta);
            Console.WriteLine(_fpsCounter.Fps);
        }

        public void Render(Graphics g)
        {
            _renderManager.Render(g, MainCamera);
        }
        
        public Vector2 GetDistanceBetweenObjects(Entity a, Entity b)
        {
            return a.CenteredPosition - b.CenteredPosition;
        }

        public Vector2 GetNormalBetweenObjects(Entity a, Entity b)
        {
            var normal = GetDistanceBetweenObjects(a, b);
            return normal.LengthSquared() > 1.0f ? Vector2.Normalize(normal) : normal;
        }

        public void MoveAndCollide(Entity entity)
        {
            Vector2 displacement = entity.Velocity;
            if (_currentDelta != 0)
                displacement *= _currentDelta;

            if (displacement.X != 0)
            {
                var previews = entity.Position;
                entity.Position = new Vector2(entity.Position.X + displacement.X, entity.Position.Y);
            }

            if (displacement.Y != 0)
            {
                var previews = entity.Position;
                entity.Position = new Vector2(entity.Position.X, entity.Position.Y + displacement.Y);
            }
        }

        #endregion
    }
}