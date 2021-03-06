﻿using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.Common.Input;

namespace BremuGb.Frontend
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
                RunWithGui(args[0]);
            else
                RunWithGui();
        }

        static void RunWithGui(string romPath = "rom.gb")
        {
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings
            {
                Icon = new WindowIcon(new Image(16, 16, Resources.IconResource.WindowIcon)),
                Title = "BremuGb",
                Size = new Vector2i(160 * 2, 144 * 2),
                WindowBorder = OpenToolkit.Windowing.Common.WindowBorder.Fixed
            };

            GameWindowSettings gameWindowSettings = new GameWindowSettings
            {
                RenderFrequency = 60,
                UpdateFrequency = 0
            };
           
            using var window = new BremuGbWindow(nativeWindowSettings, gameWindowSettings, new GameBoy(romPath));
            window.Run();
        }
    }
}
