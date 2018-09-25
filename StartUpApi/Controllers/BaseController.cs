using Data.Models;
using General.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;


namespace StartUpApi.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController()
        {
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ResponseBase OnError(string errorMessage)
        {
            ResponseBase responseBase = new ResponseBase();
            responseBase.Message = errorMessage;
            responseBase.Status = false;
            
            return responseBase;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ResponseBase<T> OnError<T>(string errorMessage)
        {
            ResponseBase<T> responseBase = new ResponseBase<T>();
            responseBase.Message = errorMessage;
            responseBase.Status = false;
            
            return responseBase;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual ResponseBase ExecuteRequest(Action executeMethod)
        {
            ResponseBase responseBase = new ResponseBase();
            try
            {
                executeMethod();
            }
            catch (ApplicationException ex)
            {
                responseBase.Message = ex.Message;
                responseBase.Status = false;
            }
            catch (Exception ex)
            {
                // only application exception are returned to the frontend, 
                // general exception like this might carries sensitive information and needs to be logged alone for developers usage
                
                //ErrorLogger.LogError(ex, HttpContext.Current.Server);
                responseBase.Message = "An error occured while processing your request";
                responseBase.Status = false;
                
            }
            return responseBase;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<ResponseBase> ExecuteRequestAsync(Func<Task> executeMethod)
        {
            ResponseBase responseBase = new ResponseBase();
            try
            {
                await executeMethod();
            }
            catch (ApplicationException ex)
            {
                responseBase.Message = ex.Message;
                responseBase.Status = false;
                
            }
            catch (Exception ex)
            {
                //ErrorLogger.LogError(ex, HttpContext.Current.Server);
                responseBase.Message = "An error occured while processing your request";
                responseBase.Status = false;
                
            }
            return responseBase;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual async Task<ResponseBase<T>> ExecuteRequestAsync<T>(Func<Task<T>> executeMethod)
        {
            ResponseBase<T> responseBase = new ResponseBase<T>();
            try
            {
                responseBase.Payload = await executeMethod();
            }
            catch (ApplicationException ex)
            {
                responseBase.Message = ex.Message;
                responseBase.Status = false;
                
            }
            catch (Exception ex)
            {
                //ErrorLogger.LogError(ex, HttpContext.Current.Server);
                responseBase.Message = "An error occured while processing your request";
                responseBase.Status = false;
                
            }
            return responseBase;
        }
       
    }
}
