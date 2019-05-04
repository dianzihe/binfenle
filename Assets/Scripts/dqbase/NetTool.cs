// #define  NEED_NET_SAFT_CHECK
#define USE_ZIP_MSG_PACKET

using System.Collections;

using System;
using System.Collections.Generic;
using System.Text;

using System.Net.Sockets;
using System.Net;
using System.Threading;


namespace Logic
{

    enum NET_PACKET_ID
    {
        NET_CHECK_REQUEST = 1,		
        NET_CHECK_RESPONSE = 2,		
        NET_COMPRESS_EVENT = 3,
        NET_EVENT = 7,		

        NET_PACKET_MAX,
    };
    //---------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------------
    public abstract class tNetTool
    {
        public abstract bool Send(tEvent evt, int target);
        public abstract bool OnReceive(ref DataBuffer data, int target);

        public abstract int tryRead(ref byte[] data, int beginPos, int count);
		public abstract int tryWrite(byte[] data, int beginPos, int count);

        public abstract int available();

        public abstract void Process(float delayTime);
		
		public abstract bool Connect(string strIp, int port);
		public abstract bool ConnectDNS(string urlDNS, int port);
		
		public abstract void SetEventCenter(tEventCenter eventCenter);
		
		public abstract void SetOk( bool bOk );
        public abstract bool IsOk();

        public abstract void Close(bool bRunCloseEvent);

        public abstract void ReConnect();

        public abstract void OnConnect();
        public abstract void OnClose();
        public abstract void OnConnectFail();

        public abstract void OnError(string errorInfo);

        public abstract void SetConnectEvent(tEvent evt);
    }
    //---------------------------------------------------------------------------------------------------------
    public abstract class BaseNetTool : tNetTool
    {
        static public int                   CONFIG_PACKHEAD_LENGTH = 5;
        static public int                   ONCE_RECEIVE_PACKET_COUNT = 10;

        protected OutputBuffer              mSendBuffer = new OutputBuffer(10240);
        protected InputBuffer               mRevBuffer = new InputBuffer(10240);

        tNetTool                            mNetTool;
        public tEventCenter                 mEventCenter;

        protected DataBuffer                mDatabuffer = new DataBuffer(2048);
        protected DataBuffer                mZipBuffer = new DataBuffer(2048);

        public tEvent                       mConnectEvent;
        public tEvent                       mNotifyConnectFinishEvent;
        public string                       mCloseEventName;
        //-----------------------------------------------------------------------------
        public BaseNetTool()
        {
            mNetTool = this;
        }

        public virtual string GetIP() { return "UNKNOW"; }
        public virtual int GetPort() { return 0; }

        public virtual void InitClear()
        {
            mSendBuffer.Clear();
            mRevBuffer.Clear();
        }

        public override void SetConnectEvent(tEvent evt)
        {
            if (mConnectEvent != null)
                mConnectEvent.Finish();

            mConnectEvent = evt;
        }

        public void Log(string info) { if (null != mEventCenter) mEventCenter.Log(info); }
        public override void SetEventCenter(tEventCenter eventCenter) { mEventCenter = eventCenter; }


        public override bool Send(tEvent evt, int target)
        {
            if (!IsOk())
            {
                if (mEventCenter != null)
                    mEventCenter.OnSendEvent(evt);
                evt.Log("***** Error: net is not connect or not ready. >>> send fail >>>");
                evt.OnSendFail(SEND_RESULT.eNetDisconnect);
                return false;
            }
            mDatabuffer.seek(CONFIG_PACKHEAD_LENGTH);
            if (!evt.serialize(ref mDatabuffer))
            {
                Log("Error: event serialize fail.");
                evt.OnSendFail(SEND_RESULT.eEventSerializeFail);
                return false;
            }

            if (mEventCenter != null)
                mEventCenter.OnSendEvent(evt);

            int count = mDatabuffer.tell();
            mDatabuffer.seek(0);
            byte id = (byte)NET_PACKET_ID.NET_EVENT;
            mDatabuffer.write(id);
            mDatabuffer.write(count - CONFIG_PACKHEAD_LENGTH);  // <--save data size
            bool bSu = mSendBuffer.Write(mDatabuffer.getData(), 0, count); //<--save data to send buffer
            if (!bSu)
                evt.OnSendFail(SEND_RESULT.eWriteSendDataFail);
            return bSu;
        }

        public override bool OnReceive(ref DataBuffer data, int target)
        {
            tEvent evt = null;
            try
            {
                evt = mEventCenter.RestoreEvent(ref data, 0, 0);
            }
            catch (Exception e)
            {
                Log("Error: restore packet data fail. may be event restore error. >>> \r\n" + e.ToString());
            }
            if (null != evt)
            {
                if (StaticDefine.debug)
                {
                    if (mEventCenter != null)
                        mEventCenter.OnReceiveEvent(evt);

                    return evt.DoEvent();
                }
                else
                {
                    try
                    {
                        if (mEventCenter != null)
                            mEventCenter.OnReceiveEvent(evt);

                        return evt.DoEvent();
                    }
                    catch (Exception e)
                    {
                        Log("Error: run net event fail. >>> " + evt.GetEventName() + ">>>" + e.ToString());
                    }
                }
            }

            return false;
        }

        public override void Process(float delayTime)
        {
            // receive data.
            int len = mRevBuffer.FillOnNet(mNetTool);
            if (IsOk())
            {
                // send data.
                mSendBuffer._NetSend(mNetTool);
            }
            else if (len>0)
            {
                SetOk(true);
            }

            if (StaticDefine.debug)
            {
                _ReceivePacket();
            }
            else
            {
                try
                {
                    _ReceivePacket();
                }
                catch (Exception e)
                {
                    Log("Error: _ReceivePacket() run fail.>>>" + e.ToString());
                }
            }
        }


        public bool _ReceivePacket()
        {
            if (mRevBuffer.empty())
                return true;            

            int receiveCount = 0;
            while (true)
            {
                if (receiveCount++ >= ONCE_RECEIVE_PACKET_COUNT || mRevBuffer.DataSize() < CONFIG_PACKHEAD_LENGTH)
                    return true;

                              
                byte[] d = new byte[CONFIG_PACKHEAD_LENGTH];

                if (!mRevBuffer.Peek(ref d, 0, CONFIG_PACKHEAD_LENGTH))
                    break;

                //when use biffult restore packet.
                //ushort id = BitConverter.ToUInt16(d, 0);
                byte id = d[0];

                uint indexAndSize = BitConverter.ToUInt32(d, sizeof(byte));
                uint packSize = indexAndSize & 0xffffff;

                if (packSize <= 0)
                    throw new Exception("Error: have packet size is zero, net error, please must close net.");

                if (mRevBuffer.DataSize()<CONFIG_PACKHEAD_LENGTH + packSize)
                    break;

                if (!mRevBuffer.Skip(CONFIG_PACKHEAD_LENGTH))
                {
                    Log(" *** XXX move receive data buffer fail. for packet head data");
                    return false;
                }

                if (id == (ushort)NET_PACKET_ID.NET_COMPRESS_EVENT)
                {
#if USE_ZIP_MSG_PACKET
                    if (mZipBuffer.size() < packSize)
                        mZipBuffer._resize((int)packSize);

                    if (!mRevBuffer.Peek(ref mZipBuffer.mData, 0, (int)packSize))
                    {
                        Log(" *** XXX read receive data fail. packet data can not read");
                        return false;
                    }
                    if (!mRevBuffer.Skip((int)packSize))
                    {
                        Log(" *** XXX move receive data buffer fail. for packet data");
                        return false;
                    }
                    // un compress
                    if (packSize > sizeof(uint))
                    {
                        if (!mZipBuffer.seek(0))
                            break;

                        uint scrSize = 0;
                        if (!mZipBuffer.read(out scrSize))
                            break;

                        if (mDatabuffer.size() < scrSize)
                            mDatabuffer._resize((int)scrSize);
						int zipSize = (int)packSize - sizeof(uint);
                        /*
						if (!MyTest.ZipTool.RestoreZipData(mZipBuffer.mData, sizeof(uint), zipSize, ref mDatabuffer.mData, (int)scrSize))
                        {
                            Log("XXX >>> un compress zip data fail.");
                            return false;
                        }
                        else
                        {
                            Log("*** Succeed to un zip data Zip size[" + (packSize - sizeof(uint)).ToString() + "], scr size[" + scrSize.ToString() + "]");
                        }
                        */
                        Log("*** Succeed to un zip data Zip size[" + (packSize - sizeof(uint)).ToString() + "], scr size[" + scrSize.ToString() + "]");
                    }
                    else
                    {
                        Log("XXX >>> zip data is error, so small");
                        return false;
                    }
                    mDatabuffer.seek(0);
                    OnReceive(ref mDatabuffer, 0);
                    continue;
#else
                    OnError("Error: now can not use zip net packet.");
                    throw new Exception("Error: now can not use zip net packet.");
#endif
                }
                else 
                {
                    if (packSize>mDatabuffer.size())
                        mDatabuffer._resize((int)packSize);

                    if (!mRevBuffer.Peek(ref mDatabuffer.mData, 0, (int)packSize))
                    {
                        Log(" *** XXX read receive data fail. packet data can not read");
                        return false;
                    }
                    if (!mRevBuffer.Skip((int)packSize))
                    {
                        Log(" *** XXX move receive data buffer fail. for packet data");
                        return false;
                    }
                    
                    mDatabuffer.seek(0);
                    OnReceive(ref mDatabuffer, 0);
                    continue;
                }

                break;
            }                     

            return true;
        }

        public override void OnConnect()
        {
            Log("*** Net connect succeed. Ok *****************************");

            if (mConnectEvent != null)
                mConnectEvent.Finish();

            if (mNotifyConnectFinishEvent != null)
            {
                mNotifyConnectFinishEvent.DoEvent();
                object obj = mNotifyConnectFinishEvent.get("NETAUTOFINISH");
                if (obj != null && obj.GetType() == typeof(bool))
                {
                    if ((bool)obj)
                    {
                        Log("Info: connect finish event set auto finish");
                        mNotifyConnectFinishEvent.Finish();
                        return;
                    }
                }
            }
            else
            {
                Log("not set connect finish event------------------");
            }
        }

        public override void OnClose()
        {
            if (mConnectEvent != null)
                mConnectEvent.Finish();

            if (mEventCenter != null)
            {
                if (mCloseEventName != "")
                {
                    tEvent evt = mEventCenter.StartEvent(mCloseEventName);
                    if (evt != null)
                    {
                        evt.set("IP", GetIP());
                        evt.set("PORT", GetPort());
                        evt.DoEvent();
                    }
                    else
                        EventCenter.Log(LOG_LEVEL.ERROR, "No register event >" + mCloseEventName);
                }
            }
            Log("XXX Connect close ===================");
        }

        public override void OnConnectFail()
        {
            Log("***XXX Net connect fail. ***************");
            if (mConnectEvent != null)            
                mConnectEvent.Finish();
        }

        public override void OnError(string errorInfo)
        {
            Log("XXX Net Error: " + errorInfo);
            if (mEventCenter != null)
            {
                tEvent evt = mEventCenter.StartEvent("NET_ERROR", false);
                if (evt != null)
                {
                    evt.set("ERROR", errorInfo);
                    evt.DoEvent();
                }
            }
        }

    }
    //---------------------------------------------------------------------------------------------------------
    // use TcpClient connect to server
    public class TcpClientNet : BaseNetTool
    {
        static public float WAIT_CONNECT_OVER_TIME = 10.0f;
        //static private ManualResetEvent connectDone = new ManualResetEvent(false);


        TcpClient mTcpClient;
        NetworkStream mNetStream;
       

        protected IPAddress mIP = null;
        protected int mPort;  
        bool mIsOk = false;

        //-------------------------------------------------------------------------

        public TcpClientNet()
        {
            mTcpClient = new TcpClient();
            mTcpClient.NoDelay = true;
            mTcpClient.ReceiveBufferSize = 1024 * 8;
            mTcpClient.ReceiveTimeout = 20;
            mTcpClient.SendBufferSize = 1024 * 8;
            mTcpClient.SendTimeout = 20;
           
        }	

        public override string GetIP(){ return mIP.ToString(); }
        public override int GetPort(){ return mPort; }

        public override void InitClear()
        {
            base.InitClear();

            if (mTcpClient != null)
                mTcpClient.Close();
            mTcpClient = null;
            SetOk(false);
        }
		
		public override void SetOk(bool bOk)
		{
			bool bRunConnect = false;
			if (mIsOk!=bOk && bOk)
				bRunConnect = true;
			
			mIsOk = bOk;
			if (bRunConnect)
				OnConnect();
		}
		
		public override bool IsOk(){ return mIsOk; }

        void __ReadySocket()
        {          
            //connectDone.Reset();
            SetOk(false);
			
            InitClear();

			if (mTcpClient==null)
            {
              

                mTcpClient = new TcpClient();
                mTcpClient.NoDelay = true;
                mTcpClient.ReceiveBufferSize = 1024 * 8;
                mTcpClient.ReceiveTimeout = 20;
                mTcpClient.SendBufferSize = 1024 * 8;
                mTcpClient.SendTimeout = 20;
               
            }
        }

        public override void Close(bool bRunCloseEvent)
        {
            SetOk(false);
            if (mTcpClient != null)
                mTcpClient.Close();

            InitClear();

            if (bRunCloseEvent)
            {
                OnClose();
            }
            else
            {
                Log("=========== Net close and not need run close event. ============");
            }
        }
		
		public override void ReConnect ()
		{
			_ReConnect();
			
			//throw new NotImplementedException ();
		}

        public void _ReConnect()
        {
            if (null == mIP)
            {
                Log("ip or url is error");
                return;
            }
            __ReadySocket();
         
            try
            {
                mTcpClient.BeginConnect(mIP, mPort,
                    new AsyncCallback(ConnectCallback), mTcpClient);
            }
            catch (Exception e)
            {
                Log(e.ToString());
                Console.WriteLine(e.ToString());
            }

           
        }


        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                TcpClient client = (TcpClient)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                //Console.WriteLine("Socket connected to {0}",
                //    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                //connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public override bool Connect(string strIp, int port)
        {
            Log("Begin connect to ip >" + strIp + ">Port>" + port.ToString());

            mPort = port;
            mIP = IPAddress.Parse(strIp);
            //IPEndPoint mIPInfo = new IPEndPoint(mIP, port);

            _ReConnect();

            return true;
        }

        public override bool ConnectDNS(string urlDNS, int port)
        {

            mPort = port;
            IPAddress[] ipInfo = Dns.GetHostAddresses(urlDNS);
            if (ipInfo.Length <= 0)
                return false;

            mIP = ipInfo[0];

            _ReConnect();

            return true;
        }


        public override void Process(float delayTime)
        {
            if (IsOk())
            {
                base.Process(delayTime);
            }
            else if (mTcpClient.Available > 0 || mTcpClient.Connected)
            {
                if (StaticDefine.debug)
                {
                    mNetStream = mTcpClient.GetStream();

                    int safeCode = StaticDefine.StringID("DJXGame");
                    if (tryWrite(BitConverter.GetBytes(safeCode), 0, 4) == 4)
                    {
                        Log("send check code succeed");
                        SetOk(true);
                    }
                    else
                        Log("NET_ERROR: send check code fail");


                }
                else
                {
                    try
                    {
                        mNetStream = mTcpClient.GetStream();

                        int safeCode = StaticDefine.StringID("DJXGame");
                        if (tryWrite(BitConverter.GetBytes(safeCode), 0, 4) == 4)
                        {
                            Log("send check code succeed");
                            SetOk(true);
                        }
                        else
                            Log("NET_ERROR: send check code fail");


                    }
                    catch (Exception e)
                    {
                        OnError("Error: get stream >>> " + e.ToString());
                        OnConnectFail();
                    }
                }
            }
        }

        // Noto: this is not same of NetTool. read is receive and write is send
        public override int tryWrite(byte[] data, int beginPos, int count)
        {
            if (count < 0)
            {
                Log(" *** XXX Code error. write net data count is lower zero");
                return 0;
            }
            if (mNetStream.CanWrite)
            {
                try
                {
                    mNetStream.Write(data, beginPos, count);
                    return count;
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                    // close event
                    SetOk(false);
                    OnClose();
                    //Close();
                }
            }

            return 0;
        }

        // receive
        public override int tryRead(ref byte[] data, int beginPos, int count)
        {
            if (count < 0)
            {
                Log(" *** XXX Code error. read net data count is lower zero");
                return 0;
            }
            int x = 0;

            try
            {
                if (mNetStream != null && mNetStream.DataAvailable)
                    x = mNetStream.Read(data, beginPos, count);
            }
            catch (Exception e)
            {
                Logic.EventCenter.Self.Log(e.ToString());
                SetOk(false);
                OnClose();
            }
            return x;

        }

        public override int available() 
        {
            if (mTcpClient!=null)
                return mTcpClient.Available;

            return 0;
        }
    }
    //---------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------------
    ////class NetTool2 : tNetTool
    ////{
    ////    static public int                   CONFIG_PACKHEAD_LENGTH = 6;
    ////    static public int                   ONCE_RECEIVE_PACKET_COUNT = 10;
    ////    static private ManualResetEvent     connectDone = new ManualResetEvent(false);

    ////    protected Socket                    mSocket;
    ////    protected LoopDataBuffer            mSendBuffer = new LoopDataBuffer(10240);
    ////    protected LoopDataBuffer            mRevBuffer = new LoopDataBuffer(10240);
 
    ////    protected tNetTool                  mNetTool;
    ////    public bool		                    mIsOk = false;       
    ////    public tEventCenter                 mEventCenter;

    ////    protected IPAddress                 mIP = null;
    ////    protected int                       mPort;

    ////    protected DataBuffer                mDatabuffer = new DataBuffer(2048);
    ////    protected DataBuffer                mZipBuffer = new DataBuffer(2048);

    ////    public tEvent                       mConnectEvent;
    ////    //-----------------------------------------------------------------------------
    ////    public NetTool2()
    ////    {            
    ////        mNetTool = this;						
    ////    }

    ////    void InitClear()
    ////    {
    ////        if (mSocket != null)
    ////        {                               
    ////            mSocket.Close();
    ////            mSocket = null;
    ////        }  
    ////        mSendBuffer.clear();
    ////        mRevBuffer.clear();
    ////        mIsOk = false;
    ////    }

    ////    public void Log(string info) { if (null != mEventCenter) mEventCenter.Log(info); }
    ////    public override void SetEventCenter(tEventCenter eventCenter){ mEventCenter = eventCenter; }

    ////    public override void SetConnectEvent(tEvent evt)
    ////    {
    ////        if (mConnectEvent != null)
    ////            mConnectEvent.Finish();

    ////        mConnectEvent = evt;
    ////    }

    ////    public void _ReConnect()
    ////    {
    ////        if (null==mIP)
    ////        {
    ////            Log("ip or url is error");
    ////            return;
    ////        }
    ////        __ReadySocket();           
      
    ////        IPEndPoint mIPInfo = new IPEndPoint(mIP, mPort);
             
    ////        try{
    ////            //mSocket.Connect(mIPInfo);
    ////            mSocket.BeginConnect(mIPInfo,
    ////                new AsyncCallback(ConnectCallback), mSocket);           
    ////         }
    ////        catch (Exception e) 
    ////        {
    ////            Log(e.ToString());
    ////            Console.WriteLine(e.ToString());
    ////        }
           
    ////    }

    ////    void __ReadySocket()
    ////    {          
    ////        connectDone.Reset();
    ////        SetOk(false);
    ////        InitClear();

    ////        mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    ////        //mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 20);
    ////        //mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 20);
    ////        mSocket.SendBufferSize = 1024 * 8;
    ////        mSocket.SendTimeout = 20;
    ////        mSocket.ReceiveTimeout = 20;
    ////        mSocket.ReceiveBufferSize = 1024 * 8;
    ////        mSocket.NoDelay = true;
    ////        mSocket.Blocking = true;

    ////    }

    ////    public override bool Connect(string strIp, int port)
    ////    {
    ////        mPort = port;
    ////        mIP = IPAddress.Parse(strIp);

    ////        _ReConnect();

    ////        return true;
    ////    }
		
    ////    public override bool ConnectDNS(string urlDNS, int port)
    ////    {                   
    ////        mPort = port;
    ////        IPAddress[] ipInfo = Dns.GetHostAddresses(urlDNS);
    ////        if (ipInfo.Length<=0)
    ////            return false;

    ////        mIP = ipInfo[0];

    ////        _ReConnect();

    ////        return true;
    ////    }
		
    ////    private static void ConnectCallback(IAsyncResult ar)
    ////    {
    ////        try
    ////        {
    ////            // Retrieve the socket from the state object.
    ////            Socket client = (Socket)ar.AsyncState;

    ////            // Complete the connection.
    ////            client.EndConnect(ar);

    ////            Console.WriteLine("Socket connected to {0}",
    ////                client.RemoteEndPoint.ToString());

    ////            // Signal that the connection has been made.
    ////            connectDone.Set();
    ////        }
    ////        catch (Exception e)
    ////        {                
    ////            Console.WriteLine(e.ToString());
    ////        }
    ////    }

    ////    public override void SetOk(bool bOk) 
    ////    {
    ////        if (mConnectEvent != null)
    ////            mConnectEvent.Finish();
            
    ////        mConnectEvent = null;

    ////        bool bSuccend = (bOk && !mIsOk);
    ////        mIsOk = bOk; 
    ////        // if before run OnConnect(), may be send event fail On event.because check mIsOk is false.
    ////        if (bSuccend)				
    ////            OnConnect(); 
    ////    }
    ////    public override bool IsOk() { return mIsOk; }

    ////    public override bool Send(tEvent evt, int target)
    ////    {
    ////        if (!mIsOk)
    ////            return false;
    ////        //DataBuffer data = new DataBuffer(128);
    ////        mDatabuffer.seek(0);

    ////        mDatabuffer.seek(CONFIG_PACKHEAD_LENGTH);
    ////        if (!evt.serialize(ref mDatabuffer))
    ////            return false;

    ////        int count = mDatabuffer.tell();
    ////        mDatabuffer.seek(0);
    ////        ushort id = 0;
    ////        mDatabuffer.write(id);
    ////        mDatabuffer.write(count - CONFIG_PACKHEAD_LENGTH);  // <--save data size
    ////        return mSendBuffer.write(mDatabuffer.getData(), 0, count); //<--save data to send buffer
    ////    }
    ////    public override bool OnReceive(ref DataBuffer data, int target)
    ////    {
    ////        tEvent evt = null;
    ////        try
    ////        {
    ////             evt = mEventCenter.RestoreEvent(ref data, 0, 0);
    ////        }
    ////        catch(Exception e)
    ////        {
    ////            Log("Error: restore packet data fail. may be event restore error. >>> \r\n"+e.ToString());
    ////        }
    ////        if (null != evt)
    ////        {
    ////             try{
    ////                 evt.Log("Begin run net message event...");
    ////                bool b = evt.DoEvent();
    ////                evt.Log("Run net event " + (b ? "Succeed " : "Fail "));
    ////             }
    ////             catch (Exception e)
    ////             {
    ////                Log("Error: run net event fail. >>> "+ evt.GetEventName() +">>>" + e.ToString());
    ////             }                    
    ////        }

    ////        return false;
    ////    }

    ////    public override void Process(float delayTime)
    ////    {
    ////        //step 1 receive data.
    ////        int len = mRevBuffer.processWrite(ref mNetTool);


    ////        // step 2 send data
    ////        if (mIsOk)
    ////        {
    ////            // send data.                
    ////            mSendBuffer.processRead(ref mNetTool);               
    ////        }
    ////        else if (len > 0)
    ////        {                
    ////            SetOk (true);
    ////        }

    ////        // step 3 run net message.
    ////        // because may ben close net in process.
    ////        try
    ////        {
    ////            _ReceivePacket();
    ////        }
    ////        catch (Exception e)
    ////        {
    ////            Log("Error: _ReceivePacket() run fail.>>>" + e.ToString());
    ////        }
    ////    }

    ////    public override int tryRead(byte[] data, int beginPos, int count)
    ////    {
    ////        try {
    ////            return mSocket.Send(data, beginPos, count, SocketFlags.None);
    ////        }
    ////        catch(Exception e)
    ////        {
    ////            // may be connect close                
    ////            //OnClose();
    ////            Logic.EventCenter.Self.Log (e.ToString());
    ////            //SetOk(false);
    ////            OnError("Error: net write buffer fail. may be net already closed.");
    ////            return 0;
    ////        }
    ////        return 0;
    ////    }
		
    ////    // receive
    ////    public override int tryWrite(ref byte[] data, int beginPos, int count)
    ////    {
    ////        if (mSocket.Available != 0)
    ////        {
    ////            int x = 0;
    ////            try
    ////            {
    ////                x = mSocket.Receive(data, beginPos, Math.Min(count, mSocket.Available), SocketFlags.None);
    ////            }
    ////            catch (System.ObjectDisposedException e)
    ////            {
    ////                Log("Error: Net already close");
    ////                OnClose();
    ////            }
    ////            catch (Exception e)
    ////            {
    ////                //string text = " receive data size >>>";
    ////                //text += x.ToString();
    ////                Logic.EventCenter.Self.Log(e.ToString());
    ////            }
    ////            return x;
    ////        }
    ////        return 0;
    ////    }

    ////    public bool _ReceivePacket()
    ////    {
    ////        int receiveCount = 0;
    ////        while (receiveCount++ < ONCE_RECEIVE_PACKET_COUNT && mRevBuffer.getDataSize() >= CONFIG_PACKHEAD_LENGTH)
    ////        {
    ////            LoopDataBuffer.POS pos = mRevBuffer.getPos();
    ////            mRevBuffer.setThrowInfo("Net 's LoopBuffer error");
    ////            byte[] d;
    ////            mRevBuffer.read(out d, 2);
    ////            //when use biffult restore packet.
    ////            ushort id = BitConverter.ToUInt16(d, 0);

    ////            mRevBuffer.read(out d, 4);

    ////            uint indexAndSize = BitConverter.ToUInt32(d, 0);
    ////            uint packSize = indexAndSize & 0xffffff;

    ////            if (packSize <= 0)
    ////                throw new Exception("Error: have packet size is zero, net error, please must close net.");

    ////            Log("get packet");
    ////            mRevBuffer.setThrowInfo("Packet data error.");

    ////            if (id == (ushort)NET_PACKET_ID.NET_COMPRESS_EVENT)
    ////            {
    ////                if (mRevBuffer.read(ref mZipBuffer, (int)packSize))
    ////                {
    ////                    // un compress
    ////                    if (packSize > sizeof(uint))
    ////                    {
    ////                        mZipBuffer.seek((int)packSize - sizeof(uint));

    ////                        uint scrSize = 0;
    ////                        mZipBuffer.read(out scrSize);

    ////                        if (mDatabuffer.size() < scrSize)
    ////                            mDatabuffer._resize((int)scrSize);

    ////                        if (!MyTest.ZipTool.RestoreZipData(mZipBuffer.getData(), 0, (int)packSize - sizeof(uint), ref mDatabuffer.mData, (int)scrSize))
    ////                        {
    ////                            Log("XXX >>> un compress zip data fail.");
    ////                            return false;
    ////                        }
    ////                        else
    ////                        {
    ////                            Log("*** Succeed to un zip data Zip size[" + (packSize - sizeof(uint)).ToString() + "], scr size[" + scrSize.ToString() + "]");
    ////                        }
    ////                    }
    ////                    else
    ////                    {
    ////                        Log("XXX >>> zip data is error, so small");
    ////                        return false;
    ////                    }
    ////                    mDatabuffer.seek(0);
    ////                    OnReceive(ref mDatabuffer, 0);
    ////                    continue;
    ////                }
    ////            }
    ////            else
    ////            {
    ////                if (mRevBuffer.read(ref mDatabuffer, (int)packSize))
    ////                {
    ////                    mDatabuffer.seek(0);
    ////                    OnReceive(ref mDatabuffer, 0);
    ////                    continue;
    ////                }
    ////            }

    ////            mRevBuffer.restore(pos);
    ////            break;

    ////        }
    ////        return false;
    ////    }

    ////    public override void Close()
    ////    {
    ////        OnClose();
    ////        InitClear();
    ////    }

    ////    public override void ReConnect()
    ////    {
    ////        _ReConnect();
    ////    }

    ////    public override void OnConnect()
    ////    {
    ////        Log("*** Net connect succeed. Ok *****************************");
    ////        if (mEventCenter != null)
    ////            mEventCenter.OnNetConnect();

    ////        OnError("Ok Connect Succeed");
    ////    }

    ////    public override void OnClose()
    ////    {
    ////        if (mEventCenter != null)
    ////            mEventCenter.OnNetClose();
    ////        Log("XXX Connect close ===================");
    ////    }

    ////    public override void OnConnectFail()
    ////    {
    ////        Log("***XXX Net connect fail. ***************");
    ////        if (mEventCenter != null)
    ////            mEventCenter.OnConnectFail();
    ////    }

    ////    public override void OnError(string errorInfo)
    ////    {
    ////        Log("XXX Net Error: " + errorInfo);
    ////        if (mEventCenter != null)
    ////        {
    ////            tEvent evt = mEventCenter.StartEvent("NET_ERROR", false);
    ////            if (evt != null)
    ////            {
    ////                evt.set("ERROR", errorInfo);
    ////                evt.DoEvent();
    ////            }
    ////        }
    ////    }
    ////}
    //---------------------------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------------------------

    ////public class LoopDataBuffer
    ////{
    ////    public struct POS
    ////    {
    ////        public int mHead;
    ////        public int mTail;
    ////    }

    ////    static public int DefineSize = 10240;
    ////    protected byte[] mData;
    ////    protected int mHead = 0;
    ////    protected int mTail = 0;
    ////    protected string mThrowInfo = null;

    ////    //-------------------------------------------------------------------------------------
    ////    public bool empty() { return mHead == mTail;  }

    ////    public void setThrowInfo(string strInfo) { mThrowInfo = strInfo; }

    ////    public void processRead(ref tNetTool tryUser)
    ////    {
    ////        if (mHead == mTail)
    ////            return;
    ////        else if (mHead < mTail)
    ////        {
    ////            int len = _tryRead(ref tryUser, mHead, mTail);
    ////            mHead += len;
    ////        }
    ////        else
    ////        {
    ////            int len = _tryRead(ref tryUser, mHead, mData.Length);
    ////            if (mData.Length - mHead == len)
    ////            {
    ////                len = _tryRead(ref tryUser, 0, mTail);
    ////                mHead = len;
    ////            }
    ////            else
    ////                mHead += len;
    ////        }
    ////    }

    ////    protected int _tryRead(ref tNetTool tryUser, int beginPos, int endPos)
    ////    {
    ////        if (endPos - beginPos <= 0)
    ////            return 0;
    ////        int len = tryUser.tryRead(mData, beginPos, endPos - beginPos);
    ////        if (len > 0)
    ////        {
    ////            if (len > endPos - beginPos)
    ////            {
    ////                throw new Exception("Error: tryUser readed size over buffer data size.");
    ////            }
    ////        }
    ////        else if (len < 0)
    ////        {
    ////            throw new Exception("Error: tryUser readed size is negative.");
    ////        }
    ////        return len;
    ////    }

    ////            // try write
    ////    public int processWrite(ref tNetTool tryUser)
    ////    {
    ////        int len = 0;
    ////        if (mHead==mTail)
    ////        {
    ////            len = _tryWrite(ref tryUser, 0, mData.Length);
    ////            mHead = 0;
    ////            mTail = len;            	
    ////        }
    ////        else if (mHead < mTail)
    ////        {
    ////            len = _tryWrite(ref tryUser, mTail, mData.Length);
    ////            if (len==mData.Length-mTail && mHead>0)
    ////            {
    ////                len = _tryWrite(ref tryUser, 0, mHead-1);
    ////                mTail = len;
    ////            }
    ////            else
    ////                mTail += len;
    ////        }
    ////        else if (mHead-mTail>1)
    ////        {
    ////            len = _tryWrite(ref tryUser, mTail, mHead-1);
    ////            mTail += len;
    ////        }

    ////        return len;
    ////        // if free size is empty then resize. now not to do.
    ////    }
    ////     protected int _tryWrite(ref tNetTool tryUser, int beginPos, int endPos)
    ////    {
    ////        if (endPos - beginPos <= 0)
    ////            return 0;
    ////        int len = tryUser.tryWrite(ref mData, beginPos, endPos - beginPos);
    ////        if (len>0)
    ////        {
    ////            if (len>endPos-beginPos)
    ////            {
    ////                throw new Exception("Error: tryUser readed size over buffer data size.");
    ////            }
    ////        }
    ////        else if (len < 0)
    ////        {
    ////            throw new Exception("Error: tryUser readed size is negative.");
    ////        }
    ////        return len;    
    ////    }

    ////    public POS getPos()
    ////    {
    ////        POS pos = new POS();
    ////        pos.mHead = mHead;
    ////        pos.mTail = mTail;
    ////        return pos;
    ////    }
    ////    // write or read , then use this function can restore data begin pos
    ////    public void restore(POS pos)
    ////    {
    ////        mHead = pos.mHead;
    ////        mTail = pos.mTail;
    ////    }

    ////    public LoopDataBuffer(int size)
    ////    {
    ////        if (size <= 0)
    ////            size = DefineSize;
    ////        resize(size);
    ////    }

    ////    bool resize(int size)
    ////    {
    ////        if (null == mData)
    ////            mData = new byte[size];
    ////        else if (size > mData.Length)
    ////        {
    ////            byte[] temp = new byte[size];

    ////            if (mHead == 0)
    ////                mData.CopyTo(temp, 0);
    ////            else if (mHead < mTail)
    ////            {
    ////                //copy data to  zero pos
    ////                int count = mTail - mHead;
    ////                for (int i = 0; i < count; ++i)
    ////                {
    ////                    temp[i] = mData[mHead + i];
    ////                }
    ////                mTail = mTail - mHead;
    ////                mHead = 0;
    ////            }
    ////            else
    ////            {
    ////                //把尾部的放到开始,头部的连接在后面
    ////                int count = mData.Length - mHead;
    ////                int i = 0;
    ////                for (; i < count; ++i)
    ////                {
    ////                    temp[i] = mData[mHead + i];
    ////                }
    ////                int pos = i;
    ////                for (i = 0; i < mTail; ++i)
    ////                {
    ////                    temp[pos + i] = mData[i];
    ////                }
    ////                mTail = pos + i;
    ////                mHead = 0;
    ////                //int off = size - mData.Length;
    ////                //int last = mData.Length - mHead;
    ////                //for (int i = 0; i < last; ++i)
    ////                //{
    ////                //    temp[mHead + off + i] = mData[mHead + i];
    ////                //}
    ////                //mHead += off;
    ////            }
    ////            mData = temp;
    ////        }
    ////        return true;
    ////    }

    ////    public bool write(byte[] data)
    ////    {
    ////        return write(data, 0, data.Length);
    ////    }

    ////    public bool write(byte[] data, int beginPos)
    ////    {
    ////        return write(data, beginPos, data.Length - beginPos);
    ////    }

    ////    enum STATE
    ////    {
    ////        EMPTY,          
    ////        A_BLOCK,        
    ////        TWO_BLOCK,      
    ////    }

    ////    //protected STATE _getState()
    ////    //{
    ////    //    if (mHead == mTail)
    ////    //        return STATE.EMPTY;
    ////    //    else if (mTail > mHead)
    ////    //        return STATE.A_BLOCK;
    ////    //    else
    ////    //        return STATE.TWO_BLOCK;
    ////    //}

    ////    public int getDataSize()
    ////    {
    ////        if (mHead == mTail)
    ////            return 0;
    ////        else if (mTail > mHead)
    ////            return mTail - mHead;
    ////        else
    ////            return mData.Length - mHead + mTail;
    ////    }

    ////    public int getFreeSize()
    ////    {
    ////        if (mHead == mTail)
    ////            return mData.Length;
    ////        else if (mHead > mTail)
    ////            return mHead - mTail - 1;
    ////        else
    ////            return mData.Length - (mTail - mHead) - 1;
    ////    }


    ////    public bool write(byte[] data, int beginPos, int count)
    ////    {
    ////        if (beginPos + count > data.Length)
    ////            return false;

    ////        if (mHead == mTail)
    ////        {
    ////            //EMPTY
    ////            if (mData.Length < count)
    ////                resize(count);

    ////            if (beginPos == 0)
    ////                data.CopyTo(mData, 0);
    ////            else
    ////            {
    ////                for (int i = 0; i < count; ++i)
    ////                {
    ////                    mData[i] = data[beginPos + i];
    ////                }
    ////            }
    ////            mHead = 0;
    ////            mTail = count;
    ////            return true;
    ////        }
    ////        else if (mTail > mHead)
    ////        {
    ////            // A_BLOCK
    ////            if (mData.Length - mTail >= count)
    ////            {
    ////                // 可以容下全部
    ////                for (int i = 0; i < count; ++i)
    ////                {
    ////                    mData[mTail++] = data[beginPos++];
    ////                }
    ////                return true;
    ////            }
    ////            else if ((mData.Length - mTail) + mHead > count)
    ////            {
    ////                // 尾部空间 + 头之前的空间可以容下
    ////                int free = mData.Length - mTail;
    ////                int i = 0;
    ////                for (; i < free; ++i)
    ////                {
    ////                    mData[mTail++] = data[beginPos++];
    ////                }
    ////                mTail = 0;
    ////                for (; i < count; ++i)
    ////                {
    ////                    mData[mTail++] = data[beginPos++];
    ////                }
    ////                return true;
    ////            }
    ////            else
    ////            {
    ////                int needSize = count - ((mData.Length - mTail) + mHead);
    ////                if (resize(mData.Length + needSize))
    ////                    return write(data, beginPos, count);
    ////            }

    ////        }
    ////        else
    ////        {
    ////            //TWO_BLOCK
    ////            // between at free block
    ////            if (mHead - mTail > count)
    ////            {
    ////                for (int i = 0; i < count; ++i)
    ////                {
    ////                    mData[mTail++] = data[beginPos++];
    ////                }
    ////                return true;
    ////            }
    ////            else
    ////            {
    ////                int needSize = count - (mHead - mTail);
    ////                if (resize(mData.Length + needSize))
    ////                    return write(data, beginPos, count);
    ////            }
    ////        }
    ////        if (null != mThrowInfo)
    ////        {
    ////            throw new Exception(mThrowInfo);
    ////        }
    ////        return false;
    ////    }
		
		
    ////    public bool read(ref DataBuffer buf, int count)
    ////    {	
    ////        if (buf.size()<count)
    ////            buf._resize(count);
    ////        byte[] d;
    ////        if (read(out d, count))
    ////        {
    ////            buf.seek(0);
    ////            buf.write(d);
    ////            //buf = new DataBuffer();
    ////            //buf._setData(d);
    ////            return true;
    ////        }
    ////        buf = null;
    ////        return false;
    ////    }

    ////    public bool read(out byte[] dest, int count)
    ////    {
    ////        dest = null;
    ////        if (mTail == mHead)
    ////            return false;

    ////        if (mTail > mHead)
    ////        {
    ////            // 如果数据块在中间
    ////            if ((mTail - mHead) >= count)
    ////            {
    ////                dest = new byte[count];
    ////                for (int i = 0; i < count; ++i)
    ////                {
    ////                    dest[i] = mData[mHead++];
    ////                    //mData[mHead - 1] = 0;
    ////                }

    ////                return true;
    ////            }
    ////        }
    ////        else if ((mData.Length - mHead) + mTail >= count)
    ////        {
    ////            // 数据在两端
    ////            // 先读尾部
    ////            if (mData.Length - mHead >= count)
    ////            {
    ////                dest = new byte[count];
    ////                for (int i = 0; i < count; ++i)
    ////                {
    ////                    dest[i] = mData[mHead++];
    ////                    //mData[mHead - 1] = 0;
    ////                }

    ////                return true;
    ////            }
    ////            else
    ////            {
    ////                dest = new byte[count];
    ////                int last = mData.Length - mHead;
    ////                int i = 0;
    ////                for (; i < last; ++i)
    ////                {
    ////                    dest[i] = mData[mHead++];
    ////                    //mData[mHead - 1] = 0;
    ////                }
    ////                mHead = 0;
    ////                for (; i < count; ++i)
    ////                {
    ////                    dest[i] = mData[mHead++];
    ////                    //mData[mHead - 1] = 0;
    ////                }

    ////                return true;
    ////            }

    ////        }
    ////        if (null != mThrowInfo)
    ////        {
    ////            throw new Exception(mThrowInfo);
    ////        }
    ////        return false;
    ////    }

    ////    public void clear()
    ////    {
    ////        mHead = 0;
    ////        mTail = 0;
    ////    }
    ////}

    //---------------------------------------------------------------------------------------------------------



    public class OutputBuffer
    {
        static public int MAX_DATA_BUFFER_SIZE = 1024 * 128;

	    byte[]		mData;
	
	    public int m_Head = 0;
	    public int m_Tail = 0;
	
	    //---------------------------------------------------------------------
	
	    public OutputBuffer(int initSize)
	    {
		    mData = new byte[initSize];
	    }
	
	    public int DataSize()
	    {
		    if (m_Head<m_Tail)
			    return m_Tail - m_Head;
		else if (m_Head>m_Tail)
		{
			return mData.Length-m_Head + m_Tail;
		}
		
		return 0;
        }
	
	public int FreeSize()
	{
        if (m_Head <= m_Tail)
            return mData.Length - m_Tail + m_Head - 1;
        else
            return m_Head - m_Tail -1;
	}
	
	public bool Write(byte[] data, int beginPos, int size)
	{
        if (data.Length<beginPos+size)
            return false;

        int freeSize = FreeSize();
		if (size>=FreeSize())
        {
            if (!Resize(size - freeSize + 1))
                return false;
        }

        if (m_Head<=m_Tail)
        {
            // data at middle
            if (m_Head==0)
            {
                freeSize = mData.Length - m_Tail -1;
                Array.Copy( data, beginPos, mData, m_Tail, size );
            }
            else
            {
                freeSize = mData.Length - m_Tail;
                if (size <= freeSize)
                    Array.Copy(data, beginPos, mData, m_Tail, size);
                else
                {
                    Array.Copy(data, beginPos, mData, m_Tail, freeSize);
                    Array.Copy(data, beginPos+freeSize, mData, 0, size-freeSize);
                }
            }
        }
        else
        {
            // data at two side.
            Array.Copy(data, beginPos, mData, m_Tail, size);
        }

        m_Tail = (m_Tail+size) % mData.Length;

        return true;
	}
	

    public int _NetSend(tNetTool tNet)
    {

        int nFlushed = 0;
        int nSend = 0;
        int nLeft = 0;

        if (m_Head < m_Tail)
        {
            nLeft = m_Tail - m_Head;

            while (nLeft > 0)
            {
                nSend = tNet.tryWrite(mData, m_Head, nLeft);
                if (nSend<=0)
                    return 0;
                nFlushed += nSend;
                nLeft -= nSend;
                m_Head += nSend;                
            }

            if (nLeft!=0)
                EventCenter.Self.Log(" *** OutputBuffer is error.");
                
        }
        else if (m_Head > m_Tail)
        {
            nLeft = mData.Length - m_Head;

            while (nLeft>0)
            {
                nSend = tNet.tryWrite(mData, m_Head, nLeft);
                if (nSend<=0)
                    return 0;

                nFlushed += nSend;
                nLeft -= nSend;
                m_Head += nSend;
            }

            if (m_Head != mData.Length)
                EventCenter.Self.Log(" *** OutputBuffer is error.");

            m_Head = 0;

            while (nLeft >0)
            {
                nSend = tNet.tryWrite(mData, m_Head, nLeft);

                if (nSend<=0)
                    return 0;

                nFlushed += nSend;
                nLeft -= nSend;
                m_Head += nSend;
            }

            if (nLeft!=0)
                EventCenter.Self.Log(" *** OutputBuffer is error.");
            
        }

        m_Head = m_Tail = 0;

        return nFlushed;

     }

        bool Resize(int addSize)
        {
            if (addSize <= 0)
                return true;

            int newSize = mData.Length + addSize;

            if (newSize>MAX_DATA_BUFFER_SIZE)
            {
                EventCenter.Self.Log(" *** XXX WARN: resize send buffer over max size.(1024*128) >>> "+newSize.ToString());
            }

            int dataSize = DataSize();

            byte[] newData = new byte[newSize];

            if (m_Head < m_Tail)
            {
                Array.Copy(mData, m_Head, newData, 0, m_Tail-m_Head);
            }
            else if (m_Head>m_Tail)
            {
                Array.Copy(mData, m_Head, newData, 0, mData.Length-m_Head);
                Array.Copy(mData, m_Tail, newData, mData.Length-m_Head, m_Tail);
            }

            mData = newData;

            m_Head = 0;
            m_Tail = dataSize;

            return true;

        }

        public void Clear()
        {
            m_Head = 0;
            m_Tail = 0;
        }

    }


    // for net receive net data

    public class InputBuffer
    {
        int m_Head;
        int m_Tail;

        byte[]    mData;

        int Size { get{return mData.Length; } }

        public InputBuffer( int initSize )
        {
            mData = new byte[initSize];

            m_Head = 0;
            m_Tail = 0;
        }

        public int DataSize()
        {
            if (m_Head<m_Tail)
                return m_Tail-m_Head;

            else if (m_Head>m_Tail)
            {
                return mData.Length - m_Head+m_Tail;
            }

            return 0;
        }

        public bool Peek(ref byte[] resultData, int indexPosition, int getDataSize)
        {
            if (resultData.Length < indexPosition + getDataSize)
                return false;

            if (getDataSize <= 0)
                return true;

            if (getDataSize > DataSize())
                return false;

            if (m_Head < m_Tail)
                Array.Copy(mData, m_Head, resultData, indexPosition, getDataSize);
            else
            {
                int rightLen = mData.Length - m_Head;
                if (getDataSize <= rightLen)
                    Array.Copy(mData, m_Head, resultData, indexPosition, getDataSize);
                else
                {
                    Array.Copy(mData, m_Head, resultData, indexPosition, rightLen);
                    Array.Copy(mData, 0, resultData, indexPosition + rightLen, getDataSize - rightLen);
                }
            }

            return true;
        }

        public bool Skip(int len)
        {
            if (len <= 0)
                return true;

            if (len > DataSize())
                return false;

            m_Head = (m_Head + len) % mData.Length;

            return true;
        }


        public int FillOnNet(tNetTool tNet)
        {
            int nFilled = 0;
            int nReceived = 0;
            int nFree = 0;

            if (m_Head <= m_Tail)
            {
                if (m_Head == 0)
                {
                    nReceived = 0;
                    nFree = mData.Length - m_Tail - 1;

                    if (nFree != 0)
                    {
                        nReceived = tNet.tryRead(ref mData, m_Tail, nFree);
                        if (nReceived <= 0)
                            return nFilled;

                        m_Tail += nReceived;
                        nFilled += nReceived;
                    }

                    if (nReceived == nFree)
                    {
                        int available = tNet.available();
                        if (available > 0)
                        {
                            if (!Resize(available + 1))
                            {
                                EventCenter.Self.Log(" XXX resize receive data buffer size fail");
                                return nFilled;
                            }

                            nReceived = tNet.tryRead(ref mData, m_Tail, available);

                            if (nReceived <= 0)
                                return nFilled;

                            m_Tail += nReceived;
                            nFilled += nReceived;
                        }
                    }
                }
                else
                {
                    //    H   T		LEN=10
			        // 0123456789
			        // ...abcd...
                    nFree = mData.Length - m_Tail;
                    nReceived = tNet.tryRead(ref mData, m_Tail, nFree);
                    if (nReceived <= 0)
                        return 0;

                    m_Tail = (m_Tail + nReceived) % mData.Length;
                    nFilled += nReceived;

                    if (nReceived == nFree)
                    {
                        nReceived = 0;
                        nFree = m_Head - 1;
                        if (nFree != 0)
                        {
                            nReceived = tNet.tryRead(ref mData, 0, nFree);
                            if (nReceived <= 0)
                                return nFilled;

                            m_Tail += nReceived;
                            nFilled += nReceived;
                        }

                        if (nReceived == nFree)
                        {
                            int available = tNet.available();
                            if (available > 0)
                            {
                                if (!Resize(available + 1))
                                    return nFilled;

                                nReceived = tNet.tryRead(ref mData, m_Tail, available);

                                if (nReceived <= 0)
                                    return nFilled;

                                m_Tail += nReceived;
                                nFilled += nReceived;
                            }
                        }
                    }
                }
            }
            else
            {
                //     T  H		LEN=10
                // 0123456789
                // abcd...efg

                nReceived = 0;
                nFree = m_Head - m_Tail - 1;
                if (nFree != 0)
                {
                    nReceived = tNet.tryRead(ref mData, m_Tail, nFree);
                    if (nReceived <= 0)
                        return nFilled;

                    m_Tail += nReceived;
                    nFilled += nReceived;
                }

                if (nReceived == nFree)
                {
                    int available = tNet.available();
                    if (available > 0)
                    {
                        if (!Resize(available + 1))
                            return nFilled;

                        nReceived = tNet.tryRead(ref mData, m_Tail, available);
                        if (nReceived<=0)
                            return nFilled;

                        m_Tail += nReceived;
                        nFilled += nReceived;
                    }
                }
            }

            return nFilled;
        }


        bool Resize(int size)
        {
            if (size <= 0)
                return true;

            int newSize = mData.Length + size;

            if (newSize > OutputBuffer.MAX_DATA_BUFFER_SIZE)
            {
                EventCenter.Self.Log(" *** XXX WARN: resize send buffer over max size.(1024*128) >>> " + newSize.ToString());
            }

            int dataSize = DataSize();

            byte[] newData = new byte[newSize];

            if (m_Head < m_Tail)
            {
                Array.Copy(mData, m_Head, newData, 0, m_Tail - m_Head);

            }
            else if (m_Head > m_Tail)
            {
                Array.Copy(mData, m_Head, newData, 0, mData.Length - m_Head);
                Array.Copy(mData, 0, newData, mData.Length - m_Head, m_Tail);
            }

            mData = newData;

            m_Head = 0;
            m_Tail = dataSize;

            return true;

        }

        public void Clear()
        {
            m_Head = 0;
            m_Tail = 0;
        }

        public bool empty()
        {
            return m_Head == m_Tail;
        }
    }

    
}
//---------------------------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------------------------