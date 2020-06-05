namespace BremuGb.Cartridge
{
    public interface IMemoryBankController
    {
        public void LoadRam(IRamManager ramManager);
        public void SaveRam(IRamManager ramManager);
    }
}
