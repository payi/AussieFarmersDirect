using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Threading;

namespace EvilDataUpload.Models
{
    public class DataConnect
    {
        private readonly string apiUrl = @"http://evilapi-env.ap-southeast-2.elasticbeanstalk.com/";
        
        public async Task<UserData> SendUploadedDataAsync(UserData dataToUpload, int requestId)
        {
            string serverResponse = string.Empty;
            string jsonDataToUpload = ComposeServerRequest(dataToUpload);

            try
            {
                using(WebClient webClient = new WebClient())
                {
                    webClient.Headers["content-type"] = "application/json";
                    
                    serverResponse = await webClient.UploadStringTaskAsync(new Uri(apiUrl + "upload", UriKind.Absolute), "POST", jsonDataToUpload);

                    dataToUpload = ParseServerResponse(serverResponse, dataToUpload);
                }
            }
            catch (Exception e)
            {
                dataToUpload.Errors.Add("Error uploading: " + e.Message);
            }
            return dataToUpload;
        }

        public async Task<UserData> DownloadDataAsync(string userInput)
        {
            string serverResponse = string.Empty;
            UserData downloadResult = new UserData() { UploadStatus = "false" };
           
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.QueryString = new NameValueCollection { {"hash", userInput} };

                    serverResponse = await webClient.DownloadStringTaskAsync(new Uri(apiUrl + "check", UriKind.Absolute));

                    downloadResult = ParseServerResponse(serverResponse, downloadResult);
                }
            }
            catch (Exception e)
            {
                downloadResult.Errors.Add(e.Message);
            }
            return downloadResult;
        }

        private UserData ParseServerResponse(string serverResponse, UserData rowUploadResult)
        {
            JsonConvert.PopulateObject(serverResponse, rowUploadResult);
            return rowUploadResult;
        }

        private string ComposeServerRequest(UserData data)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.ContractResolver = new CustomContractResolver();
            string jsonData = JsonConvert.SerializeObject(data, jsonSettings);

            return jsonData;
        }

        public async Task<int> AddToDatabase(Guid uid, UserData data)
        {
            int dbResult = 0;
            try
            {
                UploadedDataDbEntities db = new UploadedDataDbEntities();
                UploadedDataStatus rowUploadResult = new UploadedDataStatus();
                rowUploadResult.UploadStatus = data.UploadStatus;
                rowUploadResult.UploadDate = DateTime.Now;
                rowUploadResult.CustomerName = data.CustomerName;
                rowUploadResult.CustomerValue = data.CustomerValue.ToString();
                rowUploadResult.File = data.File;
                rowUploadResult.UploadGuid = uid.ToString();
                rowUploadResult.Hash = data.Hash;

                db.UploadedDataStatus.Add(rowUploadResult);
                dbResult = await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("DB insert error: " + e.Message);
            }

            return dbResult;
        }
    }
}