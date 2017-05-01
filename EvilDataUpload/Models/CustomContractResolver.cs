using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EvilDataUpload.Models
{
    public class CustomContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

            foreach(var item in properties.Where(x => x.PropertyName == "added" || x.PropertyName == "hash" || x.PropertyName == "errors"))
            {
                item.Ignored = true;
            }
            
            return properties;
        }
    }
}