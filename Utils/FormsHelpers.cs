using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformsEngine.Engine.Systems.Scene;

namespace WinformsEngine.Utils
{
    public static class FormsHelpers
    {
        public static void ConfigureForm(this Form form, Scene scene) 
        {
            form.KeyPreview = true;
            MapEvents(form, scene);
            scene.ScreenSize = new Vector2(form.ClientSize.Width, form.ClientSize.Height);
            scene.SetMainCameraViewPort(Vector2.Zero, new Vector2(form.ClientSize.Width, form.ClientSize.Height));
        }

        public static void StartGameLoop(this Form form, Scene scene) 
        {
            scene.Timer.Tick += (float delta) =>
            {
                scene.ProcessWorldTick(delta);
                form.Invalidate();
            };

            scene.Timer.Start();
        }

        public static void MapEvents(Form form, Scene scene) 
        {
            form.ResizeEnd += (object sender, EventArgs e) =>
            {
                scene.ScreenSize = new Vector2(form.ClientSize.Width, form.ClientSize.Height);
            };

            form.Paint += (object sender, PaintEventArgs e) =>
            {
                scene.Render(e.Graphics);
            };

            form.FormClosed += (object sender, FormClosedEventArgs e) =>
            {
                scene.Timer.Stop();
            };

            form.KeyDown += (s, e) => scene.Input.UpdateKeyState(e.KeyCode, true);
            form.KeyUp += (s, e) => scene.Input.UpdateKeyState(e.KeyCode, false);

            form.MouseEnter += (s, e) => scene.Input.IsMouseOnScreen = true;
            form.MouseLeave += (s, e) => scene.Input.IsMouseOnScreen = false;

            form.MouseMove += (s, e) => scene.Input.UpdateMouse(e.X, e.Y);
            form.MouseDown += (s, e) => scene.Input.UpdateMouseButtonState(e.Button, true);
            form.MouseUp += (s, e) => scene.Input.UpdateMouseButtonState(e.Button, false);
            form.MouseWheel += (s, e) => scene.Input.MouseDelta += e.Delta;
        }
    }
}