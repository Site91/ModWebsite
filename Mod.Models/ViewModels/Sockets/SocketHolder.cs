using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Models.ViewModels.Sockets
{
    public class SocketHolder
    {
        public string key;
        public string url;
        public WebSocket socket { get; set; }
        public SocketHolder(WebSocket socket, string key, string url)
        {
            this.socket = socket;
            this.key = key;
            this.url = url;
        }
    }
}
