namespace Domain.Migrations
{
    using Domain.Entities;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Domain.Concrete.EFDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Domain.Concrete.EFDbContext context)
        {
            Image testImg1 = new Image();
            Image testImg2 = new Image();

            System.Drawing.Image resource1 = Resources.TestImages.test1;
            System.Drawing.Image resource2 = Resources.TestImages.test2;

            using (MemoryStream mStream = new MemoryStream())
            {
                resource1.Save(mStream, resource1.RawFormat);
                testImg1.ImageData = mStream.ToArray();
            }

            using (MemoryStream mStream = new MemoryStream())
            {
                resource2.Save(mStream, resource2.RawFormat);
                testImg2.ImageData = mStream.ToArray();
            }

            testImg1.Name = "test1";
            testImg2.Name = "test2";

            context.Images.Add(testImg1);
            context.Images.Add(testImg2);
            context.SaveChanges();

        }
    }
}
