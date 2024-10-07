using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mod.Util
{
    public class JSONSaver
    {
        static string dir = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.FullName + "/ServerInfo/";
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
