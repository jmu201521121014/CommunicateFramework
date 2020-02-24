using System;
using System.Collections.Generic;
using System.IO;

namespace CommunicateFramework.ace.auto
{
    public class LengthEncoding
    {
        public static byte[] encode(byte[] buff)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(buff.Length);
            bw.Write(buff);

            byte[] result = new byte[(int) ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
            bw.Close();
            ms.Close();
            return result;
        }
        
        public static byte[] decode(ref List<byte> cache)
        {
            if (cache.Count < 4)
                return null;
            MemoryStream ms = new MemoryStream(cache.ToArray());
            BinaryReader br = new BinaryReader(ms);
            int length = br.ReadInt32();
            if (length > ms.Length - ms.Position)
            {
                return null;
            }

            byte[] result = br.ReadBytes(length);
            cache.Clear();
            cache.AddRange(br.ReadBytes((int)(ms.Length - ms.Position)));
            br.Close();
            ms.Close();
            return result;
        }
    }
}