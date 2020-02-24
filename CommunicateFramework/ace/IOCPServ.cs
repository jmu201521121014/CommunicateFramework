using System.Globalization;
using System.Net;
using System.Net.Sockets;
using CommunicateFramework.ace.auto;

namespace CommunicateFramework.ace
{
    public class IOCPServ
    {
        private Socket _m_sServer;
        private int _m_iUserMax;
        private int _m_iBuffSize = 1024;
        private UserTokenPool _m_userTokenPool;
        public LengthEncoding m_lengthEncoding;
//        public LengthDecode m_lengthDecode;
//        public SerEncode m_serEncode;
//        public SerDecode m_serDecode;
        public AbsHandlerCenter m_center;

        public IOCPServ(int max)
        {
//            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _m_iUserMax = max;
        }

        public void init()
        {
            _m_userTokenPool = new UserTokenPool();
        }
    }
}