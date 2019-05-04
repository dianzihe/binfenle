using UnityEngine;
using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace DataTable
{
	//--------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------
	public class FieldIndex
	{		
		tFieldInfo[]                    mFieldList = null;
		Dictionary<string, int>         mFieldMap = new Dictionary<string, int>();
		public NiceTable                mTable;
		//--------------------------------------------------------------------------------
		public tFieldInfo[] GetFieldList() { return mFieldList; }
		public int Count() { return mFieldMap.Count; }
		public bool Ready() { return mFieldList!=null && mFieldMap.Count > 0 && mFieldMap.Count == mFieldList.Length; }
		public NiceTable GetTable() { return mTable; }
		
		public void Reset(NiceTable table){ mFieldList = null; mFieldMap.Clear(); mTable = table; }
		//--------------------------------------------------------------------------------
		public bool SetField(string fieldName, FIELD_TYPE type, int col)
		{
			tFieldInfo info = FieldInfoManager.CreateFieldInfo(type);
			if (null==info)
				return false;
			
			info.setName(fieldName);
			info.setPosition(col);
			if (mFieldList == null)
				mFieldList = new tFieldInfo[col + 1];
			else if (col >= mFieldList.Length)
				Array.Resize<tFieldInfo>( ref mFieldList, (col + 1));
			mFieldList[col] = info;
			mFieldMap[fieldName] = col;
			return true;
		}
		
		public bool SetField(string fieldName, string typeName, int col)
		{
			FIELD_TYPE type = StaticDefine.ToType(typeName);
			if (type == FIELD_TYPE.FIELD_NULL)
				return false;
			
			return SetField(fieldName, type, col);
		}
		
		public tFieldInfo GetField(int col)
		{            
			if (col>=0 && null!=mFieldList && col < mFieldList.Length)
				return mFieldList[col];

			return null;
		}
		
		public tFieldInfo GetField(string fieldName)
		{           
			int col;
			if (mFieldMap.TryGetValue(fieldName, out col))
				return GetField(col);
			tFieldInfo info = null;
			return info;
		}
		
		public int GetCol(string fieldName)
		{
			int col = -1;
			if (!mFieldMap.TryGetValue(fieldName, out col))
				col = -1;
			return col;
		}

        public bool Save(ref DataBuffer resultData)
        {
            ushort count = (ushort)Count();
            resultData.write(count);
            for (int i = 0; i < count; ++i)
            {
                tFieldInfo t = GetField(i);
                byte b = (byte)t.getType();
                resultData.write(b);
                resultData.writeOne(t.getName());

                if (!t.saveParam(ref resultData))
                {
                    Logic.EventCenter.Log(LOG_LEVEL.ERROR, "save field param fail>" + t.getName());
                    return false;
                }
            }

            return true;
        }

        public bool Restore(ref DataBuffer scrData)
        {
            ushort count;
            if (!scrData.read(out count))
                return false;

            for (int i = 0; i < count; ++i)
            {
                byte b;
                if (!scrData.read(out b))
                    return false;
                string name;
                if (!scrData.readOne(out name))
                    return false;
                FIELD_TYPE type = (FIELD_TYPE)b;
                if (!SetField(name, type, i))
                {
                    Logic.EventCenter.Log(LOG_LEVEL.ERROR, "set field fail>"+name);
                    return false;
                }
                if (!mFieldList[i].restoreParam(ref scrData))
                {
                    Logic.EventCenter.Log(LOG_LEVEL.ERROR, "restore field param fail>" + name);
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            string strData = "";
			for (int i = 0; i < mFieldList.Length; ++i)
            {
                tFieldInfo t = GetField(i);                                  
				strData += "["+StaticDefine.ToStringType(t.getType()) + ":" + t.getName() + "]";
			}
            return strData;
        }
	}
	//--------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------
	public class DataRecord
	{
		FieldIndex  mFieldIndex;
		object[]    mDataList;
        //-------------------------------------------------------------------------
        public Data this[string indexName]
        {
            //实现索引器的get方法
            get
            {               
                return getData(indexName);
            }
            //实现索引器的set方法
            set
            {
                set(indexName, value.mObj);
            }
        }
		//--------------------------------------------------------------------------------
		public DataRecord()
		{
		}
		
		public DataRecord(FieldIndex fieldIndex)
		{
			SetFieldIndex( fieldIndex );
		}
		//--------------------------------------------------------------------------------
		public FieldIndex GetFieldIndex(){ return mFieldIndex; }
		
		public bool SetFieldIndex(FieldIndex fieldIndex) 
		{
			if (!fieldIndex.Ready())
				return false;
			
			mFieldIndex = fieldIndex;
			mDataList = new object[mFieldIndex.Count()];
			return true;
		}
		
		tFieldInfo GetFieldInfo(int col)
		{
			return mFieldIndex.GetField(col);
		}
		
		int GetFieldCol(string fieldName)
		{
			return mFieldIndex.GetCol(fieldName);
		}
		public object[] GetDataList(){ return mDataList; }
		
		public bool restore(ref DataBuffer data)
		{
			for (int i = 0; i < mFieldIndex.Count(); ++i)
			{
				tFieldInfo info = mFieldIndex.GetField(i);
				if (!info.restore(ref mDataList, ref data))
					return false;
			}
			return true;
		}

		//--------------------------------------------------------------------------------
		public Data getData(string colName)
		{
			return getData( GetFieldCol(colName) );
		}

		public Data getData(int col)
		{
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null != info)
			{
                return new Data(info.get(mDataList));
                //switch (info.getType())
                //{
                //case FIELD_TYPE.FIELD_INT:
                //{
                //    int v;
                //    if (info.get (mDataList, out v))
                //        value.mObj = v;
                //    break;
                //}
                //case FIELD_TYPE.FIELD_FLOAT:
                //{
                //    float v;
                //    if (info.get (mDataList, out v))
                //        value.mObj = v;
                //    break;
                //}
                //case FIELD_TYPE.FIELD_STRING:
                //{
                //    string v;
                //    if (info.get (mDataList, out v))
                //        value.mObj = v;
                //    break;
                //}
                //case FIELD_TYPE.FIELD_BOOL:
                //{
                //    int v;
                //    if (info.get (mDataList, out v))
                //        value.mObj = v;
                //    break;
                //}
                //default:
                //    Logic.EventCenter.Log(LOG_LEVEL.ERROR, "no exist type when get data from record");
                //    break;
                //}
			}

			return Data.NULL;
		}
		//--------------------------------------------------------------------------------
		
		public bool _set(int col, int nVal)
		{
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
				return false;            
			return info.set(ref mDataList, nVal);
		}
		
		public bool _set(int col, float fVal)
		{
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
				return false;
			return info.set(ref mDataList, fVal);
		}
		
		public bool _set(int col, string strVal)
		{
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
				return false;           
			return info.set(ref mDataList, strVal);
		}
		public bool _set(int col, object obj)
		{
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
				return false;            
			return info.set(ref mDataList, obj);
		}
		//--------------------------------------------------------------------------------
		
		public bool set(int col, int nVal)
		{           
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
				return false;
			mFieldIndex.GetTable().OnChanger(this, col, nVal);
			return info.set(ref mDataList, nVal);
		}
		
		public bool set(int col, float fVal)
		{
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
				return false;
			return info.set(ref mDataList, fVal);
		}
		
		public bool set(int col, string strVal)
		{            
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
				return false;
			mFieldIndex.GetTable().OnChanger(this, col, strVal);
			return info.set(ref mDataList, strVal);
		}
        public bool set(int col, UInt64 val)
        {
            tFieldInfo info = mFieldIndex.GetField(col);
            if (null == info)
                return false;
            mFieldIndex.GetTable().OnChanger(this, col, val);
            return info.set(ref mDataList, val);
        }
		public bool get(int col, out int nVal)
		{
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
			{
				nVal = 0;
				return false;
			}
			return info.get(mDataList, out nVal);
		}
		
		public bool get(int col, out float fVal)
		{
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
			{
				fVal = 0f;
				return false;
			}
			return info.get(mDataList, out fVal);
		}
		
		public bool get(int col, out string strVal)
		{
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
			{
				strVal = "";
				return false;
			}
			return info.get(mDataList, out strVal);
		}
        public bool get(int col, out UInt64 resultValue)
        {
            tFieldInfo info = mFieldIndex.GetField(col);
            if (null == info)
            {
                resultValue = 0;
                return false;
            }
            return info.get(mDataList, out resultValue);
        }
		public bool set(int col, object obj)
		{            
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
				return false;
			mFieldIndex.GetTable().OnChanger(this, col, obj);
			return info.set(ref mDataList, obj);
		}
		
		public object get(int col)
		{
			tFieldInfo info = mFieldIndex.GetField(col);
			if (null == info)
				return false;           
			return info.get(mDataList);
		}
		
		//------------------------------------------------------------------------
		public bool set(string fieldName, int nVal)
		{
			return set(GetFieldCol(fieldName), nVal);
		}
		
		public bool set(string fieldName, float fVal)
		{
			return set(GetFieldCol(fieldName), fVal);
		}
		
		public bool set(string fieldName, string strVal)
		{
			return set(GetFieldCol(fieldName), strVal);
		}
        public bool set(string fieldName, UInt64 val)
        {
            return set(GetFieldCol(fieldName), val);
        }
		public bool get(string fieldName, out int nVal)
		{
			return get(GetFieldCol(fieldName), out nVal);
		}
		
		public bool get(string fieldName, out float fVal)
		{
			return get(GetFieldCol(fieldName), out fVal);
		}
		
		public bool get(string fieldName, out string strVal)
		{
			return get(GetFieldCol(fieldName), out strVal);
		}
		
        public bool get(string fieldName, out UInt64 result)
        {
            return get(GetFieldCol(fieldName), out result);
        }

		public bool set(string fieldName, object obj)
		{
			return set(GetFieldCol(fieldName), obj);
		}
		
		public Data get(string fieldName)
		{            
			int col = GetFieldCol(fieldName);
			if (col<0)
			{
				Logic.EventCenter.Log(LOG_LEVEL.WARN, " without field >>> "+fieldName);
			}
			object dataObj = get(col);
			if (dataObj == null)
			{
				Logic.EventCenter.Log(LOG_LEVEL.WARN, " data object is null, col >>> "+fieldName );
			}
			return new Data(dataObj);
		}

        public object getObject(string fieldName)
        {
            int col = GetFieldCol(fieldName);
            if (col < 0)
            {
                Logic.EventCenter.Log(LOG_LEVEL.WARN, " without field >>> " + fieldName);
            }
            object dataObj = get(col);
            if (dataObj == null)
            {
                Logic.EventCenter.Log(LOG_LEVEL.WARN, " data object is null, col >>> " + fieldName);
            }
            return dataObj;
        }
		//-------------------------------------------------------------------------------
		
		public int getIndex()
		{
			tFieldInfo info = mFieldIndex.GetField(mFieldIndex.mTable.GetIndexCol());
			if (null != info)
				return  info.getIndex(mDataList);
			return 0;
		}
	}
	
	//--------------------------------------------------------------------------------
	//--------------------------------------------------------------------------------
	public class NiceTable
	{
		static public Encoding fileEncoding = Encoding.Unicode;
		static public bool SetFileEncoding(int codePage)
		{
			fileEncoding = Encoding.GetEncoding(codePage);
			return fileEncoding!=null;
		}
		static public void SetUnicodeEncoding()
		{
			fileEncoding = Encoding.Unicode;
		}
		
		Dictionary<int, DataRecord> mRecordMap = new Dictionary<int, DataRecord>();
		FieldIndex              mFieldIndex = new FieldIndex();
		int mIndexCol = 0;
		string mTableIndex = "NONE_C#";
		//--------------------------------------------------------------------------------
		public NiceTable()
		{
			mFieldIndex.mTable = this;
		}
		
		public void SetField(string fieldName, FIELD_TYPE type, int col)
		{
			mFieldIndex.SetField(fieldName, type, col);
		}
		
		public void SetField(string fieldName, string strType, int col)
		{
			mFieldIndex.SetField(fieldName, strType, col);
		}
		
		public void ResetField()
		{
			mRecordMap.Clear();
			mFieldIndex.Reset(this);
		}
		
		public FieldIndex GetField() { return mFieldIndex; }
		public Dictionary<int, DataRecord> GetAllRecord() { return mRecordMap; }
		public int GetRecordCount(){ return mRecordMap.Count; }
		public int GetIndexCol() { return mIndexCol; }

		public IEnumerable<DataRecord> Records ()
		{
			foreach(var recordPair in  mRecordMap)
				yield return recordPair.Value;
		}

        public Data GetData(string key, string field)
        {
            DataRecord re = GetRecord(key);
            if (re != null)
                return re.get(field);

            return new Data();
        }

        public Data GetData(int key, string field)
        {
            DataRecord re = GetRecord(key);
            if (re != null)
                return re.get(field);

            return new Data();
        }

        public override string ToString()
        {
            string strData = "{" + mFieldIndex.ToString() + "}";

            foreach (KeyValuePair<int, DataRecord> r in mRecordMap)
            {
                object[] d = r.Value.GetDataList();
                if (null != d)
                {
                    strData += "{";
                    for (int i = 0; i < d.Length; ++i)
                    {
                       
                        strData += "[";

                        if (d[i] != null)
                            strData += d[i].ToString();                     
						strData += "]";
                    }
                    strData += "}";
                }
            }

            return strData;
        }
		
		public void OnChanger(DataRecord record, int col, int destVal)
		{
			if (GetIndexCol() == col)
			{
				int index = record.getIndex();
				mRecordMap.Remove(index);
				mRecordMap[ConvertIndex(destVal)] = record;
			}
		}
		public void OnChanger(DataRecord record, int col, string destVal)
		{
			if (GetIndexCol() == col)
			{
				int index = record.getIndex();
				mRecordMap.Remove(index);
				mRecordMap[ConvertIndex( destVal ) ] = record;
			}
		}
		
		public void OnChanger(DataRecord record, int col, object destVal)
		{
			
		}
		//--------------------------------------------------------------------------------
		int ConvertIndex(int nIndex)
		{
			tFieldInfo info = mFieldIndex.GetField(GetIndexCol());
			if (null == info)
				return 0;
			return info.MakeIndex(nIndex);
		}
		
		int ConvertIndex(string strIndex)
		{
			tFieldInfo info = mFieldIndex.GetField(GetIndexCol());
			if (null == info)
				return 0;
			return info.MakeIndex(strIndex);
		}
		
		DataRecord _CreateRecord(int index)
		{
			DataRecord r = new DataRecord(mFieldIndex);
			mRecordMap[index] = r;
			return r;
		}
		
		public DataRecord CreateRecord(int index)
		{
			DataRecord r = _CreateRecord(ConvertIndex(index));
			r._set(GetIndexCol(), index);
			return r;
		}
		
		public DataRecord CreateRecord(string index)
		{
			DataRecord r = _CreateRecord(ConvertIndex(index));
			r._set(GetIndexCol(), index);
			return r;
		}
		
		public bool DeleteRecord(int index)
		{
			return mRecordMap.Remove(ConvertIndex(index));
		}
		
		public bool DeleteRecord(string index)
		{
			return mRecordMap.Remove(ConvertIndex(index));
		}
		
		public DataRecord GetRecord(int index)
		{
			DataRecord destRecord;
			mRecordMap.TryGetValue(ConvertIndex(index), out destRecord);
			return destRecord;
		}
		
		public DataRecord GetRecord(string index)
		{
			DataRecord destRecord;
			mRecordMap.TryGetValue(ConvertIndex(index), out destRecord);
			return destRecord;
		}

        public DataRecord GetFirstRecord()
        {
            foreach (KeyValuePair<int, DataRecord> kRe in mRecordMap)
            {
                return kRe.Value;
            }
            return null;
        }
		
		public bool SaveTable(string pathFileName)
		{
			return SaveTable(pathFileName, fileEncoding);
		}
		
		public bool SaveTable(string pathFileName, Encoding encoding)
		{
			FileStream fs = new FileStream(pathFileName, FileMode.Create);
			return SaveTable(fs, encoding);
		}
		
		public bool SaveTable(ref DataBuffer resultBuffer, Encoding encoding)
		{
            if (!mFieldIndex.Ready())
                return true;

            try
            {

                for (int i = 0; i < mFieldIndex.Count(); ++i)
                {
                    tFieldInfo info = mFieldIndex.GetField(i);
                    string typeName = StaticDefine.ToStringType(info.getType());
                    resultBuffer._writeString(typeName);
                    if (i < mFieldIndex.Count() - 1)
                        resultBuffer._writeString(",");
                }
               resultBuffer._writeString("\n");
                for (int i = 0; i < mFieldIndex.Count(); ++i)
                {
                    tFieldInfo info = mFieldIndex.GetField(i);
                    string name = info.getName();
                   resultBuffer._writeString(name);
                    if (i < mFieldIndex.Count() - 1)
                       resultBuffer._writeString(",");
                }
               resultBuffer._writeString("\n");
                foreach (KeyValuePair<int, DataRecord> r in mRecordMap)
                {
                    object[] d = r.Value.GetDataList();
                    if (null != d)
                    {
                        for (int i = 0; i < d.Length; ++i)
                        {
                            if (d[i] != null)
                               resultBuffer._writeString(d[i].ToString());
                            if (i < d.Length - 1)
                               resultBuffer._writeString(",");
                        }
                       resultBuffer._writeString("\n");
                    }
                }
               
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return false;
            }
            return true;
            return false;
		}
		
		public bool SaveTable(Stream resultData, Encoding encoding)
		{
			if (!mFieldIndex.Ready())
				return true;
			
			try
			{
				//StreamWriter writer = new StreamWriter(pathFileName, false, encoding);
				StreamWriter writer = new StreamWriter(resultData, encoding);        
				
				for (int i = 0; i < mFieldIndex.Count(); ++i)
				{
					tFieldInfo info = mFieldIndex.GetField(i);
					string typeName = StaticDefine.ToStringType(info.getType());
					writer.Write(typeName);
					if (i<mFieldIndex.Count()-1)
						writer.Write(',');
				}
				writer.Write("\n");
				for (int i = 0; i < mFieldIndex.Count(); ++i)
				{
					tFieldInfo info = mFieldIndex.GetField(i);
					string name = info.getName();
					writer.Write(name);
					if (i < mFieldIndex.Count() - 1)
						writer.Write(',');
				}
				writer.Write("\n");
				foreach (KeyValuePair<int, DataRecord> r in mRecordMap)
				{
					object[] d = r.Value.GetDataList();
					if (null != d)
					{
						for(int i=0; i<d.Length; ++i)
						{
							if (d[i]!=null)
								writer.Write(d[i].ToString());
							if (i < d.Length - 1)
								writer.Write(',');
						}
						writer.Write("\n");
					}
				}
				writer.Close();
			}
			catch (Exception e)
			{
				Log(e.ToString());
				return false;
			}            
			return true;
		}
		
		public bool LoadTable(string pathFileName)
		{
			return LoadTable(pathFileName, fileEncoding);
		}

        public bool LoadTable(string pathFileName, Encoding encoding)
        {
            FileStream fs = new FileStream(pathFileName, FileMode.Open);

            return LoadTable(fs, encoding);
        }
        /*
        public bool LoadTable(string pathFileName, Encoding encoding, LOAD_MODE loadMode)
        {
            FileStream fs = new FileStream(pathFileName, FileMode.Open);

            if (loadMode == LOAD_MODE.ANIS)
                return LoadTableOld(fs, encoding);
            if (loadMode == LOAD_MODE.UNICODE)
                return LoadTable(fs, encoding);
            return LoadTable(fs, encoding);
        }
		*/
		public bool LoadTable(DataBuffer data, Encoding encoding)
		{
			Stream stream = new MemoryStream(data.mData);
			return LoadTable(stream, encoding);
		}

		// read stream
		public bool LoadTable(Stream dataStream, Encoding encoding)
		{
			ResetField();
			try
			{
				//StreamReader reader = new StreamReader(pathFileName, encoding, false);
				StreamReader reader = new StreamReader(dataStream, encoding);
				string s = "";
				if ((s = reader.ReadLine()) == null)
					return true;
				
				char[] pa = { ',' };
                // get field name            
                string[] nameList = s.Split(pa);
                if (nameList.Length <= 0)
					return true;
				if ((s = reader.ReadLine()) == null)
					return true;
                // get field type            
                string[] typeList = s.Split(pa);
				if (typeList.Length!=nameList.Length)
					return false;
				
				ResetField();
				
				for (int i = 0; i < typeList.Length; ++i)
				{                
					if (!mFieldIndex.SetField(nameList[i], typeList[i], i))
					{
						Log("ERROR: load field type and name fail > "+nameList[i] + " > Type>"+typeList[i]);
						return false;
					}
				}
				while ((s = reader.ReadLine()) != null)
				{
					string[] dataList = s.Split(pa);
					if (dataList.Length!=typeList.Length)
					{
						Log("WARN: data record col count is not same with type count");
						return true;
					}                    
                    string key = dataList[GetIndexCol()];
                    if (key != "")
                    {
                        DataRecord newRe = CreateRecord(key);
                        for (int i = 0; i < dataList.Length; ++i)
                        {
                            if (i != GetIndexCol())
                            {
                                if (!newRe.set(i, dataList[i]))
                                {
                                    Log(" record set data fail");
                                    return false;
                                }
                            }
                        }
                    }
				}
				reader.Close();
			}
			catch(Exception e)
			{
				Log( e.ToString() );
				return false;
			}
			
			return true;
		}

        // read stream
        public bool LoadTableOld(Stream dataStream, Encoding encoding)
        {
            ResetField();
            try
            {
                //StreamReader reader = new StreamReader(pathFileName, encoding, false);
                StreamReader reader = new StreamReader(dataStream, encoding);
                string s = "";
                if ((s = reader.ReadLine()) == null)
                    return true;

                char[] pa = { ',' };
                // get field type
                string[] typeList = s.Split(pa);
                if (typeList.Length <= 0)
                    return true;
                if ((s = reader.ReadLine()) == null)
                    return true;
                // get field name            
                string[] nameList = s.Split(pa);
                if (typeList.Length != nameList.Length)
                    return false;

                ResetField();

                for (int i = 0; i < typeList.Length; ++i)
                {
                    if (!mFieldIndex.SetField(nameList[i], typeList[i], i))
                    {
                        Log("ERROR: load field type and name fail > " + nameList[i] + " > Type>" + typeList[i]);
                        return false;
                    }
                }
                while ((s = reader.ReadLine()) != null)
                {
                    string[] dataList = s.Split(pa);
                    if (dataList.Length != typeList.Length)
                    {
                        Log("WARN: data record col count is not same with type count");
                        return true;
                    }
                    string key = dataList[GetIndexCol()];
                    if (key != "")
                    {
                        DataRecord newRe = CreateRecord(key);
                        for (int i = 0; i < dataList.Length; ++i)
                        {
                            if (i != GetIndexCol())
                            {
                                if (!newRe.set(i, dataList[i]))
                                {
                                    Log(" record set data fail");
                                    return false;
                                }
                            }
                        }
                    }
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return false;
            }

            return true;
        }
		
		public bool SaveBinary(string pathFile)
		{
			DataBuffer data = new DataBuffer();
			if (serialize(ref data)>0)
			{								
				if (File.Exists(pathFile))
					File.Delete(pathFile);
				
				FileStream fs = new FileStream(pathFile, FileMode.CreateNew);
				fs.Write(data.getData(), 0, data.tell());
				
				fs.Close();
				return true;
			}
			return false;
		}
		
		public bool LoadBinary(string pathFile)
		{
			if (!File.Exists(pathFile))
			{
				LOG.logError("Table file no exist>"+pathFile);
				return false;
			}
			bool re = false;
			FileStream fs = new FileStream(pathFile, FileMode.Open, FileAccess.Read);
			DataBuffer data = new DataBuffer((int)fs.Length);
			if (fs.Read(data.getData(), 0, (int)fs.Length) == fs.Length)
			{
				re = restore(ref data);                
			}            
			fs.Close();
			return re;
		}

        enum TABLE_SAVE_TYPE
        {
            eSaveAll = 0,
            eOnlySaveRecordData = 1,
            eOnlyUpdateRecordData = 2,
        };

		public int serialize(ref DataBuffer data)
		{
			if (!mFieldIndex.Ready())
			{
				return 0;
			}
			int beforeSize = data.tell();

            byte dataType = (byte)TABLE_SAVE_TYPE.eSaveAll;
            data.write(dataType);
			// write table name info
			if (!data.writeOne(mTableIndex))
				return 0;
			// field info index
            if (!mFieldIndex.Save(ref data))
            {
                Log("ERROR: save field data fail>" + mTableIndex);
                return 0;
            }
			// record 
			// record num
			uint recordCount = (uint)mRecordMap.Count;
			data.write(recordCount);
			// record data
			tFieldInfo[] fieldList = mFieldIndex.GetFieldList();
			foreach(KeyValuePair<int, DataRecord> rv in mRecordMap)
			{
				DataRecord r = rv.Value;
				if (r.GetDataList().Length==fieldList.Length)
				{
					for (int i=0; i<fieldList.Length; ++i)
					{
						try
						{
							if (!fieldList[i].serialize(r.GetDataList(), ref data))
							{
								Log("Error : serialize record fail >>> " + rv.Key.ToString());
								return 0;
							}
						}
						catch (Exception e)
						{
							Log("Error : serialize record fail >>> " + e.ToString());
							return 0;
						}
					}
				}
				else
					Log("Error : serialize table have a record data count is not same of field count>>>"+rv.Key.ToString());
			}
			return data.tell()-beforeSize;
		}
		
		void Log(string strInfo)
		{
			Logic.EventCenter.Self.Log(strInfo);            
		}
		
		public bool restore(ref DataBuffer data)
		{
            byte dataType = (byte)TABLE_SAVE_TYPE.eSaveAll;
            if (!data.read(out dataType))
                return false;

            if (dataType!=(byte)TABLE_SAVE_TYPE.eSaveAll)
            {
                Log("ERROR: Must table data type is TABLE_SAVE_TYPE.eSaveAll");
                return false;
            }

			ResetField();
			// read table index;
			if (!data.readOne(out mTableIndex))
				return false;
			
			// read field
            mFieldIndex.Reset(this);
            if (!mFieldIndex.Restore(ref data))
            {
                Log("ERROR: Field restore fail>" + mTableIndex);
                return false;
            }
			
			// read record
			uint recordCount;
			if (!data.read(out recordCount))
				return false;

			for (int i = 0; i < recordCount; ++i)
			{
				DataRecord re = new DataRecord(mFieldIndex);
				if (!re.restore(ref data))
					return false;

                mRecordMap[re.getIndex()] = re;
			}
			return true;
		}
	}
	//--------------------------------------------------------------------------------
}
