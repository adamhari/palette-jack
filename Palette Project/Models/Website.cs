using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using HtmlAgilityPack;
using ExCSS;
using System.Drawing;
using static Palette_Project.Extensions.StringExtensions;
using static Palette_Project.Extensions.LINQExtensions;


namespace Palette_Project.Models
{
    public class Website
    {
        public int ID { get; set; }
        public string url { get; set; }
        public string content { get; set; }
        public List<HtmlNode> stylesheets { get; set; }
        public List<HtmlNode> excludedStylesheets { get; set; }
        public string rules { get; set; }
        public List<Color> colors { get; set; }
        public List<Property> colorRules { get; set; }
        public List<WebsiteColor> websiteColors { get; set; }


        public void FixUrl()
        {
            var wc = new System.Net.WebClient();

            //fix URL inputs
            if (!this.url.Contains("http://"))
            {
                this.url = "http://" + this.url;
            }

            this.content = wc.DownloadString(this.url);
        }


        public void GetStyleRules()
        {
            var wc = new System.Net.WebClient();

            var html = new HtmlDocument();
            html.LoadHtml(this.content);

            var root = html.DocumentNode;
            IEnumerable<HtmlNode> styleSheetElements = root.Descendants("link").Where(x => x.GetAttributeValue("rel", "").Equals("stylesheet"));

            this.stylesheets = new List<HtmlNode>();
            this.excludedStylesheets = new List<HtmlNode>();

            foreach (var styleSheetElement in styleSheetElements)
            {
                try
                {
                    string link = "";

                    //fix relative links
                    if (styleSheetElement.Attributes["href"].Value.Substring(0, 2) == "//")
                    {
                        link = "http://" + styleSheetElement.Attributes["href"].Value.Substring(2, styleSheetElement.Attributes["href"].Value.Length - 2);
                    }
                    else if (styleSheetElement.Attributes["href"].Value.Substring(0, 1) == "/")
                    {
                        link = "http://" + this.url + styleSheetElement.Attributes["href"].Value;
                    }
                    else
                    {
                        link = styleSheetElement.Attributes["href"].Value;
                    }

                    styleSheetElement.Attributes["href"].Value = link;

                    string styleSheet = wc.DownloadString(link);

                    //don't include stylesheets for CSS frameworks like bootstrap and font-awesome
                    if (!link.Contains("bootstrap") || !link.Contains("font-awesome"))
                    {
                        this.rules += styleSheet;
                        this.stylesheets.Add(styleSheetElement);
                    }
                    else
                    {
                        this.excludedStylesheets.Add(styleSheetElement);
                    }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }


        public void FilterColorRules()
        {
            var parser = new Parser();
            var stylesheet = parser.Parse(this.rules);

            //remove selectors that use a colon (hover, focus, before, after)
            IEnumerable<StyleRule> styleRules = stylesheet.StyleRules.Except(stylesheet.StyleRules.Where(x => x.Selector.ToString().Contains(":")));


            //remove selectors that end in "a" (link colors are usually not desirable)
            styleRules = styleRules.Except(styleRules.Where(x => x.Selector.ToString().Substring(x.Selector.ToString().Length - 1, 1).Equals("a")));

            //remove selectors for errors (the red is not part of most front-facing color palettes)
            styleRules = styleRules.Except(styleRules.Where(x => x.Selector.ToString().IndexOf("error", StringComparison.OrdinalIgnoreCase) >= 0));


            //filter style rules that mention color
            styleRules = styleRules.Where(x => x.Declarations.ToString().IndexOf("color", StringComparison.OrdinalIgnoreCase) >= 0);

            //get all remaining style rules that mention color, and convert them to a list of Property objects
            IEnumerable<Property> colorRules = styleRules.SelectMany(r => r.Declarations).Where(d => d.Name.Contains("color"));

            //the final list of colors for the palette
            List<WebsiteColor> websiteColors = new List<WebsiteColor>();


            //for each rule that mentions color
            foreach (var colorRule in styleRules.SelectMany(x=>x.Declarations).Where(y=>y.Name.Contains("color")))
            {
                var colorString = colorRule.Term.ToString();

                //skip color rules that mention more than one color
                if (colorString.Contains(" "))
                {
                    continue;
                }
                else
                {
                    Color thisColor = new Color();

                    //if it is a hex value
                    if (colorString.Substring(0, 1).Equals("#"))
                    {
                    }
                    else
                    {
                        if (colorString.Contains("rgba", StringComparison.OrdinalIgnoreCase))
                        {

                        }
                        else if (colorString.Contains("rgb", StringComparison.OrdinalIgnoreCase))
                        {
                            colorString = colorString.rgbToHex();
                        }
                        else
                        {
                            colorString = colorString.nameToHex();

                            if (colorString.Equals("unknown"))
                            {
                                continue;
                            }
                        }
                    }

                    thisColor = ColorTranslator.FromHtml(colorString);
                    WebsiteColor thisWebsiteColor = new WebsiteColor(thisColor, colorRule);
                    
                    thisWebsiteColor.addRule(thisWebsiteColor.property.Name);
                    websiteColors.Add(thisWebsiteColor);


                    ////tying selectors to the color
                    //foreach (var styleRule in styleRules)
                    //{
                    //    foreach (var property in styleRule.Declarations.Properties)
                    //    {
                    //        //if this term contains the color we're interested in, we need to add this term's selector to the 'WebsiteColor' object
                    //        if (property.Term.ToString().IndexOf(thisWebsiteColor.getHex()) >= 0)
                    //        {
                    //            thisWebsiteColor.addSelector(styleRule.Selector.ToString());
                    //            System.Diagnostics.Debug.WriteLine(styleRule.Selector.ToString());
                    //        }
                    //    }
                    //}
                    
                    foreach(var styleRule in styleRules)
                    {
                        var terms = styleRule.Declarations.Properties.Select(x => x.Term);

                        //if this term contains the color we're interested in, we need to add this term's selector to the 'WebsiteColor' object
                        if (terms.Select(x=>x.ToString()).Contains(thisWebsiteColor.getHex()))
                        {
                            thisWebsiteColor.addSelector(styleRule.Selector.ToString());
                            System.Diagnostics.Debug.WriteLine(styleRule.Selector.ToString());
                        }
                    }



                    thisWebsiteColor.selectors = thisWebsiteColor.selectors.Distinct().ToList();


                }
            }

            //remove inherit and transparent colors from the list
            //order by hue and brightness
            websiteColors = websiteColors.Except(websiteColors.Where(x => x.color.Name.Equals("transparent") || x.color.Name.Equals("inherit") || x.color.Name.Equals("initial"))).OrderBy(x => x.color.GetBrightness()).OrderBy(x => x.color.GetHue()).ToList();
            
            websiteColors = websiteColors.DistinctBy(x => x.getHex()).ToList();

            this.websiteColors = websiteColors;

            if (this.websiteColors.Count != 0)
            {

            }

        }



    }

    public class WebsiteDBContext : DbContext
    {
        public DbSet<Website> Websites { get; set; }
    }
}