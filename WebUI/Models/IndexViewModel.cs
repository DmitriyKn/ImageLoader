using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class IndexViewModel
    {
        public int ImageId { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public byte[] ImageData { get; set; }
    }
}