namespace BremuGb.Cartridge
{
    public interface IRamManager
    {
        public void SaveRam(byte[] ramData);
        public byte[] TryLoadRam();
    }
}
