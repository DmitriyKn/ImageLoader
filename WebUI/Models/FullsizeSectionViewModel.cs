using Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class FullsizeSectionViewModel
    {
        public int ImageId { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public byte[] ImageData { get; set; }

        public Hashtable Exif { get; set; }
        public string GpsLong { get; set; } //There is a reason why the string here, not double
        public string GpsLat { get; set; }
    }
}