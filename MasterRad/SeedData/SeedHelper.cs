using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MasterRad.SeedData
{
    public class SeedHelper
    {
        public static IEnumerable<T> SeedData<T>(string jsonProperty)
        {
            var fileName = $"{jsonProperty}.json";
            var textData = File.ReadAllText($"{Directory.GetCurrentDirectory()}\\SeedData\\{fileName}");
            var jsonData = (JObject)JsonConvert.DeserializeObject(textData);
            var jArray = (JArray)jsonData.GetValue(jsonProperty);
            return jArray.Select(x => x.ToObject<T>()).ToArray();
        }
    }
}
