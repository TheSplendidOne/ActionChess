using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ActionChessClient.Extensions
{
    internal static class DragEventArgsExtension
    {
        public static T GetSingle<T>(this DragEventArgs args)
        {
            return (T) args.Data.GetData(args.Data.GetFormats().Single());
        }
    }
}
