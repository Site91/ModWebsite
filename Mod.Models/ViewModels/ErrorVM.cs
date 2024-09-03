using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Models.ViewModels
{
    public class ErrorVM
    {
        public int code { get; set; }
        public ErrorVisual visual { get; set; }
    }
    public class ErrorVisual
    {
        public IEnumerable<string> response { get; set; }
        public int respType;
        public string suggest;
        public ErrorVisual(string response, string suggestion, int respType = 0)
        {
            this.response = new string[] { response };
            this.respType = respType;
            this.suggest = suggestion;
        }
        public ErrorVisual(string[] response, string suggestion, int respType = 0)
        {
            this.response = response;
            this.respType = respType;
            this.suggest = suggestion;
        }
    }
}
