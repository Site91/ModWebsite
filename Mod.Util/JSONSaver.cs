using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Mod.Util
{
    public class JSONSaver
    {
        static string dir;

        IHostingEnvironment _hostEnvironment;
        public JSONSaver(IHostingEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            dir = new DirectoryInfo(_hostEnvironment.WebRootPath).Parent.FullName;
        }

        public void Save(string path, JSObject obj)
        {
            File.WriteAllText( path, JsonConvert.SerializeObject(obj));
        }
        public JSObject Load(string path)
        {
            string text = File.ReadAllText(path);
            if (text != null)
            {
                var obj = JsonConvert.DeserializeObject<JSObject>(text);
                return obj;
            }
            return null;
        }
    }
}
