﻿#pragma warning disable SA1600 // Elements should be documented
namespace mpBlkReplace
{
    using System;
    using System.Collections.Generic;
    using ModPlusAPI.Interfaces;

    public class ModPlusConnector : IModPlusFunctionInterface
    {
        public SupportedProduct SupportedProduct => SupportedProduct.AutoCAD;

        public string Name => "mpBlkReplace";

#if A2013
        public string AvailProductExternalVersion => "2013";
#elif A2014
        public string AvailProductExternalVersion => "2014";
#elif A2015
        public string AvailProductExternalVersion => "2015";
#elif A2016
        public string AvailProductExternalVersion => "2016";
#elif A2017
        public string AvailProductExternalVersion => "2017";
#elif A2018
        public string AvailProductExternalVersion => "2018";
#elif A2019
        public string AvailProductExternalVersion => "2019";
#elif A2020
        public string AvailProductExternalVersion => "2020";
#elif A2021
        public string AvailProductExternalVersion => "2021";
#endif

        public string FullClassName => string.Empty;

        public string AppFullClassName => string.Empty;

        public Guid AddInId => Guid.Empty;

        public string LName => "Замена блоков";

        public string Description => "Плагин позволяет производить быструю замену блоков в чертеже на блок-образчик";

        public string Author => "Пекшев Александр aka Modis";

        public string Price => "0";

        public bool CanAddToRibbon => true;

        public string FullDescription => "Плагин работает в двух режимах: Заменить выбранные - позволяет заменить все выбранные блоки на один блок-образчик; Заменить подобные - позволяет заменить все вхождения одного блока на один блок-образчик. Позволяет удалить описание замененного блока из базы чертежа. По аналогии с командой \"Очистить\" (_PURGE). Присутствуют различные настройки.";

        public string ToolTipHelpImage => string.Empty;

        public List<string> SubFunctionsNames => new List<string>();

        public List<string> SubFunctionsLames => new List<string>();

        public List<string> SubDescriptions => new List<string>();

        public List<string> SubFullDescriptions => new List<string>();

        public List<string> SubHelpImages => new List<string>();

        public List<string> SubClassNames => new List<string>();
    }
}
#pragma warning restore SA1600 // Elements should be documented