using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spoofy
{
    internal class ytdlpHelper
    {
        private const string _uri = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-lgpl.zip";
        private const string _path = @"C:\temp\ffmpeg.zip";
        private const string __path = @"C:\temp\ffmpeg";
        private const string ffmpeg_path = @"C:\temp\ffmpeg\ffmpeg-master-latest-win64-lgpl\bin\ffmpeg.exe";

        private const string uri = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";
        private const string path = @"C:\temp\yt-dlp.exe";


        // Download YT-DLP executable to temp folder
        public static async Task Init()
        {
            Clean();
            Console.WriteLine("Download path: " + Path.GetFullPath(@".\downloads"));
            using var client = new HttpClient();

            // get ffmpeg binaries
            if (!Directory.Exists(__path))
            {
                Console.Write("Downloading FFMPEG... ");
                HttpResponseMessage _resp = await client.GetAsync(_uri);
                _resp.EnsureSuccessStatusCode();
                byte[] _content = await _resp.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(_path, _content);
                ZipFile.ExtractToDirectory(_path, __path);
                Console.WriteLine("Complete!");
            }
            

            if (!File.Exists(path))
            {
                Console.Write("Downloading YT-DLP... ");
                // get the yt-dlp binaries
                HttpResponseMessage resp = await client.GetAsync(uri);
                resp.EnsureSuccessStatusCode();
                byte[] content = await resp.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(path, content);
                Console.WriteLine("Complete!");
            }
            
        }

        public static void Setup()
        {
            if (!Path.Exists(Path.GetFullPath(@".\downloads"))) Directory.CreateDirectory(Path.GetFullPath(@".\downloads"));
        }

        public static string ToMP3(string videoUrl)
        {
            Console.WriteLine("Downloading video with URL: " + videoUrl);
            Setup();
            var psi = new ProcessStartInfo
            {
                FileName = path,
                Arguments = $"-x --audio-format mp3 \"{videoUrl}\" --ffmpeg-location \"{Path.GetFullPath(ffmpeg_path)}\" -o \"{Path.GetFullPath(@".\downloads")}/%(title)s.%(ext)s\"",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                using (var proc = Process.Start(psi))
                {
                    proc!.WaitForExit();
                }
            } catch
            {
                Console.WriteLine("An unknown error occured when downloading video with URL " + videoUrl);
            }
            string? newPath = null;
            DateTime newest = DateTime.UnixEpoch;
            var files = Directory.GetFiles(Path.GetFullPath(@".\downloads"));
            foreach (var file in files)
            {
               if (Directory.GetLastWriteTime(file) > newest)
                {
                    newPath = file;
                }
            }
            Console.WriteLine("Saved to: " + newPath ?? "N/A");
            return newPath ?? "";
        }

        public static void Clean()
        {
            Console.WriteLine("Cleaning up existing executables..");
            if (File.Exists(path)) File.Delete(path);
            if (File.Exists(_path)) File.Delete(_path);
            if (Directory.Exists(__path)) Directory.Delete(__path, true);
        }
    }
}
