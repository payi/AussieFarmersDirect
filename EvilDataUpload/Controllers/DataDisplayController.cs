using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using EvilDataUpload.Models;

namespace EvilDataUpload.Controllers
{
    public class DataDisplayController : Controller
    {
        public ActionResult DisplayData()
        {
            return View();
        }

        public async Task<ActionResult> LookupInsertedData(string userInput)
        {
           
            UserData rowDisplayResult = new UserData();
            if (!string.IsNullOrEmpty(userInput))
            {
                DataConnect connect = new DataConnect();
                rowDisplayResult = await connect.DownloadDataAsync(userInput);
            }

            return View("DisplayData", rowDisplayResult);
        }
    }
}