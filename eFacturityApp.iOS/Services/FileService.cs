using eFacturityApp.Services;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(eFacturityApp.iOS.Services.FileService))]
namespace eFacturityApp.iOS.Services
{
    public class FileService : IFileService
    {
        public string GetStorageFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
    }
}