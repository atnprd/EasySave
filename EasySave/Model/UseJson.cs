using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Model
{
    static class UseJson
    {
        static public string ReadJson(string path,string obj_json)
        {
            path = path.Replace(@"\\", @"/");
            string json = File.ReadAllText(path);
            dynamic array = JsonConvert.DeserializeObject(json);

            string oui = (string)array[0][obj_json];

            return oui;
        }
    }
}
