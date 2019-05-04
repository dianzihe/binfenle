
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;



namespace DataTable
{
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------

    public abstract class tFieldInfo
    {
        public abstract void setName(string name);
        public abstract string getName();

        public abstract FIELD_TYPE getType();
        //public abstract string getTypeName();

        public abstract void setPosition(int nPos);
        public abstract int getPosition();

        public abstract bool set(ref object[] dataList, object obj);

        public abstract object get(object[] dataList);

        public abstract bool get(object[] dataList, out int nVal);
        public abstract bool get(object[] dataList, out float fVal);
        public abstract bool get(object[] dataList, out string strVal);
		public abstract bool get(object[] dataList, out UInt64 result) ;

        public abstract bool serialize(object[] dataList, ref DataBuffer resultData);
        public abstract bool restore(ref object[] dataList, ref DataBuffer scrData);

        public abstract bool serialize(object data, ref DataBuffer resultData);
        public abstract bool restore(out object data, ref DataBuffer scrData);

        public abstract int getIndex(object[] dataList);
        public abstract int MakeIndex(int nVal);
        public abstract int MakeIndex(string strVal);

        public abstract bool saveParam(ref DataBuffer resultData);
        public abstract bool restoreParam(ref DataBuffer scrData);
    };

    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------

    abstract class BaseFieldInfo : tFieldInfo
    {
        protected string mName = null;
        protected int mPosition = 0;
		
        //public override string getTypeName() { return StaticDefine.ToStringType(getType()); }

        public override void setName(string name) { mName = name; }
        public override string getName() { return mName; }

        public override void setPosition(int nPos) { mPosition = nPos; }
        public override int getPosition() { return mPosition; }

        public override bool serialize(object[] dataList, ref DataBuffer resultData)
        {
            return serialize(dataList[mPosition], ref resultData);
        }
        public override bool restore(ref object[] dataList, ref DataBuffer scrData)
        {
            return restore(out dataList[mPosition], ref scrData);
        }

        public override int getIndex(object[] dataList) 
        {
            Logic.EventCenter.Log(LOG_LEVEL.ERROR, "No Support this index support > "+StaticDefine.ToStringType(getType()) + " at " +  getName());
            return 0; 
        }
        public override int MakeIndex(int nVal) { return 0; }
        public override int MakeIndex(string strVal) { return 0; }

        public override bool get(object[] dataList, out UInt64 result) { result = 0; return false; }

        public override bool saveParam(ref DataBuffer resultData) { return true; }
        public override bool restoreParam(ref DataBuffer scrData) { return true; }
    }
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------

    class IntFieldInfo : BaseFieldInfo
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_INT; }
        //--------------------------------------------------------------------------------

        public override bool serialize(object data, ref DataBuffer resultData)
        {
            if (null == data)
            {
                resultData.write((int)0);
                return true;
            }
            resultData.write((int)data);
            return true;
        }
        //--------------------------------------------------------------------------------

        public override bool restore(out object data, ref DataBuffer scrData)
        {
            int val;
            if (scrData.read(out val))
            {
                data = val;
                return true;
            }
            data = 0;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool set(ref object[] dataList, object obj)
        {
            if (mPosition < dataList.Length)
            {
                if (obj.GetType() == typeof(int))
                {
                    dataList[mPosition] = obj;
                }
                else if (obj.GetType() == typeof(float))
                {
                    dataList[mPosition] = (int)obj;
                }
                else if (obj.GetType() == typeof(string))
                {
                    try
                    {
                        dataList[mPosition] = int.Parse((string)obj);
                    }
                    catch
                    {
                        dataList[mPosition] = 0;
                    }
                }
                else
                    return false;

                return true;
            }
            return false;
        }
        //--------------------------------------------------------------------------------

        public override object get(object[] dataList)
        {
            if (mPosition < dataList.Length)
            {
                return dataList[mPosition];
            }
            return null;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out int nVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                nVal = (int)dataList[mPosition];
                return true;
            }
            nVal = 0;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out float fVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                fVal = (int)dataList[mPosition];
                return true;
            }
            fVal = 0.0f;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out string strVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                strVal = dataList[mPosition].ToString();
                return true;
            }
            strVal = "";
            return false;
        }
        public override int getIndex(object[] dataList) 
        {
            return (int)dataList[mPosition]; 
        }
        public override int MakeIndex(int nVal) { return nVal; }
        public override int MakeIndex(string strVal) { return int.Parse(strVal); }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------


    abstract class tFieldInfoFactory
    {
        public abstract FIELD_TYPE getType();

        public abstract tFieldInfo NewFieldInfo();
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------

    class IntFieldInfoFactory : tFieldInfoFactory
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_INT; }

        public override tFieldInfo NewFieldInfo()
        {
            return new IntFieldInfo();
        }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------

    class FieldInfoManager
    {
        protected Dictionary<FIELD_TYPE, tFieldInfoFactory> mFieldInfoMap = new Dictionary<FIELD_TYPE, tFieldInfoFactory>();
        protected Dictionary<FIELD_TYPE, tFieldInfo> mFieldMap = new Dictionary<FIELD_TYPE, tFieldInfo>();

        //--------------------------------------------------------------------------------

        FieldInfoManager()
        {
            tFieldInfoFactory info = new IntFieldInfoFactory();
            _RegisterFieldInfo(info);
            info = new FloatFieldInfoFactory();
            _RegisterFieldInfo(info);
            info = new StringFieldInfoFactory();
            _RegisterFieldInfo(info);
            info = new BoolFieldInfoFactory();
            _RegisterFieldInfo(info); 
            info = new NiceDataFieldInfoFactory();
            _RegisterFieldInfo(info);
            info = new TableFieldInfoFactory();
            _RegisterFieldInfo(info);
            info = new DataFieldInfoFactory();
            _RegisterFieldInfo(info);
            
            info = new ByteFieldInfoFactory();
            _RegisterFieldInfo(info);
            info = new UINT64FieldInfoFactory();
            _RegisterFieldInfo(info);
        }
        //--------------------------------------------------------------------------------

        public void _RegisterFieldInfo(tFieldInfoFactory fieldInfoFactory)
        {
            mFieldInfoMap.Add(fieldInfoFactory.getType(), fieldInfoFactory);
            mFieldMap.Add(fieldInfoFactory.getType(), fieldInfoFactory.NewFieldInfo());
        }
        //--------------------------------------------------------------------------------

        static public void RegisterFieldInfo(tFieldInfoFactory fieldInfoFactory)
        {
            sFieldInfoMgr.mFieldInfoMap.Add(fieldInfoFactory.getType(), fieldInfoFactory);
        }
        //--------------------------------------------------------------------------------

        static FieldInfoManager sFieldInfoMgr = new FieldInfoManager();
        //--------------------------------------------------------------------------------

        static public tFieldInfo CreateFieldInfo(FIELD_TYPE eType)
        {
            tFieldInfoFactory fieldFactory;
            if (sFieldInfoMgr.mFieldInfoMap.TryGetValue(eType, out fieldFactory))
                return fieldFactory.NewFieldInfo();

            return null;
        }
        //--------------------------------------------------------------------------------

        static public tFieldInfo CreateFieldInfo(string strType)
        {
            FIELD_TYPE eType = StaticDefine.ToType(strType);

            return CreateFieldInfo(eType);
        }

        static public tFieldInfo GetFieldInfo(FIELD_TYPE eType)
        {
            tFieldInfo fieldInfo;
            if (sFieldInfoMgr.mFieldMap.TryGetValue(eType, out fieldInfo))
                return fieldInfo;

            return null;
        }

        static public tFieldInfo GetFieldInfo(string strType)
        {
            FIELD_TYPE eType = StaticDefine.ToType(strType);
            return GetFieldInfo(eType);
        }
    };

    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
   
    class StringFieldInfo : BaseFieldInfo
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_STRING; }
        //--------------------------------------------------------------------------------

        public override bool serialize(object data, ref DataBuffer resultData)
        {
            if (null == data)
            {
                resultData.writeTwo("");
                return true;
            }
            resultData.writeTwo((string)data);
            return true;
        }
        //--------------------------------------------------------------------------------

        public override bool restore(out object data, ref DataBuffer scrData)
        {
            string val;
            if (scrData.readTwo(out val))
            {
                data = val;
                return true;
            }
            data = "";
            return false;
        }
        //------------------------------------------------------------------------------

        public override bool set(ref object[] dataList, object obj)
        {
            if (mPosition < dataList.Length)
            {
                if (obj.GetType() == typeof(int))
                {
                    dataList[mPosition] = obj.ToString();
                }
                else if (obj.GetType() == typeof(float))
                {
                    dataList[mPosition] = obj.ToString();
                }
                else if (obj.GetType() == typeof(string))
                {
                    try
                    {
                        dataList[mPosition] = (string)obj;
                    }
                    catch
                    {
                        dataList[mPosition] = "";
                    }
                }
                else
                    return false;

                return true;
            }
            return false;
        }
        //--------------------------------------------------------------------------------

        public override object get(object[] dataList)
        {
            if (mPosition < dataList.Length)
            {
                return dataList[mPosition];
            }
            return null;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out int nVal)
        {
            nVal = 0;
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                try
                {
                    nVal = int.Parse(dataList[mPosition].ToString());
                }
                catch
                {

                }
                return true;
            }
            nVal = 0;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out float fVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                try
                {
                    fVal = float.Parse(dataList[mPosition].ToString());
                }
                catch
                {
                    fVal = 0.0f;
                }
                return true;
            }
            fVal = 0.0f;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out string strVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                strVal = (string)dataList[mPosition];
                return true;
            }
            strVal = "";
            return false;
        }

        public override int getIndex(object[] dataList)
        {
            return StaticDefine.StringID( (string)dataList[mPosition] );
        }

        public override int MakeIndex(int nVal) { return StaticDefine.StringID( nVal.ToString() ); }
        public override int MakeIndex(string strVal) { return StaticDefine.StringID(strVal); }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------

   
    class FloatFieldInfo : BaseFieldInfo
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_FLOAT; }
        //--------------------------------------------------------------------------------
		public override int getIndex(object[] dataList) 
		{
			Logic.EventCenter.Log(LOG_LEVEL.WARN, "Use float type index");
			int key = 0;
			if (get(dataList, out key))
				return key;
			return 0;
		}

        public override bool serialize(object data, ref DataBuffer resultData)
        {
            if (null == data)
            {
                resultData.write((float)0f);
                return true;
            }
            resultData.write((float)data);
            return true;
        }
        //--------------------------------------------------------------------------------

        public override bool restore(out object data, ref DataBuffer scrData)
        {
            float val;
            if (scrData.read(out val))
            {
                data = val;
                return true;
            }
            data = 0.0f;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool set(ref object[] dataList, object obj)
        {
            if (mPosition < dataList.Length)
            {
                if (obj.GetType() == typeof(int))
                {
                    dataList[mPosition] = (float)obj;
                }
                else if (obj.GetType() == typeof(float))
                {
                    dataList[mPosition] = obj;
                }
                else if (obj.GetType() == typeof(string))
                {
                    try
                    {
                        dataList[mPosition] = float.Parse((string)obj);
                    }
                    catch
                    {
                        dataList[mPosition] = 0.0f;
                    }
                }
                else
                    return false;

                return true;
            }
            return false;
        }
        //--------------------------------------------------------------------------------

        public override object get(object[] dataList)
        {
            if (mPosition < dataList.Length)
            {
                return dataList[mPosition];
            }
            return null;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out int nVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                nVal = (int)dataList[mPosition];
                return true;
            }
            nVal = 0;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out float fVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                fVal = (float)dataList[mPosition];
                return true;
            }
            fVal = 0.0f;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out string strVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                strVal = dataList[mPosition].ToString();
                return true;
            }
            strVal = "";
            return false;
        }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    
    class NiceDataFieldInfo : BaseFieldInfo
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_NICEDATA; }
        //--------------------------------------------------------------------------------

        public override bool serialize(object data, ref DataBuffer resultData)
        {
            NiceData d = (NiceData)data;
            resultData.write(d!=null);
            return d.serialize(ref resultData);
        }
        //--------------------------------------------------------------------------------

        public override bool restore(out object data, ref DataBuffer scrData)
        {
            bool bExist = false;
            if (scrData.read(out bExist))
            {
                if (bExist)
                {
                    NiceData d = new NiceData();
                    if (d.restore(ref scrData))
                    {
                        data = d;
                        return true;
                    }
                }
                data = null;
                return true;
            }
            data = null;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool set(ref object[] dataList, object obj)
        {
            if (mPosition < dataList.Length)
            {
                if (obj.GetType() == typeof(NiceData))
                {
                    dataList[mPosition] = obj;
                    return true;
                }
            }
            return false;
        }
        //--------------------------------------------------------------------------------

        public override object get(object[] dataList)
        {
            if (mPosition < dataList.Length)
            {
                return dataList[mPosition];
            }
            return null;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out int nVal)
        {

            nVal = 0;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out float fVal)
        {

            fVal = 0.0f;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out string strVal)
        {

            strVal = "";
            return false;
        }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    class BoolFieldInfo : BaseFieldInfo
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_BOOL; }
        //--------------------------------------------------------------------------------

        public override bool serialize(object data, ref DataBuffer resultData)
        {
            if (null == data)
            {
                resultData.write((byte)0);
                return true;
            }
            byte d = (byte)((bool)data ? 1:0 );
            resultData.write(d);
            return true;
        }
        //--------------------------------------------------------------------------------

        public override bool restore(out object data, ref DataBuffer scrData)
        {
            byte val;
            if (scrData.read(out val))
            {
                data = (bool)(val!=0);
                return true;
            }
            data = false;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool set(ref object[] dataList, object obj)
        {
            if (mPosition < dataList.Length)
            {
                if (obj.GetType() == typeof(int))
                {
                    dataList[mPosition] = (int)obj != 0;
                }
                else if (obj.GetType() == typeof(float))
                {
                    float v = (float)obj;
                    dataList[mPosition] = v > 0.0000001 || v < -0.0000001;
                }
                else if (obj.GetType() == typeof(string))
                {
                    dataList[mPosition] = !(((string)obj) == "false" || ((string)obj) == "0" || ((string)obj) == "NO");
                }
                else if (obj.GetType() == typeof(bool))
                    dataList[mPosition] = obj;
                else
                    return false;

                return true;
            }
            return false;
        }
        //--------------------------------------------------------------------------------

        public override object get(object[] dataList)
        {
            if (mPosition < dataList.Length)
            {
                return dataList[mPosition];
            }
            return null;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out int nVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                nVal = (bool)dataList[mPosition] ? 1 : 0;
                return true;
            }
            nVal = 0;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out float fVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                fVal = (bool)dataList[mPosition] ? 1.0f : 0.0f;
                return true;
            }
            fVal = 0.0f;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out string strVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                strVal = (bool)dataList[mPosition] ? "true" : "false";
                return true;
            }
            strVal = "";
            return false;
        }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    class BoolFieldInfoFactory : tFieldInfoFactory
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_BOOL; }

        public override tFieldInfo NewFieldInfo()
        {
            return new BoolFieldInfo();
        }
    };

    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    class UINT64FieldInfo : BaseFieldInfo
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_UINT64; }
        //--------------------------------------------------------------------------------

        public override int getIndex(object[] dataList) 
        {
            int key = 0;
            if (get(dataList, out key))
                return key;
            return 0; 
        }

        public override bool serialize(object data, ref DataBuffer resultData)
        {
            if (null == data)
            {
                resultData.write((UInt64)0);
                return true;
            }

            resultData.write((UInt64)data);
            return true;
        }
        //--------------------------------------------------------------------------------

        public override bool restore(out object data, ref DataBuffer scrData)
        {
            UInt64 val;
            if (scrData.read(out val))
            {
                data = val;
                //data = (bool)(val != 0);
                return true;
            }
            data = 0;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool set(ref object[] dataList, object obj)
        {
            if (mPosition < dataList.Length)
            {
                if (obj.GetType() == typeof(int))
                {
                    dataList[mPosition] = (int)obj;
                }
                else if (obj.GetType() == typeof(float))
                {
                    float v = (float)obj;
                    dataList[mPosition] = (UInt64)v;
                }
                else if (obj.GetType() == typeof(string))
                {
                    dataList[mPosition] = UInt64.Parse((string)obj); //!(((string)obj) == "false" || ((string)obj) == "0" || ((string)obj) == "NO");
                }
                else if (obj.GetType() == typeof(bool))
                    dataList[mPosition] = 1;
                else if (obj.GetType() == typeof(UInt64))
                    dataList[mPosition] = (UInt64)obj;
                else
                    return false;

                return true;
            }
            return false;
        }
        //--------------------------------------------------------------------------------

        public override object get(object[] dataList)
        {
            if (mPosition < dataList.Length)
            {
                return dataList[mPosition];
            }
            return null;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out int nVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                nVal = (int)((UInt64)dataList[mPosition] & 0Xffffffff);
                return true;
            }
            nVal = 0;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out float fVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                fVal = (float)(UInt64)dataList[mPosition];
                return true;
            }
            fVal = 0.0f;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out string strVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                strVal = ((UInt64)dataList[mPosition]).ToString();
                return true;
            }
            strVal = "";
            return false;
        }

        public override bool get(object[] dataList, out UInt64 result) 
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
				result = (UInt64)dataList[mPosition];
                return true;
            }
            result = 0;
            return false;
        }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    class UINT64FieldInfoFactory : tFieldInfoFactory
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_UINT64; }

        public override tFieldInfo NewFieldInfo()
        {
            return new UINT64FieldInfo();
        }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------

    class NiceDataFieldInfoFactory : tFieldInfoFactory
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_NICEDATA; }

        public override tFieldInfo NewFieldInfo()
        {
            return new NiceDataFieldInfo();
        }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------

    class FloatFieldInfoFactory : tFieldInfoFactory
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_FLOAT; }

        public override tFieldInfo NewFieldInfo()
        {
            return new FloatFieldInfo();
        }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------

    class StringFieldInfoFactory : tFieldInfoFactory
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_STRING; }

        public override tFieldInfo NewFieldInfo()
        {
            return new StringFieldInfo();
        }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
    class ByteFieldInfo : BaseFieldInfo
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_BYTE; }
        //--------------------------------------------------------------------------------

        public override bool serialize(object data, ref DataBuffer resultData)
        {
            if (null == data)
            {
                resultData.write((byte)0);
                return true;
            }
            resultData.write((byte)data);
            return true;
        }
        //--------------------------------------------------------------------------------

        public override bool restore(out object data, ref DataBuffer scrData)
        {
            byte val;
            if (scrData.read(out val))
            {
                data = val;
                return true;
            }
            data = 0;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool set(ref object[] dataList, object obj)
        {
            if (mPosition < dataList.Length)
            {
                if (obj.GetType() == typeof(byte))
                    dataList[mPosition] = obj;
                else if (obj.GetType() == typeof(int))
                {
                    dataList[mPosition] = (byte)obj;
                }
                else if (obj.GetType() == typeof(float))
                {
                    dataList[mPosition] = (byte)obj;
                }
                else if (obj.GetType() == typeof(string))
                {
                    try
                    {
                        dataList[mPosition] = byte.Parse((string)obj);
                    }
                    catch
                    {
                        dataList[mPosition] = 0;
                    }
                }
                else
                    return false;

                return true;
            }
            return false;
        }
        //--------------------------------------------------------------------------------

        public override object get(object[] dataList)
        {
            if (mPosition < dataList.Length)
            {
                return dataList[mPosition];
            }
            return null;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out int nVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                nVal = (byte)dataList[mPosition];
                return true;
            }
            nVal = 0;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out float fVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                fVal = (byte)dataList[mPosition];
                return true;
            }
            fVal = 0.0f;
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool get(object[] dataList, out string strVal)
        {
            if (mPosition < dataList.Length && null != dataList[mPosition])
            {
                strVal = dataList[mPosition].ToString();
                return true;
            }
            strVal = "";
            return false;
        }
        public override int getIndex(object[] dataList)
        {
            return (int)dataList[mPosition];
        }
        public override int MakeIndex(int nVal) { return nVal; }
        public override int MakeIndex(string strVal) { return int.Parse(strVal); }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------

    class ByteFieldInfoFactory : tFieldInfoFactory
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_BYTE; }

        public override tFieldInfo NewFieldInfo()
        {
            return new ByteFieldInfo();
        }
    };
    //--------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------
}