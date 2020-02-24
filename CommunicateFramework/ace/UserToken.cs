using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace CommunicateFramework.ace
{
    public class UserToken
    {
        public SocketAsyncEventArgs m_receiveEvent;
        public SocketAsyncEventArgs m_sendEvent;
        public Socket m_connectSocket;

        public Action<SocketAsyncEventArgs> sendProcess;
        public Action<UserToken, string> closeProcess;

        public List<byte> m_lCache = new List<byte>();
        public Queue<byte[]> m_qWriteQueue = new Queue<byte[]>();

        #region 编码解码器委托

        public Action<UserToken, object> messageReceive;
        public LengthEncode lEnCode;
        public LengthDecode lDecode;
        public SerEncodes sEncode;
        public SerDecodes sDecode;
        
        #endregion

        private bool _m_bIsReading = false;
        private bool _m_bIsWriteing = false;

        public UserToken(int _buffSize)
        {
            m_receiveEvent = new SocketAsyncEventArgs();
            m_receiveEvent.UserToken = this;
            m_sendEvent = new SocketAsyncEventArgs();
            m_sendEvent.UserToken = this;
            byte[] buff = new byte[_buffSize];
            m_receiveEvent.SetBuffer(buff, 0, buff.Length);
        }

        public void receive(byte[] _buff)
        {
            m_lCache.AddRange(_buff);
            if (!_m_bIsReading)
            {
                onData();
            }
        }

        public void onData()
        {
            byte[] buff = null;
            if (lDecode != null)
            {
                buff = lDecode(ref m_lCache);
                if (buff == null)
                {
                    _m_bIsReading = false;
                    return;
                }
                else
                {
                    if(m_lCache.Count == 0)
                        return;
                    buff = m_lCache.ToArray();
                    m_lCache.Clear();
                }
                if(sDecode == null)
                    throw new Exception("message decode process is null");
                if(messageReceive == null)
                    throw new Exception("messageReceive process is null");
                object message = sDecode(buff);
                messageReceive(this, message);
                onData();
            }
        }

        public void write(object _value)
        {
            if(sEncode == null)
                throw new Exception("message encode process is null");
            byte[] decodes = sEncode(_value);
            if (lEnCode != null)
            {
                decodes = lEnCode(decodes);
            }
            write(decodes);
        }

        public void onWrite()
        {
            if (m_qWriteQueue.Count == 0)
            {
                _m_bIsWriteing = false;
                return;
            }

            byte[] buff = m_qWriteQueue.Dequeue();
            m_sendEvent.SetBuffer(buff, 0, buff.Length);
            bool result = m_connectSocket.SendAsync(m_sendEvent);
            if (!result)
            {
                sendProcess(m_sendEvent);
            }
        }

        public void sendEnd()
        {
            onWrite();
        }

        public void write(byte[] _buff)
        {
            if (m_connectSocket == null)
            {
                closeProcess(this, "发送消息给已断开的连接");
                return;
            }
            m_qWriteQueue.Enqueue(_buff);
            if (!_m_bIsWriteing)
            {
                _m_bIsWriteing = true;
                onWrite();
            }
        }
    }
}