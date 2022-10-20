using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using eFacturityApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Environment = System.Environment;

[assembly: Xamarin.Forms.Dependency(typeof(eFacturityApp.Droid.Services.FileService))]
namespace eFacturityApp.Droid.Services
{
    public class FileService : IFileService
    {
        public string GetStorageFolderPath()
        {
            //return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);

        }
    }
}