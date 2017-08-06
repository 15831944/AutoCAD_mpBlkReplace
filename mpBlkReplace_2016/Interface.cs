using System.Collections.Generic;
using ModPlusAPI.Interfaces;

namespace mpBlkReplace
{
    public class Interface : IModPlusFunctionInterface
    {
        public SupportedProduct SupportedProduct => SupportedProduct.AutoCAD;
        public string Name => "mpBlkReplace";
        public string AvailProductExternalVersion => "2016";
        public string ClassName => string.Empty;
        public string LName => "Замена блоков";
        public string Description => "Функция позволяет производить быструю замену блоков в чертеже на блок-образчик";
        public string Author => "Пекшев Александр aka Modis";
        public string Price => "0";
        public bool CanAddToRibbon => true;
        public string FullDescription => "Функция работает в двух режимах: Заменить выбранные - позволяет заменить все выбранные блоки на один блок-образчик; Заменить подобные - позволяет заменить все вхождения одного блока на один блок-образчик. Позволяет удалить описание замененного блока из базы чертежа. По аналогии с командой \"Очистить\" (_PURGE). Присутствуют различные настройки.";
        public string ToolTipHelpImage => string.Empty;
        public List<string> SubFunctionsNames => new List<string>();
        public List<string> SubFunctionsLames => new List<string>();
        public List<string> SubDescriptions => new List<string>();
        public List<string> SubFullDescriptions => new List<string>();
        public List<string> SubHelpImages => new List<string>();
        public List<string> SubClassNames => new List<string>();
    }

    public static class VersionData
    {
        public static string FuncVersion = "2016";
    }
}
