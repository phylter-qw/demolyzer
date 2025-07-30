using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Demolyzer.Model
{
    public static class DictionaryUtil
    {
        public static string GetValue(this Dictionary<string, string> dictionary, string key)
        {
            string value = null;
            dictionary.TryGetValue(key, out value);
            return value;
        }
    }
    public static class BinaryReaderUtil
    {
        public static string ReadStringSimple(this BinaryReader reader)
        {
            StringBuilder sb = new StringBuilder();
            char character;
            while (true)
            {
                byte ch = reader.ReadByte();

                if (ch == 0)
                {
                    return sb.ToString();
                }

                if (ch >= 18 && ch <= 27)
                {
                    ch += 30;
                }
                else
                {
                    if (ch >= 146 && ch < 155)
                    {
                        ch -= 98; //yellow numbers
                    }
                    else
                    {
                        ch = (byte)(ch & ~128);// Remove the highest bit in the byte (which makes the text red).
                    }
                }

                // Get rid of undefined unicode characters.
                if (((ch > 0) && (ch <= 31)) || ((ch >= 127) && (ch <= 159)))
                {
                    ch = (byte)'_';
                }

                ////if newline
                //if (ch == 10)
                //{
                //    return sb.ToString();
                //}

                character = (char)ch;

                sb.Append(character);
            }
        }

        private const float CoordAngleConst = (1.0f / 8);
        private const float Angle16Const = (360.0f / 65536);

        public static float ReadCoord(this BinaryReader reader)
        {
            return reader.ReadInt16() * CoordAngleConst;
        }

        public static float ReadAngle(this BinaryReader reader)
        {
            return reader.ReadSByte() * CoordAngleConst;
        }

        public static float ReadAngle16(this BinaryReader reader)
        {
            return reader.ReadInt16() * Angle16Const;
        }

        public static void SeekToEnd(this BinaryReader reader)
        {
            reader.BaseStream.Seek(0, SeekOrigin.End);
        }
    }
}
