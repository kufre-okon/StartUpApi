using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Interface;
using Data.Models;
using Data.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StartUpApi.Controllers
{
    /// <summary>
    /// Handle all file upload
    /// </summary>
    [AllowAnonymous]
    [Route("api/FileUpload")]
    [Produces("application/json")]
    public class FileUploadController : BaseController
    {
        readonly IImageService _imgService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="imgService"></param>
        public FileUploadController(IImageService imgService)
        {
            _imgService = imgService;
        }

        /// <summary>
        /// Upload user picture. The image file is passed through form data
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseBase<FileUploadResult>> UploadUserImage(string username)
        {
            return await ExecuteRequestAsync(() =>
             {
                 var file = Request.Form.Files[0];
                 var result = _imgService.UploadUserProfilePicture(username, file);
                 return result;
             });
        }


    }
}
