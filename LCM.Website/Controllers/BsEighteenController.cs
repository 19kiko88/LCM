using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversalLibrary.Models;
using LCM.Services.Interfaces;
using LCM.Services.Models.DataTablelHederMapping;
using LCM.Website.Dtos.Request;
using LCM.Services.Models;

namespace LCM.Website.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BsEighteenController : ControllerBase
    {
        private readonly string _fileFolder;
        private IWebHostEnvironment _env;
        private IExcelService _excelService;
        private IDataService _dataService;

        public BsEighteenController(
            IWebHostEnvironment env,
            IExcelService excelService,
            IDataService dataService
            ) 
        {
            _env = env;
            //_env.ContentRootPath => D:\Homer\Project\LCM\LCM.Website\Content\Temp
            //_env.WebRootPath => Content\Temp (避免實體路徑全部顯示)
            _fileFolder = Path.Combine($@"{_env.WebRootPath}", @"Content\Temp");
            _excelService = excelService;
            _dataService = dataService;
        }

        /// <summary>
        /// Ref：https://code-maze.com/upload-files-dot-net-core-angular/
        /// (純)上傳excel檔        
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<string>> Upload(/*[FromForm]IFormFile postFile*/[FromForm] BsEighteen.Upload data)
        {
            var result = new Result<string>() { Success = false };
            result.Success = false;            

            try
            {
                if (data != null)
                {
                    var postFile = data.postFile;//Request.Form.Files[0];
                    var uploadType = data.uploadType;

                    //Create a Folder.
                    if (!Directory.Exists(_fileFolder))
                    {
                        Directory.CreateDirectory(_fileFolder);
                    }

                    //Save the uploaded Excel file.
                    string fileName = Path.GetFileName(postFile.FileName);

                    var check = false;
                    if (uploadType == "b18")
                    {
                        check = fileName.ToLower().Contains("receipts".ToLower()) ? true : false;
                    }
                    else if (uploadType == "s18")
                    {
                        check = fileName.ToLower().Contains("ReSell".ToLower()) ? true : false;
                    }
                    else if (uploadType == "vendor")
                    {
                        check = fileName.ToLower().Contains("大小18") ? true : false;
                    }
                    if (!check) 
                    {
                        result.Message = "請確認上傳檔案類型是否正確.";
                        return result;
                    }

                    string newFileName = $"{DateTime.Now:yyyyMMddhhmmss}_{fileName}";
                    string fullFilePath = Path.Combine(_fileFolder, newFileName);
                    using (FileStream stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        postFile.CopyTo(stream);
                    }

                    result.Message = data.uploadType;
                    result.Content = fullFilePath;
                    result.Success = true;                   
                }
                else
                {
                    result.Message = "請選擇上傳檔案.";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 小18報表資料處理(get dt => 轉List => 存入db)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>資料insert db筆數</returns>
        [HttpPost]
        public async Task<Result<UPLOAD_INFO>> InsertS18(BsEighteen.InsertReport data)
        {
            var result = new Result<UPLOAD_INFO>() { Success = false };            
            result.Success = false;

            try
            {
                var dt = await _excelService.ReadExcel<XX_PORT0001_RESELL>(data.filePath, null, 2);
                if (dt != null && dt.Rows.Count > 0)
                {//bulk insert
                    result.Content = await _dataService.InsertS18(dt, User.Identity.Name);
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 大18報表資料處理(get dt => 轉List => 存入db)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>資料insert db筆數</returns>
        [HttpPost]
        public async Task<Result<UPLOAD_INFO>> InsertB18(BsEighteen.InsertReport data)
        {
            var result = new Result<UPLOAD_INFO>() { Success = false };
            result.Success = false;

            try
            {
                var dt = await _excelService.ReadExcel<XX_PO_RECEIPT>(data.filePath, null, 2);
                if (dt != null && dt.Rows.Count > 0)
                {//bulk insert
                    result.Content = await _dataService.InsertB18(dt, User.Identity.Name);
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 匯出PK報表
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpPost]
        [Filters.Exception]
        public async Task<FileStreamResult> ExportPkBs18(BsEighteen.InsertReport data)
        {
            FileStreamResult fs;

            try
            {
                var dt = await _excelService.ReadExcel<LCM.Services.Models.DataTablelHederMapping.VENDOR_REPORT>(data.filePath, 16, 1);
                var xlsxContent = await _dataService.GetPkBs18Content(dt);
                fs = await _excelService.ExportExcel(xlsxContent, data.filePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return fs;
        }
    }
}
