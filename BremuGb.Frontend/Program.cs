using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Mathematics;

namespace BremuGb.Frontend
{
    class Program
    {
        static void Main()
        {            
            RunWithGui();
        }

        static void RunWithGui()
        {
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
            {
                Title = "BremuGb",
                Size = new Vector2i(160 * 2, 144 * 2),
                WindowBorder = OpenToolkit.Windowing.Common.WindowBorder.Fixed
            };

            GameWindowSettings gameWindowSettings = new GameWindowSettings
            {
                RenderFrequency = 0,
                UpdateFrequency = 0
            };
           
            using var window = new Window(nativeWindowSettings, gameWindowSettings, new GameBoy("rom.gb"));
            window.Run();
        }
    }
}
