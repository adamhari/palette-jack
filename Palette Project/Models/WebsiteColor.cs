using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using ExCSS;

namespace Palette_Project.Models
{
    public class WebsiteColor
    {
        public Color color { get; set; }
        public Property property { get; set; }
        public List<string> selectors = new List<string>();
        public List<string> rules = new List<string>();

        public WebsiteColor(Color colorPass, Property propertyPass)
        {
            color = colorPass;
            property = propertyPass;
        }

        public void addSelector(string selector)
        {
            selectors.Add(selector);
        }

        public void addRule(string rule)
        {
            rules.Add(rule);
        }

        public List<string> getSelectors()
        {
            return selectors.ToList();
        }

        public string getSelectorsString()
        {
            var str = "";
            foreach (var selector in selectors)
            {
                str += selector + Environment.NewLine;
            }

            return str;
        }

        public string getHex()
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public string getR()
        {
            return color.R.ToString();
        }

        public string getG()
        {
            return color.G.ToString();
        }

        public string getB()
        {
            return color.B.ToString();
        }

        public float getHue()
        {
            return color.GetHue();
        }

        public float getSaturation()
        {
            return color.GetSaturation();
        }

        public float getBrightness()
        {
            return color.GetBrightness();
        }
    }
}