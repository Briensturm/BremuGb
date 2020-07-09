using System;
using System.Diagnostics;

using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Graphics.OpenGL;

using BremuGb.Input;
using BremuGb.Audio.SoundChannels;
using BremuGb.Frontend.OpenGL;

namespace BremuGb.Frontend
{
    public class BremuGbWindow : GameWindow
    {
        private SoundPlayer _soundPlayer;
        private ScreenRenderer _screenRenderer;

        private Stopwatch _stopwatch;

        private readonly GameBoy _gameBoy;

        private bool _fastForward;

        JoypadState _joypadState;

        public BremuGbWindow(NativeWindowSettings nativeWindowSettings, GameWindowSettings gameWindowSettings, GameBoy gameBoy)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _gameBoy = gameBoy;
            _gameBoy.OutputTerminalChangedEvent += new EventHandler(OnSoundOutputTerminalChanged);
            _gameBoy.MasterVolumeChangedEvent += new EventHandler(OnMasterVolumeChangedEvent);
            _gameBoy.NextFrameReadyEvent += new EventHandler(OnNextFrameReady);

            _soundPlayer = new SoundPlayer();
            _screenRenderer = new ScreenRenderer();

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        private void OnSoundOutputTerminalChanged(object sender, EventArgs e)
        {
            _soundPlayer.SetChannelPosition(Channels.Channel1, _gameBoy.GetOutputTerminal(Channels.Channel1));
            _soundPlayer.SetChannelPosition(Channels.Channel2, _gameBoy.GetOutputTerminal(Channels.Channel2));
            _soundPlayer.SetChannelPosition(Channels.Channel3, _gameBoy.GetOutputTerminal(Channels.Channel3));
            _soundPlayer.SetChannelPosition(Channels.Channel4, _gameBoy.GetOutputTerminal(Channels.Channel4));
        }

        private void OnMasterVolumeChangedEvent(object sender, EventArgs e)
        {
            _soundPlayer.SetVolume(_gameBoy.GetMasterVolumeLeft(), _gameBoy.GetMasterVolumeRight());
        }

        private void OnNextFrameReady(object sender, EventArgs e)
        {
            _screenRenderer.UpdateTexture(_gameBoy.GetScreen());
        }

        protected override void OnLoad()
        {
            _screenRenderer.Initialize();

            base.OnLoad();
        }

        public override void Close()
        {
            _screenRenderer.Close();
            _soundPlayer.Close();

            _gameBoy.SaveLog("log.txt");
            _gameBoy.SaveRam();

            base.Close();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (_screenRenderer.Render())
                SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var timespan = _stopwatch.Elapsed;
            if (timespan.TotalMilliseconds >= 16 || _fastForward == true)
            {
                _stopwatch.Restart();

                _joypadState = GetJoypadState();

                for (int i = 0; i < 17000; i++)
                {
                    _soundPlayer.QueueAudioSample(Channels.Channel1, _gameBoy.GetAudioSample(Channels.Channel1));
                    _soundPlayer.QueueAudioSample(Channels.Channel2, _gameBoy.GetAudioSample(Channels.Channel2));
                    _soundPlayer.QueueAudioSample(Channels.Channel3, _gameBoy.GetAudioSample(Channels.Channel3));
                    _soundPlayer.QueueAudioSample(Channels.Channel4, _gameBoy.GetAudioSample(Channels.Channel4));

                    _gameBoy.AdvanceMachineCycle(_joypadState);
                }                
            }

            if (KeyboardState.IsKeyDown(Key.Escape))
                Close();

            if (KeyboardState.IsKeyDown(Key.Tab))
                _fastForward = true;
            else
                _fastForward = false;

            if (KeyboardState.IsKeyDown(Key.P) && LastKeyboardState.IsKeyUp(Key.P))
            {
                if (Size.X < 640)
                    Size = new OpenToolkit.Mathematics.Vector2i(ClientSize.X + 160, ClientSize.Y + 144);
            }

            if (KeyboardState.IsKeyDown(Key.M) && LastKeyboardState.IsKeyUp(Key.M))
            {
                if (Size.X > 160)
                    Size = new OpenToolkit.Mathematics.Vector2i(ClientSize.X - 160, ClientSize.Y - 144);
            }

            if (KeyboardState.IsKeyDown(Key.L) && LastKeyboardState.IsKeyUp(Key.L))
                _gameBoy.EnableLogging();

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
            if (KeyboardState.IsKeyDown(Key.ShiftLeft))
                joypadState |= JoypadState.Select;
            if (KeyboardState.IsKeyDown(Key.S))
                joypadState |= JoypadState.A;   
            if (KeyboardState.IsKeyDown(Key.A))
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
