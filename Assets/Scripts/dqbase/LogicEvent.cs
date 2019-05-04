
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public enum LOG_LEVEL
{
    GENERAL,
    WARN,
    ERROR,
    HIGH,
}

public enum SEND_RESULT
{
    eSendSucceed,
    eEventSerializeFail,
    eNetDisconnect,
    eWriteSendDataFail,
}

namespace Logic
{
    class GameTimeManager : Logic.TimeEventManager
    {
        protected float mLastTime = Time.time;

        public override float GetNowTime()
        {
            return Time.time;
        }
    }

    public abstract class tEvent
    {
        //-----------------------------------
        public abstract bool _DoEvent();
        public abstract bool _OnEvent(tEvent other);
        public abstract bool _OnBegin();
        public abstract bool _OnFinish();
        public virtual void _OnOverTime() { }
        public virtual void OnSendFail(SEND_RESULT errorResult){}
        public virtual void CallBack(object caller) { }
        //-----------------------------------
        public abstract bool Begin();
       // public abstract void Wait();
        public abstract bool WaitTime(float waitTime);
        public abstract void StartUpdate();
        public abstract bool Send(int nTarget);
		public abstract bool Send();

        public abstract void SetEventFactory(tEventFactory fact);
        public abstract tEventFactory GetEventFactory();
        public abstract tEventCenter GetEventCenter();

        //-----------------------------------
        public abstract string GetEventName();
        public abstract string GetEventInfo();

        public abstract bool DoEvent();
        public abstract bool OnEvent(ref tEvent evt);
        public abstract void Finish();
        public abstract void DoOverTime();
        public abstract void SetFinished(bool bFinish);
        public abstract bool GetFinished();
        public abstract bool Update(float secondTime); // return true, then auto stop update
        public abstract bool needLowUpdate();

        public abstract void Log( string info );
        public abstract void Dump();
        //------------------------------------
        public abstract void setData(tNiceData data);
        public abstract tNiceData getData();

		public abstract Data get(string index);

        public abstract void set(string index, object val);
        public abstract object getObject(string index);
        public abstract bool getData(string index, out object value);
        public abstract bool get(string index, out int nVal);
        public abstract bool get(string index, out float fVal);
        public abstract bool get(string index, out string strVal);
		public abstract bool get(string index, out UInt64 val);
		public abstract bool get(string index, out tNiceData val);

        public abstract DataTable.NiceTable getTable(string index);
        public abstract NiceData getNice(string index);
        public abstract DataBuffer getData(string index);

        //-----------------------------------
        public abstract bool serialize(ref DataBuffer resultData);
        public abstract bool restore(ref DataBuffer scrData);
		//-----------------------------------
		public abstract tEvent StartEvent(string evtName);

        public Data this[string indexName]
        {
            //实现索引器的get方法
            get
            {
                return get(indexName);
            }
            //实现索引器的set方法
            set
            {
                set(indexName, value.mObj);
            }
        }
    }



    public abstract class BaseEvent : tEvent
    {
        protected tEventFactory    mFactory;
        protected bool             mFinish = false;
        public      uint           mCode;      //for debug
        static protected uint      sNum = 0;
        //-----------------------------------
        //public abstract bool _DoEvent();
        //public abstract bool _OnEvent(ref tEvent other);
        //public abstract bool _OnBegin();
        //public abstract bool _OnFinish();
		public override bool Send(){ return mFactory.SendEvent(this); }
		
		public override bool _OnEvent(tEvent other){ return true; }
        public override bool _OnBegin(){ return true; }
        public override bool _OnFinish(){ return true; }       
        public virtual void _NodifyEventFinish(){}
        public override bool needLowUpdate() { return false; }

        public BaseEvent()
        {
            mCode = ++sNum;
        }
        //-----------------------------------
        public override bool Begin()
        {
            Log("begin");
            return _OnBegin();
        }
        public override bool WaitTime(float waitTime)
        {
			GetEventCenter().OnEventWaitTime(this, waitTime);
            return true;
        }

        public override void StartUpdate()
        {
            GetEventCenter().StartUpdate(this);
        }

        public override bool Send(int nTarget)
        {
            bool b = GetEventCenter().SendEvent(this, nTarget);
            if (b)
                Log("send succeed V.");
            else
                Log("send fail X.");
            return b;
        }

        public override void SetEventFactory(tEventFactory fact)
        {
            mFactory = fact;
        }
        public override tEventFactory GetEventFactory()
        {
            return mFactory;
        }
        public override tEventCenter GetEventCenter()
        {
			if (mFactory!=null)
            	return mFactory.GetEventCenter();
			return EventCenter.Self;
        }

        //-----------------------------------
        public override string GetEventName()
        {
            return mFactory.GetEventName();
        }
        public override string GetEventInfo()
        {
            string info = GetEventName();
            info += "_";
            info += mCode;
            return info;
        }       

        public override bool DoEvent()
        {
			//getData().dump();
            //Log("begin do ...");			
            bool b = false;
            if (StaticDefine.debug)
            {
                b = _DoEvent();
            }
            else
            {
                try
                {
                    b = _DoEvent();
                }
                catch (Exception e)
                {
                    Log("Error: do event fail >>> " + e.ToString());
                    b = false;
                }
            }
            if (b)
                Log("succeed do.");
            else
                Log("do fail.");
            //Log("do finish");
            return b;
        }
        public override bool OnEvent(ref tEvent evt)
        {
            string info = evt.GetEventInfo();
            info += " come on ";
            Log(info);
            bool b = _OnEvent(evt);
            Log(" on event finish");
            return b;
        }
        public override void Finish()
        {
            if (!mFinish)
            {
				mFinish = true;
                _OnFinish();
                _NodifyEventFinish();
                //Log("event finish");
            }
        }

        public override void DoOverTime()
        {
            /// Log("time over ...");
            _OnOverTime();
        }

        public override void SetFinished(bool bFinish)
        {
            mFinish = bFinish;
        }
        public override bool GetFinished()
        {
            return mFinish;
        }
		
		public override tEvent  StartEvent(string evtName)
		{
			return EventCenter.Self.StartEvent(evtName, true);
		}
		
        public override void Log(string info)
        {
            if (GetEventFactory().NeedLog())
                GetEventCenter().Log("["+GetEventInfo()+"] "+info);
        }
        public override void Dump()
        {
            Log("event data ...");
            getData().dump();
        }
        //------------------------------------
        public override void set(string index, object val){ getData().set(index, val); }

        public override Data get(string index) { return getData().get(index); }
        public override object getObject(string index) { object re=null; getData(index, out re); return re; }
        public override bool getData(string index, out object value){ return getData().getData(index, out value); }
        public override bool get(string index, out int nVal){ return getData().get(index, out nVal); }
        public override bool get(string index, out float fVal){ return getData().get(index, out fVal); }
        public override bool get(string index, out string strVal){ return getData().get(index, out strVal);}
		public override bool get(string index, out UInt64 val){ return getData().get (index, out val); }

        public override bool get(string index, out tNiceData val) { return getData().get(index, out val); }

        public override DataTable.NiceTable getTable(string index) { return getData().getObject(index) as DataTable.NiceTable;  }
        public override NiceData getNice(string index) { return getData().getObject(index) as NiceData; }
        public override DataBuffer getData(string index) { return getData().getObject(index) as DataBuffer; }

        //-----------------------------------
        public override bool serialize(ref DataBuffer resultData)
        {
            if (resultData.writeTwo(GetEventName()))
            {
                int target = 1;
                resultData.write(target);

                return getData().serialize(ref resultData);                            
            }
            return false;
        }


        public override bool restore(ref DataBuffer scrData)
        {
            int target;
            if (!scrData.read(out target))
                return false;

            return getData().restore(ref scrData);            
        }

    }

    public abstract class CEvent : BaseEvent
    {
        protected tNiceData mData = new NiceData();

        public override void setData(tNiceData data) { mData = data; }
        public override tNiceData getData() { return mData; }
		public override bool _DoEvent()
        {
            Action action = (Action)getObject("ACTION");
            if (action != null)
            {
                action();
                action = null;
            }

            return false;
        }
        public override bool Update(float secondTime) { return false; }
    }



    public abstract class tEventFactory
    {
        public abstract tEvent NewEvent();

        public abstract void SetEventName(string name);
        public abstract string GetEventName();

        public abstract void SetEventCenter(tEventCenter center);
        public abstract tEventCenter GetEventCenter();

        public abstract string GetNextLogic();

        public virtual bool NeedLog() { return true; }

        public abstract bool SendEvent(tEvent evt);

        public virtual void _OnEvent(tEvent evt){}
        public virtual void _OnWaitEvent(tEvent evt){}
        public virtual void _RemoveWaitEvent(UInt64 evtID){}
    }



    public abstract class tEventCenter
    {        
        public abstract tEvent StartEvent(string eventName, bool bWarn);
        public abstract tEvent StartEvent(int eventID, bool bWarn);

        public abstract bool SendEvent(tEvent evt, int target);        
        public abstract tEvent RestoreEvent(ref DataBuffer scrData, int indexPos, int target);

        public abstract tEvent StartEvent(string eventName);
        public abstract void RegisterEvent(string eventName, tEventFactory fact);
		public abstract bool RemoveRegisterEvent(string evntname);
		public abstract bool RemoveRegisterEvent(int eventID);
		
        public abstract tEventFactory GetEventFactory(int nameID);
        public abstract tEventFactory GetResponseEventFactory();
		
		public abstract void Process(object param);

        public abstract void Log( string info );

        public abstract tEvent StartDefaultEvent(string eventName);
		
		public abstract void OnEventWaitTime(tEvent evt, float waitTime);

        public abstract void StartUpdate(tEvent evt);

        public abstract void OnReceiveEvent(tEvent evt);
        public abstract void OnSendEvent(tEvent evt);
    }

    public abstract class BaseFactory : tEventFactory
    {
        public string mName;
        public tEventCenter mEventCenter;

        public BaseFactory()
        {
            
        }

        public override void SetEventName(string name){ mName = name; }
        public override string  GetEventName(){ return mName; }

        public override void SetEventCenter(tEventCenter center){ mEventCenter = center;}
        public override tEventCenter GetEventCenter(){ return mEventCenter; }

        public override string GetNextLogic() { return ""; }
        //public override tEvent NewEvent(){ return new CEvent(); }

        public override bool SendEvent(tEvent evt)
        {
            return evt.Send(0);
        }
    }

    class DefineFactory<T> : BaseFactory      
        where T : tEvent, new()
    {
        public override tEvent NewEvent() { return new T(); }

        public override bool NeedLog()
        {
            return false;
        }
    }

    class DefineFactoryLog<T> : BaseFactory
    where T : tEvent, new()
    {
        public override tEvent NewEvent() { return new T(); }
    }

    public class DefaultEvent : CEvent
    {
        public override bool _DoEvent() { return true; }
        public override bool _OnEvent(tEvent other) { return true; }
        public override bool _OnBegin() { return true; }
        public override bool _OnFinish() { return true; }
    }
    class DefaultEventFactory : BaseFactory
    {
        public override tEvent NewEvent() { return new DefaultEvent(); }
    }

	public abstract class  tLog
	{
		public abstract void Log(string info);
	}
    //-------------------------------------------------------------------------

    public class TM_WaitToRunAction : BaseEvent
    {
        public object mObject;
        public System.Reflection.MethodInfo mFunMethod;
        public object mParam;

        public override void _OnOverTime()
        {
            object[] p = new object[1];
            p[0] = mParam;
			try{
            	mFunMethod.Invoke(mObject, p);
			}
			catch (Exception e)
			{
				LOG.logError("Run action > "+e.ToString());
			}
        }

		public override bool _DoEvent(){ return true; }
		public override bool Update(float space){ return false; }
        public override void setData(tNiceData data) { }
        public override tNiceData getData() { return null; }
    }
    //-------------------------------------------------------------------------

    public class TM_WaitUpdateAction : BaseEvent
    {
        public float mNowTime = 0;
        public int mRunCount = 1;
        public float mSpaceTime = 1;
        public object mObject;
        public System.Reflection.MethodInfo mFunMethod;
        public object mParam;

        public override bool needLowUpdate()
        {
            return false;
        }

        public override bool _DoEvent()
        {
            mNowTime = EventCenter.Self.mTimeEventManager.GetNowTime();
            StartUpdate();

			return true;
        }

        public override bool Update(float t)
        {
            if (EventCenter.Self.mTimeEventManager.GetNowTime() - mNowTime > mRunCount * mSpaceTime)
            {
                ++mRunCount;
                object[] p = new object[1];
                p[0] = mParam;
                try
                {
                    if (!(bool)mFunMethod.Invoke(mObject, p))
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    LOG.logError("Run action > " + e.ToString());
                    return false;
                }
            }
            return true;
        }
        public override void setData(tNiceData data) { }
        public override tNiceData getData() { return null; }
    }
    //-------------------------------------------------------------------------
	public class WindowEventFactory : BaseFactory
    {
        public object mWindowObject;
        public System.Reflection.MethodInfo mFunMethod;

        public WindowEventFactory(object win, string funName)
        {
            mWindowObject = win;
            mFunMethod = win.GetType().GetMethod(funName);
        }

        public override tEvent NewEvent()
        {
            return new WindowEvent();
        }
    }

    public class WindowEvent : CEvent
    {
        public override bool _DoEvent()
        {
            WindowEventFactory f = GetEventFactory() as WindowEventFactory;

            if (f.mFunMethod == null)
            {
                LOG.logError("Window function no exist");
                return false;
            }

            object[] p = new object[1];
            p[0] = getData();
            try
            {
				f.mFunMethod.Invoke(f.mWindowObject, p);
                return true;
            }
            catch (Exception e)
            {
                LOG.logError("Run action > " + e.ToString());
                return false;
            }

            return true;
        }
    }
    //-------------------------------------------------------------------------
    //-------------------------------------------------------------------------

    public class EventCenter : tEventCenter
    {
        static public EventCenter Self = null;
		
		public TimeEventManager								mTimeEventManager;
        tEventFactory                                       mResponseFactory;
        //static public EventCenter                           sEventCenter = new EventCenter();
        public tNetTool                                     mNetTool = null;
        public Dictionary<int, tEventFactory>               mFactoryMap = new Dictionary<int,tEventFactory>();
		protected tLog								        mLog = null;
        protected int                                       mNetID = -1;

        
#if CHECK_FOR_3G
        NetCheckFor3G                                       mNetCheckFor3G;      
#endif
        uint mNowSecond;
        ushort mTimePeriod = 0;
        ushort mCode = 0;
        //-------------------------------------------------------------------------------------------
        public EventCenter()
        {    
            // Register connect finish notify event
            RegisterEvent("NodifyClientConnectOk", new DefineFactory<SC_NodifyClientConnectOk>());

            RegisterEvent("NET_TryConnectEventOnIp", new DefineFactory<NET_TryConnectEventOnIp>());
            RegisterEvent("NET_TryConnectEventOnDNS", new DefineFactory<NET_TryConnectEventOnDNS>());

            mResponseFactory = new ResponsionFactory();
            RegisterEvent(ResponsionEvent.RESP_EVENT_NAME, mResponseFactory);

            RegisterEvent("TM_WaitToRunAction", new DefineFactory<TM_WaitToRunAction>());
            RegisterEvent("TM_WaitUpdateAction", new DefineFactory<TM_WaitUpdateAction>());
           
        }

        static public void Register(string eventName, tEventFactory factory)
        {
            Self.RegisterEvent(eventName, factory);
        }

        public void RemoveAllEvent()
        {
            mTimeEventManager.RemoveAll();

        }

		public void InitSetTimeManager(TimeEventManager timeManager)
        { 
            mTimeEventManager = timeManager; 
            mNowSecond = (uint)(mTimeEventManager.GetNowTime()*1000);
        }
        public void SetLog(tLog log) { mLog = log; }
        public tLog GetLog() { return mLog;  }

        static public void Log(bool bError, string info) {  if (bError) info = "XXX Error:"+info; Self.Log(info); }
		static public void Log(LOG_LEVEL level, string info)
		{
			if (Self == null)			
				return;

			switch (level)
			{
			case LOG_LEVEL.GENERAL:
				Self.Log("INFO: "+info);
				break;

			case LOG_LEVEL.WARN:
				Self.Log("WARN: "+info);
				break;

			case LOG_LEVEL.ERROR:
				Self.Log("ERROR: "+info);
				break;

			case LOG_LEVEL.HIGH:
				Self.Log("HIGH: "+info);
				break;

			default:
				Self.Log (info);
				break;
			}
		}
        static public tEvent Start(string eventTypeName)
        {
            return Self.StartEvent(eventTypeName);
        }
        
        static public tEvent WaitAction(object obj, string fun, object param, float waitSecond)
        {
            System.Reflection.MethodInfo info = obj.GetType().GetMethod(fun);
            if (info == null)
                return null;

			TM_WaitToRunAction waitEvt = Start("TM_WaitToRunAction") as TM_WaitToRunAction;
            if (waitEvt != null)
            {                
				waitEvt.mObject = obj;
				waitEvt.mFunMethod = info;
				waitEvt.mParam = param;
                waitEvt.WaitTime(waitSecond);
                return waitEvt;
            }
            return null;
        }

        static public tEvent WaitUpdate(object obj, string fun, object param, float waitSecond)
        {
            System.Reflection.MethodInfo info = obj.GetType().GetMethod(fun);
            if (info == null)
			{
				LOG.logError("Not find fun >"+fun);
                return null;
			}
			TM_WaitUpdateAction waitEvt = Start("TM_WaitUpdateAction") as TM_WaitUpdateAction;
            if (waitEvt != null)
            {
				waitEvt.mSpaceTime = waitSecond;
                waitEvt.mObject = obj;
                waitEvt.mFunMethod = info;
                waitEvt.mParam = param;
                waitEvt.DoEvent();
                return waitEvt;
            }
            return null;
        }
        //---------------------------------------------------------------------
        // for net
        public void SetNetTool(tNetTool netTool)
        {
            mNetTool = netTool;		
			netTool.SetEventCenter(this);
        }

        public void SetNetID(int netID) 
		{
			mNetID = netID; 
			//mNetTool.SetOk(mNetID>=0); 
		}
		
		public void SetNetOk(bool bOk)
        {
            mNetTool.SetOk(true);
        }


        public bool InitNet(string strIp, int port, bool bDNS, tEvent connectSucceedEvent, string closeEventName, string connectFailEvent, int tryCount, float overTime)
        {            
            mNetID = -1;
            if (mNetTool != null)
            {
                mNetTool.Close(false);
                mNetTool.SetOk(false);
            }

            TcpClientNet net = new TcpClientNet();
            net.mCloseEventName = closeEventName;
            net.mNotifyConnectFinishEvent = connectSucceedEvent;

            mNetTool = net;
            mNetTool.SetEventCenter(this);
            

            NET_TryConnectEventOnIp connectEvt;
            if (bDNS)
                connectEvt = (NET_TryConnectEventOnIp)StartEvent("NET_TryConnectEventOnDNS");
            else
                connectEvt = (NET_TryConnectEventOnIp)StartEvent("NET_TryConnectEventOnIp");

            if (connectEvt == null)
            {
                Log("XXX Error: start connect event is null. check [NET_TryConnectEventOnIp] *********");
                throw new Exception("XXX Error: start connect event is null. check [NET_TryConnectEventOnIp]");
            }

            connectEvt.mNet = mNetTool;
            connectEvt.mIp = strIp;
            connectEvt.mPort = port;
            connectEvt.mTryCount = tryCount;
            connectEvt.mOverTime = overTime;
            connectEvt.mConnectFailEventName = connectFailEvent;
            connectEvt.DoEvent();
            
            return true;
        }
		

#if CHECK_FOR_3G
        public void InitSetNetCheckFor3G(NetCheckFor3G netCheck)
        {

            mNetCheckFor3G = netCheck;

        }
#endif
        public override void OnReceiveEvent(tEvent evt)
        {
#if CHECK_FOR_3G
            if (mNetCheckFor3G != null)
                mNetCheckFor3G.OnReceiveEvent(evt);
#endif
        }
        public override void OnSendEvent(tEvent evt)
        {
#if CHECK_FOR_3G
            if (mNetCheckFor3G != null)
            {
                mNetCheckFor3G.OnSendEvent(evt);
            }
#endif
        }
        //-----------------------------------------------------------------------------
        public override tEvent StartEvent(string eventName)
        {
            return StartEvent(eventName, true);
        }
        public override tEvent StartEvent(string eventName, bool bWarn)
        {
            tEvent evt = StartEvent ( StaticDefine.StringID(eventName), bWarn );
            if (bWarn && null == evt)
            {
                string info = "Warning: no exist [";
                info += eventName;
                info += "] event factory.";
                Log(info);
            }
            return evt;
        }
        public override tEvent StartEvent(int eventID, bool bWarn)
        {
            tEventFactory fact = GetEventFactory( eventID );
            if (null!=fact)
            {
                tEvent evt = fact.NewEvent();
                evt.SetEventFactory(fact);
                return evt;
            }
			if (bWarn)
			{
            	string info = "Warning: no exist [";
            	info += eventID;
            	info += "] event factory.";
            	Log(info);
			}
            return null;
        }

        public override bool SendEvent(tEvent evt, int target)
        {
            if (null!=mNetTool)
            {       
                return mNetTool.Send(evt, target);
            }
            return false;
        }
        public override tEvent RestoreEvent(ref DataBuffer data, int indexPos, int target)
        {
            string name;
            if (!data.readTwo(out name))
			{
                string info = "XXX >>> Fail: RestoreEvent event at restore name. may be coding error. ";				
				Log (info);
                return null;
			}
			string info2 = "get event ";
			info2 += name;
			Log (info2);
			
            tEvent evt = StartEvent(name, true);
            if (null!=evt)		
			{
                if (evt.restore(ref data))
                    return evt;
				else
					Log ("Fail: restore event " + name);
			}
            else 
            {
                Log("event [" + name + "] failed to start!! ");
            }
            
            return null;
        }

        public override void RegisterEvent(string eventName, tEventFactory fact)
        {
            fact.SetEventName(eventName);
            fact.SetEventCenter(this);
            int id = StaticDefine.StringID(eventName);
            if (RemoveRegisterEvent(id))
                Log("Warn: event alread exist :" + eventName);

            mFactoryMap.Add(id, fact);
        }
		
		public override bool RemoveRegisterEvent(string eventname)
		{
			return mFactoryMap.Remove(StaticDefine.StringID(eventname));
		}
		public override bool RemoveRegisterEvent(int eventID)
		{
			return mFactoryMap.Remove(eventID);
		}
		
        public override tEventFactory GetEventFactory(int eventID)
        {
            tEventFactory fact;
            mFactoryMap.TryGetValue(eventID, out fact);
            return fact;
        }

        public override tEventFactory GetResponseEventFactory()
        {
            return mResponseFactory;
        }
		
		public override void Process(object param)
		{
			if (null!=mNetTool)
			{
				try{
				    mNetTool.Process((float)param);
				}
				catch(Exception e)
				{
					Log (e.ToString());

					if(StaticDefine.debug)
						throw e;
				}
			}
			if (null!=mTimeEventManager)
                mTimeEventManager.Update((float)param);
		}

        public override void Log( string info ) 
		{
			if (null!=mLog)
				mLog.Log(info);	
		}

        public override tEvent StartDefaultEvent(string eventName)
        {
            //int id = StaticDefine.StringID( eventName );
            //if (GetEventFactory(id) == null)
            //{

            //}
            tEvent evt = StartEvent(eventName, false);
			if (null==evt)
			{
				RegisterEvent(eventName, new DefaultEventFactory());
				evt = StartEvent(eventName, false);
				if (null==evt)
					throw new Exception("Error: already register default event, but can not create event >>"+eventName);
			}
			return evt;
        }
		
		public override void OnEventWaitTime(tEvent evt, float waitTime)
		{
			mTimeEventManager.OnWaitEvent(evt, waitTime);
		}

        public override void StartUpdate(tEvent evt)
        {
            mTimeEventManager.StartUpdate(evt);
        }

        

        public UInt64 AlloctEventID()
        {
            uint nowTime = (uint)(mTimeEventManager.GetNowTime() * 1000);
            if (nowTime<mNowSecond)
                ++mTimePeriod;

            mNowSecond = nowTime;

            ++mCode;

            UInt64 key = mNowSecond;
            uint m = mTimePeriod;
            key = (mNowSecond<<32) + (m<<16) + mCode;
            return key;
        }
    }
	
    // responsion event
    public class ResponsionEvent : CEvent
    {
        //static public string NEED_RESP_KEY = "__NeedResp";
        static public string RESP_ID_KEY = "RESP_ID";
        static public string RESP_KEY_TIME = "RESP_KEY_TIME";
        static public string RESP_EVENT_NAME = "ResponsionC2S";
       
		public override bool _DoEvent()
		{
			GetEventFactory()._OnEvent(this);

			return true;
		}

        public override void Dump()
		{
			string s = "From > [";
			s += (string)get("_SERVER_EVENT_");
			s += "]";
			Log( s );
		    base.Dump();
		}

        public override string GetEventInfo()
        {
            string info = base.GetEventInfo();
            info += "*";
            info += get("_SERVER_EVENT_");
            return info;
        }  
    }

    public class ResponsionFactory : BaseFactory
    {
        public Dictionary<UInt64, tEvent> mWaitEventList = new Dictionary<UInt64, tEvent>();

        public override void _OnEvent(tEvent evt) 
        {
            if (!(evt is ResponsionEvent))
            {
                EventCenter.Log(LOG_LEVEL.ERROR, "Not is resposion event > "+evt.GetEventName());
                return;
            }

			UInt64 id = 0;
			if (!evt.get(ResponsionEvent.RESP_ID_KEY, out id))
            {
                EventCenter.Log(LOG_LEVEL.ERROR, "Not set event id > " + evt.GetEventName());
                return;
            }

            tEvent waitEvt;
            if (mWaitEventList.TryGetValue(id, out waitEvt))
            {
                mWaitEventList.Remove(id);
                if (waitEvt != null)
                {
                    tServerEvent serverEvt = waitEvt as tServerEvent;
                    if (serverEvt == null)
                    {
                        string errorInfo = "Wait event is not SeverEvent >" + waitEvt.GetEventName();
                        EventCenter.Log(LOG_LEVEL.ERROR, errorInfo);
                        throw new Exception(errorInfo);
                        return;
                    }

                    if (serverEvt.GetFinished())
                        serverEvt.Log("Warn : Always finished when get responsion");
                    else if (serverEvt.GetEventID() != id)
                    {
                        string errorInfo = "Wait event id not is same in wait list >" + waitEvt.GetEventName();
                        EventCenter.Log(LOG_LEVEL.ERROR, errorInfo);
                        throw new Exception(errorInfo);
                        return;
                    }
                    else
                    {
                        serverEvt._OnEvent(evt);
                    }

                }
                else
                {
                    evt.Log("Warn: wait event always release");
                }
            }
            else
            {
                evt.Log(("Warn: not find wait server event"));
                evt.Dump();
            }
        }

        public override void _OnWaitEvent(tEvent evt)
        {
            tServerEvent serverEvt = evt as tServerEvent;
            if (serverEvt==null)
            {
                string errorInfo = "Wait event is not SeverEvent >" + evt.GetEventName();
                EventCenter.Log(LOG_LEVEL.ERROR, errorInfo);
                throw new Exception(errorInfo);
                return;
            }

            UInt64 id = serverEvt.GetEventID();
            if (id==0)
            {
                string errorInfo = "Server event not set event ID >" + evt.GetEventName();
                EventCenter.Log(LOG_LEVEL.ERROR, errorInfo);
                throw new Exception(errorInfo);
                return;
            }

            tEvent existEvt;
            if (mWaitEventList.TryGetValue(id, out existEvt))
            {
                if (existEvt == evt)
                    return;
                else
                {
                    string errorInfo = "Always exist wait server event >" + evt.GetEventName();
                    EventCenter.Log(LOG_LEVEL.ERROR, errorInfo);
                    throw new Exception(errorInfo);
                    return;
                }
            }

            mWaitEventList.Add(id, evt);
        }
        public override void _RemoveWaitEvent(UInt64 evtID) { mWaitEventList.Remove(evtID); }

        public override tEvent NewEvent()
        {
            return new ResponsionEvent();
        }
    }
    //-------------------------------------------------------------------------    
    // Net event
    public class DefaultNetEvent : CEvent
    {
        public object mSendData = null;

        public override bool _DoEvent() { return true; }
        public override bool _OnEvent(tEvent other) { return true; }
        public override bool _OnBegin() { return true; }
        public override bool _OnFinish() { return true; }

        public override bool serialize(ref DataBuffer resultData)
        {
            if (resultData.writeTwo(GetEventName()))
            {
                if (mSendData!=null)
                    getData().FullData(mSendData);

                int target = 1;
                resultData.write(target);

                return getData().serialize(ref resultData);                            
            }
            return false;
        }


        public override bool restore(ref DataBuffer scrData)
        {
            int target;
            scrData.read(out target);
            return getData().restore(ref scrData);
        }


    }

    //request event (or server event)
    public abstract class tServerEvent : DefaultNetEvent
    {
        float mWaitSecondTime = 3;

        DateTime mSendTime;
        DateTime mRespTime;

        public abstract void _OnResp(tEvent respEvent);
        public virtual void _Send()
        {
            if(!Send())
                OnSendFail(SEND_RESULT.eNetDisconnect);
        }

        public override bool DoEvent()
        {
            bool re = base.DoEvent();

            if (GetFinished())
                return re;

            RemoveFromWaitList();
            
            AlloctRespID();
            GetEventCenter().GetResponseEventFactory()._OnWaitEvent(this);

            _Send();

            if (StaticDefine.debug)
                mSendTime = DateTime.Now;

            base.WaitTime(mWaitSecondTime);

			return re;
        }

		public override bool WaitTime(float waitSecond){ mWaitSecondTime = waitSecond; return true; }

        public override bool _OnEvent(tEvent evt)
        {
            mRespTime = DateTime.Now;
            object e = this;
            evt.getData().RestoreToTarget(ref e);
            _OnResp(evt);

            ReleaseRespID();

            if (_NeedFinishWhenResponsed())
                Finish();

            return true;
        }

        public override void _NodifyEventFinish(){ RemoveFromWaitList(); }

        public UInt64 GetEventID()
        {
			UInt64 respID;
			get (ResponsionEvent.RESP_ID_KEY, out respID);

            return respID;
        }

        void ReleaseRespID()
        {

        }

        protected virtual void AlloctRespID()
        {
            ReleaseRespID();

            UInt64 id = EventCenter.Self.AlloctEventID();
            set(ResponsionEvent.RESP_ID_KEY, id);
            Log("Alloct event id >"+id.ToString());
        }

        public virtual bool _NeedFinishWhenResponsed() { return true; }

    	public void RemoveFromWaitList()
	    {
			 GetEventCenter().GetResponseEventFactory()._RemoveWaitEvent(GetEventID());		
	    }

        public override void Finish()
        {
            bool bAlwaysFinish = GetFinished();
            base.Finish();

            if (!bAlwaysFinish && StaticDefine.debug)
            {
                string info = "";
                if (mSendTime != null)
                {
					info += "Send [" + mSendTime.ToString("T") + "], ";
                    if (mRespTime != null)
                    {
                        var span = mRespTime - mSendTime;
						info += "Response [" + mRespTime.ToString("T");
                        info += "], Wait [" + span.TotalMilliseconds;
						info += "], ";
                    }
                    else
                        info += " not response when finish!";

                    info += " Total use [" + (DateTime.Now - mSendTime).TotalMilliseconds + "]milsecond";
                }
                else
                    info += "not send may be do not DoEvent!";
				info += ", finish [" + DateTime.Now.ToString("T") + "]";
				Log (info);
            }                
        }
    }
	
    // client event
    public abstract class tClientEvent : DefaultNetEvent
    {
        public override bool _DoEvent()
        {
            bool re = base._DoEvent();
            object d = get("WaitTime");
            if (null != d && d.GetType()==typeof(int))
            {
                int time = (int)d;
                WaitTime((float)d/1000f);
            }
            //else
            Finish();
            return true;
        }
        //        #define RESP_ID_KEY		"RESP_ID"
        //#define RESP_KEY_TIME	"RESP_KEY_TIME"
        //#define NEED_RESP_KEY	"__NeedResp"
        //#define RESP_EVENT_NAME "ResponsionC2S"


        public override bool _OnFinish()
        {
            ////base._OnFinish();
            //object b = get(ResponsionEvent.NEED_RESP_KEY);
            //// 回复服务器
            //if (b!=null && (bool)b)
            {
                object respID;
                //object time = get(RESP_KEY_TIME);
                //if (null == respID || null == time)
                if (!getData(ResponsionEvent.RESP_ID_KEY, out respID) || !(respID is UInt64))
                {
                    Log("Error; run event no exist data [RESP_ID]");
                    return false;
                }
                tEvent hResp = null;
                object resp = null;
                if (getData("_RESP_EVENT_", out resp))
                {
                    if (resp is tEvent)
                        hResp = (tEvent)resp;
                }
                
                if (hResp==null)
				    hResp = GetEventCenter().StartEvent(ResponsionEvent.RESP_EVENT_NAME);

                SetRespData(ref hResp);
                hResp.set(ResponsionEvent.RESP_ID_KEY, (UInt64)respID);
                if (StaticDefine.debug)
                    hResp.set("_SERVER_EVENT_", GetEventName());
                //hResp.set(RESP_KEY_TIME, (int)time);
                hResp.Send(0);
            }
            return true;
        }

        public abstract void SetRespData(ref tEvent evt);
		
        public tEvent GetRespEvent()
        {
            object resp;
            if (getData("_RESP_EVENT_", out resp))
            {
                if (resp is tEvent)
                    return (tEvent)resp;
            }

            tEvent respEvt = StartEvent(ResponsionEvent.RESP_EVENT_NAME);
            set("_RESP_EVENT_", respEvt);
            return respEvt;
        }

        public override bool restore(ref DataBuffer scrData)
        {
            int target;
            scrData.read(out target);
            if (getData().restore(ref scrData))
            {
                object e = this;
                return getData().RestoreToTarget(ref e);
            }
            return false;
        }

    }

    // use connect server event
    class SC_Check_C : tClientEvent
    {

        public override bool _DoEvent()
        {
			int id;
            if (get("ID", out id)) 
			{ 
                EventCenter.Self.SetNetID(id);
                Log("Info: connect succed, then wait server check");
			}
            else
			{
				Log ("Error: not set net [ID] at  server check net event ");
                //throw new Exception("Error:  not set net [ID] at  server check net event");
			}
            base._DoEvent();
            return true;
        }
        public override bool _OnEvent(tEvent other)
        {
            return true;
        }
        public override bool _OnBegin()
        {
            return true;
        }
        public override bool _OnFinish()
        {
            base._OnFinish();
            return true;
        }

        public override void SetRespData(ref tEvent evt)
        {
            int id = StaticDefine.StringID(GetEventName());
            evt.set("checkID", id);
        }
    }

    class SC_Check_C_Factory : BaseFactory
    {
        public SC_Check_C_Factory()
        {

        }
        public override tEvent NewEvent() { return new SC_Check_C(); }

    }


    class SC_NodifyClientConnectOk : CEvent
    {
        public override bool _DoEvent()
        {
            Log("OK >>> Server Respone Connect Ok Notify.");
            EventCenter.Self.SetNetOk(true);
            object id = get("ID");
            if (null != id)
                EventCenter.Self.SetNetID((int)id);
            {             
                Log("Finish OK > Connect succeed.");
                Log("*******************************************************************");
                //mIsOk = true;
#if    NEED_NET_SAFT_CHECK
                    int id = StaticDefine.StringID("CHECK");
                    mSendBuffer.write(BitConverter.GetBytes(id));
#endif
            }          

            return true;
        }
    }

    // for net try connect
    class NET_TryConnectEventOnIp : CEvent
    {
        public tNetTool     mNet;
        public  string      mIp;
        public  int         mPort;
        public float        mOverTime = 10.0f;
        public int          mTryCount = 6;
               
        protected int       mCurrentTryCount = 0;
        public string       mConnectFailEventName;
        //------------------------------------------------------------------
        public override bool _DoEvent()
        {
            mNet.SetConnectEvent(this);
            Log("*** Begin connect to [" + mIp + "]:[" + mPort.ToString() + "] *******************");
            mNet.Connect(mIp, mPort);
            if (NetProcess())
                Finish();
            else
                WaitTime(mOverTime);
            return true;
            //throw new NotImplementedException();
        }

        public override void _OnOverTime()
        {
            if (!mNet.IsOk())
            {
                if (++mCurrentTryCount >= mTryCount)
                {
                    Finish();
                    mNet.OnConnectFail();
                    mNet.OnError("XXXXXX Fail: connect to "+IpInfo());

                    if (mConnectFailEventName != "")
                    {
                        BaseNetTool net = mNet as BaseNetTool;
                        tEvent evt = net.mEventCenter.StartEvent(mConnectFailEventName);
                        if (evt != null)
                        {
                            evt.set("IP", mIp);
                            evt.set("PORT", mPort);
                            evt.DoEvent();
                        }
                        else
                            EventCenter.Log(LOG_LEVEL.ERROR, "No register event > " + mConnectFailEventName);
                    }

                    return;
                }
                mNet.OnError("XXX Try connect fail once time >>>" + mCurrentTryCount.ToString() + " to " +IpInfo() );
                Log("XXX Try [" + mCurrentTryCount.ToString() + "] fail, now try once time to connect >>> [" + mIp + "]:[" + mPort.ToString() + "] *******");               
                mNet.ReConnect();
                if (NetProcess())
                    Finish();
                else
                    WaitTime(mOverTime);
            }
            else
                Finish();
            //base._OnOverTime();
        }

        public bool NetProcess()
        {
            System.Threading.Thread.Sleep(20);
            mNet.Process(20);
            return mNet.IsOk();
        }

        public string IpInfo()
        {
            return "[" + mIp + "]:[" + mPort.ToString() + "]";
        }
    }

    class NET_TryConnectEventOnDNS : NET_TryConnectEventOnIp
    {
        public override bool _DoEvent()
        {
            mNet.SetConnectEvent(this);
            Log("*** Begin connect to [" + mIp + "]:[" + mPort.ToString() + "] *******************");
            mNet.ConnectDNS(mIp, mPort);
            if (NetProcess())
                Finish();
            else
                WaitTime(mOverTime);
            return true;
            //throw new NotImplementedException();
        }
    }


}