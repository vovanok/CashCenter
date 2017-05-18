using System;
using System.IO;

namespace CashCenter.Common
{
    public class FileBuffer : IDisposable
    {
        public enum BufferType
        {
            Read,
            Create
        }

        private const string BUFFER_FILE_NAME = "bufimp";

        public string OriginalFilename { get; private set; }
        public string BufferFilename { get; private set; }
        public BufferType Type { get; private set; }

        public FileBuffer(string filename, BufferType type)
        {
            OriginalFilename = filename;
            Type = type;

            switch (Type)
            {
                case BufferType.Read:
                    {
                        if (!File.Exists(OriginalFilename))
                            throw new ApplicationException("DBF файл не задан или не существует");

                        BufferFilename = GenerateBufferFilename();
                        DeleteFileIfExist(BufferFilename);

                        File.Copy(OriginalFilename, BufferFilename);
                        break;
                    }

                case BufferType.Create:
                    {
                        BufferFilename = GenerateBufferFilename();
                        DeleteFileIfExist(BufferFilename);

                        var directory = Path.GetDirectoryName(BufferFilename);
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        break;
                    }
            }
        }

        public void Dispose()
        {
            switch (Type)
            {
                case BufferType.Read:
                    {
                        DeleteFileIfExist(BufferFilename);
                        break;
                    }

                case BufferType.Create:
                    {
                        DeleteFileIfExist(OriginalFilename);

                        File.Copy(BufferFilename, OriginalFilename);
                        File.Delete(BufferFilename);

                        break;
                    }
            }
        }

        private void DeleteFileIfExist(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
        }

        private string GenerateBufferFilename()
        {
            return Path.Combine(Path.GetDirectoryName(OriginalFilename), BUFFER_FILE_NAME) + Path.GetExtension(OriginalFilename);
        }
    }
}