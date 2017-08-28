using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace PerformanceTests
{
    public class FileIOPerformanceTests
    {
        private static IEnumerable<DriveInfo> _drives;
        private List<string> _filenames = new List<string>();
        //private const string _filename = "d:\\tmp\\testdata.txt";
        private const string _filename = "{0}testdata.txt";

        const int Size = 20000000;

        [GlobalSetup]
        public void Setup()
        {
            _drives = System.IO.DriveInfo.GetDrives().Where(di => di.IsReady && di.DriveType == DriveType.Fixed);

            foreach (var drive in _drives)
            {
                string filename = GetFileName(drive);
                createTestData(filename, Size);
                _filenames.Add(filename);
            }
        }

        [GlobalCleanup]
        public void Teardown()
        {
            foreach (var file in _filenames)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        private static string GetFileName(DriveInfo drive)
        {
            string file = string.Format(_filename, drive.Name);
            return file;
        }

        [Benchmark]
        public void ReadFile_multiple_times_with_caching_enabled()
        {
            foreach (var file in _filenames)
            {
                var x = File.ReadAllText(file);
            }
        }

        [Benchmark]
        public void ReadFile_multiple_times_without_caching_enabled()
        {
            foreach (var file in _filenames)
            {
                FileSystemTools.FlushFSCacheForFile(file);
                var x = File.ReadAllText(file);
            }
        }

        private void createTestData(string path, int size)
        {
            var file = new FileInfo(path);
            if (file.Exists && file.Length == size)
            {
                return;
            }

            byte[] charArray = Encoding.UTF8.GetBytes("abcdefghij");
            using (var fs = new FileStream(path,FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                for (int i = 0; i < size/10; i++)
                {
                    fs.Write(charArray,0,10);
                }
            }
        }
    }

    class FileSystemTools
    {
        const int FILE_FLAG_NO_BUFFERING = 0x20000000;
        const FileOptions Unbuffered = (FileOptions)FILE_FLAG_NO_BUFFERING;

        /// <summary>
        /// Flush the file system cache for this file. This ensures that all subsequent operations on the file really cause disc
        /// access and you can measure the real disc access time.
        /// </summary>
        /// <param name="file">full path to file.</param>
        public static void FlushFSCacheForFile(string file)
        {
            using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.None | Unbuffered))
            {
            }
        }
    }
}
