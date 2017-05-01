using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json;
using EvilDataUpload.Models;
using System.Net;
using System.Threading.Tasks;

namespace EvilDataUpload.Controllers
{
    public class DataUploadController : AsyncController
    {
        public ActionResult UploadFile()
        {
            return View("UploadData");
        }

        [HttpPost]
        public async Task<ActionResult> UploadSubmittedFile(HttpPostedFileBase fileInput)
        {
            List<UserData> uploadedRowsList = new List<UserData>();
            
            if (fileInput != null && Path.GetExtension(fileInput.FileName).ToLower() == ".csv")
            {
                string fileContent = string.Empty;
                string tempResult = String.Empty;
                string line = string.Empty;
                int counterSent = 0;
                int counterTotal = 0;
                int counterUploadFailed = 0;
                string errors = string.Empty;

                using (StreamReader streamReader = new StreamReader(fileInput.InputStream))
                {
                    fileContent = streamReader.ReadToEnd();
                }

                string[] lines = fileContent.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                
                if (lines.Length > 0)
                {
                    Guid uid = Guid.NewGuid();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        UserData rowUploadResult = new UserData();
                        string[] lineValues = lines[i].Split(new char[] { ',' });
                        counterTotal++;
                        
                        if (lineValues.Length == 2 && (lineValues[0] != string.Empty || lineValues[1] != string.Empty))
                        {
                            uploadedRowsList.Add(rowUploadResult);

                            if(lineValues[0] == string.Empty)
                                rowUploadResult.Errors.Add("Error: the customer name is empty.");

                            int intValue = 0;
                            if (!Int32.TryParse(lineValues[1], out intValue))
                            {
                                rowUploadResult.Errors.Add("Error: the value is not numeric: " + lineValues[1]);

                                if (lineValues[0] == string.Empty)
                                    continue;
                            }

                            rowUploadResult.UploaderName = "Irina";
                            rowUploadResult.CustomerName = lineValues[0];
                            rowUploadResult.Action = "order created";
                            rowUploadResult.CustomerValue = intValue;
                            rowUploadResult.File = fileInput.FileName;
                             
                            DataConnect connect = new DataConnect();
                            rowUploadResult = await connect.SendUploadedDataAsync(rowUploadResult, counterSent);

                            if (rowUploadResult.Hash.Length != 32)
                            {
                                rowUploadResult.Hash = string.Empty;
                                rowUploadResult.Errors.Add("Error: invalid hash in server response.");
                            }

                            int dbUpdate = await connect.AddToDatabase(uid, rowUploadResult);
                            
                            if(rowUploadResult.UploadStatus == "false")
                                counterUploadFailed++;
                            
                            counterSent++;
                        }
                    }
                }
            }
            else
            {
                uploadedRowsList.Add(new UserData() { Errors = new List<string>() { "Please upload a .csv file" } });
            }

            return View("UploadData", uploadedRowsList);
        }
    }
}