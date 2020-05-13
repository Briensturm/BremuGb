﻿using System.IO;

namespace BremuGb.Cartridge 
{
    public class FileRomLoader : IRomLoader
    {
        private string _filePath;
        public FileRomLoader(string filePath)
        {
            _filePath = filePath;
        }

        public byte[] LoadRom()
        {
            return File.ReadAllBytes(_filePath);
        }
    }
}
