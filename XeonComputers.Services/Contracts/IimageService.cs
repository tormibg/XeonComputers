﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XeonComputers.Services.Contracts
{
    public interface IImageService
    {
        void UploadImage(IFormFile formImage, string path);

        Task<IList<string>> UploadImages(IList<IFormFile> formImages, int count, string template, int id);
    }
}