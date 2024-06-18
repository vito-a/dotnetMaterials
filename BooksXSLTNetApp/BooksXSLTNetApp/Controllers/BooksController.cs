/* Code for Controllers/BooksController.cs */
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using System.Xml.Xsl;
using System.IO;

namespace XsltWebApp.Controllers
{
    public class BooksController : Controller
    {
        public IActionResult Index()
        {
            string xmlPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "books.xml");
            string xsltPath = Path.Combine(Directory.GetCurrentDirectory(), "Xsl", "books.xslt");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xsltPath);

            using (StringWriter sw = new StringWriter())
            using (XmlWriter xw = XmlWriter.Create(sw, xslt.OutputSettings))
            {
                xslt.Transform(xmlDoc, xw);
                string htmlResult = sw.ToString();
                return Content(htmlResult, "text/html");
            }
        }
    }
}
