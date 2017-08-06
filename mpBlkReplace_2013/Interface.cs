using mpPInterface;

namespace mpBlkReplace
{
    public class Interface : IPluginInterface
    {
        public string Name => "mpBlkReplace";
        public string AvailCad => "2013";
        public string LName => "Замена блоков";
        public string Description => "Функция позволяет производить быструю замену блоков в чертеже на блок-образчик";
        public string Author => "Пекшев Александр aka Modis";
        public string Price => "0";
    }

    public static class VersionData
    {
        public static string FuncVersion = "2013";
    }
}
