using System.IO;
using System.IO.Compression;
using System.Text;

namespace 一丙英文背起來
{
    namespace Compress
    {
        public class Zip
        {
            /// <summary>
            /// ZIP解壓縮
            /// </summary>
            /// <param name="zipPath">ZIP檔案路徑</param>
            /// <param name="extractPath">解壓縮到目錄</param>
            /// <param name="overwrite">覆寫</param>
            public static void Decompress(string zipPath, string extractPath, bool overwrite = false)
            {
                try
                {
                    ZipFile.ExtractToDirectory(zipPath, extractPath, Encoding.GetEncoding("big5"));
                }
                catch (IOException)
                {
                    if (overwrite.Equals(true))
                    {
                        Directory.Delete(Path.Combine(extractPath,Path.GetFileNameWithoutExtension(zipPath)), true);
                        Decompress(zipPath, extractPath);
                    }
                }
            }


            /// <summary>
            /// ZIP壓縮
            /// </summary>
            /// <param name="path"></param>
            /// <param name="zipName"></param>
            public static void Compress(string path, string zipName)
            {
                if (Directory.Exists(path))
                {
                    ZipFile.CreateFromDirectory(path, zipName, CompressionLevel.Optimal, true);
                }
            }
        }
    }
}
