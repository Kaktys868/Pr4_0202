using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ftp_Dan_True
{
<<<<<<< HEAD
    public class ViewModelMessage
=======
    internal class ViewModelMessage
>>>>>>> 1f9304ef871e456c5e4445866da1ec525c794193
    {
        public string Command { get; set; }
        public string Data { get; set; }
        public ViewModelMessage(string command, string data)
        {
            Command = command;
            Data = data;
        }
    }
}
