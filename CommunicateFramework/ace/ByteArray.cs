using System.IO;

namespace CommunicateFramework.ace
{
    public class ByteArray
    {
        private MemoryStream _m_memoryStream = new MemoryStream();

        private BinaryWriter bw;
        private BinaryReader br;

        public void Close()
        {
            bw.Close();
            br.Close();
            _m_memoryStream.Close();
        }

        public ByteArray(byte[] buff)
        {
            if(_m_memoryStream != null)
                _m_memoryStream.Close();
            _m_memoryStream = new MemoryStream(buff);
            bw = new BinaryWriter(_m_memoryStream);
            br = new BinaryReader(_m_memoryStream);
        }

        public ByteArray()
        {
            bw = new BinaryWriter(_m_memoryStream);
            br = new BinaryReader(_m_memoryStream);
        }

        public int Position { get { return (int) _m_memoryStream.Position; } }
        public int Length { get { return (int) _m_memoryStream.Length; } }
        
        public bool Readable { get { return _m_memoryStream.Length > _m_memoryStream.Position; } }

        #region write

        public void write(int value)
        {
            bw.Write(value);
        }
        public void write(byte value)
        {
            bw.Write(value);
        }
        public void write(bool value)
        {
            bw.Write(value);
        }
        public void write(string value)
        {
            bw.Write(value);
        }
        public void write(byte[] value)
        {
            bw.Write(value);
        }
        public void write(double value)
        {
            bw.Write(value);
        }
        public void write(float value)
        {
            bw.Write(value);
        }

        #endregion

        #region read

        

        #endregion
    }
}