using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstruct
{
    public interface IImageRepository
    {
        IEnumerable<Image> Images { get; }
        void SaveImage(Image newImage);
        void DeleteAllImages();
        void SaveComment(int? imageId, String newComment);
    }
}
