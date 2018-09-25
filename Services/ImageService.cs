using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data.Repository;
using Data.DTOs;
using General;
using Services.Interface;

namespace Services
{
    /// <summary>
    /// Handle Image related tasks
    /// </summary>
    public class ImageService : IImageService
    {
        readonly IApplicationUserRepository _userRepo;
        readonly IHostingEnvironment _env;

        /// <summary>
        /// Image service constructor
        /// </summary>
        public ImageService(IHostingEnvironment env, IApplicationUserRepository userRepo)
        {
            _userRepo = userRepo;
            _env = env;
        }

        /// <summary>
        /// upload user profile picture file
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<FileUploadResult> UploadUserProfilePicture(string username, IFormFile file)
        {
            var user = await _userRepo.FindUserByUsername(username);
            var oldImageUrl = user?.ProfilePictureUrl;
            // delete the old image if present
            if (!string.IsNullOrWhiteSpace(oldImageUrl))
            {
                DeleteFile(oldImageUrl, false);
            }
            return UploadFile(file, Constants.USER_PROFILE_PICTURE_DIR, (user?.UserName ?? username));
        }

        /// <summary>
        /// Checks if the file is an image file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool IsImage( IFormFile file)
        {
            return ((file != null) && System.Text.RegularExpressions.Regex.IsMatch(file.ContentType, "image/\\S+") && (file.Length > 0));
        }

        /// <summary>
        /// Checks if the file is of size less than or equal to the size specified.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="size"></param>
        /// <param name="isSizeInKb">indicate if the size is in kilobyte or megabyte </param>
        /// <returns></returns>
        public bool IsOfSize( IFormFile file, int size, bool isSizeInKb = true)
        {
            long iFileSize = file.Length;
            return (iFileSize <= size * (isSizeInKb ? 1024 : 1048576));
        }

        /// <summary>
        /// Upload File
        /// </summary>
        /// <param name="file"></param>
        /// <param name="folder"></param>
        /// <param name="newFileName">The new file name for the file</param>
        /// <returns></returns>
        public FileUploadResult UploadFile( IFormFile file, string folder, string newFileName=null)
        {
            FileUploadResult fileUploadResult = new FileUploadResult();
            if (file.FileName.Length > 0)
            {
                string filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                string dirPath = "/media/" + (folder ?? "images");
                string mapDirPath = getFullPath(dirPath);
                if (!CreateDirIfNotExits(mapDirPath))
                    return fileUploadResult;
                string extension = Path.GetExtension(filename);
                string filePath = GetUniqueFilePath(mapDirPath, newFileName?? filename, extension);
                SaveFile(file, filePath);
                //strip off root paths 
                filePath = filePath.Replace("\\", "/");
                var regex = new Regex($@"({dirPath}(?:[^\s]+))", RegexOptions.IgnoreCase);
                if (regex.IsMatch(filePath))
                    filePath = regex.Match(filePath).Captures[0].Value;
                fileUploadResult = new FileUploadResult
                {
                    LocalFilePath = filePath,
                    FileName = filename,
                    FileLength = file.Length
                };
            }
            return fileUploadResult;
        }

        /// <summary>
        /// Delete the specified file
        /// </summary>
        /// <param name="filepath">Full physical file path</param>
        /// <param name="isPhysicalPath">Indicate whether the <paramref name="filepath"/> is relative or physical </param>
        public void DeleteFile(string filepath, bool isPhysicalPath = true)
        {
            if (!isPhysicalPath)
                filepath = getFullPath(filepath);
            if (File.Exists(filepath))
                File.Delete(filepath);
        }

        #region private

        private string getFullPath(params string[] paths)
        {
            var webRoot = _env.WebRootPath;
            var _paths = paths.ToList();
            _paths.Insert(0, webRoot);
            paths = _paths.ToArray();
            var fullPath = System.IO.Path.Combine(paths);
            return fullPath;
        }

        private void SaveFile(IFormFile source, string fileName)
        {
            using (FileStream output = File.Create(fileName))
                source.CopyTo(output);
        }

        private bool CreateDirIfNotExits(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                return true;
            }
            catch { return false; }
        }

        private string GetUniqueFilePath(string dirPath, string fileName, string extension)
        {           
            string filename = Path.GetFileNameWithoutExtension(fileName);

            string filePath = Path.Combine(dirPath, fileName);
            for (int i = 1; File.Exists(filePath); i++)
            {
                string temp = string.Format("{0}({1}){2}", filename, i, extension);
                filePath = Path.Combine(dirPath, temp);
            }
            return filePath;
        }

        #endregion
    }
}
