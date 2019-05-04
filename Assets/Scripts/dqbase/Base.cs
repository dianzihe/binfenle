//#define NICEDATA_USE_INDEX_ID
//#define SAVE_STRING_LENGTH
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using DataTable;
using Logic;



/*
enum FIELD_TYPE
{
	FIELD_NULL = 0,
	FIELD_INT,
	FIELD_FLOAT,
	FIELD_STRING,
}
 */
//--------------------------------------------------------------------------------
// 基本字段类型
// 此类型定义值必须要与C++代码中的进行对应
//--------------------------------------------------------------------------------

public enum FIELD_TYPE
{
    FIELD_NULL = 0,
    FIELD_INT,
    FIELD_FLOAT,
	FIELD_STRING,
	FIELD_BOOL,                     
    FIELD_BYTE,
    FIELD_SHORT,
    FIELD_INT_RELATION,
    FIELD_STRING_RELATION,
    FIELD_DATA,
    FIELD_TABLE,
    FIELD_RECORD,
    FIELD_NICEDATA,
    FIELD_EVENT,
    FIELD_SKIP_1,
    FIELD_SKIP_2,
    FIELD_UINT64,
    FIELD_CHAR_STRING,
    FIELD_TYPE_MAX 
};
//--------------------------------------------------------------------------------
//--------------------------------------------------------------------------------

struct StaticDefine
{

#if DEBUG || UNITY_DEBUG
    static public bool debug = true;            // use debug mode
#else
	static public bool debug = true;
#endif
	static public float netDelayTollarence = 1.0f;
	static public bool  useHeatBeat = false;
    static public float heartBeatInterval = 30.0f;   //seconds
    static public bool  isDynamicResources = false;
    /*static public int IntType = 0;
    static public float FloatType = 0.0f;
    static public bool BoolType = false;
    static public string StringType = "";
    static public NiceTable mTable = new NiceTable();
    static public NiceData mNiceData = new NiceData();
	 */
    //--------------------------------------------------------------------------------
    static public Type ToObjectType(FIELD_TYPE eType)
    {
        switch (eType)
        {
            case FIELD_TYPE.FIELD_INT:
                return typeof(int);
            case FIELD_TYPE.FIELD_FLOAT:
                return typeof(float);
            case FIELD_TYPE.FIELD_STRING:
                return typeof(float);
            case FIELD_TYPE.FIELD_BOOL:
                return typeof(bool);
            case FIELD_TYPE.FIELD_BYTE:
                return typeof(byte);
            case FIELD_TYPE.FIELD_TABLE:
                return typeof(NiceTable);
            case FIELD_TYPE.FIELD_NICEDATA:
                return typeof(NiceData);
            case FIELD_TYPE.FIELD_DATA:
                return typeof(DataBuffer);

            case FIELD_TYPE.FIELD_UINT64:
                return typeof(UInt64);
        }
        Type t = null;
        return t;
    }
    //--------------------------------------------------------------------------------
	
    static public string ToStringType(FIELD_TYPE eType)
    {
        switch (eType)
        {
            case FIELD_TYPE.FIELD_INT:
                return "INT";
            case FIELD_TYPE.FIELD_FLOAT:
                return "FLOAT";
            case FIELD_TYPE.FIELD_STRING:
                return "STRING";
		    case FIELD_TYPE.FIELD_BOOL:
				return "BOOL";
            case FIELD_TYPE.FIELD_BYTE:
                return "BYTE";
            case FIELD_TYPE.FIELD_UINT64:
                return "UINT64";

            case FIELD_TYPE.FIELD_NICEDATA:
			return "NICEDATA";

		case FIELD_TYPE.FIELD_DATA:
			return "DATA";

		case FIELD_TYPE.FIELD_TABLE:
			return "TABLE";
        }
        return "";
    }
    //--------------------------------------------------------------------------------

    static public FIELD_TYPE ToType(string strType)
    {
        switch (strType)
        {
            case "INT":
                return FIELD_TYPE.FIELD_INT;

            case "FLOAT":
                return FIELD_TYPE.FIELD_FLOAT;

            case "STRING":
                return FIELD_TYPE.FIELD_STRING;

            case "BOOL":
                return FIELD_TYPE.FIELD_BOOL;
            case "BYTE":
                return FIELD_TYPE.FIELD_BYTE;	
            case "UINT64":
                return FIELD_TYPE.FIELD_UINT64;

		case "DATA":
			return FIELD_TYPE.FIELD_DATA;

		case "TABLE":
			return FIELD_TYPE.FIELD_TABLE;

        }
        return FIELD_TYPE.FIELD_NULL;
    }
	   //--------------------------------------------------------------------------------
    static public int GetType(object obj)
    {
        if (obj.GetType() == typeof(int))
            return (int)FIELD_TYPE.FIELD_INT;
        else if (obj.GetType() == typeof(float))
            return (int)FIELD_TYPE.FIELD_FLOAT;
        else if (obj.GetType() == typeof(string))
            return (int)FIELD_TYPE.FIELD_STRING;
        else if (obj.GetType() == typeof(bool))
            return (int)FIELD_TYPE.FIELD_BOOL;
        else if (obj.GetType() == typeof(byte))
            return (int)FIELD_TYPE.FIELD_BYTE;
        else if (obj.GetType() == typeof(NiceTable))
            return (int)FIELD_TYPE.FIELD_TABLE;
        else if (obj.GetType() == typeof(NiceData))
            return (int)FIELD_TYPE.FIELD_NICEDATA;
        else if (obj.GetType() == typeof(DataBuffer))
            return (int)FIELD_TYPE.FIELD_DATA;
        else if (obj.GetType() == typeof(UInt64))
            return (int)FIELD_TYPE.FIELD_UINT64;

        return (int)FIELD_TYPE.FIELD_NULL;
    }
	   //--------------------------------------------------------------------------------
    static void Add(ref uint x, uint y, ref uint flag)
    {
        UInt64 t64 = (UInt64)x + y;
        if ((t64 >> 32) > 0)
            flag = 1;
        else
            flag = 0;
        x = (uint)t64;
    }

    static void Adc(ref uint x, uint y, ref uint flag)
    {
        UInt64 t64 = (UInt64)x + y + flag;
        if ((t64 >> 32) > 0)
            flag = 1;
        else
            flag = 0;
        x = (uint)t64;
    }

    static void Mul(ref uint one, ref uint two, ref uint flag)
    {
        UInt64 t64 = (UInt64)one * two;
        two = (uint)(t64 >> 32);
        one = (uint)t64;
        if (two == 0)
            flag = 0;
        else
            flag = 1;
    }

    static void Mov(ref uint x, uint val)
    {
        x = val;
    }

    static void Xor(ref uint x, uint val)
    {
        x = x ^ val;
    }

    static void Rol(ref uint v, char bit, ref uint flag)
    {
        flag = v >> (32 - bit);
        v = v << bit;
        v += flag;
        flag = v >> 31;
    }
    // set use code page when get id from string .
    static void SetCodePage(int codepage)
    {
        Encoding en = null;
        try{
            en = Encoding.GetEncoding(codepage);
        }
        catch
        {
            if (null==en)
            {
                throw new Exception("not find string use code page.");
            }
        }
        mUseCodePage = en;
        mStringCodePage = codepage;
    }

    // get data from string at seting code page.
    static public bool GetBytes(string strVal, ref byte[] destData, int destPos)
    {
        if (strVal==null)
            return false;
		byte[] d = Encoding.Default.GetBytes(strVal);
		byte[] re = Encoding.Convert(Encoding.Default, mUseCodePage, d);
		if (re.Length>destData.Length-destPos)
			return false;
		
		re.CopyTo(destData, destPos);
        //mUseCodePage.GetBytes(strVal, 0, strVal.Length, destData, 0);
        return true;
    }

    static public byte[] GetBytes(string strVal)
    {
		if (null==strVal)
			return null;
        return mUseCodePage.GetBytes(strVal);

        //byte[] d = Encoding.Default.GetBytes(strVal);
        //byte[] re = Encoding.Convert(Encoding.Default, mUseCodePage, d);		
		//return re;
    }

    static public string GetString(byte[] scrData, int indexPos, int length)
    {
        return mUseCodePage.GetString(scrData, indexPos, length);

        //if (scrData.Length<=0)
        //    return "";
        //byte[] d = Encoding.Convert(mUseCodePage, Encoding.Default, scrData, indexPos, length);
        //return Encoding.Default.GetString(d);
		
    }

    static int mStringCodePage = 936;
    static Encoding mUseCodePage = Encoding.UTF8; //GetEncoding(mStringCodePage);
    static uint[] m = new uint[70];

    static public int StringID(string strVal)
    {		
		int id = StringID ( GetBytes(strVal) );		
		string info = " make id = ";
		info += id.ToString();
		//Logic.EventCenter.Self.Log(info);*/
		return id;
	}
	
	static public int StringID(byte[] strUTF8Data)
	{
		if (strUTF8Data.Length <= 0 || strUTF8Data.Length > 260)
            return 0;
		
        int i = 0;
        int c = 0;
        while (i + 4 <= strUTF8Data.Length)
        {
            m[c] = BitConverter.ToUInt32(strUTF8Data, i);
            i += 4;
            ++c;
        }
        if (i < strUTF8Data.Length)
        {
            byte[] temp = new byte[4];
            // ut8.CopyTo(temp, i, ut8.Length-i);
            int x = 0;
            for (x = 0; x < 4; ++x)
            {
                if (i < strUTF8Data.Length)
                    temp[x] = strUTF8Data[i];
                else
                    temp[x] = 0;
                ++i;
            }
            m[c] = BitConverter.ToUInt32(temp, 0);
            ++c;
        }
        i = c;
        // must use strncpy because strncpy do full zero for last char , but strncpy_s not done. by Ywg 20110612
        //strncpy( (char *)m, str, i * 4 );  

        uint v = 0;
        //for (i=0;i<TEMP_BUFFER_SIZE && m[i];i++) ;
        m[i++] = 0x9BE74448;
        m[i++] = 0x66F42C48;
        v = 0xF4FA8928;
        //---------------------------------------------------------

        uint eax = 0;
        uint ebx = 0;

        uint edx = 0;
        uint esi = 0;
        uint edi = 0;

        uint j = 0;

        uint f = 0;

        {
            Mov(ref esi, 0x37A8470E);
            Mov(ref edi, 0x7758B42B);

            j = 0;

        _loop:
            Mov(ref ebx, 0x267B0B11);
            Rol(ref v, (char)1, ref  f);
            Xor(ref ebx, v);

            Mov(ref eax, m[j]);
            Mov(ref edx, ebx);

            //1
            Xor(ref esi, eax);
            Xor(ref edi, eax);

            //2
            Add(ref edx, edi, ref  f);
            edx = edx | 0x2040801;
            edx = edx & 0xBFEF7FDF;
            Mov(ref eax, esi);
            Mul(ref eax, ref  edx, ref  f);

            //3
            Adc(ref eax, edx, ref  f);
            //4
            Mov(ref edx, ebx);
            Adc(ref eax, 0, ref  f);

            //5
            Add(ref edx, esi, ref  f);

            edx = edx | 0x804021;
            edx = edx & 0x7DFEFBFF;
            //6
            Mov(ref esi, eax);
            Mov(ref eax, edi);
            Mul(ref eax, ref edx, ref  f);

            //7
            Add(ref edx, edx, ref  f);
            Adc(ref eax, edx, ref  f);
            if (f == 0)
                goto _skip;
            //8
            Add(ref eax, 2, ref f);

        _skip:
            j++;
            Mov(ref edi, eax);
            if (j < i)
                goto _loop;

            //9
            Xor(ref esi, edi);
            Mov(ref v, esi);
        }

        return (int)v;

    }

}

//--------------------------------------------------------------------------------
//--------------------------------------------------------------------------------


public class DataBuffer
{
    public byte[] mData = null;

    protected int mCurrent = 0;
	
	public DataBuffer()
	{
		mData = new byte[8];
		mCurrent = 0;
	}

    public DataBuffer(int initSize)
    {
        if (initSize <= 0)
            initSize = 64;
        mData = new byte[initSize];
    }

    public int size(){ return mData.Length; }
	
	public void _setData(byte[] data)
	{
		mData = data;
		mCurrent = 0;
	}

    public void _resize(int size)
    {
        int x = mData.Length;
        if (size < x)
            x = size;
        byte[] temp = new byte[size];
        for (int i = 0; i < x; ++i)
        {
            temp[i] = mData[i];
        }
        mData = temp;
        if (mCurrent > size)
            mCurrent = size;
    }

    public void resizeTwoLength(int size)
    {
        int s = mData.Length;
        if (s <= 0)
            s = 1;
        do
        {
            s *= 2;
        } while (s < size);

        _resize(s);
    }

    public byte[] getData() { return mData; }

    public int tell() { return mCurrent; }

    public bool seek(int size)
    {
        if (size <= mData.Length)
        {
            mCurrent = size;
            return true;
        }
        return false;
    }

    public bool end() { return mCurrent >= mData.Length; }


    public int write(byte[] scr, int len)
    {
        int reLen = len;
        if (reLen > scr.Length)
            reLen = scr.Length;
        if (mCurrent + reLen > mData.Length)
            resizeTwoLength(mCurrent + reLen);

        //for (int i = 0; i < reLen; ++i)
        //{
        //    mData[mCurrent + i] = scr[i];
        //}
        Array.Copy(scr, 0, mData, mCurrent, reLen);

        mCurrent += reLen;
        return reLen;
    }

    public int write(byte[] scr, int scrIndex, int len)
    {
        if (scrIndex<0 
            || len<=0 
            || scrIndex + len > scr.Length)
            return 0;

        int reLen = len;
 
        if (mCurrent + reLen > mData.Length)
            resizeTwoLength(mCurrent + reLen);

        Array.Copy(scr, scrIndex, mData, mCurrent, reLen);

        mCurrent += reLen;

        return reLen;
    }

    public int write(byte[] scr)
    {
        if (mCurrent + scr.Length > mData.Length)
            resizeTwoLength(mCurrent + scr.Length);
        //for (int i = 0; i < scr.Length; ++i)
        //{
        //    mData[mCurrent + i] = scr[i];
        //}
        scr.CopyTo(mData, mCurrent);
        mCurrent += scr.Length;
        return scr.Length;
    }

    public void write(byte val)
    {
        if (mCurrent + 1 > mData.Length)
            resizeTwoLength(mCurrent + 1);
        mData[mCurrent++] = val;
    }

    public void write(bool bVal)
    {
        write(bVal ? (byte)1 : (byte)0);
    }

    public void write(int val)
    {
        byte[] temp = BitConverter.GetBytes(val);
        write(temp, temp.Length);
    }
    public void write(uint val)
    {
        byte[] temp = BitConverter.GetBytes(val);
        write(temp, temp.Length);
    }

    public void write(float val)
    {
        byte[] temp = BitConverter.GetBytes(val);
        write(temp, temp.Length);
    }

    public void write(ushort val)
    {
        byte[] temp = BitConverter.GetBytes(val);
        write(temp, temp.Length);
    }

    public void write(UInt64 val)
    {
        byte[] temp = BitConverter.GetBytes(val);
        write(temp, temp.Length);
    }

    virtual public bool writeOne(string strVal)
    {
#if SAVE_STRING_LENGTH
        if (strVal == null)
        {
            write((byte)0);
            return true;
        }
        byte[] str = StaticDefine.GetBytes(strVal);
        if (str.Length > 255)
            return false;
       
        byte d = (byte)(str.Length & 0xff);
        write(d);
        write(str);

        return true;
#else
        return writeTwo(strVal);
#endif
    }

    virtual public bool writeTwo(string strVal)
    {
        if (strVal == null)
        {
            write((ushort)0);
            return true;
        }
        byte[] str = StaticDefine.GetBytes(strVal);
        if (str.Length > 65535)
            return false;
        ushort len = (ushort)(str.Length & 0xFFFF);
        //byte[] d = BitConverter.GetBytes(len);
		write(len);
		
        write(str);

        return true;
    }

    public byte[] read(uint size)
    {
        if (mCurrent + size <= mData.Length)
        {
            byte[] temp = new byte[size];
            for (int i = 0; i < size; ++i)
            {
                temp[i] = mData[mCurrent + i];
            }
            mCurrent += (int)size;
            return temp;
        }
        return null;
    }

    public bool read(out byte dest)
    {
        if (mCurrent + 1 <= mData.Length)
        {
            dest = mData[mCurrent++];           
            return true;
        }
        dest = 0;
        return false;
    }

    public bool read(out bool dest)
    {        
        byte val;
        if (read(out val))
        {
            dest = val != 0;
            return true;
        }
        dest = false;
        return false;
    }

    public bool read(out int dest)
    {
        if (mCurrent + 4 <= mData.Length)
        {
            dest = BitConverter.ToInt32(mData, mCurrent);
            mCurrent += 4;
            return true;
        }
        dest = 0;
        return false;
    }
    public bool read(out uint dest)
    {
        if (mCurrent + sizeof(int) <= mData.Length)
        {
            dest = BitConverter.ToUInt32(mData, mCurrent);
            mCurrent += sizeof(uint);
            return true;
        }
        dest = 0;
        return false;
    }
    public bool read(out ushort dest)
    {
        if (mCurrent + sizeof(ushort) <= mData.Length)
        {
            dest = BitConverter.ToUInt16(mData, mCurrent);
            mCurrent += sizeof(ushort);
            return true;
        }
        dest = 0;
        return false;
    }
    public bool read(out float dest)
    {
        if (mCurrent + 4 <= mData.Length)
        {
            dest = BitConverter.ToSingle(mData, mCurrent);
            mCurrent += 4;
            return true;
        }
        dest = 0;
        return false;
    }

    public bool read(out UInt64 dest)
    {
        if (mCurrent + sizeof(UInt64) <= mData.Length)
        {
            dest = BitConverter.ToUInt64(mData, mCurrent);
            mCurrent += sizeof(UInt64);
            return true;
        }
        dest = 0;
        return false;
    }

    virtual public bool readOne(out string dest)
    {
#if SAVE_STRING_LENGTH
        byte b;
        if (!read(out b))
        {
            dest = "";
            return false;
        }

        if (b == 0)
        {
            dest = "";
            return true;
        }
            
        if (mCurrent + b <= mData.Length)
        {
            dest = StaticDefine.GetString(mData, mCurrent, (int)b);
            mCurrent += b;
            return true;
        }
        
        dest = "";
        return false;
#else
        return readTwo(out dest);
#endif
    }

    virtual public bool readTwo(out string dest)
    {        
        ushort b;
        if (!read(out b))
        {
            dest = "";
            return false;
        }
        
        if (b == 0)
        {
            dest = "";
            return true;
        }

        if (mCurrent + b <= mData.Length)
        {
            dest = StaticDefine.GetString(mData, mCurrent, (int)b);
            mCurrent += b;
            return true;            
        }
        dest = "";
        return false;
    }


    public bool read(ref DataBuffer destBuffer, int readSize, int destBeginPos)
    {
        if (mData==null || readSize > size() - tell())
            return false;

        if (destBuffer.size() - destBeginPos < readSize)
            destBuffer._resize((int)(destBeginPos + readSize));
        try
        {
            Array.Copy(mData, tell(), destBuffer.mData, destBeginPos, (int)readSize);
            mCurrent += readSize;
            return true;
        }
        catch (Exception e)
        {
            EventCenter.Self.Log(e.ToString());
            return false;
        }
    }


	// write string to debug string
	public bool _writeString(string strValue)
	{
		byte[] strData = StaticDefine.GetBytes(strValue);
		return write(strData)==strData.Length;
	}
}

//--------------------------------------------------------------------------------
//--------------------------------------------------------------------------------
public abstract class tNiceData
{
    public abstract void set(string indexName, object objVal);

    public abstract Data get(string indexName);
    public abstract object getObject(string indexName);
    public abstract bool getData(string indexName, out object value);

    public abstract bool get(string indexName, out int nVal);

    public abstract bool get(string indexName, out float fVal);

    public abstract bool get(string indexName, out string strVal);

	public abstract bool get(string index, out UInt64 val);

	public abstract bool get(string index, out tNiceData val);

    public abstract bool serialize(ref DataBuffer resultData);

    public abstract bool restore(ref DataBuffer scrData);
 
	public abstract void dump();

    public abstract bool FullData(object scrObject);
    public abstract bool RestoreToTarget(ref object targetObject);
}

//--------------------------------------------------------------------------------
//--------------------------------------------------------------------------------
public struct Data
{
    static public Data NULL = new Data(null);
#if NICEDATA_USE_INDEX_ID
    public string mName;
#endif
    public object mObj;

    //public Data() { }
    public Data(object val) { mObj = val; }

    public bool Empty() { return mObj == null; }

    public override string ToString()
    {
#if NICEDATA_USE_INDEX_ID
        return "[" + mName + "]:[" + mObj.ToString() + "]";
#else
        return mObj.ToString();
#endif
    }

    static public implicit operator int (Data current)
    {
		if (/*current==null ||*/ current.mObj==null)
			return 0;

		if (typeof(int) == current.mObj.GetType())
			return (int)current.mObj;

		if (typeof(float) == current.mObj.GetType())
			return (int)(float)current.mObj;

		if (typeof(UInt64) == current.mObj.GetType())
			return (int)(UInt64)current.mObj;

		if (typeof(string) == current.mObj.GetType ()) 
		{
			string s = (string)current.mObj;
			if (s != "")
				return int.Parse (s);
		}
        return 0;
    }

    static public implicit operator float (Data current)
    {
        if (/*current==null ||*/ current.mObj == null)
			return 0;

		if (typeof(int) == current.mObj.GetType())
			return (float)(int)current.mObj;

		if (typeof(float) == current.mObj.GetType())
			return (float)current.mObj;

		if (typeof(UInt64) == current.mObj.GetType())
			return (float)(UInt64)current.mObj;

		if (typeof(string) == current.mObj.GetType())
		{
			string s = (string)current.mObj;
			if (s != "")
				return float.Parse (s);
		}

        return 0;
    }

    static public implicit operator string (Data current)
    {
        if (/*current==null ||*/ current.mObj == null)
			return "";

		if (typeof(int) == current.mObj.GetType())
			return ((int)current.mObj).ToString();

		if (typeof(float) == current.mObj.GetType())
			return ((float)current.mObj).ToString();

		if (typeof(string) == current.mObj.GetType())
			return (string)current.mObj;

        return "";
    }

    static public implicit operator bool(Data current)
    {
        if (/*current==null ||*/ current.mObj == null)
            return false;

         if (typeof(bool) == current.mObj.GetType())
            return ((bool)current.mObj);

        if (typeof(int) == current.mObj.GetType())
            return ((int)current.mObj)!=0;

        if (typeof(float) == current.mObj.GetType())
        {
            float v = (float)current.mObj;
            return v<-0.00001f || v>0.00001f;
        }

        if (typeof(string) == current.mObj.GetType())
        {
            string v = (string)current.mObj;   
            v = v.ToLower();
            if (v=="yes" || v=="true")
                return true;

            return int.Parse(v)!=0;
        }

        return false;
    }

	static public implicit operator UInt64(Data current)
	{
        if (/*current==null ||*/ current.mObj == null)
			return 0; //(UInt64)(0xffffffffffffffff);

		if (typeof(UInt64) == current.mObj.GetType())
			return ((UInt64)current.mObj);

		if (typeof(string) == current.mObj.GetType())
		{
			string s = (string)current.mObj;
			if (s != "")
				return UInt64.Parse (s);
		}

		return (UInt64)((int)current);
	}

    //static public implicit operator object (Data current)
    //{       
    //    return current.mObj;
    //}
}

#if NICEDATA_USE_INDEX_ID
//-------------------------------------------------------------------------
public class NiceData : tNiceData
{
    protected Dictionary<int, object> mDataMap = new Dictionary<int, object>();
    //--------------------------------------------------------------------------------

    public override void set(string indexName, object objVal)
    {
        if (StaticDefine.debug)
        {
            Data temp = new Data();
            temp.mName = indexName;
            temp.mObj = objVal;
            objVal = temp;
        }
        //mDataMap.Add((int)StaticDefine.StringID(indexName), objVal);
		mDataMap[(int)StaticDefine.StringID(indexName)] = objVal;
    }

    public override Data get(string indexName)
    {
        object val;

        if (!mDataMap.TryGetValue((int)StaticDefine.StringID(indexName), out val))
        {
            Logic.EventCenter.Self.Log("XXX WARN: no exist data >>>" + indexName);
			val = new Data();
        }
        return (Data)val;
    }

    public virtual bool remove(string keyName)
    {
		return mDataMap.Remove((int)StaticDefine.StringID(keyName));
    }
    //--------------------------------------------------------------------------------

    public override bool getData(string indexName, out object value)
    {
        value = null;
        object val = null;

        if (!mDataMap.TryGetValue((int)StaticDefine.StringID(indexName), out val))
        {
            Logic.EventCenter.Self.Log("XXX WARN: no exist data >>>" + indexName);
            return false;
        }
        if (StaticDefine.debug)
        {
            Data temp = (Data)val;
            value = temp.mObj;          
        }
        else
            value = val;

        return true;

    }
    //--------------------------------------------------------------------------------

    public override bool get(string indexName, out int nVal)
    {
        object val;
        nVal = 0;
        if (StaticDefine.debug)
        {
            if (mDataMap.TryGetValue((int)StaticDefine.StringID(indexName), out val))
            {
                Data temp = (Data)val;
                if (temp.mObj.GetType()==nVal.GetType())
                {
                    nVal = (int)temp.mObj;
                    return true;
                }
            }
            return false;
        }
        if (mDataMap.TryGetValue((int)StaticDefine.StringID(indexName), out val) && val.GetType() == nVal.GetType())
        {
            nVal = (int)val;
            return true;
        }
        return false;
    }
    //--------------------------------------------------------------------------------

    public override bool get(string indexName, out float fVal)
    {
        object val;
        fVal = 0.0f;
        if (StaticDefine.debug)
        {
            if (mDataMap.TryGetValue((int)StaticDefine.StringID(indexName), out val))
            {
                Data temp = (Data)val;
                if (temp.mObj.GetType()==fVal.GetType())
                {
                    fVal = (float)temp.mObj;
                    return true;
                }
            }
            return false;
        }
        if (mDataMap.TryGetValue((int)StaticDefine.StringID(indexName), out val) && val.GetType() == fVal.GetType())
        {
            fVal = (float)val;
            return true;
        }
        return false;
    }
    //--------------------------------------------------------------------------------

    public override bool get(string indexName, out string strVal)
    {
        strVal = "";
        object val;
        if (StaticDefine.debug)
        {
            if (mDataMap.TryGetValue((int)StaticDefine.StringID(indexName), out val))
            {
                Data temp = (Data)val;
                if (temp.mObj.GetType()==strVal.GetType())
                {
                    strVal = (string)temp.mObj;
                    return true;
                }
            }
            return false;
        }
        if (mDataMap.TryGetValue((int)StaticDefine.StringID(indexName), out val) && val.GetType() == strVal.GetType())
        {
            strVal = (string)val;
            return true;
        }
        return false;
    }

    public override bool serialize(ref DataBuffer resultData)
    {
        // save count
        int count = mDataMap.Count;
        resultData.write(count);
        // each save data
        foreach (KeyValuePair<int, object> it in mDataMap)
        {
            // save key id
            resultData.write(it.Key);

            object val = it.Value;
            // save name for debug mode.
            if (StaticDefine.debug)
            {
                Data temp = (Data)val;
                resultData.writeOne(temp.mName);
                val = temp.mObj;
            }
            // save type
            byte type = (byte)StaticDefine.GetType(val);
            resultData.write(type);
            tFieldInfo info = FieldInfoManager.GetFieldInfo((FIELD_TYPE)type);
            if (null!=info)
            {
                if (!info.serialize(val, ref resultData))
                    return false;
            }
            else
            {
                throw new Exception("Error: not find data type field infomation.");
            }
        }
        return true;
    }
    //--------------------------------------------------------------------------------

    public override bool restore(ref DataBuffer scrData)
    {
        // read count
        int count;
        if (!scrData.read(out count))
            return false;
        for (int i = 0; i < count; ++i)
        {
            // read id
            int id;
            if (!scrData.read(out id))
                return false;

            Data val = new Data();  
           if (StaticDefine.debug)
            {
                // read name for debug            
                if (!scrData.readOne(out val.mName))
                    return false;

                int nId = StaticDefine.StringID(val.mName);
                if (nId!=id)
                {
                    throw new Exception("Error: string id is same, may be use diffult code page, or is not same program.");
                }                
            }
            byte type;
            if (!scrData.read(out type))
                return false;
            tFieldInfo info = FieldInfoManager.GetFieldInfo((FIELD_TYPE)type);
            if (null != info)
            {
                object obj;
                if (!info.restore(out obj, ref scrData))
                    return false;
                if (StaticDefine.debug)
                {
                    val.mObj = obj;
                    //mDataMap.Add(id, val);
                    mDataMap[id] = val;
                }
                else
                {                    
                    //mDataMap.Add(id, obj);
                    mDataMap[id] = obj;
                }   
            }
            else
            {
				if (StaticDefine.debug)
                {
                    val.mObj = "UNKNOW";
                    //mDataMap.Add(id, val);
                    mDataMap[id] = val;
                }
                else
                {                    
                    //mDataMap.Add(id, "UNKNOW");
                    mDataMap[id] = "UNKNOW";
                }  
                //throw new Exception("Error: not find data type field infomation.");
            }

        }
        return true;
    }
	
	public override void dump()
	{
		EventCenter.Self.Log("--------------------------------------------------------------------");
		foreach (KeyValuePair<int, object> it in mDataMap)
        {
            // save key id
            string info = "[";
            if (StaticDefine.debug)
            {
                Data temp = (Data)it.Value;
                info += temp.mName + "] : [";
                info += temp.mObj.ToString();
            }
            else
            {
                info += it.Key + "] : [";
                info += it.Value.ToString();
            }
            info += "]";
            EventCenter.Self.Log( info );            
		}
		EventCenter.Self.Log("--------------------------------------------------------------------");
	}
};
//--------------------------------------------------------------------------------
#else
//-------------------------------------------------------------------------
public class NiceData : tNiceData
{
    protected Dictionary<string, object> mDataMap = new Dictionary<string, object>();
    //--------------------------------------------------------------------------------
    public object this[string indexName]
    {
        //实现索引器的get方法
        get
        {
            object val;

            if (!mDataMap.TryGetValue(indexName, out val))
            {
                Logic.EventCenter.Self.Log("XXX WARN: no exist data >>>" + indexName);
            }
			return val;
        }
        //实现索引器的set方法
        set
        {
            mDataMap[indexName] = value;
        }
    }

    public override void set(string indexName, object objVal)
    {
        //mDataMap.Add((int)StaticDefine.StringID(indexName), objVal);
        //LOG.Log("basic set--> " + indexName + "->" + objVal.ToString());
        mDataMap[indexName] = objVal;
    }

    public override Data get(string indexName)
    {
        object val;

        if (!mDataMap.TryGetValue(indexName, out val))
        {
            Logic.EventCenter.Self.Log("XXX WARN: no exist data >>>" + indexName);
        }
        return new Data(val);
        //return (Data)val;
    }

    public override object getObject(string indexName)
    {
        object val = null;

        if (!mDataMap.TryGetValue(indexName, out val))
        {
            Logic.EventCenter.Self.Log("XXX WARN: no exist data >>>" + indexName);
        }
        return val;
    }

    public virtual bool remove(string keyName)
    {
		return mDataMap.Remove(keyName);
    }
    //--------------------------------------------------------------------------------

    public override bool getData(string indexName, out object value)
    {
        if (!mDataMap.TryGetValue(indexName, out value))
        {
            //Logic.EventCenter.Self.Log("XXX WARN: no exist data >>>" + indexName);
            return false;
        }

        return true;

    }
    //--------------------------------------------------------------------------------

    public override bool get(string indexName, out int nVal)
    {
        object val;

        bool b = getData(indexName, out val);
        if (b && val is int)
        {
            nVal = (int)val;
			return true;
        }
        else
            nVal = 0;

        return false;
    }
    //--------------------------------------------------------------------------------

    public override bool get(string indexName, out float fVal)
    {
        object val;

        bool b = getData(indexName, out val);
        if (b && val is float)
        {
            fVal = (float)val;
			return true;
        }
        else
            fVal = 0;

        return false;
    }
    //--------------------------------------------------------------------------------

    public override bool get(string indexName, out string strVal)
    {
        object val;

        bool b = getData(indexName, out val);
        if (b && val is string)
        {
            strVal = (string)val;
			return true;
        }
        else
            strVal = "";

        return false;
    }

	public override bool get(string indexName, out UInt64 resultVal)
	{
		object val;
		
		bool b = getData(indexName, out val);
		if (b && val is UInt64)
		{
			resultVal = (UInt64)val;
			return true;
		}
		else
			resultVal = 0;
		
		return false;
	}

	public override bool get(string indexName, out tNiceData resultVal)
	{
		object val;
		
		bool b = getData(indexName, out val);
		if (b && val is tNiceData)
		{
			resultVal = (tNiceData)val;
			return true;
		}
		else
			resultVal = null;
		
		return false;
	}

    public override bool serialize(ref DataBuffer resultData)
    {
        // save count
        byte count = (byte)mDataMap.Count;
        resultData.write(count);
        // each save data
        foreach (KeyValuePair<string, object> it in mDataMap)
        {
            // save key id
            resultData.writeTwo(it.Key);

            object val = it.Value;

            // save type
            byte type = (byte)StaticDefine.GetType(val);
            resultData.write(type);
            tFieldInfo info = FieldInfoManager.GetFieldInfo((FIELD_TYPE)type);
            if (null!=info)
            {
                if (!info.serialize(val, ref resultData))
                    return false;
            }
            else
            {
                throw new Exception("Error: not find data type field infomation.");
            }
        }
        return true;
    }
    //--------------------------------------------------------------------------------

    public override bool restore(ref DataBuffer scrData)
    {
        // read count
        byte count;
        if (!scrData.read(out count))
            return false;
        for (int i = 0; i < count; ++i)
        {
            // read id
            string key;
            if (!scrData.readTwo(out key))
                return false;

            //EventCenter.Log(false, "Read key>" + key);

            if (mDataMap.ContainsKey(key))
                EventCenter.Log(LOG_LEVEL.WARN, "already add key > " + key);

            byte type;
            if (!scrData.read(out type))
                return false;

            tFieldInfo info = FieldInfoManager.GetFieldInfo((FIELD_TYPE)type);
            if (null != info)
            {
                object obj;
                if (!info.restore(out obj, ref scrData))
                    return false;

                //mDataMap.Add(id, obj);
                mDataMap[key] = obj;

            }
            else
            {

                EventCenter.Log(LOG_LEVEL.ERROR, "unkown type > " + type.ToString());

                mDataMap[key] = "UNKNOW";

                //throw new Exception("Error: not find data type field infomation.");
            }

        }
        return true;
    }
	
	public override void dump()
	{
		EventCenter.Self.Log("--------------------------------------------------------------------");
		foreach (KeyValuePair<string, object> it in mDataMap)
        {
            // save key id
            string info = "[";
            FIELD_TYPE type = (FIELD_TYPE)StaticDefine.GetType(it.Value);
            info += StaticDefine.ToStringType( type );
			info += "]:[";
            {
                info += it.Key + "] = ";
                info += type == FIELD_TYPE.FIELD_TABLE ? "TABLE" : it.Value.ToString();
            }           
            EventCenter.Self.Log( info );
#if UNITY_IPHONE || UNITY_ANDROID || DEBUG_IO
#else
            if (type == FIELD_TYPE.FIELD_TABLE)
            {
                NiceTable t = it.Value as NiceTable;
                if (t != null)
                    t.SaveTable(it.Key + "_.csv", Encoding.Default);
            }
#endif
		}
		EventCenter.Self.Log("--------------------------------------------------------------------");
	}

    public override bool FullData(object scrObject)
    {
        Type type = scrObject.GetType();

        FieldInfo[] fields = type.GetFields();
        foreach ( FieldInfo field in fields )
        {
            object objVal = field.GetValue(scrObject);

            int nType = StaticDefine.GetType(objVal);
            if (nType != (int)FIELD_TYPE.FIELD_NULL)
            {
                set(field.Name, objVal);
            }
        }
        return true;
    }

    public override bool RestoreToTarget(ref object targetObject)
    {
        Type type = targetObject.GetType();
        
         foreach (KeyValuePair<string, object> it in mDataMap)
         {
             FieldInfo field = type.GetField(it.Key);
             if (field != null)
             {
                 if (field.FieldType == it.Value.GetType())
                     field.SetValue(targetObject, it.Value);
             }
         }
        return true;
    }
};
//--------------------------------------------------------------------------------
#endif
//--------------------------------------------------------------------------------
public class StateData
{
    public StateData()
    {
        mStateData = new byte[1];
    }
    public StateData(uint count)
    {
        mStateData = new byte[(count + 7) / 8];
    }

    // return true only if this have other state.
    public bool Compare(StateData other)
    {
        if (other.mStateData.Length != mStateData.Length)
            return false;

        for (int i = 0; i < mStateData.Length; ++i)
        {
            byte x = mStateData[i];
            byte x2 = other.mStateData[i];
            if ( (mStateData[i] & other.mStateData[i])!=0 )
                return true;
        }

        return false;
    }

    public void InitClear()
    {
        for (int i = 0; i < mStateData.Length; ++i)
        {
            mStateData[i] = 0;
        }
    }

    public void InitOpen()
    {
        for (int i = 0; i < mStateData.Length; ++i)
        {
            mStateData[i] = 0xFF;
        }
    }

    public bool Set(int index, bool bOpen)
    {
        if (bOpen)
            return Open(index);
        else
            return Close(index);
        //int pos = index / 8;
        //int bit = index % 8;
        //if (pos >= mStateData.Length)
        //    return false;
        //// free space is 1.
        //if (!bOpen)
        //    mStateData[pos] |= (byte)(1 << bit);
        //else
        //{
        //    mStateData[pos] &= (byte)~(1 << bit);
        //}
        //return true;
    }

    public bool Open(int index)
    {
        int pos = index / 8;
        if (pos >= mStateData.Length)
            return false;
        int bit = index % 8;

        mStateData[pos] |= (byte)(1 << bit);

        return true;
    }

    public bool Close(int index)
    {
        int pos = index / 8;
        if (pos >= mStateData.Length)
            return false;
        int bit = index % 8;

        mStateData[pos] &= (byte)~(1 << bit);

        return true;
    }

    public bool IsOpen(int index)
    {
        int pos = index / 8;
        int bit = index % 8;
        if (pos >= mStateData.Length)
            return false;

        return (mStateData[pos] & (1 << bit)) != 0;
    }

    public bool HasOpen()
    {
        for (int i = 0; i < mStateData.Length; ++i )
        {
            if (mStateData[i] != 0)
                return true;
        }
        return false;
    }

    //---------------------------------------------
    protected byte[] mStateData;
}

//--------------------------------------------------------------------------------