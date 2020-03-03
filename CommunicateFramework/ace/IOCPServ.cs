﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CommunicateFramework.ace
{
   public class IOCPServ
    {
        Socket server;
        private int userMax;
        private Semaphore maxAcceptClients;
        private int buffSize=1024;
        private UserTokenPool userPool;
        public LengthEncode lengthEncode;
        public LengthDecode lengthDecode;
        public SerEncode serEncode;
        public SerDecode serDecode;
        public AbsHandlerCenter center;
        public IOCPServ(int max) {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            userMax = max;
        }

        public void init() {
            userPool = new UserTokenPool(userMax);
            maxAcceptClients = new Semaphore(userMax, userMax);
            if (serEncode == null || serDecode==null) throw new Exception(" message encode or decode is null");
            if (center == null) throw new Exception(" center is null");
            for (int i = 0; i < userMax; i++) {
                UserToken token = new UserToken(buffSize);
                token.receiveEvent.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                token.sendEvent.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                token.lEnCode = lengthEncode;
                token.lDecode = lengthDecode;                
                token.sEncode = serEncode;
                token.sDecode = serDecode;
                token.sendProcess = ProcessSend;
                token.closeProcess = closeClient;
                token.messageReceive = center.MessageReceive;
                userPool.push(token);
            }
        }

        public void Start(int port) {
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(10);
            StartAccept(null);
        }

        public void StartAccept(SocketAsyncEventArgs e) {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_Completed);
            }
            else {
                e.AcceptSocket = null;
            }
            maxAcceptClients.WaitOne();
            bool result = server.AcceptAsync(e);
            if (!result) {
                ProcessAccept(e);
            }
        }

        public void ProcessAccept(SocketAsyncEventArgs e) {
            Console.WriteLine("客户端连接，当前第" + userPool.getSize());
            UserToken userToken = userPool.pop();
            userToken.connectSocket = e.AcceptSocket;            
            center.ClientConnect(userToken);
            StartReceive(userToken);
            StartAccept(e);
        }

        public void StartReceive(UserToken userToken) {
            try
            {
                bool result = userToken.connectSocket.ReceiveAsync(userToken.receiveEvent);
                if (!result)
                {
                    lock (userToken)
                    {
                        ProcessReceive(userToken.receiveEvent);
                    }
                }
            }
            catch (Exception E) {
                throw new Exception("receive message error"+E);                
            }
        }

        public void ProcessReceive(SocketAsyncEventArgs e) {
            UserToken userToken= e.UserToken as UserToken;
            if (userToken.receiveEvent.BytesTransferred > 0 && userToken.receiveEvent.SocketError == SocketError.Success)
            {
                byte[] bs=new byte[userToken.receiveEvent.BytesTransferred];
                Buffer.BlockCopy(userToken.receiveEvent.Buffer, 0, bs, 0, userToken.receiveEvent.BytesTransferred);
                userToken.receive(bs);
                StartReceive(userToken);
            }
            else {
                string error = "";
                if (userToken.receiveEvent.SocketError != SocketError.Success)
                {
                    error = userToken.receiveEvent.SocketError.ToString();
                }
                else {
                    error = "远程客户端主动断开连接";
                }
                closeClient(userToken,error);
            }
        }

        

        public void Accept_Completed(object sender, SocketAsyncEventArgs e) {
            try
            {
                ProcessAccept(e);
            }
            catch(Exception E) {
                Console.WriteLine(E.Message);
            }
        }

        public void IO_Completed(object sender, SocketAsyncEventArgs e) {
            UserToken token = e.UserToken as UserToken;
            try
            {
                lock (token)
                {
                    if (e.LastOperation == SocketAsyncOperation.Receive)
                        ProcessReceive(e);
                    else if (e.LastOperation == SocketAsyncOperation.Send)
                        ProcessSend(e);
                }
            }
            catch (Exception E)
            {
                closeClient(token,E.Message);
            }
        }
        public void ProcessSend(SocketAsyncEventArgs e) {
            UserToken userToken = e.UserToken as UserToken;
            if (e.SocketError != SocketError.Success) {
                closeClient(userToken,e.SocketError.ToString());
            }
            else
            {
                userToken.sendEnd();
            }
            
        }

        public void closeClient(UserToken token,string error) {
            if (token.connectSocket != null)
            {
                lock (token)
                {
                    center.ClientClose(token, error);
                    token.colse();
                    maxAcceptClients.Release();
                    userPool.push(token);
                }
            }
        }

    }
}
