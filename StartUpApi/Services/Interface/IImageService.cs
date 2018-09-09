using StartUpApi.Data.Models;
using StartUpApi.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Pager.Interface;
using Microsoft.AspNetCore.Http;
using StartUpApi.Data.DTOs;

namespace StartUpApi.Services.Interface
{
    /// <summary>
    /// Image service interface
    /// </summary>
    public interface IImageService
    {
        /// <summary>
        /// upload user profile picture file
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<FileUploadResult> UploadUserProfilePicture(string userId, IFormFile file);
        /// <summary>
        /// Checks if the file is an image file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool IsImage(IFormFile file);
        /// <summary>
        /// Checks if the file is of size less than or equal to the size specified.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="size"></param>
        /// <param name="isSizeInKb">indicate if the size is in kilobyte or megabyte </param>
        /// <returns></returns>
        bool IsOfSize(IFormFile file, int size, bool isSizeInKb = true);
        /// <summary>
        /// Upload File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folder"></param>
        /// <param name="newFileName">The new file name for the file</param>
        /// <returns></returns>
        FileUploadResult UploadFile(IFormFile file, string folder, string newFileName = null);
        /// <summary>
        /// Delete the specified file
        /// </summary>
        /// <param name="filepath">Full physical file path</param>
        /// <param name="isPhysicalPath">Indicate whether the <paramref name="filepath"/> is relative or physical </param>
        void DeleteFile(string filepath, bool isPhysicalPath = true);
    }
}
