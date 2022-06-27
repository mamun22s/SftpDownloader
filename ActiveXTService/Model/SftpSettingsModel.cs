using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveXTService.Model
{
    public class SftpSettingsModel
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public string FileLocalSavePath { get; set; }
        public  string Interval { get; set; }
    }
}
