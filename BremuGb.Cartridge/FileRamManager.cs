using System.IO;

namespace BremuGb.Cartridge 
{
    public class FileRamManager : IRamManager
    {
        private readonly string _filePath;

        public FileRamManager(string filePath)
        {
            _filePath = filePath;
        }

        public byte[] LoadRam()
        {
            if(!File.Exists(_filePath))
            {
                var ramData = new byte[0x8000];
                for (int i = 0; i < ramData.Length; i++)
                    ramData[i] = 0;

                File.WriteAllBytes(_filePath, ramData);
            }

            return File.ReadAllBytes(_filePath);
        }

        public void SaveRam(byte[] ramData)
        {
            File.WriteAllBytes(_filePath, ramData);
        }
    }
}
