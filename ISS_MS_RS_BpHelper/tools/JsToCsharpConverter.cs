using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.WinForms;

namespace ISS_MS_RS_BpHelper
{
    public class JsToCsharpConverter
    {
        [JavascriptIgnore]
        public ChromiumWebBrowser browser { get; set; }

        [JavascriptIgnore]
        public BPHelper form { get; set; }

        public string contentHtml { get; set; }

        public string contentScript { get; set; }

        public JsToCsharpConverter(ChromiumWebBrowser browser, BPHelper form)
        {
            this.browser = browser;
            this.form = form;
        }
        //functions in bounded object should start with lower case(js lower case function names automatically) 
        public string getContentHtml()
        {
            return contentHtml;
        }
        //functions in bounded object should start with lower case(js lower case function names automatically) 
        public string getContentScript()
        {
            return contentScript;
        }
        //functions in bounded object should start with lower case(js lower case function names automatically) 
        public string requestService(string msg)
        {
            return form.mapService(msg);
        }
    }
}
