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

        public byte[] TryLoadRam()
        {
            if(!File.Exists(_filePath))
                return null;

            return File.ReadAllBytes(_filePath);
        }

        public void SaveRam(byte[] ramData)
        {
            File.WriteAllBytes(_filePath, ramData);
        }
    }
}
