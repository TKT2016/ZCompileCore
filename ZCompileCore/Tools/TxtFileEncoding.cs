using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZCompileCore.Tools
{
    public static class TxtFileEncoding
    {
        /// <summary>
        /// 取得一个文本文件的编码方式。如果无法在文件头部找到有效的前导符，Encoding.Default将被返回。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(string fileName)
        {
            return GetEncoding(fileName, Encoding.Default);
        }

        /// <summary>
        /// 取得一个文本文件流的编码方式。
        /// </summary>
        /// <param name="stream">文本文件流。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(FileStream stream)
        {
            return GetEncoding(stream, Encoding.Default);
        }

        /// <summary>
        /// 取得一个文本文件的编码方式。
        /// </summary>
        /// <param name="fileName">文件名。</param>
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            Encoding targetEncoding = GetEncoding(fs, defaultEncoding);
            fs.Close();
            return targetEncoding;
        }
        /*
        public static bool GetResult(byte[] Value, int index, int judge)
        {
            return Value.Length > index ? Value[index] == judge : false;
        }*/

        /// <summary>
        /// 取得一个文本文件流的编码方式。
        /// </summary>
        /// <param name="stream">文本文件流。</param>
        /// <param name="defaultEncoding">默认编码方式。当该方法无法从文件的头部取得有效的前导符时，将返回该编码方式。</param>
        /// <returns></returns>
        public static Encoding GetEncoding(FileStream fStream, Encoding defaultEncoding)
        {
            Encoding encoding = defaultEncoding;
            //FileStream fStream = File.Open(filePath, FileMode.Open);
            if (fStream.CanSeek)
            {
                byte[] data = new byte[4];
                fStream.Read(data, 0, 4);
                int markerLength = 0;
                switch (data[0])
                {
                    case 0xef: // UTF8 
                        markerLength = 3;
                        if (data.Length < 3)
                            break;
                        if (data[1] == 0xbb && data[2] == 0xbf)
                            encoding = Encoding.UTF8;
                        break;
                    case 0xfe: // UTF 16 BE 
                        markerLength = 2;
                        if (data[1] == 0xff)
                            encoding = Encoding.BigEndianUnicode;
                        break;
                    case 0xff: // UTF 16 LE 
                        markerLength = 2;
                        if (data[1] == 0xfe)
                            encoding = Encoding.Unicode;
                        break;
                    default:
                        encoding = Encoding.ASCII;
                        break;
                }
            }
            fStream.Dispose();
            fStream.Close();
            fStream = null;
            return encoding; 
            
        }
    }
}
