/********************************************************************
	created:	2012/11/02
	created:	2:11:2012   3:20
	filename: 	C:\NewGame\SkyLive\Assets\Script\Base\TimeEvent.cs
	file path:	C:\NewGame\SkyLive\Assets\Script\Base
	file base:	TimeEvent
	file ext:	cs
	author:		Wenge Yang
	
	purpose:	
*********************************************************************/
//#define  LOG_WAIT_TIME_EVENT_LIST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic;


namespace Logic
{
	public abstract class TimeEventManager
	{

        public struct WaitTimeEventData 
        {
            public tEvent mWaitEvent;
            public float mWaitTime;

            public WaitTimeEventData(float waitTime, tEvent waitEvt)
            {
                mWaitEvent = waitEvt;
                mWaitTime = waitTime;
            }
        }

        public abstract float GetNowTime();

        public List<WaitTimeEventData> mWaitEventList = new List<WaitTimeEventData>();
		public List<WaitTimeEventData> mWillWaitEventList = new List<WaitTimeEventData>();

        public List<tEvent> mWillUpdateList = new List<tEvent>();
        public List<tEvent> mUpdateList = new List<tEvent>();

        protected float mUpdateTime = 0;

        public void  RemoveAll()
        {
            mWaitEventList.Clear();
            mWillWaitEventList.Clear();
            mWillWaitEventList.Clear();
            mUpdateList.Clear();
        }

        public void OnWaitEvent(tEvent waitEvent, float timeSecond)
        {
            float overTime = GetNowTime()+timeSecond;
           
			mWillWaitEventList.Add(new WaitTimeEventData(overTime, waitEvent));
			
		}

        public void _AppendWillWaitList()
        {
            for (int n = 0; n < mWillWaitEventList.Count; ++n)
            {
                WaitTimeEventData waitData = mWillWaitEventList[n];
                // Note: Check event is exist when insert before, because wait time will change, so list is not sqrt state. change bad.
                // so must delete already exist this event.
                for (int i = 0; i < mWaitEventList.Count; ++i)
                {
                    if (mWaitEventList[i].mWaitEvent == waitData.mWaitEvent)
                    {
                       // waitData.mWaitEvent.Log(" !!!!!! remove because already exist");
                        mWaitEventList.RemoveAt(i);
                        break;
                    }
                }

                int count = mWaitEventList.Count;

                for (int nIndex = 0; nIndex < mWaitEventList.Count; ++nIndex)
                {
                    tEvent evt = mWaitEventList[nIndex].mWaitEvent;

                    if (mWaitEventList[nIndex].mWaitTime > waitData.mWaitTime)
                    {
                       // waitData.mWaitEvent.Log("+++++++ Insert to " + nIndex.ToString());
                        mWaitEventList.Insert(nIndex, waitData);

                        break;
                        //return;
                    }
                }
                if (count == mWaitEventList.Count)
                {
                   // waitData.mWaitEvent.Log("******* Insert to " + mWaitEventList.Count.ToString());
                    mWaitEventList.Insert(mWaitEventList.Count, waitData);
                }
#if LOG_WAIT_TIME_EVENT_LIST
                waitData.mWaitEvent.Log("---------------------------- wait time evnt list ------------------------------------");
                for (int i = 0; i < mWaitEventList.Count; ++i)
                {
                    mWaitEventList[i].mWaitEvent.Log("[" + i.ToString() + "] Wait >>>" + mWaitEventList[i].mWaitTime.ToString());
                }
                waitData.mWaitEvent.Log("---------------------------- end list ------------------------------------");
#endif
            }
            mWillWaitEventList.Clear();
        }

        public void StartUpdate(tEvent needUpdateEvent)
        {
            mWillUpdateList.Add(needUpdateEvent);
            //mWillUpdateList.Insert(mUpdateList.Count, needUpdateEvent);
        }

        virtual public void Update(float onceTime)
        {           
            bool bLowLoop = false;
            mUpdateTime += onceTime;
            if (mUpdateTime >= 0.1f)    // Once loop by 0.1 second
            {
                mUpdateTime -= 0.1f;
                bLowLoop = true;

                if (mWillWaitEventList.Count > 0)
                    _AppendWillWaitList();

                float nowTime = GetNowTime();
                for (int i = 0; i < mWaitEventList.Count; )
                {
                    tEvent evt = mWaitEventList[i].mWaitEvent;
                    if (evt.GetFinished())
                    {
                        ///evt.Log(" ------- remove from wait time list, because already finish.");
                        mWaitEventList.RemoveAt(i);
                    }
                    else
                    {
                        float overTime = mWaitEventList[i].mWaitTime;

                        if (overTime <= nowTime)
                        {
                            // Note : must remove before Do over time, because may be repeat wait time in over time process.
                            // then will remove this event, so after romove event is other event.
                            // So, use will wait time list to temp save append event.
                            mWaitEventList.RemoveAt(i);

                            ///evt.Log(" ======= Run over time ... >>>>>>>>>>>>>>>>>>>>>>>>>>");
                            evt.DoOverTime();

                        }
                        else
                        {
                            // this will exit loop.
                            break;
                            //++i;
                        }
                    }

                }
            }

            if (mWillUpdateList.Count>0)
            {
                foreach (tEvent evt in mWillUpdateList)
                {
                    mUpdateList.Remove(evt);
                    if (!mUpdateList.Contains(evt))
                        mUpdateList.Add(evt);
                }
                mWillUpdateList.Clear();
            }

            if (mUpdateList.Count>0)
            {
                for (int i = 0; i < mUpdateList.Count; )
                {
                    if (mUpdateList[i].GetFinished())
                    {
                        mUpdateList.RemoveAt(i);
                        continue;
                    }
                    else if(mUpdateList[i].needLowUpdate())
                    {
                        if (bLowLoop)
                        {
                            if (!mUpdateList[i].Update(0.1f))
                            {
                                mUpdateList.RemoveAt(i);
                                continue;
                            }
                        }
                    }
                    else if (!mUpdateList[i].Update(onceTime))
                    {
                        mUpdateList.RemoveAt(i);
                        continue;
                    }

                    ++i;
                }
            }

        }       
	}
}