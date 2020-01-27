using System;
using System.Windows;

namespace ActionChessClient
{
    internal static class ThicknessExtension
    {
        public static Double Difference(this Thickness valueA, Thickness valueB)
        {
            return Math.Sqrt(Math.Pow(valueA.Left - valueB.Left, 2) + Math.Pow(valueA.Top - valueB.Top, 2));
        }
    }
}
