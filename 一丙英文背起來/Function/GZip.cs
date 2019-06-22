using System.IO;
using System.IO.Compression;

public class GZip
{
    public static byte[] Decompress(byte[] rawData)
    {
        using (GZipStream decompressionStream = new GZipStream(new MemoryStream(rawData), CompressionMode.Decompress))
        {
            int size = 1024; //這邊訂多少都可以(下面有while)
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
                }while (count > 0);

                ms.Close();
                return ms.ToArray();
            }
        }
    }

    public static byte[] Compress(byte[] rawData)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true))
            {
                compressedzipStream.Write(rawData, 0, rawData.Length);
                compressedzipStream.Close();
                return ms.ToArray();
            }
        }
    }
}