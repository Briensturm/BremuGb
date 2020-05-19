using System;

using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Graphics.OpenGL;

using BremuGb.GameBoy;

namespace BremuGb.UI
{
    public class Window : GameWindow
    {
        private Shader _shader;
        private Texture _texture;
        private Quad _quad;

        private Emulator _emulator;

        public Window(NativeWindowSettings nativeWindowSettings, GameWindowSettings gameWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _emulator = new Emulator();
        }                    

        protected override void OnLoad()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            var data = new byte[160 * 144 * 3];
            var random = new Random();
            for (int i = 0; i < 160 * 144 * 3; i++)
            {
                data[i] = (byte)random.Next(0, 256);
            }

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

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            var frame = _emulator.EmulateNextFrame();

            var data = new byte[160 * 144 * 3];

            for(int i = 0; i<frame.Length; i++)
            {
                byte color;
                switch(frame[i])
                {
                    case 0:
                        color = 0xFF;
                        break;
                    case 1:
                        color = 0xAA;
                        break;
                    case 2:
                        color = 0x55;
                        break;
                    case 3:
                        color = 0x00;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                data[i*3] = color;
                data[i * 3 + 1] = color;
                data[i * 3 + 2] = color;
            }

            _texture.UpdateTextureData(data, 160, 144, PixelFormat.Rgb);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            _quad.Render();

            SwapBuffers();

            OpenGlUtility.ThrowIfOpenGlError();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {            
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
    }
}
