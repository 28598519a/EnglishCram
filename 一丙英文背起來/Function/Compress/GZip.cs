using System.IO;
using System.IO.Compression;

namespace 一丙英文背起來
{
    namespace Compress
    {
        public class GZip
        {
            /// <summary>
            /// GZIP解壓縮
            /// </summary>
            /// <param name="rawData">byte[]資料</param>
            /// <returns>byte[]資料</returns>
            public static byte[] Decompress(byte[] rawData)
            {
                using (GZipStream decompressionStream = new GZipStream(new MemoryStream(rawData), CompressionMode.Decompress))
                {
                    int size = 1024; //這邊訂多少都可以(下面有while)，但建議設2的次方數
                    byte[] buffer = new byte[size];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = decompressionStream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                ms.Write(buffer, 0, count);
                            }
                        } while (count > 0);

                        return ms.ToArray();
                    }
                }
            }

            /// <summary>
            /// GZIP壓縮
            /// </summary>
            /// <param name="rawData">byte[]資料</param>
            /// <returns>byte[]資料</returns>
            public static byte[] Compress(byte[] rawData)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true))
                    {
                        compressedzipStream.Write(rawData, 0, rawData.Length);
                        return ms.ToArray();
                    }
                }
            }
        }
    }
}