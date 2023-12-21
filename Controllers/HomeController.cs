using MetadataExtractor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotoExif.Models;
using System.Diagnostics;

namespace PhotoExif.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            string rootDirectory = "D:\\FOTOGRAF";

            // Tüm fotoğraf dosyalarını belirtilen dizin ve alt dizinlerinden al
            string[] photoFiles = System.IO.Directory.GetFiles(rootDirectory, "*.*", SearchOption.AllDirectories)
                                           .Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".png"))
                                           .ToArray();
            
            foreach (var photoFile in photoFiles)
            {
                Console.WriteLine($"Processing: {photoFile}");
                
                

                try
                {
                    var directories = ImageMetadataReader.ReadMetadata(photoFile);
                    var PhotoModel = new Photo();
                    PhotoModel.FilePath = photoFile;
                    foreach (var directory in directories)
                    {
                        
                        foreach (var tag in directory.Tags)
                        {
                            if(tag.TagName == "File Name")
                            {
                                PhotoModel.FileName = tag.Description;
                            }
                            if (tag.TagName == "File Size")
                            {
                                PhotoModel.FileSize = tag.Description;
                            }
                            if (tag.TagName == "Date/Time Original")
                            {

                                string originalDateStr = tag.Description;
                                DateTime originalDate;

                                // Verilen formattaki tarihi DateTime nesnesine çevir
                                if (DateTime.TryParseExact(originalDateStr, "yyyy:MM:dd HH:mm:ss", null,              System.Globalization.DateTimeStyles.None, out originalDate))
                                {
                                    // Tarihi istediğiniz formata çevir
                                    string formattedDate = originalDate.ToString("yyyy-MM-dd HH:mm:ss");
                                    PhotoModel.DateTimeOriginal = DateTime.Parse(formattedDate);
                                }
                            }
                        }
                    }
                    _context.Photo.Add(PhotoModel);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing {photoFile}: {ex.Message}");
                }

                Console.WriteLine();
            }


            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}