using Microsoft.AspNetCore.Mvc;
using StartUpApi.Models;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StartUpApi.Controllers
{
    public class BaseController : Controller
    {
        protected virtual ResponseBase ExecuteRequest(Action executeMethod)
        {
            ResponseBase responseBase = new ResponseBase();
            try
            {
                executeMethod();
            }
            catch (Exception ex)
            {               
                responseBase.Message = ex.Message;
                responseBase.Status = false;
                responseBase.StatusCode = ResponseCode.Bad_Request;
            }
            return responseBase;
        }

        protected virtual ResponseBase<T> ExecuteRequest<T>(Func<T> executeMethod)
        {
            ResponseBase<T> responseBase = new ResponseBase<T>();
            try
            {
                responseBase.Payload = executeMethod();
            }
            catch (Exception ex)
            {
                responseBase.Message = ex.Message;
                responseBase.Status = false;
                responseBase.StatusCode = ResponseCode.Bad_Request;
            }
            return responseBase;
        }

        protected virtual async Task<ResponseBase<T>> ExecuteRequestAsync<T>(Func<Task<T>> executeMethod)
        {
            ResponseBase<T> responseBase = new ResponseBase<T>();

            try
            {
                responseBase.Payload = await executeMethod();
            }
            catch (Exception ex)
            {
                responseBase.Message = ex.Message;
                responseBase.Status = false;
                responseBase.StatusCode = ResponseCode.Bad_Request;
            }
            return responseBase;

        }


    }
}
