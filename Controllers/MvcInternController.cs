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
    public class MvcInternController : Controller
    {
        private readonly InternHelper _internHelper;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public MvcInternController(InternHelper internHelper, IWebHostEnvironment webHostEnvironment)
        {
            _internHelper = internHelper;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult GetAllInterns()
        {
            var intern = _internHelper.GetAllInterns();
            ViewBag.internData = intern;
            return View();
        }

        [HttpGet]
        public IActionResult AddIntern()
        {
            ViewBag.Topics = _internHelper.GetTopics();
            return View();
        }


        [HttpPost]
        public IActionResult AddIntern(t_Intern intern, IFormFile imagePath)
        {
            if (imagePath != null && imagePath.Length > 0)
            {
                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "./../wwwroot/intern_images");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagePath.FileName);
                string fullPath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    imagePath.CopyTo(stream);
                }
                intern.ImagePath = fileName;
            }
            
            _internHelper.AddIntern(intern);
            return RedirectToAction("GetAllInterns");
        }


        [HttpGet]
        public IActionResult DeleteIntern(int id)
        {
            var intern = _internHelper.GetInternById(id);
            if (intern == null)
                return NotFound();
            return View(intern);
        }


        [HttpPost]
        public IActionResult DeleteIntern(t_Intern intern)
        {

            string result = _internHelper.DeleteIntern(intern.c_InternId);
            TempData["msg"] = result;
            return RedirectToAction("GetAllInterns");
        }


        [HttpGet]
        public IActionResult UpdateInternData(int id)
        {
       
            var intern = _internHelper.GetInternById(id);
            if (intern == null) return NotFound();

            ViewBag.Topics = _internHelper.GetTopics();
        
            return View("AddIntern", intern);
        }

        [HttpPost]
        public IActionResult UpdateInternData(t_Intern intern, IFormFile imagePath, string ExistingImagePath)
        {
            if (imagePath != null && imagePath.Length > 0)
            {
                
                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "intern_images");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagePath.FileName);
                using (var stream = new FileStream(Path.Combine(folderPath, fileName), FileMode.Create))
                {
                    imagePath.CopyTo(stream);
                }
                intern.ImagePath = fileName;
            }
            else
            {
               
                intern.ImagePath = ExistingImagePath;
            }

            bool success = _internHelper.UpdateIntern(intern);
            if (success)
            {
                TempData["msg"] = "Intern Updated Successfully";
                return RedirectToAction("GetAllInterns");
            }

            ViewBag.Topics = _internHelper.GetTopics();
            return View("AddIntern", intern);
        }


        public IActionResult Index()
        {
            var topics = _internHelper.GetTopics();
            ViewBag.Topics = topics;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}