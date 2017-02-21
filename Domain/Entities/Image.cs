using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Image
    {
        public int ImageId { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public byte[] ImageData { get; set; }
    }
}
