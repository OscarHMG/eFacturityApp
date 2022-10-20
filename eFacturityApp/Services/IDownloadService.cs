using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace eFacturityApp.Services
{
    public interface IDownloadService
    {
        Task DownloadFileAsync(string url, IProgress<double> progress, CancellationToken token, string nombreArchivo, string extension);

    }
}
