using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfCustomControlLibrary.Controls
{

    public enum IconType
    {
        None,
        ArrowLeft,
        ArrowRight,
        AngleLeft,
        AngleRight,
        AngleDoubleLeft,
        AngleDoubleRight,
        Briefcase,
        Calendar,
        Check,
        CheckSquare,
        ChevronDown,
        Comment,
        Dollar,
        EyeSlash,
        Filter,
        FilterSlash,
        Minus,
        InfoCircle,
        List,
        LockOpen,
        Pencil,
        Percentage,
        Plus,
        Refresh,
        Search,
        SignOut,
        Times,
        User,
        Save,
        Spinner,
        Eye,
        Table,
        Link,
        Image,
        Code,
        Circle,
        Stop
    }

    public static class IconLibrary
    {
        public readonly static Dictionary<IconType, string> Icons = new()
        {
            [IconType.None] = "",
            [IconType.ArrowLeft] = "\ue91a",
            [IconType.ArrowRight] = "\ue91b",
            [IconType.AngleLeft] = "\ue931",
            [IconType.AngleRight] = "\ue932",
            [IconType.AngleDoubleLeft] = "\ue92d",
            [IconType.AngleDoubleRight] = "\ue92e",
            [IconType.Briefcase] = "\ue97d",
            [IconType.Calendar] = "\ue927",
            [IconType.Check] = "\ue909",
            [IconType.CheckSquare] = "\ue98c",
            [IconType.ChevronDown] = "\ue902",
            [IconType.Comment] = "\ue97f",
            [IconType.Dollar] = "\ue96b",
            [IconType.EyeSlash] = "\ue965",
            [IconType.Filter] = "\ue94c",
            [IconType.FilterSlash] = "\ue9b7",
            [IconType.Minus] = "\ue90f",
            [IconType.InfoCircle] = "\ue924",
            [IconType.LockOpen] = "\ue96a",
            [IconType.List] = "\ue967",
            [IconType.Pencil] = "\ue942",
            [IconType.Percentage] = "\ue9be",
            [IconType.Plus] = "\ue90d",
            [IconType.Refresh] = "\ue938",
            [IconType.Save] = "\ue95b",
            [IconType.Search] = "\ue908",
            [IconType.SignOut] = "\ue971",
            [IconType.Times] = "\ue90b",
            [IconType.User] = "\ue939",
            [IconType.Spinner] = "\ue926",
            [IconType.Eye] = "\ue966",
            [IconType.Table] = "\ue969",
            [IconType.Link] = "\ue9c1",
            [IconType.Image] = "\ue972",
            [IconType.Code] = "\ue9e7",
            [IconType.Circle] = "\ue9dc",
            [IconType.Stop] = "\ue9d1"
        };
    }
}
