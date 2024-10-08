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
        public WebSocket socket { get; set; }
        public SocketHolder(WebSocket socket, string key)
        {
            this.socket = socket;
            this.key = key;
        }
    }
}
