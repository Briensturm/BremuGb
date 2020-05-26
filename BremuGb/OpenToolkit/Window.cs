using System;

using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Graphics.OpenGL;
using BremuGb.Input;

namespace BremuGb.UI
{
    public class Window : GameWindow
    {
        private Shader _shader;
        private Texture _texture;
        private Quad _quad;

        private readonly GameBoy _gameBoy;

        public Window(NativeWindowSettings nativeWindowSettings, GameWindowSettings gameWindowSettings, GameBoy gameBoy)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _gameBoy = gameBoy;
            //_emulator.EnableLogging();
        }

        private void UpdateTexture(byte[] frameBitmap)
        {
            var data = new byte[160 * 144 * 3];

            for (int i = 0; i < frameBitmap.Length; i++)
            {
                byte color = (frameBitmap[i]) switch
                {
                    0 => 0xFF,
                    1 => 0xAA,
                    2 => 0x55,
                    3 => 0x00,
                    _ => throw new InvalidOperationException(),
                };

                data[i * 3] = color;
                data[i * 3 + 1] = color;
                data[i * 3 + 2] = color;
            }

            _texture.UpdateTextureData(data, 160, 144, PixelFormat.Rgb);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            var data = new byte[160 * 144 * 3];

            _shader = new Shader(Resources.ShaderResource.VertexShader, Resources.ShaderResource.FragmentShader);
            _shader.UseShaderProgram();

            _texture = new Texture(data, 160, 144, PixelFormat.Rgb);
            _texture.Use();

            _quad = new Quad();
            _quad.Bind();

            //check for OpenGL errors
            OpenGlUtility.ThrowIfOpenGlError();

            base.OnLoad();
        }

        public override void Close()
        {
            //todo: OpenGL cleanup

            _gameBoy.SaveLog("log.txt");

            base.Close();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //GL.Clear(ClearBufferMask.ColorBufferBit);
            _quad.Render();

            SwapBuffers();

            OpenGlUtility.ThrowIfOpenGlError();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //todo: separate render and update handlers

            var joypadState = GetJoypadState();

            for (int i = 0; i < 17556; i++)
            {
                var nextFrameReady = _gameBoy.AdvanceMachineCycle(joypadState);

                if (nextFrameReady)
                    UpdateTexture(_gameBoy.GetScreen());
            }
            

            if (KeyboardState.IsKeyDown(Key.Escape))        
                Close();            
            
            if (KeyboardState.IsKeyDown(Key.P) && LastKeyboardState.IsKeyUp(Key.P))
            {
                if(Size.X < 640)
                Size = new OpenToolkit.Mathematics.Vector2i(ClientSize.X + 160, ClientSize.Y + 144);                
            }

            if (KeyboardState.IsKeyDown(Key.M) && LastKeyboardState.IsKeyUp(Key.M))
            {
                if(Size.X > 160)
                    Size = new OpenToolkit.Mathematics.Vector2i(ClientSize.X - 160, ClientSize.Y - 144);
            }

            if (KeyboardState.IsKeyDown(Key.L) && LastKeyboardState.IsKeyUp(Key.L))
            {
                _gameBoy.EnableLogging();
            }

            base.OnUpdateFrame(e);
        }         

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnMinimized(MinimizedEventArgs e)
        {
            base.OnMinimized(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        private JoypadState GetJoypadState()
        {
            JoypadState joypadState = 0;

            if (KeyboardState.IsKeyDown(Key.Enter))
                joypadState |= JoypadState.Start;
            if (KeyboardState.IsKeyDown(Key.BackSpace))
                joypadState |= JoypadState.Select;
            if (KeyboardState.IsKeyDown(Key.A))
                joypadState |= JoypadState.A;
            if (KeyboardState.IsKeyDown(Key.B))
                joypadState |= JoypadState.B;
            if (KeyboardState.IsKeyDown(Key.Left))
                joypadState |= JoypadState.Left;
            if (KeyboardState.IsKeyDown(Key.Right))
                joypadState |= JoypadState.Right;
            if (KeyboardState.IsKeyDown(Key.Up))
                joypadState |= JoypadState.Up;
            if (KeyboardState.IsKeyDown(Key.Down))
                joypadState |= JoypadState.Down;

            return joypadState;
        }
    }
}
