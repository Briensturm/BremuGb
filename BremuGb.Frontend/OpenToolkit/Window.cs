using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Graphics.OpenGL;

using BremuGb.Input;

namespace BremuGb.Frontend
{
    public class Window : GameWindow
    {
        private Shader _shader;
        private Texture _texture;
        private Quad _quad;

        private SoundPlayer _soundPlayer;

        private readonly GameBoy _gameBoy;

        public Window(NativeWindowSettings nativeWindowSettings, GameWindowSettings gameWindowSettings, GameBoy gameBoy)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _gameBoy = gameBoy;

            _soundPlayer = new SoundPlayer();

            //_emulator.EnableLogging();
        }

        private void UpdateTexture(byte[] frameBitmap)
        {
            _texture.UpdateTextureData(frameBitmap, 160, 144, PixelFormat.Rgb);
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

            _soundPlayer.Close();

            _gameBoy.SaveLog("log.txt");
            _gameBoy.SaveRam();

            base.Close();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            var joypadState = GetJoypadState();

            int audioCounter = 0;

            for (int i = 0; i < 16384; i++)
            {
                var nextFrameReady = _gameBoy.AdvanceMachineCycle(joypadState);

                if (nextFrameReady)
                    UpdateTexture(_gameBoy.GetScreen());

                audioCounter++;
                if (audioCounter == 23)
                {
                    audioCounter = 0;
                    _soundPlayer.QueueSample(_gameBoy.GetAudioSample());
                }
            }

            //GL.Clear(ClearBufferMask.ColorBufferBit);
            _quad.Render();

            SwapBuffers();

            OpenGlUtility.ThrowIfOpenGlError();
            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //todo: separate render and update handlers

                      

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
            if (KeyboardState.IsKeyDown(Key.Left) && KeyboardState.IsKeyUp(Key.Right))
                joypadState |= JoypadState.Left;
            if (KeyboardState.IsKeyDown(Key.Right) && KeyboardState.IsKeyUp(Key.Left))
                joypadState |= JoypadState.Right;
            if (KeyboardState.IsKeyDown(Key.Up) && KeyboardState.IsKeyUp(Key.Down))
                joypadState |= JoypadState.Up;
            if (KeyboardState.IsKeyDown(Key.Down) && KeyboardState.IsKeyUp(Key.Up))
                joypadState |= JoypadState.Down;

            return joypadState;
        }
    }
}
