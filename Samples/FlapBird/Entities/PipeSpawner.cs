using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WinformsEngine.Engine.Systems.Scene;

namespace WinformsEngine.Samples.FlapBird.Entities
{
    public class PipeSpawner
    {
        private Random _random = new Random();
        private Scene _scene;
        public Pipe BottomPipe;
        public Pipe TopPipe;

        public float gapSize = 120;
        public float TopMargin = 32;
        public float BottomMargin = 32;
        public float PipeWidth = 64;
        public float PipeSpeed = 30;
        public float IntialSpeed = 30;
        public float PipeMaxSpeed = 100;

        public event Action PointEvent;

        public PipeSpawner(Scene scene)
        {
            _scene = scene;
        }

        public void Start() 
        {
            PipeSpeed = IntialSpeed;
            SpawnNewPipe();
        }

        public void Stop() 
        {
            DestroyCurrentPipe();
        }

        private void OnPipePastMidScreen() 
        {
            PointEvent?.Invoke();
        }

        private void OnPipeLeftScreen() 
        {
            PipeSpeed *= 1.05f;

            if (PipeSpeed > PipeMaxSpeed)
                PipeSpeed = PipeMaxSpeed;

            DestroyCurrentPipe();    
            SpawnNewPipe();
        }

        private void DestroyCurrentPipe() 
        {
            if (TopPipe == null && BottomPipe == null)
                return;

            TopPipe.LeftScreenEvent -= OnPipeLeftScreen;
            TopPipe.MidScreenEvent -= OnPipePastMidScreen;

            BottomPipe.Destroy();
            TopPipe.Destroy();
        }

        private void SpawnNewPipe() 
        {
            float minHeight = _scene.ScreenTop + TopMargin;
            float maxHeight = _scene.ScreenBottom - gapSize - BottomMargin;

            float actualHeight = (float)_random.Next((int)minHeight, (int)maxHeight);

            TopPipe = new Pipe(
                scene: _scene,
                size: new Vector2(PipeWidth, actualHeight),
                position: new Vector2(_scene.ScreenRight + PipeWidth, 0),
                speed: PipeSpeed,
                maxSpeed: PipeMaxSpeed);

            BottomPipe = new Pipe(
                scene: _scene,
                size: new Vector2(PipeWidth, _scene.ScreenBottom - (actualHeight + gapSize)),
                position: new Vector2(_scene.ScreenRight + PipeWidth, _scene.ScreenTop + actualHeight + gapSize),
                speed: PipeSpeed,
                maxSpeed: PipeMaxSpeed);

            TopPipe.LeftScreenEvent += OnPipeLeftScreen;
            TopPipe.MidScreenEvent += OnPipePastMidScreen;

            _scene.AddEntity(TopPipe);
            _scene.AddEntity(BottomPipe);
        }
    }
}