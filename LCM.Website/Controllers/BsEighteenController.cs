using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversalLibrary.Models;
using Microsoft.Extensions.Options;

using LCM.Services.Models;
using LCM.Services.Interfaces;
using LCM.Services.Models.DataTablelHederMapping;
using LCM.Website.Dtos.Request;
using LCM.Website.Dtos.Response;
using LCM.Services.Implements;

namespace LCM.Website.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BsEighteenController : ControllerBase
    {
        private IExcelService _excelService;
        private IDataService _dataService;
        private readonly AppSettings.PathSettings _optPathSettings;

        public BsEighteenController(
            IExcelService excelService,
            IDataService dataService,
            IOptions<AppSettings.PathSettings> optPathSettings
            ) 
        {
            _optPathSettings = optPathSettings.Value;
            _excelService = excelService;
            _dataService = dataService;
        }

        /// <summary>
        /// Ref：https://code-maze.com/upload-files-dot-net-core-angular/
        /// 檔案上傳
        /// </summary>
        /// <param name="postFile"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{uploadType?}")]
        public async Task<Result<UploadInfo>> Upload([FromForm] IFormFile postFile, string? uploadType)
        {
            var result = new Result<UploadInfo>() { };

            try
            {
                var uploadInfo = new UploadInfo();
                var errorMsg = "";

                if (postFile != null)
                {
                    var uploadPath = Path.Combine(_optPathSettings.UploadPath, uploadType);

                    //Create a Folder.
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    //excel上傳
                    string fileName = Path.GetFileName(postFile.FileName);
                    var newFileName = $"{DateTime.Now:yyyyMMddHHmmss}_{fileName}";
                    var fullFilePath = Path.Combine(uploadPath, newFileName);
                    using (FileStream stream = new FileStream(fullFilePath, FileMode.Create))
                    {
                        postFile.CopyTo(stream);
                    }

                    uploadInfo.Message = errorMsg;
                    uploadInfo.UploadFileType = uploadType;
                    uploadInfo.FileName = newFileName;

                    result.Content = uploadInfo;

                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        throw new Exception(errorMsg);
                    }

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
        /// <param name="data">上傳檔案的資訊(路徑.檔案類別...)</param>
        /// <returns>資料insert db筆數</returns>
        [HttpPost]
        public async Task<Result<string>> InsertS18(ReportInfo data)
        {
            var result = new Result<string>() { };            
            result.Success = false;

            try
            {
                var fullFilePath = Path.Combine(_optPathSettings.UploadPath, $"{data.FileType}\\{data.FileName}");
                var dt = await _excelService.ReadExcel<XX_PORT0001_RESELL>(fullFilePath, null, 2);
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
        /// <param name="data">上傳檔案的資訊(路徑.檔案類別...)</param>
        /// <returns>資料insert db筆數</returns>
        [HttpPost]
        public async Task<Result<string>> InsertB18(ReportInfo data)
        {
            var result = new Result<string>() { };
            result.Success = false;

            try
            {
                var fullFilePath = Path.Combine(_optPathSettings.UploadPath, $"{data.FileType}\\{data.FileName}");
                var dt = await _excelService.ReadExcel<XX_PO_RECEIPT>(fullFilePath, null, 2);
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
        /// <param name="data">上傳檔案的資訊(路徑.檔案類別...)</param>
        /// <returns></returns>
        [HttpPost]
        [Filters.Exception]
        public async Task<FileStreamResult> ExportPkBs18(ReportInfo data)
        {
            FileStreamResult fs;

            try
            {
                var fullFilePath = Path.Combine(_optPathSettings.UploadPath, $"{data.FileType}\\{data.FileName}");
                var dt = await _excelService.ReadExcel(fullFilePath, 16, 1);
                var xlsxContent = await _dataService.GetPkBs18Content(dt);
                fs = await _excelService.ExportExcel(xlsxContent, fullFilePath);

                //更新小18資料狀態 & 廠商備註欄位
                _dataService.UserName = User.Identity.Name;
                await _dataService.UpdateS18(xlsxContent[0], xlsxContent[1]);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return fs;
        }

        /// <summary>
        /// 手動變更小18結案狀態
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<int>> ManualClose(UploadInfo data)
        {
            var result = new Result<int>() { };
            result.Success = false;

            try
            {
                var fullFilePath = Path.Combine(_optPathSettings.UploadPath, $"{data.UploadFileType}\\{data.FileName}");
                var sheetCount = Services.Helpers.ExcelHelper.GetSheetCount(fullFilePath);
                var dt = await _excelService.ReadExcel<PkResultWithError>(fullFilePath, 32, 3, sheetCount);

                //手動結案
                var res = await _dataService.ManualClose(dt, User.Identity.Name);

                result.Content = res;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
