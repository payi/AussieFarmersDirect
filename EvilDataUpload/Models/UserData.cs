using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace EvilDataUpload.Models
{
    public class UserData
    {
        [JsonProperty("customer")]
        public string CustomerName { get; set; }

        [JsonProperty("value")]
        public int CustomerValue { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("added")]
        public string UploadStatus { get; set; }

        [JsonProperty("errors")]
        public List<string> Errors { get; set; }

        [JsonProperty("property")]
        public string UploaderName { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        public UserData()
        {
            Errors = new List<string>();
            UploadStatus = "false";
            Hash = string.Empty;
        }
    }
}