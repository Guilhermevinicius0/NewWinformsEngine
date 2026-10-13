using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsEngine.Engine.Systems.Inputs
{
    public class KeyboardMouseInput
    {
        public HashSet<Keys> PressedKeys = new HashSet<Keys>();
        
        private float keyLifeSpan = 0.3f;
        private int maxBufferLenght = 15;
        private List<KeyLog> InputBuffer = new List<KeyLog>();

        public event Action<List<Keys>> BufferChanged;

        public HashSet<MouseButtons> PressedMouseButtons = new HashSet<MouseButtons>();
        public HashSet<MouseButtons> PreviewMouseButtons = new HashSet<MouseButtons>();

        public int MouseDelta = 0;
        public bool IsMouseOnScreen = false;

        public Vector2 CurrentMousePosition = Vector2.Zero;
        public Vector2 PreviewsMousePosition = Vector2.Zero;

        public void UpdateMouse(float x, float y)
        {
            CurrentMousePosition.X = x;
            CurrentMousePosition.Y = y;
        }

        public void UpdateMouseButtonState(MouseButtons button, bool state)
        {
            if (state)
                PressedMouseButtons.Add(button);
            else
                PressedMouseButtons.Remove(button);
        }

        public bool IsMouseButtonPressed(MouseButtons button)
        => PressedMouseButtons.Contains(button);

        public bool IsMouseButtonJustPressed(MouseButtons button)
            => PressedMouseButtons.Contains(button) && !PreviewMouseButtons.Contains(button);

        public Vector2 GetMousePositionDelta()
            => CurrentMousePosition - PreviewsMousePosition;

        public float GetScrollWheelDelta()
            => MouseDelta / 120f;

        public void UpdateKeyState(Keys key, bool state)
        {
            if (state)
            {
                if (!GetKeyState(key))
                {
                    var keylog = new KeyLog(key, keyLifeSpan);
                    InputBuffer.Add(keylog);

                    if (InputBuffer.Count > maxBufferLenght)
                        InputBuffer.RemoveAt(0);
                }

                PressedKeys.Add(key);

                BufferChanged?.Invoke(GetBufferKeys());
            }
            else
                PressedKeys.Remove(key);
        }

        public void ClearBuffer()
        {
            InputBuffer.Clear();
            BufferChanged?.Invoke(GetBufferKeys());
        }

        public List<Keys> GetBufferKeys()
        {
            return InputBuffer.Select(k => k.Key).ToList();
        }

        public void UpdateKeyBuffers(float delta)
        {
            bool changed = false;
            for (int i = 0; i < InputBuffer.Count; i++)
            {
                var keylog = InputBuffer[i];
                keylog.TimeLeft -= delta;

                InputBuffer[i] = keylog;

                if (!GetKeyState(keylog.Key) && keylog.TimeLeft <= 0)
                {
                    InputBuffer.RemoveAt(i);
                    i--;
                    changed = true;
                }
            }

            if (changed)
                BufferChanged?.Invoke(GetBufferKeys());
        }

        public bool GetKeyState(Keys key)
         => PressedKeys.Contains(key);

        public bool IsKeyJustPressed(Keys key)
        {
            bool pressed = false;

            for (int i = 0; i < InputBuffer.Count; i++)
            {
                if (InputBuffer[i].Key == key)
                {
                    pressed = true;
                    break;
                }
            }

            return pressed;
        }

        public bool MatchCommands(List<Keys> command)
        {
            if (command == null || command.Count == 0 || InputBuffer.Count < command.Count)
                return false;

            int cmd = command.Count - 1;

            for (int i = InputBuffer.Count - 1; i >= 0 && cmd >= 0; i--)
                if (InputBuffer[i].Key == command[cmd])
                    cmd--;

            if (cmd == -1)
            {
                ClearBuffer();
                return true;
            }

            return false;
        }

        public int GetAxis(Keys directionA, Keys directionB)
        {
            int direction = 0;

            if (GetKeyState(directionA) && !GetKeyState(directionB))
                direction -= 1;

            if (!GetKeyState(directionA) && GetKeyState(directionB))
                direction += 1;

            return direction;
        }

        public Vector2 GetVector(Keys left, Keys right, Keys up, Keys down)
        {
            Vector2 vector = new Vector2(GetAxis(left, right), GetAxis(up, down));

            if (vector.Length() > 1)
                return Vector2.Normalize(vector);

            return vector;
        }

        public void UpdateInputs(float delta)
        {
            UpdateKeyBuffers(delta);
            PreviewMouseButtons.Clear();
            PreviewMouseButtons.UnionWith(PressedMouseButtons);
        }
    }

    public struct KeyLog
    {
        public Keys Key;
        public float TimeLeft;
        public DateTime Time;

        public KeyLog(Keys key, float time)
        {
            Key = key;
            TimeLeft = time;
            Time = DateTime.Now;
        }
    }
}
