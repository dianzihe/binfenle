using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTable
{
    //--------------------------------------------------------------------------------

    class TableFieldInfo : BaseFieldInfo
    {
        public FieldIndex mSubFieldIndex = null;

        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_TABLE; }

        public override bool saveParam(ref DataBuffer resultData)
        {
            if (mSubFieldIndex != null)
            {
                resultData.write(true);
                return mSubFieldIndex.Save(ref resultData);
            }
            else
                resultData.write(false);

            return true;
        }

        public override bool restoreParam(ref DataBuffer scrData)
        {
            bool bHave = false;
            if (!scrData.read(out bHave))
                return false;

            if (bHave)
            {
                if (mSubFieldIndex == null)
                    mSubFieldIndex = new FieldIndex();

                return mSubFieldIndex.Restore(ref scrData);
            }

            return false;            
        }
        //--------------------------------------------------------------------------------

        public override bool serialize(object data, ref DataBuffer resultData)
        {
            NiceTable d = (NiceTable)data;
            if (d != null)
            {
                resultData.write((bool)true);

                int checkCode = 0;
                resultData.write(checkCode);
                //resultData.writeOne("Nice");
                return d.serialize(ref resultData) > 0;
            }
            else
                resultData.write((bool)false);

            return true;
        }
        //--------------------------------------------------------------------------------

        public override bool restore(out object data, ref DataBuffer scrData)
        {
            bool bHave = false;
            if (scrData.read(out bHave))
            {
                if (bHave)
                {
                    int checkCode = 0;
                    if (!scrData.read(out checkCode))
                    {
						data = new NiceTable();
                        Logic.EventCenter.Log(true, "read table check code fail");
                        return false;
                    }
                    //string tableType;
                    //if (!scrData.readOne(out tableType))
                    //{
                    //    data = new NiceData();
                    //    return false;
                    //}
                    NiceTable d = new NiceTable();
                    if (d.restore(ref scrData))
                    {
                        data = d;
                        return true;
                    }
                }
				data = new NiceTable();
                return true;
            }
            data = new NiceTable();
            return false;
        }
        //--------------------------------------------------------------------------------

        public override bool set(ref object[] dataList, object obj)
        {
            if (mPosition < dataList.Length)
            {
                if (obj.GetType() == StaticDefine.ToObjectType(getType()))
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

    class TableFieldInfoFactory : tFieldInfoFactory
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_TABLE; }

        public override tFieldInfo NewFieldInfo()
        {
            return new TableFieldInfo();
        }
    };
    //--------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------

    class DataFieldInfo : BaseFieldInfo
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_DATA; }
        //--------------------------------------------------------------------------------

        public override bool serialize(object data, ref DataBuffer resultData)
        {
			//resultData.writeOne("Nice");
            DataBuffer d = (DataBuffer)data;
            resultData.write((uint)d.size());
            return resultData.write(d.getData())==d.size();
        }
        //--------------------------------------------------------------------------------

        public override bool restore(out object data, ref DataBuffer scrData)
        {
            uint size;
            scrData.read(out size);
            
            DataBuffer d = new DataBuffer((int)size);
            byte[] dd = scrData.read((uint)size);
            
            if (dd==null || dd.Length!=size)
			{
				data = null;
                return false;
			}
            d.write(dd);

            data = d;
            return true;
        }
        //--------------------------------------------------------------------------------

        public override bool set(ref object[] dataList, object obj)
        {
            if (mPosition < dataList.Length)
            {
                if (obj.GetType() == StaticDefine.ToObjectType(getType()))
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

    class DataFieldInfoFactory : tFieldInfoFactory
    {
        public override FIELD_TYPE getType() { return FIELD_TYPE.FIELD_DATA; }

        public override tFieldInfo NewFieldInfo()
        {
            return new DataFieldInfo();
        }
    };
    //--------------------------------------------------------------------------------
}
