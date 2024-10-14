using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Mod.Util
{
    public class JSONSaver
    {
        static string dir;

        IWebHostEnvironment _hostEnvironment;
        public JSONSaver(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            dir = new DirectoryInfo(_hostEnvironment.WebRootPath).Parent.FullName;
        }

        public void Save(string path, JsonObject obj)
        {
            File.WriteAllText($"{dir}/{path}", JsonConvert.SerializeObject(obj));
        }
        public JsonObject Load(string path)
        {
            string text = File.ReadAllText($"{dir}/{path}");
            if (text != null)
            {
                var obj = JsonConvert.DeserializeObject<JsonObject>(text);
                return obj;
            }
            return null;
        }
    }
}
