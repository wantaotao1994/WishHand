using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WishHand.Service;
using WishHand.Model;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace WishHand.Controllers
{
    public class HomeController : Controller
    {
        private readonly Translator _translator;
        private readonly ExcelHelper _excelHelper;

        public HomeController(Translator translator,ExcelHelper excelHelper) {
            _translator = translator;

            _excelHelper = excelHelper;
        }
        public IActionResult Index(string  id)
        {

            return View();
        }


        public async Task<IActionResult> Translate(TranslateRequest translateModel)
        {
            //if (string.IsNullOrEmpty(translateModel.source))
            //{

            //}
            //if (string.IsNullOrEmpty(translateModel.text))
            //{

            //}
            //if (string.IsNullOrEmpty(translateModel.code))
            //{

            //}

            //string[] arr = translateModel.text.Split(',');


            //string result =await Translator.Translateasync(translateModel.source,translateModel.code, arr.ToList<string>());


            return Json("");
        }

        [HttpPost]
        public async Task<FileStreamResult> TranslateExcelAsync(IFormCollection collection)
        {

            var files = Request.Form.Files;



            IList<IList<string>> resultList = new List<IList<string>>();

            var filePath = @"data/excel/";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            IList<string> list = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                   string fileFullPath = filePath + DateTime.Now.Millisecond + formFile.FileName ;

                    using (var stream = new FileStream(fileFullPath, FileMode.Create))
                    {
                        list.Add(fileFullPath);
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            foreach (var item in list)
            {
                var excelData = _excelHelper.ReadExcel(item);

                //此处只读第一列 

                IList<string> textList = new List<string>();
                foreach (var text in excelData)
                {
               
                        textList.Add(text[0]); //只读第一列 

                    
                }
                resultList.Add(textList);


                List<Task> taskList = new List<Task>();

                var task2 =  _translator.Translateasync("en", "fr", textList);  //翻译成法语
                var task1=_translator.Translateasync("en", "de", textList);  //翻译成德文

                await Task.WhenAll(task1,task2);

                resultList.Add(task2.Result);
                resultList.Add(task1.Result);
                string savePath = $"data/{DateTime.Now.ToFileTime()}.xlsx";
                _excelHelper.CreateExcel(resultList, savePath);


                FileStream fileStream = new FileStream(savePath, FileMode.Open);
                return File(fileStream, "text/comma-separated-values", "result" + DateTime.Now.ToFileTime() + ".xlsx");
            }
            return null;
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            //    return Ok(new { result });
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
