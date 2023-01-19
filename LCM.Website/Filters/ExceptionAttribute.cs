//using System.Web.Http.Filters;
using Microsoft.AspNetCore.Mvc.Filters;//如果不是apicontroller要用這個namespace
using Microsoft.AspNetCore.Mvc;
using UniversalLibrary.Models;

namespace LCM.Website.Filters
{
    /// <summary>
    /// 捕捉錯誤exception for 回傳型別不為Result的API(ex: excel下載)
    /// 
    /// Ref：使用 Filter 統一 API 的回傳格式和例外處理
    /// https://ithelp.ithome.com.tw/articles/10198206
    /// Ref：ExceptionFilter OnException not being called
    /// https://stackoverflow.com/questions/45023364/exceptionfilter-onexception-not-being-called
    /// </summary>
    public class ExceptionAttribute: ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext actionExecutedContext)
        {
            var result = new Result
            {
                Success = false,
                Message = actionExecutedContext.Exception.Message
            };
            actionExecutedContext.Result = new JsonResult(result);
        }
    }
}
