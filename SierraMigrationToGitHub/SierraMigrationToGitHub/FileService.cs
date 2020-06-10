using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SierraMigrationToGitHub
{
    public class FileService
    {
        public async Task<string> SaveFileAsync(string fileFolder, string fileName, Stream fileStream)
        {
            Directory.CreateDirectory(fileFolder);
            var filePath = Path.Combine(fileFolder, fileName);
            using FileStream stream = File.Create(filePath);//new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await fileStream.CopyToAsync(stream);
            return filePath;
        }
        public async Task<string> SaveFileAsync(StreamFileSaveModel streamFileSaveModel)
        {
            return await SaveFileAsync(streamFileSaveModel.FileFolder, streamFileSaveModel.FileName, streamFileSaveModel.FileStream);
        }
        public async Task<List<string>> SaveFilesAsync(List<StreamFileSaveModel> streamFileSaveModels)
        {
            var saveFilesTasks = streamFileSaveModels.Select(SaveFileAsync);
            var filePathes = await Task.WhenAll(saveFilesTasks);
            return filePathes.ToList();
        }
        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }
        public class StreamFileSaveModel
        {
            public string FileFolder { get; set; }
            public string FileName { get; set; }
            public Stream FileStream { get; set; }

        }
    }
}
