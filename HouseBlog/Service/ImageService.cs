using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HouseBlog.Service
{
    public class ImageService
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IConfiguration _configuration;
        public ImageService(IHostingEnvironment appEnvironment, IConfiguration configuration)
        {
            _configuration = configuration;
            _appEnvironment = appEnvironment;
        }
        public async Task<string> SaveImageAsync(IFormFile uploadedFile, int flag)
        {
            if (uploadedFile != null)
            {
                string path = "";
                if (flag == 0)
                {
                    path = _configuration["ImagePathHouses"] + uploadedFile.FileName;
                }
                else
                {
                    path = _configuration["ImagePathPosts"] + uploadedFile.FileName;
                }
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                return path;
            }
            return "";
        }
    }
}
