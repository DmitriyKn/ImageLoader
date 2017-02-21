using Domain.Concrete;
using Domain.Abstruct;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Windows.Media.Imaging;
using MetadataExtractor;
using WebUI.Models;
using System.Collections;
using System.Text.RegularExpressions;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        IImageRepository repository;

        public HomeController(IImageRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(repository.Images);
        }

        [HttpPost]
        public void DeleteAllImages()
        {
            repository.DeleteAllImages();
        }

        public ActionResult PreviewSection()
        {
            if (repository.Images.ToList().Count <= 0)
            {
                return HttpNotFound();
            }
            return PartialView(repository.Images);
        }

        public ActionResult GmapsSection()
        {
            return PartialView();
        }

        public string GetImageComment(int? id)
        {
            if (id != null)
            {
                return repository.Images.Where(a => a.ImageId == id).First().Comment;
            }
            else
            {
                return String.Empty;
            }
        }

        public ActionResult FullsizeSection(int? id)
        {

            FullsizeSectionViewModel model = new FullsizeSectionViewModel();

            if (id != null && repository.Images.Count() != 0)
            {
                byte[] imageData = repository.Images.Where(a => a.ImageId == id).First().ImageData;

                try
                {
                    var directories = ImageMetadataReader.ReadMetadata(new MemoryStream(imageData));
                    Hashtable exif = new Hashtable();



                    foreach (var directory in directories)
                    {

                        if (directory.ToString().Contains("Exif"))
                        {
                            foreach (var tag in directory.Tags)
                            {
                                exif.Add("(" + tag.DirectoryName + ")" + tag.Name, tag.Description);
                            }
                        }
                        else
                        {
                            if (directory.ToString().Contains("GPS"))
                            {
                                foreach (var tag in directory.Tags)
                                {
                                    switch (tag.Name)
                                    {
                                        case "GPS Longitude":
                                            {
                                                model.GpsLong = CoordConvert(tag.Description);
                                                break;
                                            }
                                        case "GPS Latitude":
                                            {
                                                model.GpsLat = CoordConvert(tag.Description);
                                                break;
                                            }
                                        default:
                                            {
                                                break;
                                            }
                                    }
                                }

                            }
                        }
                    }
                    model.Image = repository.Images.Where(a => a.ImageId == id).First();
                    model.Exif = exif;
                }
                catch (Exception e)
                {
                    return PartialView("ErrorView", "Incorrect file format");
                }
                finally { }
            }
            return PartialView(model);
        }

        private string CoordConvert(string str)
        {
            string patternDegrees = "-?[\\d,.]+(?=°)";
            string patternMinutes = "-?[\\d,.]+(?=')";
            string patternSeconds = "-?[\\d,.]+(?=\")";

            Regex regex = new Regex(patternDegrees);
            Match match = regex.Match(str);

            double result = 0.000;

            result += Double.Parse(match.Value);

            regex = new Regex(patternMinutes);
            match = regex.Match(str);

            result += Double.Parse(match.Value) / 60;

            regex = new Regex(patternSeconds);
            match = regex.Match(str);

            result += Double.Parse(match.Value) / 3600;

            return result.ToString().Replace(',', '.');
        }


        [HttpPost]
        public JsonResult Upload()
        {
            foreach (string file in Request.Files)
            {
                var uploadImage = Request.Files[file];

                if (uploadImage != null)
                {
                    string fileName = System.IO.Path.GetFileName(uploadImage.FileName);

                    byte[] imageData = null;

                    using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                    }
                    Image newImage = new Image();
                    newImage.Name = fileName;
                    newImage.ImageData = imageData;

                    repository.SaveImage(newImage);
                }
            }
            return Json("файл загружен");
        }

        [HttpGet]
        public int GetLastImageId()
        {
            if (repository.Images.Count() != 0)
            {
                return repository.Images.Max(a => a.ImageId);
            }
            else
            {
                return -1;
            }
        }

        [HttpPost]
        public string GetLastImageByteData(string idStr)
        {
            int id;
            string result;

            try
            {
                id = Int32.Parse(idStr);

                result = Convert.ToBase64String(repository.Images.FirstOrDefault(a => a.ImageId == id).ImageData).ToString();
            }
            catch
            {
                //Error image byte data
                result = "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";
            }
            finally{}
            return result;
        }

        [HttpGet]
        public bool IsImageCollectionEmpty()
        {
            return repository.Images.ToList().Count == 0;
        }

        [HttpPost]
        public void SaveComment()
        {
            int? imageId = Int32.Parse(Request.Params["param1"]);
            string newComment = Request.Params["param2"];

            if (imageId != null && newComment != null)
            {
                repository.SaveComment(imageId, newComment);
            }
        }
    }
}