using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediSearch.Core.Application.Helpers
{
    public static class ImageUpload
    {
        //I hope this will be usuful for you guys. Code with 🎯

        public static string UploadImageUser(IFormFile file, bool isEditMode = false, string imagePath = "")
        {
            if (isEditMode && file == null)
            {
                return imagePath;
            }

            string basePath = $"/Assets/Images/Users/";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            //create folder if not exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //get file extension
            if (file != null)
            {
                Guid guid = Guid.NewGuid();
                FileInfo fileInfo = new(file.FileName);
                string fileName = guid + fileInfo.Extension;

                string fileNameWithPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                if (isEditMode)
                {
                    if (imagePath != null)
                    {
                        string[] oldImagePart = imagePath.Split("/");
                        string oldImagePath = oldImagePart[^1];
                        string completeImageOldPath = Path.Combine(path, oldImagePath);

                        if (System.IO.File.Exists(completeImageOldPath))
                        {
                            System.IO.File.Delete(completeImageOldPath);
                        }
                    }

                }
                return $"{basePath}{fileName}";
            }
            return null;
        }

        public static string UploadImageChat(IFormFile file, string ItemId, bool isEditMode = false,  string imagePath = "")
        {
            if (isEditMode && file == null)
            {
                return imagePath;
            }

            string basePath = $"/Assets/Images/Chats/{ItemId}/";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            //create folder if not exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //get file extension
            if (file != null)
            {
                Guid guid = Guid.NewGuid();
                FileInfo fileInfo = new(file.FileName);
                string fileName = guid + fileInfo.Extension;

                string fileNameWithPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                if (isEditMode)
                {
                    if (imagePath != null)
                    {
                        string[] oldImagePart = imagePath.Split("/");
                        string oldImagePath = oldImagePart[^1];
                        string completeImageOldPath = Path.Combine(path, oldImagePath);

                        if (System.IO.File.Exists(completeImageOldPath))
                        {
                            System.IO.File.Delete(completeImageOldPath);
                        }
                    }

                }
                return $"{basePath}{fileName}";
            }
            return null;
        }

        public static string UploadImageCompany(IFormFile file, bool isEditMode = false, string imagePath = "")
        {
            if (isEditMode && file == null)
            {
                return imagePath;
            }

            string basePath = $"/Assets/Images/Companies/";
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot{basePath}");

            //create folder if not exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //get file extension
            if (file != null)
            {
                Guid guid = Guid.NewGuid();
                FileInfo fileInfo = new(file.FileName);
                string fileName = guid + fileInfo.Extension;

                string fileNameWithPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                if (isEditMode)
                {
                    if (imagePath != null)
                    {
                        string[] oldImagePart = imagePath.Split("/");
                        string oldImagePath = oldImagePart[^1];
                        string completeImageOldPath = Path.Combine(path, oldImagePath);

                        if (System.IO.File.Exists(completeImageOldPath))
                        {
                            System.IO.File.Delete(completeImageOldPath);
                        }
                    }

                }
                return $"{basePath}{fileName}";
            }
            return null;
        }

        public static async Task<List<string>> UploadImagesProduct(IFormFile[] fileForms, string ItemId, bool IsUpdateMode = false, List<string> currentsImgUrl = null)
        {
            List<string> imgUrl = new List<string>();

            //Get current directory
            string basePath = $"/Assets/Images/Products/{ItemId}";

            string servePath = Directory.GetCurrentDirectory();

            string ServerAndBasePath = Path.Combine(servePath, $"wwwroot{basePath}");

            if (!Directory.Exists(ServerAndBasePath))
            {
                Directory.CreateDirectory(ServerAndBasePath);
            }

            int index = 0;

            foreach (var fileForm in fileForms)
            {

                if (fileForm != null)
                {
                    FileInfo fileInfo = new FileInfo(fileForm.FileName);
                    Guid guid = Guid.NewGuid();

                    string uniqueFileName = guid + fileInfo.Extension;

                    string uniqueFileWithBaseServePath = Path.Combine(ServerAndBasePath, uniqueFileName);

                    using (FileStream stream = new FileStream(uniqueFileWithBaseServePath, FileMode.Create))
                    {
                        await fileForm.CopyToAsync(stream);
                    }

                    int size = index + 1;
                    //DELETE THE OLD IMAGE
                    if (IsUpdateMode)
                    {
                        if (currentsImgUrl.Count >= size)
                        {
                            if (!string.IsNullOrWhiteSpace(currentsImgUrl[index]))
                            {
                                string[] oldImagePart = currentsImgUrl[index].Split("/");
                                string oldImageFileName = oldImagePart[^1];
                                string completeOldImagePath = Path.Combine(ServerAndBasePath, oldImageFileName);

                                if (File.Exists(completeOldImagePath))
                                {
                                    File.Delete(completeOldImagePath);
                                }

                            }
                        }
                    }


                    imgUrl.Insert(index, $"{basePath}/{uniqueFileName}");
                }
                else
                {
                    imgUrl.Insert(index, currentsImgUrl[index]);
                }

                index++;
            }

            if (currentsImgUrl != null)
            {
                if (imgUrl.Count < currentsImgUrl.Count)
                {
                    while (imgUrl.Count != currentsImgUrl.Count)
                    {
                        imgUrl.Insert(index, currentsImgUrl[index]);
                        index++;
                    }
                }
            }

            return imgUrl;
        }

        //Helpful when we wan to delete the entire entity e.g: User and his/her image profile.
        public static void DeleteFile(string url)
        {

            //Get current directory
            string basePath = url;

            if (basePath != "/Assets/Images/default.jpg")
            {
                string servePath = Directory.GetCurrentDirectory();

                string path = Path.Combine(servePath, $"wwwroot{basePath}");

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        public static void DeleteFiles(string ItemId)
        {

            //Get current directory
            string basePath = $"/Assets/Images/Products/{ItemId}";

            string servePath = Directory.GetCurrentDirectory();

            string ServerAndBasePath = Path.Combine(servePath, $"wwwroot{basePath}");

            if (Directory.Exists(ServerAndBasePath))
            {

                DirectoryInfo directoryInfo = new DirectoryInfo(ServerAndBasePath);

                foreach (FileInfo files in directoryInfo.GetFiles())
                {
                    files.Delete();
                }

                foreach (DirectoryInfo directories in directoryInfo.GetDirectories())
                {
                    directories.Delete(true);
                }

                Directory.Delete(ServerAndBasePath, true);
            }


        }

    }
}
