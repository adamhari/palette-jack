using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Palette_Project.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }

        public static string rgbToHex(this string source)
        {
            source = source.Substring(4, source.Length - 5);

            string[] pieces = source.Split(',');

            int red = Convert.ToInt32(pieces[0]);
            int green = Convert.ToInt32(pieces[1]);
            int blue = Convert.ToInt32(pieces[2]);

            return "#" + red.ToString("X2") + green.ToString("X2") + blue.ToString("X2");


        }

        public static string nameToHex(this string source)
        {
            source = source.ToLower();

            switch (source)
            {
                case "red":
                    return "#FF0000";
                case "green":
                    return "#00FF00";
                case "blue":
                    return "#0000FF";
                case "cyan":
                case "aqua":
                    return "#00FFFF";
                case "white":
                    return "#FFFFFF";
                case "black":
                    return "#000000";
                case "darkgray":
                case "darkgrey":
                    return "#A9A9A9";
                case "gray":
                case "grey":
                    return "#808080";
                case "lightgray":
                case "lightgrey":
                    return "#D3D3D3";
                case "yellow":
                    return "#FFFF00";
                default:
                    return "unknown";
            }
        }
    }
}