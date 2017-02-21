using Domain.Abstruct;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Concrete
{
    public class EFImageRepository: IImageRepository
    {
        EFDbContext context = new EFDbContext();

        public IEnumerable<Image> Images
        {
            get { return context.Images; }
        }

        public void SaveImage(Image newImage)
        {
            if (newImage != null)
            {
                context.Images.Add(newImage);
                context.SaveChanges();
            }
        }
        public void DeleteAllImages()
        {
            if (context.Images.Count() != 0)
            {
                foreach (var item in context.Images)
                {
                    context.Images.Remove(item);
                }
                context.SaveChanges();
            }
        }

        public void SaveComment(int? imageId, String newComment)
        {
            if (imageId != null && newComment != null)
            {
                context.Images.Find(imageId).Comment = newComment;
                context.SaveChanges();   
            }
        }
    }
}
