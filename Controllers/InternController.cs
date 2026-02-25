using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InternApp.BAL;
using InternApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace InternApp.Controllers
{
    public class InternController : Controller
    {
        private readonly ILogger<InternController> _logger;

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly InternHelper _internHelper;

        public InternController(ILogger<InternController> logger, IWebHostEnvironment webHostEnvironment, InternHelper internHelper)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _internHelper = internHelper;
        }

        public IActionResult Index()
        {
            var topics = _internHelper.GetTopics();
            ViewBag.Topics = topics;

            var interns = _internHelper.GetAllInterns();
            ViewBag.internData = interns;

            return View();
        }

        public IActionResult ViewAll()
        {
            var interns = _internHelper.GetAllInterns();
            ViewBag.internData = interns;
            return View();
        }

        [HttpGet]
        public IActionResult GetAllInterns()
        {
            var interns = _internHelper.GetAllInterns();
            return Json(interns);
        }


        [HttpPost]
        public IActionResult AddIntern(t_Intern intern, IFormFile imagePath)
        {
            try
            {
                if (imagePath != null && imagePath.Length > 0)
                {
                    string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "intern_images");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileExtension = Path.GetExtension(imagePath.FileName);
                    string uniFileName = Guid.NewGuid().ToString() + fileExtension;
                    string fullPath = Path.Combine(folderPath, uniFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        imagePath.CopyTo(stream);
                    }
                    intern.ImagePath = uniFileName;

                    string res = _internHelper.AddIntern(intern);
                    TempData["msg"] = res;
                    return RedirectToAction("GetAllInterns");
                }
                else
                {
                    ModelState.AddModelError("", "Please upload an image.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.Topics = _internHelper.GetTopics();
            return View(intern);
        }



        [HttpPost]
        public IActionResult DeleteIntern(int id)
        {
            _internHelper.DeleteIntern(id);
            return Json(new { success = true, message = "Intern deleted successfully" });
        }


        [HttpGet]
        public IActionResult GetTopics()
        {
            var topics = _internHelper.GetTopics();
            return Json(topics);
        }


        [HttpGet]
        public IActionResult GetInternById(int id)
        {
            var data = new t_Intern();
            try
            {
                data = _internHelper.GetInternById(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                return Json(data);
            }
        }


        [HttpPost]
        public IActionResult UpdateIntern(t_Intern intern, IFormFile ImageFile, string OldImagePath)
        {
            if (ImageFile != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/intern_images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                if (!string.IsNullOrEmpty(OldImagePath))
                {
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/intern_images", OldImagePath);

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                intern.ImagePath = fileName;
            }
            else
            {
                intern.ImagePath = OldImagePath;
            }

            bool result = _internHelper.UpdateIntern(intern);

            return Json(new { success = result, message = "Intern Updated Successfully" });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}