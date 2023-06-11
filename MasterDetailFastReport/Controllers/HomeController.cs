using FastReport.Data;
using FastReport.Web;
using MasterDetailFastReport.Data;
using MasterDetailFastReport.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using FastReport.Export.PdfSimple;

namespace MasterDetailFastReport.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            this._webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public IActionResult Index()
        {
            var purchase = _context.Purchases.ToList();
            return View(purchase);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Report(int id)
        {
            var FindPurchase = _context.Purchases.Where(x => x.ID == id).FirstOrDefault();
            var invoice = FindPurchase.InvoiceNO;
            var date = FindPurchase.CreateDate;
            var purchaseID = FindPurchase.ID;

            var FindDetail = _context.Details.Where(x => x.TblPurchaseID == purchaseID).FirstOrDefault();
            var productName = FindDetail.ProductName;
            var productPrice = FindDetail.ProductPrice;
            var productQTY = FindDetail.QTY;
            var detailID = FindDetail.ID;

            var expanse = _context.Expanses.Where(x => x.TblPurchaseDetailID == detailID).ToList();

            var webReport = new WebReport();
            var mssqlDataConnection = new MsSqlDataConnection();
            mssqlDataConnection.ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            webReport.Report.Dictionary.Connections.Add(mssqlDataConnection);
           
            var path = $"{this._webHostEnvironment.WebRootPath}\\Reports\\TestReport.frx";
            webReport.Report.Load(path);

            var product = _context.Expanses.ToList();

            webReport.Report.RegisterData(expanse, "Expanse");

            webReport.Report.SetParameterValue("InvoiceNumber", invoice);
            webReport.Report.SetParameterValue("InvoiceDate", date);
            webReport.Report.SetParameterValue("ProductName", productName);
            webReport.Report.SetParameterValue("ProductPrice", productPrice);
            webReport.Report.SetParameterValue("ProductQTY", productQTY);

            //// prepare report
            webReport.Report.Prepare();
            //// save file in stream
            Stream stream = new MemoryStream();
            webReport.Report.Export(new PDFSimpleExport(), stream);
            stream.Position = 0;
            //// return stream in browser
            return File(stream, "application/zip", "report.pdf");

            //return View(webReport);
        }

        static DataTable GetTable<TEntity>(IEnumerable<TEntity> table, string name) where TEntity : class
        {
            var offset = 78;
            DataTable result = new DataTable(name);
            PropertyInfo[] infos = typeof(TEntity).GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (info.PropertyType.IsGenericType
                && info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    result.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType)));
                }
                else
                {
                    result.Columns.Add(new DataColumn(info.Name, info.PropertyType));
                }
            }
            foreach (var el in table)
            {
                DataRow row = result.NewRow();
                foreach (PropertyInfo info in infos)
                {
                    if (info.PropertyType.IsGenericType &&
                        info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        object t = info.GetValue(el);
                        if (t == null)
                        {
                            t = Activator.CreateInstance(Nullable.GetUnderlyingType(info.PropertyType));
                        }

                        row[info.Name] = t;
                    }
                    else
                    {
                        if (info.PropertyType == typeof(byte[]))
                        {
                            //Fix for Image issue.
                            var imageData = (byte[])info.GetValue(el);
                            var bytes = new byte[imageData.Length - offset];
                            Array.Copy(imageData, offset, bytes, 0, bytes.Length);
                            row[info.Name] = bytes;
                        }
                        else
                        {
                            row[info.Name] = info.GetValue(el);
                        }
                    }
                }
                result.Rows.Add(row);
            }

            return result;
        }

    }
}