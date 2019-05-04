
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using DataTable;
//--------------------------------------------------------------------------------------------------

public class CommonParam
{
#if UNITY_IPHONE || UNITY_ANDROID || DEBUG_IO
    static public bool RunOnWindow = false;
#else
    static public bool RunOnWindow = true;
#endif

#if UNITY_EDITOR
    static public string ResourceTheater = "default";
#elif UNITY_IPHONE
    static public string ResourceTheater = "iphdefault";
#elif UNITY_ANDROID
    static public string ResourceTheater = "anddefault";
#else 
    static public string ResourceTheater = "default";
#endif
    static public string UIRootName = "UI Root";
    //static public bool IsSaveByte = false;
    static public bool UseUpdate = true;
    static public bool NeedLog = true;  // read from GlobalConfig.csv > LOG YES or NO
    static public bool LogOnScreen = false; // read from GlobalConfig.csv > LOG_ON_SCREEN YES or NO
    static public bool LogOnConsole = false; // read from GlobalConfig.csv > LOG_ON_CONSOLE YES or NO
    static public bool UseSDK = false;

    static public string ClientVer = "1.01";
    static public int SelfCamp = 1000;
    static public int EnemyCamp = 2000;
	static public int PlayerLayer = 8;
    static public int MonsterLayer = 9;
    static public int OutlineLayer = 30;
	static public int ObstructLayer = 11;
    static public int GroundLayer = 12;
	static public int EffectLayer = 13;
	static public int AbsorptionLayer = 14;
    static public int CharacterLayer = 15;
    static public int UILayer = 20;
    static public int UI_3D = 30;

    static public float DefaultVolume = 1;
    static public float MusicVolume = 0.4f;

    static public float FollowBound = 3.5f;
    static public float LeaveBound = 12.0f;
	static public float BirthBound = 16.0f;
    static public bool StartBirth = false;

	static public float MiniMapMaskSize = 128.0f;
	static public float CreateMiniMapSize = 192.0f;
	static public float MonsterPointTextureSize = 16.0f;
    static public int MonsterCamp = 2000;
    static public int PlayerCamp = 1000;
    static public int PVP_PET_START_POS = 16;
	static public bool bIsNetworkGame = true;

    static public NiceTable mConfig;


    static public Int64 mServerTime = 0;
    static public Int64 mServerTimeOff = 0;

	//static public MAIN_WINDOW_INDEX OpenWindowIndex = MAIN_WINDOW_INDEX.None_Window;

    //static public STAGE_TYPE mCurrentLevelType = STAGE_TYPE.MAIN_COMMON;
    //static public ELEMENT_TYPE mCurrentLevelElement = ELEMENT_TYPE.GREEN;

    static public string mAccount = "";
    static public string mStrToken = "";
    static public string mUId = "";
    static public string mZoneID = "1";
    static public string LoginIP = "";
    static public string LoginPort = "";
    static public string GameServerIP = "";
    static public string GameServerPort = "";
    static public string ChatServerIP = "";
    static public string ChatServerPort = "";
    /*
    static public void InitConfig()
    {
        mConfig = TableManager.GetTable("Global");
        if (mConfig==null)
        {
            Logic.EventCenter.Log(true, "Global config table is not exist");
            return;
        }
        FollowBound = GetConfig( "FOLLOW_BOUND" );
        LeaveBound = GetConfig( "LEAVE_BOUND" );

        LoginIP = mConfig.GetData("LOGIN_IP", "VALUE");
        LoginPort = mConfig.GetData("LOGIN_PORT", "VALUE");
        ChatServerIP = mConfig.GetData("CHAT_IP", "VALUE");
        ChatServerPort = mConfig.GetData("CHAT_PORT", "VALUE");
    }

    public static Data GetConfig(string key)
    {
        DataRecord re =  mConfig.GetRecord(key);
        if (re!=null)
            return re.getData("VALUE");
		Logic.EventCenter.Log(true, "Global config table is not exist config >"+key);
        return Data.NULL;
    }
    */
    public static void SetServerTime(UInt64 serverTime)
    {
        mServerTime = (Int64)serverTime;
        mServerTimeOff = (Int64)serverTime - (Int64)Time.realtimeSinceStartup;
    }

    static public Int64 NowServerTime()
    {
		return (Int64)Time.realtimeSinceStartup + mServerTimeOff;
    }
}
//-------------------------------------------------------------------------

public class LOG
{
    static public void Log(string info)
    {
        Logic.EventCenter.Log(LOG_LEVEL.GENERAL, info);
        /*
        if (CommonParam.NeedLog)
        {
            Logic.EventCenter.Log(LOG_LEVEL.GENERAL, info);
        }
        
        if (CommonParam.LogOnScreen)
        {
            ScreenLogger.Log(info, LOG_LEVEL.GENERAL);
        }

        if(CommonParam.LogOnConsole)
        {
            Debug.Log(info);
        }
        */
    }

    static public void logError(string info)
    {
        Logic.EventCenter.Log(LOG_LEVEL.ERROR, info);
        Debug.LogError(info);
        /*
        if (CommonParam.NeedLog)
        {
            Logic.EventCenter.Log(LOG_LEVEL.ERROR, info);
            Debug.LogError(info);
        }

        if (CommonParam.LogOnScreen)
        {
            ScreenLogger.Log(info, LOG_LEVEL.ERROR);
        }

        if (CommonParam.LogOnConsole)
        {
            Debug.LogError(info);
        }
        */
    }

    static public void logWarn(string info)
    {
        Logic.EventCenter.Log(LOG_LEVEL.WARN, info);
        /*
        if (CommonParam.NeedLog)
        {
            Logic.EventCenter.Log(LOG_LEVEL.WARN, info);
        }

        if (CommonParam.LogOnScreen)
        {
            ScreenLogger.Log(info, LOG_LEVEL.WARN);
        }

        if (CommonParam.LogOnConsole)
        {
            Debug.LogWarning(info);
        }
        */
    }
}
//-------------------------------------------------------------------------
public class CallBack
{
    public object mObject;
    public string mFunction;
    public object mParam;

    public CallBack(object obj, string fun, object param)
    {
        mObject = obj;
        mFunction = fun;
        mParam = param;
    }

    public object Run()
    {
        if (mObject == null)
        {
            LOG.logError("CallBack object is null");
            return null;
        }
        System.Reflection.MethodInfo info = mObject.GetType().GetMethod(mFunction);
        if (info != null)
        {
            object[] p = new object[1];
            p[0] = mParam;
            try
            {
                return info.Invoke(mObject, p);
            }
            catch
            {
				LOG.logError( "CallBack object function param is error >" + mFunction);
                return null;
            }
        }
        else
        {
			LOG.logError( "CallBack object no exist function >"+mFunction);
            return null;
        }

        return null;
    }
}
public class GameCommon
{
    public enum ResourceType
    {
        RES_GENERIC = 0,
        RES_PREFAB = 1,
        RES_TEXTURE = 2,
        RES_MODEL = 3
    }

    public enum ResourceGroup
    {
        RESG_ALL = 0,
        RESG_UI = 1,
        RESG_EFFECT = 2
    }
    public static string MakeGamePathFileName(string strFileName)
    {
        string fileName = "";

#if !(UNITY_EDITOR || UNITY_STANDALONE)
	    fileName += Application.persistentDataPath + "/" + strFileName ;
#else
        fileName += Application.dataPath + "/" + strFileName;
#endif
        return fileName;
    }

}
#if false
//-------------------------------------------------------------------------
public class GameCommon
{
    public enum ResourceType
    {
        RES_GENERIC     = 0,
        RES_PREFAB      = 1,
        RES_TEXTURE     = 2,
        RES_MODEL       = 3
    }

    public enum ResourceGroup
    {
        RESG_ALL        = 0,
        RESG_UI         = 1,
        RESG_EFFECT     = 2
    }

    /// <summary>
    /// NOTE: DO NOT use mResources outside of resource manager; use GameCommon's public method instead
    /// </summary>
    static public MResources mResources = new MResources();
	static public GameObject FindObject(GameObject parentObject, string targetObjName)
	{
		if (parentObject==null)
			return null;

		foreach( Transform f in parentObject.transform)
		{
			string n = f.gameObject.name;

			if (n==targetObjName)
				return f.gameObject;

			GameObject o = GameCommon.FindObject(f.gameObject, targetObjName);
			if (o!=null)
				return o;
		}	

		return null;
	}

    static public void SetLayer(GameObject gameObject, int layer)
    {
        if (gameObject==null)
			return;

        gameObject.layer = layer;

		foreach( Transform f in gameObject.transform)
		{			
            SetLayer(f.gameObject, layer);
        }
    }


	static public string MakeResourceName(string resName)
	{
		string temp = resName.ToLower();
		char[] p = { '/' };
		string[] str = temp.Split(p);
		if (str[0]=="resources")
			temp = resName.Substring(10, resName.Length-10);
		string path = Path.GetDirectoryName(temp);
		return path + "/" + Path.GetFileNameWithoutExtension(temp);
	}

    public static void Swap<T>(ref T t1, ref T t2)
    {
        T t = t1;
        t1 = t2;
        t2 = t;
    }

    static public bool InBound(float scrX, float scrY, float destX1, float destY1, float destX2, float destY2)
    {
        return scrX >= destX1
            && scrX <= destX2
            && scrY >= destY1
            && scrY <= destY2;
    }

    static public bool BoundIntersect(float scrX1, float scrY1, float scrX2, float scrY2,  float destX1, float destY1, float destX2, float destY2)
    {
        return !
            (scrX1 > destX2
            || scrX2 < destX1
            || scrY1 > destY2
            || scrY2 < destY1
            );
    }


    public static string MakePathFileNameForWWW(string strFileName)
    {
        string fileName = "file://";

#if !(UNITY_EDITOR || UNITY_STANDALONE)
	    fileName += Application.persistentDataPath + "/" + strFileName ;
#else
        fileName += Application.dataPath + "/" + strFileName;
#endif
        return fileName;
    }
	
	public static string MakeGamePathFileName(string strFileName)
    {
        string fileName = "";

#if !(UNITY_EDITOR || UNITY_STANDALONE)
	    fileName += Application.persistentDataPath + "/" + strFileName ;
#else
        fileName += Application.dataPath + "/" + strFileName;
#endif
        return fileName;
    }

	public static void SaveUsrLoginDataFromUnity(string usrName, string pw) {
		if(!string.IsNullOrEmpty(usrName)) {
			PlayerPrefs.SetString("USRNAME", usrName);
		}else {
			Debug.Log("Save USRNAME is Null!");
		}

		if(!string.IsNullOrEmpty(pw)) {
			PlayerPrefs.SetString("PW", pw);
		}else {
			Debug.Log("Save PW is Null!");
		}
		PlayerPrefs.Save();
	}

	public static string[] GetSavedLoginDataFromUnity() {
		string[] data = new string[2];
		data[0] = PlayerPrefs.GetString("USRNAME");
		data[1] = PlayerPrefs.GetString("PW");
		return data;
	}

    public static void ReadyMesh(ref Mesh mesh, bool bCenter)
    {
        // ready mesh.
        Vector3[] vec = new Vector3[4];
       
        float zF = -0.5f;
        float iF = 0.5f;

        if (!bCenter)
        {
            zF = 0;
            iF = 1;
        }

        //zF = (zF - 0.5f) * scale;
        //iF = (iF - 0.5f) * scale;
        vec[0] = new Vector3(zF, zF, 0.0f);
        vec[1] = new Vector3(iF, zF, 0.0f);
        vec[2] = new Vector3(zF, iF, 0.0f);
        vec[3] = new Vector3(iF, iF, 0.0f);

        Color[] color = new Color[4];
        color[0] = Color.white;
        color[1] = Color.white;
        color[2] = Color.white;
        color[3] = Color.white;

        int[] tg = { 2, 1, 0, 3, 1, 2 }; // 0, 1, 2, 2, 1, 3 };

        mesh.vertices = vec;        
        mesh.triangles = tg;

        //if (bUV)
        {
			Vector2[] uv = new Vector2[4];
            float u2 = 1.0f;
            float v2 = 1.0f;
			zF = 0.0f;
            uv[0] = new Vector2(zF, zF);
            uv[1] = new Vector2(u2, zF);
            uv[2] = new Vector2(zF, v2);
            uv[3] = new Vector2(u2, v2);

            mesh.uv = uv;
        }

        
        Vector3[] normal = new Vector3[4];
        for (int i=0; i<4; ++i)
        {
            normal[i] = Vector3.forward;
        }
        mesh.normals = normal;

        mesh.colors = color;
    }

    public static Vector4[] ReadyTextureAnimaiton(string textureFileName, string xmlDataFile, out Texture destTexture, out float SpiritWidth, out float SpiritHeight)
    {
        destTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false, false);
        destTexture.wrapMode = TextureWrapMode.Clamp;
        string fileName = MakePathFileNameForWWW(textureFileName);
        WWW z = new WWW(fileName);
        z.LoadImageIntoTexture((Texture2D)destTexture);
		
		return LoadUVAnimation(destTexture, xmlDataFile, out SpiritWidth, out SpiritHeight);
	}
	public static Vector4[] LoadUVAnimation(Texture tex, string xmlDataFile, out float SpiritWidth, out float SpiritHeight)
	{
        SpiritWidth = 0;
        SpiritHeight = 0;
        Vector4[] uvInfo = null;
        XmlDocument xml = new XmlDocument();
        try
        {
            string xmlFileName = MakePathFileNameForWWW(xmlDataFile);
            xml.Load(xmlFileName);
            XmlNodeList node = xml.GetElementsByTagName("Frame");

            float width = (float)tex.width;
            float height = (float)tex.height;

            int mCount = node.Count;

            if (mCount <= 0)
                return uvInfo;

            uvInfo = new Vector4[mCount];
            for (int i = 0; i < mCount; ++i)
            {
                XmlNode n = node[i];
                int h = int.Parse(n.Attributes[0].Value);
                int w = int.Parse(n.Attributes[2].Value);
                int x = int.Parse(n.Attributes[3].Value);
                int y = int.Parse(n.Attributes[4].Value);

                uvInfo[i].x = (float)x;
                uvInfo[i].y = (float)y;

                uvInfo[i].w = (float)w;
                uvInfo[i].z = (float)h;
            }
            SpiritWidth = uvInfo[0].w;
            SpiritHeight = uvInfo[0].z;
            for (int i = 0; i < mCount; ++i)
            {
                uvInfo[i].w = (uvInfo[i].x + uvInfo[i].w) / width;
                uvInfo[i].z = (height - (uvInfo[i].y + uvInfo[i].z)) / height;
                uvInfo[i].x /= width;
                uvInfo[i].y = (height - uvInfo[i].y) / height;
            }
            return uvInfo;
        }
        catch (System.Exception e)
        {
            uvInfo = null;
            Logic.EventCenter.Log(LOG_LEVEL.GENERAL, e.ToString());
        }
        return uvInfo;
    }

    public static Vector4 ReadyTextureUV(string xmlDataFile, int pos, float texWidth, float texHeight, out float SpiritWidth, out float SpiritHeight)
    {
        SpiritWidth = 0;
        SpiritHeight = 0;
        Vector4 uvInfo = new Vector4();
        XmlDocument xml = new XmlDocument();
        try
        {
            string xmlFileName = MakePathFileNameForWWW(xmlDataFile);
            xml.Load(xmlFileName);
            XmlNodeList node = xml.GetElementsByTagName("Frame");

            float width = texWidth;
            float height = texHeight;

            int mCount = node.Count;

            if (mCount <= pos)
                return uvInfo;
           
            XmlNode n = node[pos];
            int h = int.Parse(n.Attributes[4].Value);
            int w = int.Parse(n.Attributes[3].Value);
            int x = int.Parse(n.Attributes[1].Value);
            int y = int.Parse(n.Attributes[2].Value);

            uvInfo.x = (float)x;
            uvInfo.y = (float)y;

            uvInfo.w = (float)w;
            uvInfo.z = (float)h;

            SpiritWidth = uvInfo.w;
            SpiritHeight = uvInfo.z;
            uvInfo.w = (uvInfo.x + uvInfo.w) / width;
            uvInfo.z = (height - (uvInfo.y + uvInfo.z)) / height;
            uvInfo.x /= width;
            uvInfo.y = (height - uvInfo.y) / height;

            return uvInfo;
        }
        catch (System.Exception e)
        {           
            Debug.Log(e.ToString());
        }
        return uvInfo;
    }

    public static void SetMeshUV(ref Mesh mesh, Vector4 uv)
    {
        Vector2[] mMeshUV = mesh.uv;

        mMeshUV[0].x = uv.x;
        mMeshUV[0].y = uv.y;

        mMeshUV[1].x = uv.z;
        mMeshUV[1].y = uv.y;

        mMeshUV[2].x = uv.x;
        mMeshUV[2].y = uv.w;

        mMeshUV[3].x = uv.z;
        mMeshUV[3].y = uv.w;

        mesh.uv = mMeshUV;
    }

    //public static Texture LoadTexture(string textureFileName)
    //{
    //    Texture destTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false, false);
    //    destTexture.wrapMode = TextureWrapMode.Clamp;
    //    string fileName = MakePathFileNameForWWW(textureFileName);
    //    WWW z = new WWW(fileName);
    //    z.LoadImageIntoTexture((Texture2D)destTexture);
    //    return destTexture;
    //}

    // auto load resources, first try load from file data (run dir for update), then load from resources.
    public static DataBuffer LoadData(string fileName, LOAD_MODE loadMode)
    {
        DataBuffer data = null;
        switch(loadMode)
        {
            case LOAD_MODE.BYTES:
                /*
                if (RuntimePlatform.WindowsWebPlayer != Application.platform)
                {
                    Game.Resource res = Game.ResourceManager.Self.GetResource(fileName, "FILE");
                    data = res.ReadData();
                }
                */
                break;

            case LOAD_MODE.BYTES_RES:
			{
                Game.Resource res = Game.ResourceManager.Self.GetResource(fileName, "UNITY");
                data = res.ReadData();
                break;
			}
            case LOAD_MODE.BYTES_TRY_PATH:
			{
                Game.Resource res = Game.ResourceManager.Self.GetResource(fileName, "FILE");
                data = res.ReadData();
                if (data==null)
                {
                    res = Game.ResourceManager.Self.GetResource(fileName, "UNITY");
                    data = res.ReadData();
                }
                break;
			}
            case LOAD_MODE.BYTES_TRY_RES:
			{
                Game.Resource res = Game.ResourceManager.Self.GetResource(fileName, "UNITY");
                data = res.ReadData();
                if (data==null)
                {
                    res = Game.ResourceManager.Self.GetResource(fileName, "FILE");
                    data = res.ReadData();
                }
                break;
			}
        }
        return data;
    }

    //public static Texture LoadTextureFromResources(string textureFile)
    //{
    //    char[] pa = { '.' };
    //    string[] re = textureFile.Split(pa);
    //    if (re.Length >= 1)
    //    {
    //        //Texture2D destTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false, false);
    //        UnityEngine.Object obj = Resources.Load(re[0], typeof(Texture));
    //        return obj as Texture;
    //    }
    //    return null;
    //}

    static public Texture LoadTexture(string textureFile)
    {
        return mResources.LoadTexture(textureFile);
        //return Resources.Load(textureFile, typeof(Texture)) as Texture;
        //return Resources.Load<Texture>(textureFile);
    }

    public static Texture LoadTexture(string textureFile, LOAD_MODE loadMode)
    {
        Texture2D destTexture = null;

        switch (loadMode)
        {
            case LOAD_MODE.SCR_FILE:
            case LOAD_MODE.WWW:
                {
                    destTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false, false);
                    destTexture.wrapMode = TextureWrapMode.Clamp;
                    string fileName = MakePathFileNameForWWW(textureFile);
                    WWW z = new WWW(fileName);
                    z.LoadImageIntoTexture((Texture2D)destTexture);
                }
                break;
            case LOAD_MODE.RESOURCE:
                {								
					destTexture = LoadTexture(MakeResourceName(textureFile)) as Texture2D;
					if (destTexture!=null)
                     	destTexture.wrapMode = TextureWrapMode.Clamp;                    
					else 
					{
						int b = 2;
					}
                }
                break;
			
            default:
                DataBuffer data = LoadData(textureFile, loadMode);
                if (data != null)
                {
                    destTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false, false);

                    destTexture.LoadImage(data.getData());
                    destTexture.wrapMode = TextureWrapMode.Clamp;
                    Logic.EventCenter.Log(false, "succeed load texture >" + textureFile);
                    return destTexture;
                }
                else
                    Logic.EventCenter.Log(true, "fail load textrue >" + textureFile);
			break;
        }
        return destTexture;
    }

    public static GameObject LoadModel(string resourcePath)
    {
        UnityEngine.Object obj = mResources.LoadModel(resourcePath);
        if(obj != null)
            return GameObject.Instantiate(obj) as GameObject;

        return CreateObject(resourcePath, ResourceType.RES_MODEL);
    }

    public static RuntimeAnimatorController LoadController(string resourcePath)
    {
        var obj = mResources.LoadController(resourcePath);
        if(obj != null)
            return obj;

        return Resources.Load(resourcePath, typeof(RuntimeAnimatorController)) as RuntimeAnimatorController;
    }

    //public static bool LoadTableOnUnity(string tableName, out NiceTable destTable)
    //{
    //    destTable = new NiceTable();
    //    UnityEngine.Object obj = Resources.Load(tableName, typeof(TextAsset));
    //    if (obj == null)
    //        return false;

    //    TextAsset byteTextAsset = (TextAsset)obj;
    //    DataBuffer data = new DataBuffer();
    //    data._setData(byteTextAsset.bytes);
    //    bool b = destTable.restore(ref data);        
    //    Resources.UnloadUnusedAssets();
    //    return b;
    //}

    public static NiceTable LoadTable(string tableName, LOAD_MODE loadMode)
    {        
        NiceTable   resultTable = new NiceTable();
        bool bResult = false;
        switch (loadMode)
        {
            case LOAD_MODE.UNICODE:
                bResult = resultTable.LoadTable(GameCommon.MakeGamePathFileName(tableName), System.Text.Encoding.UTF8, loadMode);
            break;

            case LOAD_MODE.ANIS:
                bResult = resultTable.LoadTable(GameCommon.MakeGamePathFileName(tableName), System.Text.Encoding.Default, loadMode);
            break;

            case LOAD_MODE.BYTES:
            case LOAD_MODE.BYTES_RES:
            case LOAD_MODE.BYTES_TRY_PATH:
            case LOAD_MODE.BYTES_TRY_RES:                
                DataBuffer data = LoadData(tableName, loadMode);
                bResult = resultTable.restore(ref data);

            break;
        }
        //DataBuffer data = AutoLoadData(tableName);
        //if (null != data)
        //{
        //    NiceTable destTable = new NiceTable();
        //    if (destTable.restore(ref data))
        //        return destTable;
        //}
        if (bResult)
            return resultTable;

        return null;
    }

    //public static NiceTable LoadTableOnPath(string tableName, System.Text.Encoding fileEncoding)
    //{
    //    NiceTable t = new NiceTable();
    //    if (t.LoadTable(MakeGamePathFileName(tableName), fileEncoding))
    //        return t;
    //    return null;
    //}

	static public GameObject LoadAndIntanciateUIPrefabs(string strPrefabsName, string strParentName)
	{
        return LoadAndIntanciateUIPrefabs(UICommonDefine.strUIPrefabsPath, strPrefabsName, strParentName);
	}

	static public GameObject LoadUIPrefabs(string strPrefabsName, GameObject parentObj)
	{
        return LoadAndIntanciateUIPrefabs(UICommonDefine.strUIPrefabsPath, strPrefabsName, parentObj);
	}

    static public GameObject LoadAndIntanciateUIPrefabs(string strPath, string strPrefabsName, string strParentName)
    {
        if(string.IsNullOrEmpty(strPrefabsName))
            return null;

        GameObject obj = GameCommon.CreateObject(strPath + strPrefabsName, strParentName, ResourceType.RES_PREFAB, ResourceGroup.RESG_UI);

        if (obj == null)
        {
            Logic.EventCenter.Log(LOG_LEVEL.WARN, "this prefab is not exist>" + strPrefabsName);
        }
        return obj;
    }

    static public GameObject LoadAndIntanciateUIPrefabs(string strPath, string strPrefabsName, GameObject parentObj)
    {
        GameObject obj = GameCommon.CreateObject(strPath + strPrefabsName, parentObj, ResourceType.RES_PREFAB, ResourceGroup.RESG_UI);

        if (obj == null)
        {
            Logic.EventCenter.Log(LOG_LEVEL.WARN, "this prefab is not exist>" + strPrefabsName);
        }
        else
            obj.name = strPrefabsName;
        return obj;
    }

    static public GameObject LoadAndIntanciateEffectPrefabs(string strPrefabsName, string strParentName)
    {
        GameObject obj = GameCommon.CreateObject(strPrefabsName, strParentName, ResourceType.RES_PREFAB, ResourceGroup.RESG_EFFECT);

        if (obj == null)
        {
            Logic.EventCenter.Log(LOG_LEVEL.WARN, "this prefab is not exist>" + strPrefabsName);
        }
        return obj;
    }

    static public GameObject LoadAndIntanciateEffectPrefabs(string strPrefabsName, GameObject parentObj)
    {
        GameObject obj = GameCommon.CreateObject(strPrefabsName, parentObj, ResourceType.RES_PREFAB, ResourceGroup.RESG_EFFECT);

        if (obj == null)
        {
            Logic.EventCenter.Log(LOG_LEVEL.WARN, "this prefab is not exist>" + strPrefabsName);
        }
        else
            obj.name = strPrefabsName;
        return obj;
    }

    static public GameObject LoadAndIntanciateEffectPrefabs(string prefabName)
    {
        GameObject uiObject = CreateObject(prefabName, ResourceType.RES_PREFAB, ResourceGroup.RESG_EFFECT);
        return uiObject;
    }

    static public GameObject LoadAndIntanciatePrefabs(string strPrefabsName, string strParentName)
    {
        GameObject obj = GameCommon.CreateObject(strPrefabsName, strParentName, ResourceType.RES_PREFAB);

        if (obj == null)
        {
            Logic.EventCenter.Log(LOG_LEVEL.WARN, "this prefab is not exist>" + strPrefabsName);
        }
        return obj;
    }

    static public GameObject LoadAndIntanciatePrefabs(string strPrefabsName, GameObject parentObj)
    {
        GameObject obj = GameCommon.CreateObject(strPrefabsName, parentObj, ResourceType.RES_PREFAB);

        if (obj == null)
        {
            Logic.EventCenter.Log(LOG_LEVEL.WARN, "this prefab is not exist>" + strPrefabsName);
        }
        else
            obj.name = strPrefabsName;
        return obj;
    }

    static public GameObject LoadAndIntanciatePrefabs(string prefabName)
    {
        return CreateObject(prefabName, ResourceType.RES_PREFAB);
    }

    static public GameObject LoadPrefabs(string prefabName)
    {
        return mResources.LoadPrefab(prefabName, "");
    }

    static public GameObject LoadPrefabs(string prefabName, string hint)
    {
        return mResources.LoadPrefab(prefabName, hint);
    }

    static public AudioClip LoadAudioClip(string soundResName)
    {
        //return Resources.Load(soundResName, typeof(AudioClip)) as AudioClip;
        return mResources.LoadSound(soundResName);
    }

    static public Shader FindShader(string shaderName)
    {
        var shader = mResources.LoadShader(shaderName);
        if(shader != null)
            return shader;
        return Shader.Find(shaderName);
    }

    static public void LoadLevel(string scene)
    {
        mResources.LoadLevel(scene);
    }

    static public IEnumerator LoadLevelAsync(string scene)
    {
        //mResources.LoadLevel();
        yield break;
    }

    static public string FormatTime(double fSecordTime)
    {
        TimeSpan span = new TimeSpan((long)fSecordTime * 10000000);
        return span.ToString().Replace(".", "天");
    }

    static public DateTime TotalSeconds2DateTime(Int64 serverTime)
    {
        TimeSpan elapse = new TimeSpan(serverTime * 10000000);
        DateTime begin = new DateTime(1970, 1, 1, 8, 0, 0); // China Standard Time is 1970-1-1 8:0:0 when UTC Time is 1970-1-1 0:0:0
        return begin + elapse;
    }

    static public Int64 DateTime2TotalSeconds(DateTime data)
    {
        DateTime begin = new DateTime(1970, 1, 1, 8, 0, 0); // China Standard Time is 1970-1-1 8:0:0 when UTC Time is 1970-1-1 0:0:0
        TimeSpan elapse = data - begin;
        return (Int64)elapse.TotalSeconds;
    }

    static public DateTime NowDateTime()
    {
        return TotalSeconds2DateTime(CommonParam.NowServerTime());
    }

    /// <summary>
    /// 加载的公用窗口
    /// </summary>
    /// <param name="strPrefabsName">预制件名字</param>
    /// <param name="strParentName">需要挂载的父窗口名字</param>
    /// <returns></returns>
	static public GameObject InitCommonUI(string strPrefabsName, string strParentName)
	{
		GameObject commonUI = null;
        if (!GameCommon.bIsLogicDataExist(strPrefabsName))
		{
			commonUI = LoadAndIntanciateUIPrefabs(strPrefabsName, strParentName);
			commonUI.transform.localPosition = new Vector3(0, 0, -1000.0f);
		}
		return commonUI;
	}

    static public GameObject CreateObject(string resourceNamePath)
    {
        return CreateObject(resourceNamePath, ResourceType.RES_GENERIC, ResourceGroup.RESG_ALL);
    }

    static public GameObject CreateObject(string resourceNamePath, ResourceType resType)
    {
        return CreateObject(resourceNamePath, resType, ResourceGroup.RESG_ALL);
    }


    static public GameObject CreateObject(string resourceNamePath, ResourceType resType, ResourceGroup resGroup)
    {
        UnityEngine.Object obj = null;
        
        switch (resType)
        {
            case ResourceType.RES_PREFAB:
                if(resGroup == ResourceGroup.RESG_EFFECT)
                    obj = mResources.LoadPrefab(resourceNamePath, "EFFECT");
                else if(resGroup == ResourceGroup.RESG_UI)
                    obj = mResources.LoadPrefab(resourceNamePath, "UI");
                else 
                    obj = mResources.LoadPrefab(resourceNamePath, "");
                break;
            case ResourceType.RES_MODEL:
                obj = mResources.LoadModel(resourceNamePath);
                break;
            case ResourceType.RES_TEXTURE:
                obj = mResources.LoadTexture(resourceNamePath);
                break;
            default:
                obj = Resources.Load(resourceNamePath, typeof(UnityEngine.Object)); //
                break;
        }

        if (obj != null) {
            Debug.Log(string.Format("GameCommon CreateObject {0}; result: {1} success ****", resourceNamePath, resType));
            return GameObject.Instantiate(obj) as GameObject;
        }
        Debug.Log(string.Format("GameCommon CreateObject failed **********"));
        return null;
    }

    static public GameObject CreateObject(string resourceNamePath, string strParentName)
    {
        return CreateObject(resourceNamePath, strParentName, ResourceType.RES_GENERIC, ResourceGroup.RESG_ALL);
    }

    static public GameObject CreateObject(string resourceNamePath, string strParentName, ResourceType resType)
    {
        return CreateObject(resourceNamePath, strParentName, resType, ResourceGroup.RESG_ALL);
    }

	static public GameObject CreateObject(string prefabName, string strParentName, ResourceType resType, ResourceGroup resGroup)
	{
        /*
		GameObject ui = GameCommon.FindUI(strParentName);
		if (ui==null)
		{
			Logic.EventCenter.Log(LOG_LEVEL.ERROR, "No exist gameobject > "+strParentName);
			return null;
		}
        */
		GameObject uiObject = CreateObject(prefabName, resType, resGroup);
        if (uiObject != null)
		{
            //uiObject.transform.parent = ui.transform;
            uiObject.transform.localPosition = new Vector3(0, 0, 0);
            uiObject.transform.localScale = new Vector3(1, 1, 1);
		}
		return uiObject;
	}

    static public GameObject CreateObject(string resourceNamePath, GameObject parentObj)
    {
        return CreateObject(resourceNamePath, parentObj, ResourceType.RES_GENERIC, ResourceGroup.RESG_ALL);
    }

    static public GameObject CreateObject(string resourceNamePath, GameObject parentObj, ResourceType resType)
    {
        return CreateObject(resourceNamePath, parentObj, resType, ResourceGroup.RESG_ALL);
    }

	static public GameObject CreateObject(string prefabName,  GameObject parentObj, ResourceType resType, ResourceGroup resGroup)
	{
		if (parentObj==null)
		{
			Logic.EventCenter.Log(LOG_LEVEL.ERROR, "No exist gameobject parentObj ");
			return null;
		}
		GameObject uiObject = CreateObject(prefabName, resType, resGroup);
		if (uiObject != null)
		{
			uiObject.transform.parent = parentObj.transform;
			uiObject.transform.localPosition = new Vector3(0, 0, 0);
			uiObject.transform.localScale = new Vector3(1, 1, 1);
		}
		return uiObject;
	}
	
	static public bool PlaySound(string soundResName, Vector3 playPos)
    {
        if (!Settings.IsSoundEffectEnabled())
            return false;

        if (soundResName != "")
        {
            AudioClip clip = LoadAudioClip(soundResName);
			PlaySound(clip, playPos);
        }
		return false;
    }

	static public bool PlaySound(AudioClip clip, Vector3 playPos)
	{
		if (!Settings.IsSoundEffectEnabled())
			return false;

		if (clip != null)
		{
			AudioSource.PlayClipAtPoint(clip, playPos, CommonParam.DefaultVolume);
			///Debug.Log(" ! play back music > " + clip.name);
			return true;
		}

		return false;
	}
	
	static public bool RandPlaySound(string soundResName, Vector3 playPos)
	{
		char[] pa = { ' ' };
		string[] soundList = soundResName.Split(pa);
        int r = UnityEngine.Random.Range(0, soundList.Length);
        return PlaySound(soundList[r], playPos);
    }

	static public bool SetBackgroundSound(string strPath, float fVolume)
	{
		GameObject cameraObj = GetMainCameraObj();
		if(cameraObj != null)
		{
			AudioSource sound = cameraObj.GetComponent<AudioSource>();
			if (sound==null || !sound.enabled)
				return false;
			
			string backSound = strPath;
            AudioClip clip = LoadAudioClip(backSound);
			if (clip != null)
			{
				sound.clip = clip;
				sound.loop = true;
				float v = fVolume;
				if (v > 0.001)
					sound.volume = 0.35f;
				else
					sound.volume = CommonParam.MusicVolume;

                if(Settings.IsMusicEnabled())
				    sound.Play();
				
				return true;
			}
		}
		return false;
	}
	
	static public bool RemoveBackgroundSound()
	{
		GameObject cameraObj = GetMainCameraObj();
		if(cameraObj != null)
		{
			AudioSource sound = cameraObj.GetComponent<AudioSource>();
			if (sound==null || !sound.enabled)
				return false;

			sound.clip = null;
				
			return true;
		}
		return false;
	}
	
	static public GameObject FindUI(string windowName)
    {
        GameObject obj = GameObject.Find(CommonParam.UIRootName);
        if (obj != null)
            return FindObject(obj, windowName);

        return null;
    }
       
    static public bool SetUIText(GameObject winObj, string lableOwnerName, string textInfo)
    {
        GameObject obj = GameCommon.FindObject(winObj, lableOwnerName);
        if (obj != null)
        {
			UILabel lable = obj.GetComponent<UILabel>();
			if (lable==null)
            	lable = obj.GetComponentInChildren<UILabel>();
            if (lable != null)
            {
				lable.text = textInfo;
                return true;
            }
        }
        return false;
    }

    static public void SetUITextRecursively(GameObject winObj, string labelOwnerName, string textInfo)
    {
        GameObject obj = GameCommon.FindObject(winObj, labelOwnerName);

        if (obj != null)
        {
            UILabel[] labels = obj.GetComponentsInChildren<UILabel>();

            foreach (var label in labels)
                label.text = textInfo;
        }
    }

    static public bool SetUIVisiable(GameObject winObj, string uiName, bool bVisiable)
    {
         GameObject obj = GameCommon.FindObject(winObj, uiName);
         if (obj != null)
         {
             obj.SetActive(bVisiable);
             return true;
         }
         return false;
    }

	static public bool SetUIVisiable(GameObject parentObj, bool bVisiable, params string[] uiNames)
	{
		foreach(string s in uiNames)
		{
			GameObject obj = GameCommon.FindObject (parentObj, s);
			if(obj != null)
				obj.SetActive (bVisiable);
			else 
				LOG.logWarn ("can not  find " + s + "in" + obj.ToString ());
		}
		return true;
	}

    static public bool SetUISprite(GameObject winObj, string SpriteOwnerName, string argSpriteName)
    {
        GameObject obj = GameCommon.FindObject(winObj, SpriteOwnerName);
        if (obj != null)
        {
            UISprite sprite = obj.GetComponentInChildren<UISprite>();
            if (sprite != null)
            {
                sprite.spriteName = argSpriteName;
                return true;
            }
        }
        return false;
    }

    static public bool SetUISprite(GameObject winObj, string SpriteOwnerName, string argSpriteName, UIAtlas argAtlas)
    {
        GameObject obj = GameCommon.FindObject(winObj, SpriteOwnerName);
        if (obj != null)
        {
            UISprite sprite = obj.GetComponentInChildren<UISprite>();
            if (sprite != null)
            {
                sprite.atlas = argAtlas;
                sprite.spriteName = argSpriteName;
                //sprite.MakePixelPerfect();
                return true;
            }
        }
        return false;
    }

    static public bool SetUISprite(GameObject winObj, string SpriteOwnerName, string argAtlasName, string argSpriteName)
    {
        UIAtlas atlas = LoadUIAtlas(argAtlasName);
        if (atlas == null)
        {
            LOG.logError("No exist atlas >" + argAtlasName);
            return false;
        }
        return SetUISprite(winObj, SpriteOwnerName, argSpriteName, atlas);
    }

    static public bool SetSprite(GameObject spriteObject, string atlasName, string atlasSpriteName)
    {
        if (spriteObject == null)
            return false;

        UISprite sprite = spriteObject.GetComponent<UISprite>();
        if (sprite != null)
        {
            sprite.atlas = LoadUIAtlas(atlasName);
            sprite.spriteName = atlasSpriteName;           
            return true;
        }
        return false;
    }

    static public UIAtlas LoadUIAtlas(string argAtlasName)
    {
        if(string.IsNullOrEmpty(argAtlasName))
            return null;

        string fullAtlasName = "textures/UItextures/" + argAtlasName;
        return mResources.LoadUIAtlas(fullAtlasName, "UIAtlas");
    }

    static public bool SetMonsterIcon(GameObject parentWin, string iconUIName, int monstConfigID)
    {
        string strAtlasName = TableCommon.GetStringFromActiveCongfig(monstConfigID, "HEAD_ATLAS_NAME");
        string strSpriteName = TableCommon.GetStringFromActiveCongfig(monstConfigID, "HEAD_SPRITE_NAME");

        UIAtlas tu = GameCommon.LoadUIAtlas(strAtlasName);
        return SetUISprite(parentWin, iconUIName, strSpriteName, tu);
    }

    static public void SetMonsterIcon(GameObject iconObj, int index)
    {
        MonsterIcon icon = new MonsterIcon(iconObj);
        icon.Reset();
        icon.Set(index);
    }

	static public void SetRoleTitle(UISprite sprite)
	{
		SetRoleTitle(sprite, RoleLogicData.GetMainRole().starLevel);
	}

	static public void SetRoleTitle(UISprite sprite, int iStarLevel)
	{
		SetIcon(sprite, "CardAtlas", "ui_chenghao_" + iStarLevel.ToString());
	}

    static public void SetRoleTitle(UILabel label)
    {
        SetRoleTitle(label, RoleLogicData.GetMainRole().starLevel);
    }

    static public void SetRoleTitle(UILabel label, int iStarLevel)
    {
        switch (iStarLevel)
        {
            case 1:
                label.text = "驱鬼师";
                break;
            case 2:
                label.text = "御妖师";
                break;
            case 3:
                label.text = "伏魔师";
                break;
            default:
                label.text = "";
                break;
        }
    }

	static public void SetIcon(UISprite sprite, string atlasName, string spriteName)
	{
		if(sprite != null)
		{
			UIAtlas atlas = LoadUIAtlas(atlasName);
			sprite.atlas = atlas;
			sprite.spriteName = spriteName;
		}
	}

	static public void SetIcon(GameObject parentObj, string iconName, string strSpriteName, string atlasName)
	{
		string strAtlasName = atlasName;
		
		UIAtlas tu = GameCommon.LoadUIAtlas(strAtlasName);
        
        GameObject obj = GameCommon.FindObject (parentObj, iconName);
        if(obj != null)
        {
			UISprite icon = obj.GetComponent<UISprite>();

			SetIcon(icon, atlasName, strSpriteName);
        }
	}

    static public NiceData GetButtonData(GameObject winObject, string buttonName)
    {
		GameObject obj;
        if (winObject == null)
            obj = FindUI(buttonName);
        else
            obj = FindObject(winObject, buttonName);

        if (obj != null)
        {
            UIButtonEvent butEvt = obj.GetComponent<UIButtonEvent>();
            if (butEvt != null)
                return butEvt.mData;
        }
        return null;
    }

    static public NiceData GetButtonData(GameObject buttonObj)
    {
		if(buttonObj == null)
			return null;
        UIButtonEvent butEvt = buttonObj.GetComponent<UIButtonEvent>();
        if (butEvt != null)
            return butEvt.mData;
        return null;
    }

    static public UIImageButton GetUIButton(GameObject winObject, string buttonName)
    {
        GameObject obj;
        if (winObject == null)
            obj = FindUI(buttonName);
        else
            obj = FindObject(winObject, buttonName);

        if (obj != null)
        {
            return obj.GetComponent<UIImageButton>();
        }
        return null;
    }

    static public string MakeMD5(string scrString)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bytValue, bytHash;
        bytValue = StaticDefine.GetBytes(scrString);
        bytHash = md5.ComputeHash(bytValue);
        md5.Clear();

        string result = "";
        for (int i = 0; i < bytHash.Length; i++)
        {
            result += bytHash[i].ToString("X").PadLeft(2, '0');
        }
        //return result.ToLower();
		return scrString;
    }

    static public string MakeMD5(byte[] scrData, int size)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bytHash;
        
        bytHash = md5.ComputeHash(scrData, 0, size);
        md5.Clear();

        string result = "";
        for (int i = 0; i < bytHash.Length; i++)
        {
            result += bytHash[i].ToString("X").PadLeft(2, '0');
        }
        return result.ToLower();
    }

    static public void SetObjectTexture(GameObject obj, Texture tex)
    {
        if (obj == null)
            return;

        Renderer[] rend = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer mR in rend)
        {
            mR.material.mainTexture = tex;
        }
    }

	static public void SetCardInfo(string strName, int iModelIndex, int iLevel, int iStrengthenLevel, GameObject obj)
	{
		DataCenter.SetData("CardGroupWindow" + strName, "SET_SELECT_PET_COLLIDER_UI", obj);
		SetCardInfo(strName, iModelIndex, iLevel, iStrengthenLevel, true);
	}

	static public void SetCardInfo(string strName, int iModelIndex, int iLevel, int iStrengthenLevel, bool bIsCanOperate)
	{
		DataCenter.SetData("CardGroupWindow" + strName, "SET_SELECT_PET_OPERATE_STATE", bIsCanOperate);
		DataCenter.SetData("CardGroupWindow" + strName, "SET_SELECT_PET_LEVEL", iLevel);
		DataCenter.SetData("CardGroupWindow" + strName, "SET_SELECT_PET_STRENGTHEN_LEVEL", iStrengthenLevel);
		DataCenter.SetData("CardGroupWindow" + strName, "SET_SELECT_PET_BY_MODEL_INDEX", iModelIndex);
	}

    static GameObject msDebugInfoObject;
    static public void ShowDebugInfo(float screenX, float screenY, string debugInfo)
    {
        LOG.logError("DEBUG >" + debugInfo);
        return;

        if (msDebugInfoObject == null)
        {
            GameObject uiCamera = FindUI("Camera");
            msDebugInfoObject = LoadUIPrefabs("prefabs/" + "DebugInfo", uiCamera);
            msDebugInfoObject.name = "_debug_info_";
			msDebugInfoObject.transform.localPosition = Vector3.zero;
            msDebugInfoObject.transform.localScale = Vector3.one;
        }
        msDebugInfoObject.GetComponentInChildren<UILabel>().text = debugInfo;
		//msDebugInfoObject.transform.position = new Vector3(screenX, screenY, 0);
    }

    public static ELEMENT_RELATION GetElementRelation(int iSrcElement, int iAimElement)
    {
        if (iSrcElement < (int)ELEMENT_TYPE.RED || iSrcElement >= (int)ELEMENT_TYPE.MAX
            || iAimElement < (int)ELEMENT_TYPE.RED || iAimElement >= (int)ELEMENT_TYPE.MAX)
            return ELEMENT_RELATION.BLANCE;

        if (iSrcElement <= (int)ELEMENT_TYPE.GREEN && iAimElement <= (int)ELEMENT_TYPE.GREEN)
        {
            if ((iSrcElement + 3 - iAimElement) % 3 == 1)
            {
                return ELEMENT_RELATION.ADVANTAGEOUS;
            }
            if ((iAimElement + 3 - iSrcElement) % 3 == 1)
            {
                return ELEMENT_RELATION.INFERIOR;
            }
        }

        if (iSrcElement >= (int)ELEMENT_TYPE.GOLD && iAimElement >= (int)ELEMENT_TYPE.GOLD)
        {
            if (iAimElement != iSrcElement)
                return ELEMENT_RELATION.ADVANTAGEOUS;
        }

        return ELEMENT_RELATION.BLANCE;
    }

	static public int GetActiveObjectElement(DataRecord record)
	{
		return GetActiveObjectElement(record, false);
	}

	static public int GetActiveObjectElement(DataRecord record, bool bIsPvpOpponent)
    {
        string type = record.get("CLASS");

        if (type == "CHAR")
        {
			string strRoleEquipData = "EQUIP_DATA";
			if(bIsPvpOpponent)
			{
				strRoleEquipData = "";
				return record.get("ELEMENT_INDEX");
			}

            RoleEquipLogicData roleEquipLogicData = DataCenter.GetData("EQUIP_DATA") as RoleEquipLogicData;
            EquipData curUseEquip = roleEquipLogicData.GetUseEquip();
            return curUseEquip == null ? record.get("ELEMENT_INDEX") : (int)curUseEquip.mElementType;
        }
        else
        {
            return record.get("ELEMENT_INDEX");
        }
    }

	static public ELEMENT_RELATION GetElmentRaletion(DataRecord srcDataRecord, DataRecord aimDataRecord)
	{
		return GetElmentRaletion(srcDataRecord, false, aimDataRecord, false);
	}

	static public ELEMENT_RELATION GetElmentRaletion(DataRecord srcDataRecord, bool srcIsPvpOpponent, DataRecord aimDataRecord, bool aimIsPvpOpponent)
	{
		if(srcDataRecord == null && aimDataRecord == null)
			return ELEMENT_RELATION.BLANCE;

		int iSrcElement = GetActiveObjectElement(srcDataRecord, srcIsPvpOpponent);
		int iAimElement = GetActiveObjectElement(aimDataRecord, aimIsPvpOpponent);

        return GetElementRelation(iSrcElement, iAimElement);
	}

	static public float GetElmentRaletionRatio(DataRecord srcDataRecord, bool srcIsPvpOpponent, DataRecord aimDataRecord, bool aimIsPvpOpponent)
	{
		float fRatio = 1.0f;
		ELEMENT_RELATION elementRelation = GameCommon.GetElmentRaletion(srcDataRecord, srcIsPvpOpponent, aimDataRecord, aimIsPvpOpponent);
		if(elementRelation == ELEMENT_RELATION.ADVANTAGEOUS)
		{
            fRatio *= (1.0f + DataCenter.mGlobalConfig.GetData("ELEMENT_RATIO", "VALUE"));
		}

		return fRatio;
	}

    // Note: result Y default zero
    static public Vector3 RandInCircularGround(Vector3 originPosition, float rangeRadius)
    {
		float rY = UnityEngine.Random.Range(0, 360);
        Quaternion ra = Quaternion.Euler(0, rY, 0);
		Vector3 result = ra * (Vector3.forward * UnityEngine.Random.Range(0, rangeRadius));
        return originPosition + result;
    }

    // Note: result Y default zero
    static public Vector3 RandInRectGround(Vector3 leftTop, Vector3 rightBotton)
    {
		return new Vector3(UnityEngine.Random.Range(leftTop.x, rightBotton.x), 0, UnityEngine.Random.Range(leftTop.z, rightBotton.z));
    }

    static public int SplitToInt(string scrString, char splitFlag, out int[] result)
    {
        char[] p = { splitFlag };

        if (scrString=="")
        {
            result = null;
            return 0;
        }

        string[] x = scrString.Split(p);
        result = new int[x.Length];
        for(int i=0; i<x.Length; ++i)
        {
            try{
                result[i] = int.Parse(x[i]);
            }
            catch
            {
                result[i] = 0;
            }
        }
        return result.Length;
    }

    static public bool SplitToVector(string scrString, out Vector3 resultVec)
    {
        char[] p = { ' ' };

        if (scrString == "")
        {
			resultVec = Vector3.zero;
            return false;
        }

        string[] x = scrString.Split(p);

        if (x.Length >= 3)
        {
            resultVec = new Vector3();
            resultVec.x = float.Parse(x[0]);
            resultVec.y = float.Parse(x[1]);
            resultVec.z = float.Parse(x[2]);

            return true;
        }
		resultVec = Vector3.zero;
        return false;
    }

    static public Vector2 GetRowAndColumnByIndex(int iIndex, int iMaxColumn)
    {
        int iRow = iIndex / iMaxColumn;
        int iColumn = iIndex % iMaxColumn;
        return new Vector2(iRow, iColumn);
    }

    static public Vector3 GetPostion(int iPosX0, int iPosY0, int iDX, int iDY, int iWidth, int iHeight, Vector2 vec)
    {
        int iPosX = iPosX0 + (iDX + iWidth) * (int)vec.y;
        int iPosY = (iPosY0 + (iDY + iHeight) * (int)vec.x) * (-1);
        return new Vector3(iPosX, iPosY, 0);
    }

    static public Vector3 GetPostion(int iPosX0, int iPosY0, int iDX, int iDY, int iWidth, int iHeight, int iIndex, int iMaxColumn)
    {
        Vector2 vec = GameCommon.GetRowAndColumnByIndex(iIndex, iMaxColumn);
        return GetPostion(iPosX0, iPosY0, iDX, iDY, iWidth, iHeight, vec);
    }

    static public bool bIsLogicDataExist(string strName)
    {
        object obj;
        return DataCenter.Self.getData(strName, out obj);
    }

    static public bool bIsWindowOpen(string strWindowName)
    {
        object obj;
        DataCenter.Self.getData(strWindowName, out obj);

        if (obj != null)
        {
            tWindow win = obj as tWindow;

            if (win != null)
            {
                return win.mGameObjUI != null && win.mGameObjUI.activeSelf;
            }
        }

        return false;
    }

    static public bool SetWindowData(string strWindowName, string keyIndex, object objVal)
    {
        bool bIsExist = bIsLogicDataExist(strWindowName);
        if (bIsExist)
            DataCenter.SetData(strWindowName, keyIndex, objVal);

        return bIsExist;
    }

    static public void ToggleTrue(GameObject obj)
    {
        if (obj != null)
        {
            UIToggle toggle = obj.GetComponent<UIToggle>();
            if(toggle != null)
                toggle.value = true;
        }
    }

    static public void ToggleFalse(GameObject obj)
    {
        if (obj != null)
        {
            UIToggle toggle = obj.GetComponent<UIToggle>();
            if (toggle != null)
                toggle.value = false;
        }
    }

    //---------------------------------------------------------------------------------
    // attribute type
    static public AFFECT_TYPE ToAffectTypeEnum(string strEnumName)
    {
        switch (strEnumName)
        {
            case "ATTACK":
                return AFFECT_TYPE.ATTACK;

            case "ATTACK_RATE":
                return AFFECT_TYPE.ATTACK_RATE;

            case "DEFENSE":
                return AFFECT_TYPE.DEFENSE;

            case "DEFENSE_RATE":
                return AFFECT_TYPE.DEFENSE_RATE;

            case "HP":
                return AFFECT_TYPE.HP;

            case "HP_RATE":
                return AFFECT_TYPE.HP_RATE;

            case "HP_MAX":
                return AFFECT_TYPE.HP_MAX;

            case "HP_MAX_RATE":
                return AFFECT_TYPE.HP_MAX_RATE;

            case "MP":
                return AFFECT_TYPE.MP;

            case "MP_RATE":
                return AFFECT_TYPE.MP_RATE;

            case "MP_MAX":
                return AFFECT_TYPE.MP_MAX;

            case "MP_MAX_RATE":
                return AFFECT_TYPE.MP_MAX_RATE;

            case "HIT_TARGET":
                return AFFECT_TYPE.HIT_TARGET;

            case "HIT_TARGET_RATE":
                return AFFECT_TYPE.HIT_TARGET_RATE;

            case "CRITICAL_STRIKE":
                return AFFECT_TYPE.CRITICAL_STRIKE;

            case "CRITICAL_STRIKE_RATE":
                return AFFECT_TYPE.CRITICAL_STRIKE_RATE;

            case "HIT_CRITICAL_STRIKE_RATE":
                return AFFECT_TYPE.HIT_CRITICAL_STRIKE_RATE;

            case "DEFENSE_CHARM_RATE":
                return AFFECT_TYPE.DEFENSE_CHARM_RATE;

            case "DEFENSE_FEAR_RATE":
                return AFFECT_TYPE.DEFENSE_FEAR_RATE;

            case "DEFENSE_WOOZY_RATE":
                return AFFECT_TYPE.DEFENSE_WOOZY_RATE;

            case "DEFENSE_ICE_RATE":
                return AFFECT_TYPE.DEFENSE_ICE_RATE;

            case "DEFENSE_BEATBACK_RATE":
                return AFFECT_TYPE.DEFENSE_BEATBACK_RATE;

            case "DEFENSE_DOWN_RATE":
                return AFFECT_TYPE.DEFENSE_DOWN_RATE;

            case "DEFENSE_HIT_RATE":
                return AFFECT_TYPE.DEFENSE_HIT_RATE;

            case "MOVE_SPEED":
                return AFFECT_TYPE.MOVE_SPEED;

            case "MOVE_SPEED_RATE":
                return AFFECT_TYPE.MOVE_SPEED_RATE;

            case "ATTACK_SPEED":
                return AFFECT_TYPE.ATTACK_SPEED;

            case "ATTACK_SPEED_RATE":
                return AFFECT_TYPE.ATTACK_SPEED_RATE;

            case "DODGE":
                return AFFECT_TYPE.DODGE;

            case "DODGE_RATE":
                return AFFECT_TYPE.DODGE_RATE;

			case "DEFENSE_CRITICAL_DAMAGE_RATE":
				return AFFECT_TYPE.DEFENSE_CRITICAL_DAMAGE_RATE;

			case "RED_REDUCTION_RATE":
				return AFFECT_TYPE.RED_REDUCTION_RATE;

			case "GREEN_REDUCTION_RATE":
				return AFFECT_TYPE.GREEN_REDUCTION_RATE;

			case "BLUE_REDUCTION_RATE":
				return AFFECT_TYPE.BLUE_REDUCTION_RATE;

			case "SHADOW_REDUCTION_RATE":
				return AFFECT_TYPE.SHADOW_REDUCTION_RATE;

			case "GOLD_REDUCTION_RATE":
				return AFFECT_TYPE.GOLD_REDUCTION_RATE;

            case "PHYSICAL_DEFENCE":
                return AFFECT_TYPE.PHYSICAL_DEFENCE;

            case "MAGIC_DEFENCE":
                return AFFECT_TYPE.MAGIC_DEFENCE;

            case "PHYSICAL_DEFENCE_RATE":
                return AFFECT_TYPE.PHYSICAL_DEFENCE_RATE;

            case "MAGIC_DEFENCE_RATE":
                return AFFECT_TYPE.MAGIC_DEFENCE_RATE;

		default:
                return AFFECT_TYPE.NONE;
        }
    }

    static public string ToAffectTypeString(AFFECT_TYPE affectType)
    {
        switch (affectType)
        {
            case AFFECT_TYPE.ATTACK:
                return "ATTACK";

            case AFFECT_TYPE.ATTACK_RATE:
                return "ATTACK_RATE";

            case AFFECT_TYPE.DEFENSE:
                return "DEFENSE";

            case AFFECT_TYPE.DEFENSE_RATE:
                return"DEFENSE_RATE";

            case AFFECT_TYPE.HP:
                return "HP";

            case AFFECT_TYPE.HP_RATE:
                return "HP_RATE";

            case AFFECT_TYPE.HP_MAX:
                return "HP_MAX";

            case AFFECT_TYPE.HP_MAX_RATE:
                return "HP_MAX_RATE";

            case AFFECT_TYPE.MP:
                return "MP";

            case AFFECT_TYPE.MP_RATE:
                return "MP_RATE";

            case AFFECT_TYPE.MP_MAX:
                return "MP_MAX";

            case AFFECT_TYPE.MP_MAX_RATE:
                return "MP_MAX_RATE";

            case AFFECT_TYPE.HIT_TARGET:
                return "HIT_TARGET";

            case AFFECT_TYPE.HIT_TARGET_RATE:
                return "HIT_TARGET_RATE";

            case AFFECT_TYPE.CRITICAL_STRIKE:
                return "CRITICAL_STRIKE";

            case AFFECT_TYPE.CRITICAL_STRIKE_RATE:
                return "CRITICAL_STRIKE_RATE";

            case AFFECT_TYPE.HIT_CRITICAL_STRIKE_RATE:
                return "HIT_CRITICAL_STRIKE_RATE";

            case AFFECT_TYPE.DEFENSE_CHARM_RATE:
                return "DEFENSE_CHARM_RATE";

            case AFFECT_TYPE.DEFENSE_FEAR_RATE:
                return "DEFENSE_FEAR_RATE";

            case AFFECT_TYPE.DEFENSE_WOOZY_RATE:
                return "DEFENSE_WOOZY_RATE";

            case AFFECT_TYPE.DEFENSE_ICE_RATE:
                return "DEFENSE_ICE_RATE";

            case AFFECT_TYPE.DEFENSE_BEATBACK_RATE:
                return "DEFENSE_BEATBACK_RATE";

            case AFFECT_TYPE.DEFENSE_DOWN_RATE:
                return "DEFENSE_DOWN_RATE";

            case AFFECT_TYPE.DEFENSE_HIT_RATE:
                return "DEFENSE_HIT_RATE";

            case AFFECT_TYPE.MOVE_SPEED:
                return "MOVE_SPEED";

            case AFFECT_TYPE.MOVE_SPEED_RATE:
                return "MOVE_SPEED_RATE";

            case AFFECT_TYPE.ATTACK_SPEED:
                return "ATTACK_SPEED";

            case AFFECT_TYPE.ATTACK_SPEED_RATE:
                return "ATTACK_SPEED_RATE";

            case AFFECT_TYPE.DODGE:
                return "DODGE";

            case AFFECT_TYPE.DODGE_RATE:
                return "DODGE_RATE";

			case AFFECT_TYPE.DEFENSE_CRITICAL_DAMAGE_RATE:
				return "DEFENSE_CRITICAL_DAMAGE_RATE";

			case AFFECT_TYPE.RED_REDUCTION_RATE:
				return "RED_REDUCTION_RATE";

			case AFFECT_TYPE.GREEN_REDUCTION_RATE:
				return "GREEN_REDUCTION_RATE";

			case AFFECT_TYPE.BLUE_REDUCTION_RATE:
				return "BLUE_REDUCTION_RATE";

			case AFFECT_TYPE.SHADOW_REDUCTION_RATE:
				return "SHADOW_REDUCTION_RATE";

			case AFFECT_TYPE.GOLD_REDUCTION_RATE:
				return "GOLD_REDUCTION_RATE";

            case AFFECT_TYPE.PHYSICAL_DEFENCE:
                return "PHYSICAL_DEFENCE";

            case AFFECT_TYPE.MAGIC_DEFENCE:
                return "MAGIC_DEFENCE";

            case AFFECT_TYPE.PHYSICAL_DEFENCE_RATE:
                return "PHYSICAL_DEFENCE_RATE";

            case AFFECT_TYPE.MAGIC_DEFENCE_RATE:
                return "MAGIC_DEFENCE_RATE";

		default:
                return "NONE";
        }
    }

    static public AFFECT_TYPE GetAffactType(AFFECT_TYPE affectType)
    {
        string strType = ToAffectTypeString(affectType);
        if (strType.LastIndexOf("_RATE") > 0)
        {
            strType.Replace("_RATE", "");
        }

        return ToAffectTypeEnum(strType);
    }

    static public AFFECT_TYPE GetAffactRateType(AFFECT_TYPE affectType)
    {
        string strType = ToAffectTypeString(affectType);
        if (strType.LastIndexOf("_RATE") <= 0)
        {
            strType = strType + "_RATE";
        }

        return ToAffectTypeEnum(strType);
    }

	//---------------------------------------------------------------------------------
	// set pet icon
	static public void SetPetIcon(GameObject obj, int iModelIndex)
	{
		SetPetIcon(obj, iModelIndex, "Background");
	}

	static public void SetPetIcon(GameObject obj, int iModelIndex, string strObjName)
	{
		string strAtlasName = "CommonUIAtlas";
		string strSpriteName = "ui_background_sz";
        if (iModelIndex > 0)
        {
            // use pet
            strAtlasName = TableCommon.GetStringFromActiveCongfig(iModelIndex, "HEAD_ATLAS_NAME");
            strSpriteName = TableCommon.GetStringFromActiveCongfig(iModelIndex, "HEAD_SPRITE_NAME");
        }

        SetIcon(obj, strObjName, strSpriteName, strAtlasName);
	}
	
	// set role equip icon
	static public void SetRoleEquipIcon(GameObject obj, int iModelIndex)
	{
		SetRoleEquipIcon(obj, iModelIndex, "icon_sprite");
	}

	static public void SetRoleEquipIcon(GameObject obj, int iModelIndex, string strObjName)
	{
        string strAtlasName = TableCommon.GetStringFromRoleEquipConfig(iModelIndex, "ICON_ATLAS_NAME");
        string strSpriteName = TableCommon.GetStringFromRoleEquipConfig(iModelIndex, "ICON_SPRITE_NAME");

        SetIcon(obj, strObjName, strSpriteName, strAtlasName);
	}
	
	// set pet equip icon
	static public void SetPetEquipIcon(GameObject obj, int iModelIndex)
	{
		SetPetEquipIcon(obj, iModelIndex, "icon_sprite");
	}

	static public void SetPetEquipIcon(GameObject obj, int iModelIndex, string strObjName)
	{
        string strAtlasName = TableCommon.GetStringFromRoleEquipConfig(iModelIndex, "ICON_ATLAS_NAME");
        string strSpriteName = TableCommon.GetStringFromRoleEquipConfig(iModelIndex, "ICON_SPRITE_NAME");

        SetIcon(obj, strObjName, strSpriteName, strAtlasName);
	}

	// set element icon
	static public void SetElementIcon(GameObject obj, int iIndex)
	{
		SetElementIcon(obj, "Element", iIndex);
	}
	
	static public void SetElementIcon(GameObject obj, string elementObject, int iIndex)
	{
		string strAtlasName = TableCommon.GetStringFromElement(iIndex, "ELEMENT_ATLAS_NAME");
		string strSpriteName = TableCommon.GetStringFromElement(iIndex, "ELEMENT_SPRITE_NAME");
		
		SetIcon(obj, elementObject, strSpriteName, strAtlasName);
	}

    // set role equip element icon
    static public void SetRoleEquipElementIcon(GameObject obj, int iIndex)
	{
        SetRoleEquipElementIcon(obj, "Element", iIndex);
	}
    static public void SetRoleEquipElementIcon(GameObject obj, string elementObject, int iIndex)
    {
        string strAtlasName = TableCommon.GetStringFromElement(iIndex, "EQUIP_ELEMENT_ATLAS_NAME");
        string strSpriteName = TableCommon.GetStringFromElement(iIndex, "EQUIP_ELEMENT_SPRITE_NAME");

        SetIcon(obj, elementObject, strSpriteName, strAtlasName);
    }

	// set element fragement icon
	static public void SetElementPetFragmentIcon(GameObject obj, int iIndex)
	{
		SetElementPetFragmentIcon(obj, "sprite", iIndex);
	}

    static public void SetElementPetFragmentIcon(GameObject obj, string elementObject, int iIndex)
    {
        string strAtlasName = TableCommon.GetStringFromElement(iIndex, "FRAGMENT_ATLAS_NAME");
        string strSpriteName = TableCommon.GetStringFromElement(iIndex, "FRAGMENT_SPRITE_NAME");

        SetIcon(obj, elementObject, strSpriteName, strAtlasName);
    }

    static public void SetElementBackground(GameObject obj, string backgroundObject, int iIndex)
    {
        string strAtlasName = TableCommon.GetStringFromElement(iIndex, "ELEMENT_ATLAS_NAME");
        string strSpriteName = TableCommon.GetStringFromElement(iIndex, "ELEMENT_ICON_SPRITE_NAME");

        SetIcon(obj, backgroundObject, strSpriteName, strAtlasName);
    }

	// set equip element background icon
	static public void SetEquipElementBgIcon(GameObject obj, int iIndex)
	{
		SetEquipElementBgIcon(obj, "background_sprite", iIndex);
	}

	static public void SetEquipElementBgIcon(GameObject obj, string elementObject, int iIndex)
    {
		string strAtlasName = TableCommon.GetStringFromElement(iIndex, "ELEMENT_ICON_ATLAS_NAME");
		string strSpriteName = TableCommon.GetStringFromElement(iIndex, "ELEMENT_ICON_SPRITE_NAME");

        SetIcon(obj, elementObject, strSpriteName, strAtlasName);
    }
	
	static public void SetEquipElementBgIcons(GameObject obj, int iIndex, int iElementIndex)
	{
		SetEquipElementBgIcons(obj, "Element", "background_sprite", iIndex, iElementIndex);
	}

	static public void SetEquipElementBgIcons(GameObject obj, string elementObject, string backgroundObject, int iIndex, int iElementIndex)
	{
		SetEquipElementIcon(obj, elementObject, iIndex, iElementIndex);
		SetEquipElementBgIcon(obj, backgroundObject, iIndex, iElementIndex);
	}

	static public void SetEquipElementBgIcon(GameObject obj, string backgroundObject, int iIndex, int iElementIndex)
	{
		if(TableCommon.GetNumberFromRoleEquipConfig(iIndex, "ROLEEQUIP_TYPE") == (int)EQUIP_TYPE.ELEMENT_EQUIP)
		{
			// set equip element background icon
			GameCommon.SetEquipElementBgIcon(obj, backgroundObject, iElementIndex);
		}
		else
		{
			GameCommon.SetIcon(obj, backgroundObject, UICommonDefine.strEquipIconBackgroundSprite, UICommonDefine.strEquipIconBackgroundAtlas);
		}
	}

	static public void SetEquipElementIcon(GameObject obj, string elementObject, int iIndex, int iElementIndex)
	{
		if(TableCommon.GetNumberFromRoleEquipConfig(iIndex, "ROLEEQUIP_TYPE") == (int)EQUIP_TYPE.ELEMENT_EQUIP)
		{
			// set element icon
            GameCommon.SetRoleEquipElementIcon(obj, elementObject, iElementIndex);
		}
		else
		{
			GameCommon.SetIcon(obj, elementObject, "", "");
		}
	}

	// set level label
	static public void SetLevelLabel(GameObject obj, int iValue)
	{
		SetLevelLabel(obj, iValue, "LevelLabel");
	}

	static public void SetLevelLabel(GameObject obj, int iValue, string strObjName)
	{
		GameObject levelLabelObj = GameCommon.FindObject(obj, strObjName);
		UILabel label = levelLabelObj.GetComponent<UILabel>();
		label.text = "Lv." + iValue.ToString();
		//label.text = iValue.ToString();
	}

    // set star level label
    static public void SetStarLevelLabel(GameObject obj, int iValue)
    {       
		SetStarLevelLabel(obj, iValue, "star_level_label");
    }

	static public void SetStarLevelLabel(GameObject obj, int iValue, string strObjName)
	{
        // 由于采用新布局，出于兼容考虑，此处逻辑修改为：若存在grid则采用grid布局，否则采用label布局
        // 如果采用grid布局，请手动隐藏或删除相应label布局控件
        GameObject grid = GameCommon.FindObject(obj, "stars_grid");

        if (grid != null)
        {
            Vector3 pos = grid.transform.localPosition;
            UIGridContainer container = grid.GetComponent<UIGridContainer>();
            container.MaxCount = iValue;
            grid.transform.localPosition = new Vector3((1 - iValue) * container.CellWidth / 2f, pos.y, pos.z);
        }
        else
        {
            UILabel starLevelLabel = GameCommon.FindComponent<UILabel>(obj, strObjName);
            if (starLevelLabel != null)
                starLevelLabel.text = iValue.ToString();
        }
	}
	
	// set strengthen level label
    static public void SetStrengthenLevelLabel(GameObject obj, int iValue)
    {
		SetStrengthenLevelLabel(obj, iValue, "StrengthenLevelLabel");
    }

	static public void SetStrengthenLevelLabel(GameObject obj, int iValue, string strObjName)
	{
		GameObject numLabelObj = GameCommon.FindObject(obj, strObjName);	
		UILabel label = numLabelObj.GetComponent<UILabel>();

        // 由于新布局下label有阴影背景，因此无强化时改用隐藏label而非设置空字符串的方式
        if (iValue > 0)
        {
            numLabelObj.SetActive(true);
            label.text = "+" + iValue.ToString();
            return;
        }
        else 
        {
            numLabelObj.SetActive(false);
        }
	}
	
	//---------------------------------------------------------------------------------------
	// role change glod
	static public void RoleChangeGold(int iGoldNum)
	{
		RoleLogicData logicData = RoleLogicData.Self;
		if(logicData != null)
			logicData.AddGold(iGoldNum);
	}

    // role change stamina
	static public void RoleChangeStamina(int iStamina)
	{
		RoleLogicData logicData = RoleLogicData.Self;
		if(logicData != null)
			logicData.AddStamina(iStamina);
	}

	// role change diamond
	static public void RoleChangeDiamond(int iDiamond)
	{
		RoleLogicData logicData = RoleLogicData.Self;
		if(logicData != null)
			logicData.AddDiamond(iDiamond);
	}
    
    // role change spirit
	static public void RoleChangeSpirit(int iSpirit)
	{
		RoleLogicData logicData = RoleLogicData.Self;
		if(logicData != null)
			logicData.AddSpirit(iSpirit);
	}

    // role change soulPoint
    static public void RoleChangeSoulPoint(int iSoulPoint)
    {
        RoleLogicData logicData = RoleLogicData.Self;
        if (logicData != null)
            logicData.AddSoulPoint(iSoulPoint);
    }

    // role change reputation
    static public void RoleChangeReputation(int iReputation)
    {
        RoleLogicData logicData = RoleLogicData.Self;
        if (logicData != null)
            logicData.AddReputation(iReputation);
    }

    // role change prestige
    static public void RoleChangePrestige(int iPrestige)
    {
        RoleLogicData logicData = RoleLogicData.Self;
        if (logicData != null)
            logicData.AddPrestige(iPrestige);
    }

    // role change battleAchv
    static public void RoleChangeBattleAchv(int iBattleAchv)
    {
        RoleLogicData logicData = RoleLogicData.Self;
        if (logicData != null)
            logicData.AddBattleAchv(iBattleAchv);
    }

    // role change unionContr
    static public void RoleChangeUnionContr(int iUnionContr)
    {
        RoleLogicData logicData = RoleLogicData.Self;
        if (logicData != null)
            logicData.AddUnionContr(iUnionContr);
    }

    // role change beatDemonCard
    static public void RoleChangeBeatDemonCard(int iBeatDemonCard)
    {
        RoleLogicData logicData = RoleLogicData.Self;
        if (logicData != null)
            logicData.AddBeatDemonCard(iBeatDemonCard);
    }















    // role add mail
    static public void RoleChangeMail(int iMail)
    {
        RoleLogicData logicData = RoleLogicData.Self;
        if (logicData != null)
			logicData.AddMailNum (iMail);
    }

	//role functional prop
	static public void RoleChangeFunctionalProp(ITEM_TYPE type, int count)
	{
		RoleLogicData logicData = RoleLogicData.Self;
		if(logicData != null)
			logicData.AddFunctionalProp (type, count);
	}

	static public void RoleChangeFunctionalProp(int index, int count)
	{
		ITEM_TYPE type = ITEM_TYPE.SAODANG_POINT;
		if(index == 1001)
			type = ITEM_TYPE.RESET_POINT;
		else if(index == 1002)
			type = ITEM_TYPE.SAODANG_POINT;
		else if(index == 1003)
			type = ITEM_TYPE.LOCK_POINT;
		else 
			return;

		RoleChangeFunctionalProp(type, count);
	}

	static public void RoleChangeNumericalAboutRole(int iItemType, int iItemCount)
	{
		switch(iItemType)
		{
            case (int)ITEM_TYPE.YUANBAO:
                GameCommon.RoleChangeDiamond(iItemCount);
                break;
            case (int)ITEM_TYPE.GOLD:
                GameCommon.RoleChangeGold(iItemCount);
                break;
            case (int)ITEM_TYPE.POWER:
                GameCommon.RoleChangeStamina(iItemCount);
                break;
            case (int)ITEM_TYPE.PET_SOUL:
                GameCommon.RoleChangeSoulPoint(iItemCount);
                break;
            case (int)ITEM_TYPE.SPIRIT:
                GameCommon.RoleChangeSpirit(iItemCount);
                break;
            case (int)ITEM_TYPE.REPUTATION:
                GameCommon.RoleChangeReputation(iItemCount);
                break;
            case (int)ITEM_TYPE.PRESTIGE:
                GameCommon.RoleChangePrestige(iItemCount);
                break;
            case (int)ITEM_TYPE.BATTLEACHV:
                GameCommon.RoleChangeBattleAchv(iItemCount);
                break;
            case (int)ITEM_TYPE.UNIONCONTR:
                GameCommon.RoleChangeUnionContr(iItemCount);
                break;
            case (int)ITEM_TYPE.BEATDEMONCARD:
                GameCommon.RoleChangeBeatDemonCard(iItemCount);
                break;
            case (int)ITEM_TYPE.CHARACTER_EXP:
                RoleLogicData.GetMainRole().AddExp(iItemCount);
                break;

            case (int)ITEM_TYPE.HONOR_POINT:
                RoleLogicData.Self.AddHonorPoint(iItemCount);
                break;
            case (int)ITEM_TYPE.SAODANG_POINT:
            case (int)ITEM_TYPE.RESET_POINT:
            case (int)ITEM_TYPE.LOCK_POINT:
                GameCommon.RoleChangeFunctionalProp((ITEM_TYPE)iItemType, iItemCount);
                break;
        }
	}

    static public void RoleChangeItem(IItemDataProvider provider)
    {
        int count = provider.GetCount();

        for (int i = 0; i < count; ++i)
        {
            ItemData data = provider.GetItem(i);
            RoleChangeNumericalAboutRole(data.mType, data.mNumber);
        }
    }

    //---------------------------------------------------------------------------------------
    static public void SetPetIcon(UISprite sprite, int index)
    {
        if (sprite == null)
        {
            return;
        }

        if (index <= 0)
        {
            sprite.atlas = null;
            return;
        }
        string atlasName = TableCommon.GetStringFromActiveCongfig(index, "HEAD_ATLAS_NAME");
        string spriteName = TableCommon.GetStringFromActiveCongfig(index, "HEAD_SPRITE_NAME");
        SetIcon(sprite, atlasName, spriteName);
    }

    static public void SetGemIcon(UISprite sprite, int index)
    {
        if (sprite == null)
        {
            return;
        }

        if (index <= 0)
        {
            sprite.atlas = null;
            return;
        }
        string atlasName = TableCommon.GetStringFromStoneTypeIconConfig(index, "STONE_ATLAS_NAME");
        string spriteName = TableCommon.GetStringFromStoneTypeIconConfig(index, "STONE_SPRITE_NAME");
        SetIcon(sprite, atlasName, spriteName);
    }

    static public void SetElementIcon(UISprite sprite, int index)
    {
        if (sprite == null)
        {
            return;
        }

        if (index < 0 || index > 4)
        {
            sprite.atlas = null;
            return;
        }
        string atlasName = TableCommon.GetStringFromElement(index, "ELEMENT_ATLAS_NAME");
        string spriteName = TableCommon.GetStringFromElement(index, "ELEMENT_SPRITE_NAME");
        SetIcon(sprite, atlasName, spriteName);
    }

    static public void SetElementBackground(UISprite sprite, int iIndex)
    {
        string strAtlasName = TableCommon.GetStringFromElement(iIndex, "ELEMENT_ATLAS_NAME");
        string strSpriteName = TableCommon.GetStringFromElement(iIndex, "ELEMENT_ICON_SPRITE_NAME");

        SetIcon(sprite, strSpriteName, strAtlasName);
    }

    static public void SetGoldIcon(UISprite sprite)
    {
        string atlasName = DataCenter.mItemIcon.GetData((int)ITEM_TYPE.GOLD, "ITEM_ICON_ATLAS");
        string spriteName = DataCenter.mItemIcon.GetData((int)ITEM_TYPE.GOLD, "ITEM_ICON_SPRITE");
        SetIcon(sprite, atlasName, spriteName);
    }

    static public void SetDiamondIcon(UISprite sprite)
    {
        string atlasName = DataCenter.mItemIcon.GetData((int)ITEM_TYPE.YUANBAO, "ITEM_ICON_ATLAS");
        string spriteName = DataCenter.mItemIcon.GetData((int)ITEM_TYPE.YUANBAO, "ITEM_ICON_SPRITE");
        SetIcon(sprite, atlasName, spriteName);
    }

    static public void SetPowerIcon(UISprite sprite)
    {
        string atlasName = DataCenter.mItemIcon.GetData((int)ITEM_TYPE.POWER, "ITEM_ICON_ATLAS");
        string spriteName = DataCenter.mItemIcon.GetData((int)ITEM_TYPE.POWER, "ITEM_ICON_SPRITE");
        SetIcon(sprite, atlasName, spriteName);
    }

	static public void SetHonorPointIcon(UISprite sprite)
	{
		string atlasName = DataCenter.mItemIcon.GetData((int)ITEM_TYPE.HONOR_POINT, "ITEM_ICON_ATLAS");
		string spriteName = DataCenter.mItemIcon.GetData((int)ITEM_TYPE.HONOR_POINT, "ITEM_ICON_SPRITE");
		SetIcon(sprite, atlasName, spriteName);
	}


	static public void SetSpiritIcon(UISprite sprite)
	{
		string atlasName = DataCenter.mItemIcon.GetData((int)ITEM_TYPE.SPIRIT, "ITEM_ICON_ATLAS");
		string spriteName = DataCenter.mItemIcon.GetData((int)ITEM_TYPE.SPIRIT, "ITEM_ICON_SPRITE");
		SetIcon(sprite, atlasName, spriteName);
	}

	static public void SetNumericalIcon(UISprite sprite, ITEM_TYPE type)
	{
		string atlasName = DataCenter.mItemIcon.GetData((int)type, "ITEM_ICON_ATLAS");
		string spriteName = DataCenter.mItemIcon.GetData((int)type, "ITEM_ICON_SPRITE");
		SetIcon(sprite, atlasName, spriteName);
	}

	static public void SetConsumeIcon(UISprite sprite, int index)
	{
		string strAtlas = TableCommon.GetStringFromConsumeConfig (index, "ITEM_ATLAS_NAME");
		string strSpriteName = TableCommon.GetStringFromConsumeConfig (index, "ITEM_SPRITE_NAME");
		SetIcon (sprite, strAtlas, strSpriteName);
	}

    static public void SetMaterialIcon(UISprite sprite, int index)
    {
        string strAtlas = TableCommon.GetStringFromMaterialConfig(index, "ICON_ATLAS_NAME");
        string strSpriteName = TableCommon.GetStringFromMaterialConfig(index, "ICON_SPRITE_NAME");
        SetIcon(sprite, strAtlas, strSpriteName);
    }

    static public void SetMaterialFragmentIcon(UISprite sprite, int index)
    {
        int iMaterialIndex = TableCommon.GetNumberFromMaterialFragment(index, "MATERIAL_INDEX");
        string strAtlas = TableCommon.GetStringFromMaterialConfig(iMaterialIndex, "ICON_ATLAS_NAME");
        string strSpriteName = TableCommon.GetStringFromMaterialConfig(iMaterialIndex, "ICON_SPRITE_NAME");
        SetIcon(sprite, strAtlas, strSpriteName);
    }

	static public void SetRoleEquipIcon(UISprite sprite, int index)
	{
		if (index <= 0)
		{
			sprite.atlas = null;
			return;
		}
		string atlasName = TableCommon.GetStringFromRoleEquipConfig(index, "ICON_ATLAS_NAME");
		string spriteName = TableCommon.GetStringFromRoleEquipConfig(index, "ICON_SPRITE_NAME");
		SetIcon(sprite, atlasName, spriteName);
	}

    static public void SetItemGrid(GameObject gridContainer, IItemDataProvider dataProvider)
    {
        UIGridContainer container = gridContainer.GetComponent<UIGridContainer>();

        if (container == null)
        {
            container = gridContainer.GetComponentInChildren<UIGridContainer>();
        }

        if (container != null)
        {
            ItemGrid grid = new ItemGrid(container);
            grid.Reset();
            grid.Set(dataProvider);
        }      
    }

    static public void SetItemIcon(GameObject iconObj, ItemData itemData)
    {
        ItemIcon icon = new ItemIcon(iconObj);
        icon.Reset();
        icon.Set(itemData);
    }

    static public void SetItemIcon(UISprite sprite, ITEM_TYPE type, int index)
    {
        switch (type)
        {
            case ITEM_TYPE.PET:
                SetPetIcon(sprite, index);
                break;
            case ITEM_TYPE.GEM:
                SetGemIcon(sprite, index);
                break;
            case ITEM_TYPE.GOLD:
                SetGoldIcon(sprite);
                break;
            case ITEM_TYPE.YUANBAO:
                SetDiamondIcon(sprite);
                break;
            case ITEM_TYPE.POWER:
                SetPowerIcon(sprite);
                break;
			case ITEM_TYPE.HONOR_POINT:
				SetHonorPointIcon(sprite);
				break;
			case ITEM_TYPE.EQUIP:
				SetRoleEquipIcon(sprite, index);
				break;
			case ITEM_TYPE.SPIRIT:
				SetSpiritIcon(sprite);
				break;
			case ITEM_TYPE.SAODANG_POINT:
			case ITEM_TYPE.RESET_POINT:
			case ITEM_TYPE.LOCK_POINT:
				SetNumericalIcon (sprite, type);
				break;
			case ITEM_TYPE.CONSUME_ITEM:
				SetConsumeIcon (sprite, index);
                break;
            case ITEM_TYPE.MATERIAL:
                SetMaterialIcon(sprite, index);
                break;
            case ITEM_TYPE.MATERIAL_FRAGMENT:
                SetMaterialFragmentIcon(sprite, index);
				break;
        }
    }

    static public void SetItemIcon(UISprite sprite, int type, int index)
    {
        SetItemIcon(sprite, (ITEM_TYPE)type, index);
    }

    static public void SetItemSmallIcon(UISprite sprite, int type)
    {
        string atlasName = DataCenter.mItemIcon.GetData(type, "ITEM_ATLAS_NAME");
        string spriteName = DataCenter.mItemIcon.GetData(type, "ITEM_SPRITE_NAME");
        SetIcon(sprite, atlasName, spriteName);
    }

    static public void SetItemSmallIcon(UISprite sprite, ITEM_TYPE type)
    {
        SetItemSmallIcon(sprite, (int)type);
    }

	public static float GetEquipAffectAttribute(DataRecord configRecord, AFFECT_TYPE affectType, float fValue, bool bIsPvpOpponent)
    {
        float fAffectValue = fValue;
		if (configRecord == null)
            return fAffectValue;

		string strClassName = configRecord.get ("CLASS");
        if (strClassName == "CHAR")
        {
			string equipLogicDataName = "EQUIP_DATA";
			if(bIsPvpOpponent)
			{
				equipLogicDataName = "OPPONENT_ROLE_EQUIP_DATA";
				return fAffectValue;
			}

			RoleEquipLogicData logic = DataCenter.GetData(equipLogicDataName) as RoleEquipLogicData;

            if (logic != null)
            {
				for(int i = (int)EQUIP_TYPE.ARM_EQUIP; i < (int)EQUIP_TYPE.MAX; i++)
				{
					EquipData equip = logic.GetUseEquip(i);
					if (equip != null)
					{
						fAffectValue += (equip.ApplyAffect(affectType, fValue) - fValue);
					}
				}
            }
        }
        else if (strClassName == "PET")
        {

        }

        return fAffectValue;
    }

	//--------------------------------------------------------------------------------------------------
	// affect attribute

	// max hp
	public static int GetMaxHP(int iModelIndex, int iLevel, int iStrengthenLevel)
	{
		return GetMaxHP(iModelIndex, iLevel, iStrengthenLevel, false);
	}

	public static int GetMaxHP(int iModelIndex, int iLevel, int iStrengthenLevel, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 1;

		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetMaxHP(dataRecord, iLevel, iStrengthenLevel, bIsPvpOpponent);
	}

	public static int GetMaxHP(DataRecord dataRecord, int iLevel, int iStrengthenLevel)
	{
		return GetMaxHP (dataRecord, iLevel, iStrengthenLevel, false);
	}

	public static int GetMaxHP(DataRecord dataRecord, int iLevel, int iStrengthenLevel, bool bIsPvpOpponent)
	{
		float fValue = GetBaseMaxHP(dataRecord, iLevel, iStrengthenLevel);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.HP_MAX, fValue, bIsPvpOpponent);
                if (strClass == "CHAR")
                {
                    fValue += GetRoleSkinHP(dataRecord);
                }
			}
		}
		
		return (int)fValue;
	}

    public static int GetRoleSkinHP(DataRecord dataRecord)
    {
        float fValue = 0;
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR" || strClass == "PET")
            {
                fValue = TableCommon.GetNumberFromRoleSkinConfig(dataRecord["INDEX"], "ROLE_SKIN_HP");
            }
        }

        return (int)fValue;
    }

	public static float GetBaseMaxHP(int iModelIndex, int iLevel, int iStrengthenLevel)
	{
		if(iModelIndex < 100000)
			return 1.0f;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseMaxHP(dataRecord, iLevel, iStrengthenLevel);
	}

	public static float GetBaseMaxHP(DataRecord dataRecord, int iLevel, int iStrengthenLevel)
	{
		if (dataRecord == null)
		{
			LOG.logError("GetBaseMaxHP..........dataRecord == null");
			return 1.0f;
		}

		float fBaseValue = (float)(dataRecord.get("BASE_HP"));

		string strClass = dataRecord.get ("CLASS");
		if(strClass == "CHAR" || strClass == "PET")
		{
			fBaseValue = (fBaseValue + (float)(dataRecord.get("ADD_HP")) * (iLevel - 1)) * (1 + 0.05f * iStrengthenLevel);
		}
		return fBaseValue;
	}

	// max mp
	public static int GetMaxMP(int iModelIndex, int iLevel, int iStrengthenLevel)
	{
		return GetMaxMP(iModelIndex, iLevel, iStrengthenLevel, false);
	}

	public static int GetMaxMP(int iModelIndex, int iLevel, int iStrengthenLevel, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 1;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetMaxMP(dataRecord, iLevel, iStrengthenLevel,  bIsPvpOpponent);
	}

	public static int GetMaxMP(DataRecord dataRecord, int iLevel, int iStrengthenLevel, bool bIsPvpOpponent)
	{
		float fValue = GetBaseMaxMP(dataRecord, iLevel, iStrengthenLevel);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.MP_MAX, fValue, bIsPvpOpponent);

                fValue += GetRoleSkinMP(dataRecord);
			}
		}

		return (int)fValue;
	}

    public static int GetRoleSkinMP(DataRecord dataRecord)
    {
        float fValue = 0;
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR")
            {
                fValue = TableCommon.GetNumberFromRoleSkinConfig(dataRecord["INDEX"], "ROLE_SKIN_MP");
            }
        }

        return (int)fValue;
    }

	public static float GetBaseMaxMP(int iModelIndex, int iLevel, int iStrengthenLevel)
	{
		if(iModelIndex < 100000)
			return 1.0f;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseMaxMP(dataRecord, iLevel, iStrengthenLevel);
	}

	public static float GetBaseMaxMP(DataRecord dataRecord, int iLevel, int iStrengthenLevel)
	{
		if (dataRecord == null)
		{
			LOG.logError("GetBaseMaxMP..........dataRecord == null");
			return 1.0f;
		}
		
		float fBaseValue = (float)(dataRecord.get("BASE_MP"));
		
		string strClass = dataRecord.get ("CLASS");
		if(strClass == "CHAR")
		{
			fBaseValue = (fBaseValue + (int)(dataRecord.get("ADD_MP")) * (iLevel - 1)) * (1 + 0.05f * iStrengthenLevel);
		}

		return fBaseValue;
	}    

	// attack
	public static float GetAttack(int iModelIndex, int iLevel, int iStrengthenLevel)
	{
		return GetAttack(iModelIndex, iLevel, iStrengthenLevel, false);
	}

	public static float GetAttack(int iModelIndex, int iLevel, int iStrengthenLevel, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 1.0f;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetAttack(dataRecord, iLevel, iStrengthenLevel, bIsPvpOpponent);
	}

	public static float GetAttack(DataRecord dataRecord, int iLevel, int iStrengthenLevel)
	{
		return GetAttack (dataRecord, iLevel, iStrengthenLevel, false);
	}

	public static float GetAttack(DataRecord dataRecord, int iLevel, int iStrengthenLevel, bool bIsPvpOpponent)
	{
		float fValue = GetBaseAttack(dataRecord, iLevel, iStrengthenLevel);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.ATTACK, fValue, bIsPvpOpponent);

                if (strClass == "CHAR")
                {
                    fValue += GetRoleSkinAttack(dataRecord);
                }
			}
		}
		return fValue;
	}

    public static int GetRoleSkinAttack(DataRecord dataRecord)
    {
        float fValue = 0;
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR")
            {
                fValue = TableCommon.GetNumberFromRoleSkinConfig(dataRecord["INDEX"], "ROLE_SKIN_ATTACK");
            }
        }

        return (int)fValue;
    }

	public static float GetBaseAttack(int iModelIndex, int iLevel, int iStrengthenLevel)
	{
		if(iModelIndex < 100000)
			return 1.0f;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseAttack(dataRecord, iLevel, iStrengthenLevel);
	}

	public static float GetBaseAttack(DataRecord dataRecord, int iLevel, int iStrengthenLevel)
	{
		if (dataRecord == null)
		{
			LOG.logError("GetBaseMaxMP..........dataRecord == null");
			return 0;
		}

		float fBaseValue = dataRecord.get("BASE_ATTACK");
		string strClass = dataRecord.get ("CLASS");
		if(strClass == "CHAR" || strClass == "PET")
		{
			fBaseValue = (fBaseValue + (float)(dataRecord.get("ADD_ATTACK")) * (iLevel - 1)) * (1 + 0.05f * iStrengthenLevel);
		}
		return fBaseValue;
	}

	// defence
	public static int GetDefence(int iModelIndex, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetDefence(dataRecord, bIsPvpOpponent);
	}

	public static int GetDefence(DataRecord dataRecord)
	{
		return GetDefence (dataRecord, false);
	}

	public static int GetDefence(DataRecord dataRecord, bool bIsPvpOpponent)
	{
		float fValue = GetBaseDefence(dataRecord);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.DEFENSE, fValue,  bIsPvpOpponent);
			}
		}
		return (int)fValue;
	}

	public static float GetBaseDefence(int iModelIndex)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseDefence(dataRecord);
	}

	public static float GetBaseDefence(DataRecord dataRecord)
	{
		if (dataRecord == null)
		{
			LOG.logError("GetBaseMaxMP..........dataRecord == null");
			return 0;
		}

        float fBaseValue = dataRecord.get("BASE_PHYSICAL_DEFENCE");
		return fBaseValue;
	}

	//defence rate

	// critical strike rate
	public static float GetCriticalStrikeRate(int iModelIndex, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetCriticalStrikeRate(dataRecord, bIsPvpOpponent);
	}

	public static float GetCriticalStrikeRate(DataRecord dataRecord)
	{
		return GetCriticalStrikeRate (dataRecord, false);
	}

	public static float GetCriticalStrikeRate(DataRecord dataRecord, bool bIsPvpOpponent)
	{
		float fValue = GetBaseCriticalStrikeRate(dataRecord);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.CRITICAL_STRIKE_RATE, fValue, bIsPvpOpponent);
			}
		}
		return fValue;
	}

	public static float GetBaseCriticalStrikeRate(int iModelIndex)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseCriticalStrikeRate(dataRecord);
	}
	
	public static float GetBaseCriticalStrikeRate(DataRecord dataRecord)
	{
		if (dataRecord == null)
		{
			LOG.logWarn("GetBaseCriticalStrikeRate..........dataRecord == null");
			return 0;
		}

		float fBaseValue = dataRecord.get("CRITICAL_STRIKE") / 100.0f;
		return fBaseValue;
	}

	// critical strike damage rate
	public static float GetCriticalStrikeDamageRate(int iModelIndex, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetCriticalStrikeDamageRate(dataRecord, bIsPvpOpponent);
	}

	public static float GetCriticalStrikeDamageRate(DataRecord dataRecord, bool bIsPvpOpponent)
	{
		float fValue = GetBaseCriticalStrikeDamageRate(dataRecord);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.HIT_CRITICAL_STRIKE_RATE, fValue, bIsPvpOpponent);
			}
		}
		return fValue;
	}

	public static float GetBaseCriticalStrikeDamageRate(int iModelIndex)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseCriticalStrikeDamageRate(dataRecord);
	}

	public static float GetBaseCriticalStrikeDamageRate(DataRecord dataRecord)
	{
		if (dataRecord == null)
		{
			LOG.logWarn("GetBaseCriticalStrikeDamageRate..........dataRecord == null");
			return 0;
		}

		float fBaseValue = dataRecord.get("CRITICAL_STRIKE_DAMAGE") / 100.0f;
		return fBaseValue;
	}

	// critical strike damage derate rate
	public static float GetCriticalStrikeDamageDerateRate(int iModelIndex, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetCriticalStrikeDamageDerateRate(dataRecord, bIsPvpOpponent);
	}

	public static float GetCriticalStrikeDamageDerateRate(DataRecord dataRecord, bool bIsPvpOpponent)
	{
		float fValue = GetBaseCriticalStrikeDamageDerateRate(dataRecord);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.DEFENSE_CRITICAL_DAMAGE_RATE, fValue, bIsPvpOpponent);
			}
		}

		return fValue;
	}

	public static float GetBaseCriticalStrikeDamageDerateRate(int iModelIndex)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseCriticalStrikeDamageDerateRate(dataRecord);
	}

	public static float GetBaseCriticalStrikeDamageDerateRate(DataRecord dataRecord)
	{
		if (dataRecord == null)
		{
			LOG.logWarn("GetBaseCriticalStrikeDamageDerateRate..........dataRecord == null");
			return 0;
		}

		float fBaseValue = dataRecord.get("CRITICAL_DAMAGE_DERATE") / 100.0f;
		return fBaseValue;
	}

	// hit rate
	public static float GetHitRate(int iModelIndex, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetHitRate(dataRecord, bIsPvpOpponent);
	}

	public static float GetHitRate(DataRecord dataRecord)
	{
		return GetHitRate (dataRecord, false);
	}

	public static float GetHitRate(DataRecord dataRecord, bool bIsPvpOpponent)
	{
		float fValue = GetBaseHitRate(dataRecord);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.HIT_TARGET_RATE, fValue, bIsPvpOpponent);
			}
		}

		return fValue;
	}

	public static float GetBaseHitRate(int iModelIndex)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseHitRate(dataRecord);
	}

	public static float GetBaseHitRate(DataRecord dataRecord)
	{
		if (dataRecord == null)
		{
			LOG.logWarn("GetBaseHitRate..........dataRecord == null");
			return 0;
		}

		float fBaseValue = dataRecord.get("HIT") / 100.0f;
		return fBaseValue;
	}

	// dodge rate
	public static float GetDodgeRate(int iModelIndex, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetDodgeRate(dataRecord, bIsPvpOpponent);
	}

	public static float GetDodgeRate(DataRecord dataRecord)
	{
		return  GetDodgeRate(dataRecord, false);
	}

	public static float GetDodgeRate(DataRecord dataRecord, bool bIsPvpOpponent)
	{
		float fValue = GetBaseDodgeRate(dataRecord);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.DODGE_RATE, fValue, bIsPvpOpponent);
			}
		}

		return fValue;
	}

	public static float GetBaseDodgeRate(int iModelIndex)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseDodgeRate(dataRecord);
	}

	public static float GetBaseDodgeRate(DataRecord dataRecord)
	{
		if (dataRecord == null)
		{
			LOG.logWarn("GetBaseDodgeRate..........dataRecord == null");
			return 0;
		}

		float fBaseValue = dataRecord.get("DODGE") / 100.0f;
		return fBaseValue;
	}

	// attack speed
	public static float GetAttackSpeed(int iModelIndex, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetAttackSpeed(dataRecord, bIsPvpOpponent);
	}

	public static float GetAttackSpeed(DataRecord dataRecord, bool bIsPvpOpponent)
	{
		float fValue = GetBaseAttackSpeed(dataRecord);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.ATTACK_SPEED, fValue, bIsPvpOpponent);
			}
		}

		return fValue;
	}

	public static float GetBaseAttackSpeed(int iModelIndex)
	{
		if(iModelIndex < 100000)
			return 0f;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseAttackSpeed(dataRecord);
	}

	public static float GetBaseAttackSpeed(DataRecord dataRecord)
	{
		if (dataRecord == null)
		{
			LOG.logWarn("GetBaseAttackSpeed..........dataRecord == null");
			return 0;
		}
		
		float fBaseValue = dataRecord.get("ATTACKSPEED") / 100.0f + 1.0f;
		return fBaseValue;
	}

	// damege mitigation rate
	public static float GetDamegeMitigationRate(int iModelIndex, bool bIsPvpOpponent)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetDamegeMitigationRate(dataRecord, bIsPvpOpponent);
	}

	public static float GetDamegeMitigationRate(DataRecord dataRecord)
	{
		return GetDamegeMitigationRate (dataRecord, false);
	}

	public static float GetDamegeMitigationRate(DataRecord dataRecord, bool bIsPvpOpponent)
	{
		float fValue = GetBaseDamegeMitigationRate(dataRecord);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.DEFENSE_HIT_RATE, fValue, bIsPvpOpponent);
			}
		}

		return fValue;
	}

	public static float GetBaseDamegeMitigationRate(int iModelIndex)
	{
		if(iModelIndex < 100000)
			return 0;
		
		DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetBaseDamegeMitigationRate(dataRecord);
	}

	public static float GetBaseDamegeMitigationRate(DataRecord dataRecord)
	{
		if (dataRecord == null)
		{
			LOG.logWarn("GetBaseDamegeMitigationRate..........dataRecord == null");
			return 0;
		}
		
		float fBaseValue = dataRecord.get("MITIGATIONG") / 100.0f;
		return fBaseValue;
	}

	//Element reduction rate
	public static float GetElementReductionRate(DataRecord dataRecord, int elementType)
	{
		return GetElementReductionRate (dataRecord, elementType, false);
	}

	public static float GetElementReductionRate(DataRecord dataRecord, int elementType, bool bIsPvpOpponent)
	{
		if(elementType >= (int)ELEMENT_TYPE.MAX)
			return 0;

		float fValue = GetBaseElementReductionRate(dataRecord, elementType);
		if(dataRecord != null)
		{
			string strClass = dataRecord.get ("CLASS");
			if(strClass == "CHAR" || strClass == "PET")
			{
				AFFECT_TYPE type = (AFFECT_TYPE)(elementType + (int)AFFECT_TYPE.RED_REDUCTION_RATE);

				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, type, fValue, bIsPvpOpponent);
			}
		}
		
		return fValue;
	}

	public static float GetBaseElementReductionRate(DataRecord dataRecord, int elementType)
	{
		if(elementType >= (int)ELEMENT_TYPE.MAX)
			return 0;

		if (dataRecord == null)
		{
			LOG.logWarn("GetBaseElementReductionRate..........dataRecord == null");
			return 0;
		}

		string strField = "RED_REDUCTION";
		strField = GameCommon.ToAffectTypeString ((AFFECT_TYPE)(elementType +  (int)AFFECT_TYPE.RED_REDUCTION_RATE));
		strField = strField.Replace ("_RATE", "");

		float fBaseValue = dataRecord.get(strField) / 100.0f;
		return fBaseValue;
	}

    // Move Speed
	public static float GetMoveSpeed(int iModelIndex, bool bIsPvpOpponent)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetMoveSpeed(dataRecord, bIsPvpOpponent);
    }

	public static float GetMoveSpeed(DataRecord dataRecord, bool bIsPvpOpponent)
    {
        float fValue = GetBaseMoveSpeed(dataRecord);
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR" || strClass == "PET")
            {
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.MOVE_SPEED, fValue, bIsPvpOpponent);
            }
        }

        return fValue;
    }

    public static float GetBaseMoveSpeed(int iModelIndex)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
        return GetBaseMoveSpeed(dataRecord);
    }

    public static float GetBaseMoveSpeed(DataRecord dataRecord)
    {
        if (dataRecord == null)
        {
            LOG.logWarn("GetBaseMoveSpeed..........dataRecord == null");
            return 0;
        }

        float fBaseValue = dataRecord.get("MOVE_SPEED");
        return fBaseValue;
    }

    // Defence Charm Rate
	public static float GetDefenceCharmRate(int iModelIndex, bool bIsPvpOpponent)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetDefenceCharmRate(dataRecord,  bIsPvpOpponent);
    }

	public static float GetDefenceCharmRate(DataRecord dataRecord, bool bIsPvpOpponent)
    {
        float fValue = GetBaseDefenceCharmRate(dataRecord);
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR" || strClass == "PET")
            {
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.DEFENSE_CHARM_RATE, fValue, bIsPvpOpponent);
            }
        }

        return fValue;
    }

    public static float GetBaseDefenceCharmRate(int iModelIndex)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
        return GetBaseDefenceCharmRate(dataRecord);
    }

    public static float GetBaseDefenceCharmRate(DataRecord dataRecord)
    {
        if (dataRecord == null)
        {
            LOG.logWarn("GetBaseDefenceCharmRate..........dataRecord == null");
            return 0;
        }

        float fBaseValue = dataRecord.get("CHARM_DEF") / 100f;
        return fBaseValue;
    }


    // Defence Fear Rate
	public static float GetDefenceFearRate(int iModelIndex, bool bIsPvpOpponent)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetDefenceFearRate(dataRecord,  bIsPvpOpponent);
    }

	public static float GetDefenceFearRate(DataRecord dataRecord, bool bIsPvpOpponent)
    {
        float fValue = GetBaseDefenceFearRate(dataRecord);
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR" || strClass == "PET")
            {
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.DEFENSE_FEAR_RATE, fValue, bIsPvpOpponent);
            }
        }

        return fValue;
    }

    public static float GetBaseDefenceFearRate(int iModelIndex)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
        return GetBaseDefenceFearRate(dataRecord);
    }

    public static float GetBaseDefenceFearRate(DataRecord dataRecord)
    {
        if (dataRecord == null)
        {
            LOG.logWarn("GetBaseDefenceFearRate..........dataRecord == null");
            return 0;
        }

        float fBaseValue = dataRecord.get("FEAR_DEF") / 100f;
        return fBaseValue;
    }

    // Defence Ice Rate
	public static float GetDefenceIceRate(int iModelIndex, bool bIsPvpOpponent)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetDefenceIceRate(dataRecord, bIsPvpOpponent);
    }

	public static float GetDefenceIceRate(DataRecord dataRecord, bool bIsPvpOpponent)
    {
        float fValue = GetBaseDefenceIceRate(dataRecord);
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR" || strClass == "PET")
            {
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.DEFENSE_ICE_RATE, fValue, bIsPvpOpponent);
            }
        }

        return fValue;
    }

    public static float GetBaseDefenceIceRate(int iModelIndex)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
        return GetBaseDefenceIceRate(dataRecord);
    }

    public static float GetBaseDefenceIceRate(DataRecord dataRecord)
    {
        if (dataRecord == null)
        {
            LOG.logWarn("GetBaseDefenceIceRate..........dataRecord == null");
            return 0;
        }

        float fBaseValue = dataRecord.get("FREEZE_DEF") / 100f;
        return fBaseValue;
    }


    // Defence Woozy Rate
	public static float GetDefenceWoozyRate(int iModelIndex, bool bIsPvpOpponent)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetDefenceWoozyRate(dataRecord, bIsPvpOpponent);
    }

	public static float GetDefenceWoozyRate(DataRecord dataRecord, bool bIsPvpOpponent)
    {
        float fValue = GetBaseDefenceWoozyRate(dataRecord);
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR" || strClass == "PET")
            {
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.DEFENSE_WOOZY_RATE, fValue, bIsPvpOpponent);
            }
        }

        return fValue;
    }

    public static float GetBaseDefenceWoozyRate(int iModelIndex)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
        return GetBaseDefenceWoozyRate(dataRecord);
    }

    public static float GetBaseDefenceWoozyRate(DataRecord dataRecord)
    {
        if (dataRecord == null)
        {
            LOG.logWarn("GetBaseDefenceWoozyRate..........dataRecord == null");
            return 0;
        }

        float fBaseValue = dataRecord.get("STUN_DEF") / 100f;
        return fBaseValue;
    }


    // Defence BeatBack Rate
	public static float GetDefenceBeatBackRate(int iModelIndex, bool bIsPvpOpponent)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetDefenceBeatBackRate(dataRecord, bIsPvpOpponent);
    }

	public static float GetDefenceBeatBackRate(DataRecord dataRecord, bool bIsPvpOpponent)
    {
        float fValue = GetBaseDefenceWoozyRate(dataRecord);
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR" || strClass == "PET")
            {
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.DEFENSE_BEATBACK_RATE, fValue,  bIsPvpOpponent);
            }
        }

        return fValue;
    }

    public static float GetBaseDefenceBeatBackRate(int iModelIndex)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
        return GetBaseDefenceBeatBackRate(dataRecord);
    }

    public static float GetBaseDefenceBeatBackRate(DataRecord dataRecord)
    {
        if (dataRecord == null)
        {
            LOG.logWarn("GetBaseDefenceBeatBackRate..........dataRecord == null");
            return 0;
        }

        float fBaseValue = dataRecord.get("BEATBACK_DEF") / 100f;
        return fBaseValue;
    }

    // Defence Down Rate
	public static float GetDefenceDownRate(int iModelIndex, bool bIsPvpOpponent)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetDefenceDownRate(dataRecord, bIsPvpOpponent);
    }

	public static float GetDefenceDownRate(DataRecord dataRecord, bool bIsPvpOpponent)
    {
        float fValue = GetBaseDefenceDownRate(dataRecord);
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR" || strClass == "PET")
            {
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.DEFENSE_DOWN_RATE, fValue,  bIsPvpOpponent);
            }
        }

        return fValue;
    }

    public static float GetBaseDefenceDownRate(int iModelIndex)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
        return GetBaseDefenceDownRate(dataRecord);
    }

    public static float GetBaseDefenceDownRate(DataRecord dataRecord)
    {
        if (dataRecord == null)
        {
            LOG.logWarn("GetBaseDefenceDownRate..........dataRecord == null");
            return 0;
        }

        float fBaseValue = dataRecord.get("KNOCKDOWN_DEF") / 100f;
        return fBaseValue;
    }


    // Auto HP
	public static float GetAutoHp(int iModelIndex, bool bIsPvpOpponent)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetAutoHp(dataRecord, bIsPvpOpponent);
    }

	public static float GetAutoHp(DataRecord dataRecord, bool bIsPvpOpponent)
    {
        float fValue = GetBaseAutoHp(dataRecord);
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR" || strClass == "PET")
            {
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.AUTO_HP, fValue, bIsPvpOpponent);
            }
        }

        return fValue;
    }

    public static float GetBaseAutoHp(int iModelIndex)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
        return GetBaseAutoHp(dataRecord);
    }

    public static float GetBaseAutoHp(DataRecord dataRecord)
    {
        if (dataRecord == null)
        {
            LOG.logWarn("GetBaseAutoHp..........dataRecord == null");
            return 0;
        }

        float fBaseValue = dataRecord.get("AUTO_HP");
        return fBaseValue;
    }


    // Auto MP
	public static float GetAutoMp(int iModelIndex, bool bIsPvpOpponent)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
		return GetAutoMp(dataRecord, bIsPvpOpponent);
    }

	public static float GetAutoMp(DataRecord dataRecord, bool bIsPvpOpponent)
    {
        float fValue = GetBaseAutoMp(dataRecord);
        if (dataRecord != null)
        {
            string strClass = dataRecord.get("CLASS");
            if (strClass == "CHAR" || strClass == "PET")
            {
				fValue = GameCommon.GetEquipAffectAttribute(dataRecord, AFFECT_TYPE.AUTO_MP, fValue, bIsPvpOpponent);
            }
        }

        return fValue;
    }

    public static float GetBaseAutoMp(int iModelIndex)
    {
        if (iModelIndex < 100000)
            return 0;

        DataRecord dataRecord = DataCenter.mActiveConfigTable.GetRecord(iModelIndex);
        return GetBaseAutoMp(dataRecord);
    }

    public static float GetBaseAutoMp(DataRecord dataRecord)
    {
        if (dataRecord == null)
        {
            LOG.logWarn("GetBaseAutoMp..........dataRecord == null");
            return 0;
        }

        float fBaseValue = dataRecord.get("AUTO_MP");
        return fBaseValue;
    }


    //---------------------------------------------------------------------------------------

    public static BaseObject ShowModel(int modelIndex, GameObject uiPoint, float scale)
    {
        uiPoint.SetActive(false);
        BaseObject obj = ObjectManager.Self.CreateObject(modelIndex, true, false);

        if (obj == null || obj.mMainObject == null)
            return null;

        UnityEngine.AI.NavMeshAgent agent = obj.mMainObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.enabled = false;

        float originScale = obj.mConfigRecord.get("UI_SCALE") * 100f;
        obj.mMainObject.transform.parent = uiPoint.transform.parent;
        obj.SetPosition(uiPoint.transform.position);
        obj.mMainObject.transform.localScale = Vector3.one * originScale * scale;
        obj.mMainObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        //obj.mMainObject.transform.Rotate(Vector3.up, 180, Space.World);
        obj.SetVisible(true);
        obj.OnIdle();
        GameCommon.SetLayer(obj.mMainObject, CommonParam.UILayer);

		if(ObjectManager.Self.GetObjectType(modelIndex) == OBJECT_TYPE.CHARATOR)
		{
			obj.SetLightColor(new Color(0, 0, 0, 1));
            Renderer[] rendlist = obj.mBodyObject.GetComponentsInChildren<Renderer>();
			foreach (Renderer render in rendlist)
			{
				if (render != null)
				{
					render.castShadows = false;
					render.receiveShadows = false;
                    render.material.shader = GameCommon.FindShader("Easy/TwoPassTransparent");
				}
			}
		}

        return obj;
    }

    public static BaseObject ShowCharactorModel(GameObject uiPoint, float scale)
    {
        uiPoint.SetActive(false);
        RoleData mainRole = RoleLogicData.GetMainRole();
        if (mainRole == null)
        {
            return null;
        }
        int modelIndex = mainRole.tid;
        if (modelIndex > 0)
        {
			JudgeRestoreCharacterConfigRecord();

            return ShowModel(modelIndex, uiPoint, scale);
        }     
        return null;
    }

    public static BaseObject ShowCharactorModel(GameObject uiPoint, int modelIndex, float scale)
    {
        uiPoint.SetActive(false);
        for (int m = 0; m < uiPoint.transform.parent.childCount; m++ )
        {
            GameObject obj = uiPoint.transform.parent.GetChild(m).gameObject;
            if (obj.activeSelf)
                GameObject.Destroy(obj);
        }
        if (modelIndex > 0)
        {
			JudgeRestoreCharacterConfigRecord();
		
            return ShowModel(modelIndex, uiPoint, scale);
        }
        return null;
    }

	public static void StoreCharacterConfigRecord()
	{
		DataRecord record = DataCenter.mActiveConfigTable.GetRecord (RoleLogicData.GetMainRole().tid);
		RolePartCongifRecord data = new RolePartCongifRecord();
		data.mIndex = record["INDEX"];
		data.mModelIndex = record["MODEL"];
		data.mAttackSkill = record["ATTACK_SKILL"];
		data.mScale = record["SCALE"];
		data.mUIScale = record["UI_SCALE"];
		
		DataCenter.Set ("ORIGINAL_ROLE_MODEL_RECORD", data);
	}

	//Note : must need store  before  change
	public static void ChangeCharacterConfigRecord()
	{
		ConsumeItemLogicStatus logic = DataCenter.GetData ("CONSUME_ITEM_STATUS") as ConsumeItemLogicStatus;
		if(logic != null && logic.GetChangeModelIndex () != 0)
		{
			StoreCharacterConfigRecord();

			DataRecord record = DataCenter.mActiveConfigTable.GetRecord (RoleLogicData.GetMainRole().tid);
			int changeModelIndex = logic.GetChangeModelIndex ();

			float fScale = DataCenter.mActiveConfigTable.GetRecord (changeModelIndex)["SCALE"];
			float fUIScale = DataCenter.mActiveConfigTable.GetRecord (changeModelIndex)["UI_SCALE"];
			record.set ("MODEL", TableCommon.GetNumberFromActiveCongfig (changeModelIndex, "MODEL"));
			record.set ("ATTACK_SKILL", TableCommon.GetNumberFromActiveCongfig (changeModelIndex, "ATTACK_SKILL"));
			record.set ("EFFECT", TableCommon.GetNumberFromActiveCongfig (changeModelIndex, "EFFECT"));
			record.set ("UI_EFFECT", TableCommon.GetNumberFromActiveCongfig (changeModelIndex, "UI_EFFECT"));
			record.set ("SCALE", fScale);
			record.set ("UI_SCALE", fUIScale);
		}
	}

	public static void JudgeRestoreCharacterConfigRecord()
	{
		ConsumeItemLogicStatus logic = DataCenter.GetData ("CONSUME_ITEM_STATUS") as ConsumeItemLogicStatus;
		if(logic == null || (logic != null && logic.GetChangeModelIndex () == 0))
			RestoreCharacterConfigRecord ();
	}
	
	public static bool RestoreCharacterConfigRecord()
	{
		object obj;
		DataCenter.Self.getData ("ORIGINAL_ROLE_MODEL_RECORD", out obj);
		if(obj != null)
		{
			RolePartCongifRecord data = obj as RolePartCongifRecord;
			if(data != null)
			{
				DataRecord record = DataCenter.mActiveConfigTable.GetRecord (data.mIndex);
				record.set ("MODEL", data.mModelIndex);
				record.set ("ATTACK_SKILL", data.mAttackSkill);
				record.set ("EFFECT", data.mEffect);
				record.set ("UI_EFFECT", data.mUIEffect);
				record.set ("SCALE", data.mScale);
				record.set ("UI_SCALE", data.mUIScale);

				DataCenter.Set ("ORIGINAL_ROLE_MODEL_RECORD", null);

				return true;
			}
		}

		return false;
	}

    public static T FindComponent<T>(GameObject parentObject, string targetObjName) where T : Component
    {
        GameObject obj = FindObject(parentObject, targetObjName);
        if (obj != null)
        {
            return obj.GetComponent<T>();
        }
        return null;
    }

    static public void SetBufferIcon(UISprite sprite, int index)
    {
        if (index <= 0)
        {
            sprite.atlas = null;
            return;
        }

        string atlasName = TableCommon.GetStringFromAffectBuffer(index, "BUFFER_ATLAS_NAME");
        string spriteName = TableCommon.GetStringFromAffectBuffer(index, "BUFFER_SPRITE_NAME");
        sprite.atlas = LoadUIAtlas(atlasName);
        sprite.spriteName = spriteName;
    }

    static public void SetPetIconWithElementAndStar(GameObject obj, string iconName, string elementName, string starName, int modelIndex)
    {
        int elementIndex = TableCommon.GetNumberFromActiveCongfig(modelIndex, "ELEMENT_INDEX");
        int starLevel = TableCommon.GetNumberFromActiveCongfig(modelIndex, "STAR_LEVEL");
        UISprite icon = FindComponent<UISprite>(obj, iconName);
        UISprite element = FindComponent<UISprite>(obj, elementName);
        SetPetIcon(icon, modelIndex);
        SetElementIcon(element, elementIndex);
        //SetUIText(obj, starName, starLevel.ToString());
        SetStarLevelLabel(obj, starLevel, starName);
    }

	static public void SetPetIconWithElementAndStarAndLevel(GameObject obj, string iconName, string elementName, string starName, string levelName, int levelNum, int modelIndex)
	{
		SetPetIconWithElementAndStar(obj, iconName, elementName, starName, modelIndex);
		GameCommon.SetUIText (obj, levelName, "Lv." + levelNum.ToString ());
	}

    static public void SetMonsterIconWithStar(GameObject obj, string iconName, string starName, int index)
    {
        if (index <= 0)
            return;

        DataRecord record = DataCenter.mMonsterObject.GetRecord(index);

        if (record == null)
            return;

        string atlasName = record.getData("HEAD_ATLAS_NAME");
        string spriteName = record.getData("HEAD_SPRITE_NAME");
        int starLevel = record.getData("STAR_LEVEL");
        SetIcon(obj, iconName, spriteName, atlasName);
        SetUIText(obj, starName, starLevel.ToString());
    }

    static public void RemoveFromDictionary<T>(Dictionary<int, T> dic, Predicate<T> match)
    {
        List<int> wantToRemoveKeys = new List<int>();

        foreach (KeyValuePair<int, T> pair in dic)
        {
            if (match(pair.Value))
            {
                wantToRemoveKeys.Add(pair.Key);
            }
        }

        foreach (int key in wantToRemoveKeys)
        {
            dic.Remove(key);
        }
    }

    static public void RemoveFromNiceTable(NiceTable table, Predicate<DataRecord> match)
    {
        List<int> wantToRemoveKeys = new List<int>();
    
        foreach (KeyValuePair<int, DataRecord> pair in table.GetAllRecord())
        {
            if (match(pair.Value))
            {
                wantToRemoveKeys.Add(pair.Key);
            }
        }
    
        foreach (int key in wantToRemoveKeys)
        {
            table.DeleteRecord(key);
        }  
    }

    static public void RemoveAirFromItemTable(NiceTable itemTable, string itemTypeFieldName)
    {
        RemoveFromNiceTable(itemTable, r => r[itemTypeFieldName] == (int)ITEM_TYPE.AIR);   
    }

    static public bool TryGetEnumFromString<T>(string enumString, ref T enumValue)
    {
        Type enumType = typeof(T);
        if (Enum.IsDefined(enumType, enumString))
        {
            enumValue = (T)Enum.Parse(enumType, enumString);
            return true;
        }
        return false;
    }

    static public T GetEnumFromString<T>(string enumString, T defaultEnumValue)
    {
        T enumValue = defaultEnumValue;
        TryGetEnumFromString<T>(enumString, ref enumValue);
        return enumValue;
    }

    static public T GetEnumFromString<T>(string enumString)
    {
        return GetEnumFromString<T>(enumString, default(T));
    }

    static public GameObject FindObject(GameObject parentObj, params string[] pathNames)
    {
        GameObject current = parentObj;

        foreach (string targetName in pathNames)
        {
            if (current == null)
                return null;

            current = FindObject(current, targetName);
        }

        return current;
    }

    static public GameObject FindUI(params string[] pathNames)
    {
        GameObject obj = GameObject.Find(CommonParam.UIRootName);

        if (obj != null)
            return FindObject(obj, pathNames);

        return null;
    }

    static public bool SetUIButtonEnabled(GameObject parentObj, string btnName, bool isEnabled)
    {
        GameObject btn = FindObject(parentObj, btnName);
        
        if (btn == null)
            return false;

        UIButton uiButton = btn.GetComponent<UIButton>();

        if (uiButton == null)
        {
            UIImageButton uiImageButton = btn.GetComponent<UIImageButton>();

            if (uiImageButton == null)
            {
                return false;
            }
            else
            {
                uiImageButton.isEnabled = isEnabled;
                return true;
            }
        }
        else
        {
            uiButton.isEnabled = isEnabled;
            return true;
        }
    }

	public static GameObject GetMainCameraObj()
	{
		GameObject worldCenter = GameObject.Find("world_center");
		if(worldCenter != null)
		{
			GameObject cameraObj = GameCommon.FindObject(worldCenter, "Camera");
			if(cameraObj != null)
				return cameraObj;
		}

		return null;
	}

	public static Camera GetMainCamera()
	{
		GameObject cameraObj = GetMainCameraObj();
		if(cameraObj != null)
		{
			return cameraObj.GetComponent<Camera>();
		}
		return null;
	}

	public static bool SetMainCameraEnable(bool isEnable)
	{
		Camera camera = GameCommon.GetMainCamera();
		if(camera != null)
		{
			camera.enabled = isEnable;
			return true;
		}
		return false;
	}

	public static int GetFightingStrength(int index, int level, int strengthenLevel)
	{
		return GetFightingStrength (index, level, strengthenLevel, false);
	}

    public static int GetFightingStrength(int index, int level, int strengthenLevel, bool bIsPvpOpponent)
    {
        if (index == 0)
            return 0;

        DataRecord record = DataCenter.mActiveConfigTable.GetRecord(index);

        if (record == null)
            return 0;

		int hp = GameCommon.GetMaxHP(index, level, strengthenLevel,  bIsPvpOpponent);
		int mp = GameCommon.GetMaxMP(index, level, strengthenLevel,  bIsPvpOpponent);
		float attack = GameCommon.GetAttack(index, level, strengthenLevel,  bIsPvpOpponent);
		int defence = GameCommon.GetDefence(record, bIsPvpOpponent);

		float hitRate = GameCommon.GetHitRate(record, bIsPvpOpponent);
		float dodgeRate = GameCommon.GetDodgeRate(record, bIsPvpOpponent);
		float criticalRate = GameCommon.GetCriticalStrikeRate(record, bIsPvpOpponent);
		float criticalDamage = GameCommon.GetCriticalStrikeDamageRate(record, bIsPvpOpponent);
		float criticalDamageDerate = GameCommon.GetCriticalStrikeDamageDerateRate(record, bIsPvpOpponent);
		float attackSpeed = GameCommon.GetAttackSpeed(record, bIsPvpOpponent);
		float charmDef = GameCommon.GetDefenceCharmRate(record, bIsPvpOpponent);
		float fearDef = GameCommon.GetDefenceFearRate(record, bIsPvpOpponent);
		float freezeDef = GameCommon.GetDefenceIceRate(record, bIsPvpOpponent);
		float woozyDef = GameCommon.GetDefenceWoozyRate(record, bIsPvpOpponent);
		float beatbackDef = GameCommon.GetDefenceBeatBackRate(record, bIsPvpOpponent);
		float knockdownDef = GameCommon.GetDefenceDownRate(record, bIsPvpOpponent);
		float mitigation = GameCommon.GetDamegeMitigationRate(record, bIsPvpOpponent);

        float num1 = hp * 1f + attack * 15f + defence * 10f + mp * 5f;

        float num2 = 1f + hitRate * 1f + dodgeRate * 1f + criticalRate * 2f + criticalDamage * 1f + attackSpeed * 1f
            + criticalDamageDerate * 1f + charmDef * 1f + fearDef * 1f + freezeDef * 1f + woozyDef * 1f
            + beatbackDef * 1f + knockdownDef * 1f + mitigation * 2f;

        int strength = (int)(num1 * num2 / 6f);

        if (ObjectManager.Self.GetObjectType(index) == OBJECT_TYPE.CHARATOR)
        {
            strength += DataCenter.mRoleSkinConfig.GetData(RoleLogicData.GetMainRole().tid, "FIGHT_STRENGTH");
        }
        else if (ObjectManager.Self.GetObjectType(index) == OBJECT_TYPE.OPPONENT_CHARACTER && OpponentCharacter.mInstance != null)
        {
            strength += DataCenter.mRoleSkinConfig.GetData(OpponentCharacter.mInstance.mConfigIndex, "FIGHT_STRENGTH");            
        }

        return strength;
    }

    public static int GetTotalFightingStrength()
    {
        RoleData roleData = RoleLogicData.Self.character;
        int sum = 0;

        if (roleData != null)
        {
            sum += GameCommon.GetFightingStrength(roleData.tid, roleData.level, 0);
        }

        PetData[] petDatas = PetLogicData.Self.GetPetDatasByTeam(PetLogicData.Self.mCurrentTeam);

        foreach (var data in petDatas)
        {
            if (data != null)
            {
                sum += GameCommon.GetFightingStrength(data.tid, data.level, data.strengthenLevel);
            }
        }

        return sum;
    }

	public static string GetAttackType(int iPetIndex)
	{
		int iPetType = TableCommon.GetNumberFromActiveCongfig(iPetIndex, "PET_TYPE");
		return(TableCommon.GetStringFromPetType(iPetType, "NAME"));
	}
	
	static public void SplitConsumeData(ref ConsumeItemData data, ConsumeItemLogicData logicData)
	{
		if(data.itemNum > 900)
		{
			data.itemNum -= 900;
			ConsumeItemData d = new ConsumeItemData();
			d.itemNum = 900;
			d.mCountdownTime = data.mCountdownTime;
			d.tid = data.tid;
			logicData.mConsumeItemList.Add (d);
			SplitConsumeData (ref data, logicData);
		}
	}

	static public void AddConsumeData(int index, int count, ConsumeItemLogicData logicData)
	{
		if(count <= 0)
			return;

		int num = logicData.mConsumeItemList.Count;
		for(int i = 0; i < num; i++)
		{
			ConsumeItemData consumeData = logicData.mConsumeItemList[i];
			if(consumeData.tid == index)
			{
				logicData.mConsumeItemList.Remove (consumeData);
				i --;
				num --;
			}
		}

		ConsumeItemData consumeItemData = new ConsumeItemData();
		consumeItemData.tid = index;
		consumeItemData.itemNum = count;
		GameCommon.SplitConsumeData(ref consumeItemData, logicData);
		
		logicData.mConsumeItemList.Add (consumeItemData);
	}

	static public bool FunctionIsUnlock(UNLOCK_FUNCTION_TYPE type)
	{
		int iType = (int)type;
		if(iType == 0)
			return true;

		int iTypeInConfig = 0;
		foreach( KeyValuePair<int, DataRecord> re in DataCenter.mPlayerLevelConfig.GetAllRecord())
		{
			if(iType == re.Value["UNLOCK_FUNCTION_1"] || iType == re.Value["UNLOCK_FUNCTION_2"])
			{
				iTypeInConfig++;
				int iNeedPlayerLevel = re.Key;
				if(RoleLogicData.Self.chaLevel >= iNeedPlayerLevel)
					return true;
			}
		}

		if(iTypeInConfig == 0)
			return true;

		return false;
	}

	static public int FunctionUnlockNeedLevel(UNLOCK_FUNCTION_TYPE type)
	{
		int iType = (int)type;
		foreach( KeyValuePair<int, DataRecord> re in DataCenter.mPlayerLevelConfig.GetAllRecord())
		{
			if(iType == re.Value["UNLOCK_FUNCTION_1"] || iType == re.Value["UNLOCK_FUNCTION_2"])
			{
				return re.Key;
			}
		}
		return 0;
	}

	static public void AddPlayerLevelExpWhenPveWin()
	{
		int curStageIndex = DataCenter.Get ("CURRENT_STAGE");
		int playerAddExp = TableCommon.GetNumberFromStageConfig (curStageIndex, "PLAYER_EXP");
		RoleLogicData.Self.AddPlayerLevelExp (playerAddExp);
	}

	static public void ToggleCloseButCanClick(GameObject obj, UNLOCK_FUNCTION_TYPE type, string strVisibleName)
	{
		if(obj == null)
			return;

		obj.GetComponent<UIButtonEvent>().mData.set ("TYPE", type);

		bool bIsUnlock = FunctionIsUnlock (type);
		obj.GetComponent<UIToggle>().enabled = bIsUnlock;
		FindObject (obj, strVisibleName).SetActive (bIsUnlock);

		UILabel[] labels = obj.GetComponentsInChildren<UILabel>();
		Color color = bIsUnlock ? Color.white : Color.gray;
	}

	static public void ButtonEnbleButCanClick(GameObject obj, string strName, UNLOCK_FUNCTION_TYPE type)
	{
		bool bVisible = FunctionIsUnlock (type);
		SetUIVisiable (obj, strName , bVisible);
		SetUIVisiable (obj, strName + "_gray", !bVisible);
		GetButtonData (obj, strName + "_gray").set ("TYPE", type);
	}

	static public bool ButtonEnbleShowInfo(UNLOCK_FUNCTION_TYPE type)
	{
		if(!FunctionIsUnlock (type))
		{
			DataCenter.OpenWindow ("PLAYER_LEVEL_UP_SHOW_WINDOW", false);
			DataCenter.SetData ("PLAYER_LEVEL_UP_SHOW_WINDOW", "NEED_LEVEL", type);
		}

		return !FunctionIsUnlock (type);
	}

	static public int GetRoleEquipSetIndex()
	{
		RoleEquipLogicData roleEquipLogicData = DataCenter.GetData("EQUIP_DATA") as RoleEquipLogicData;
		Dictionary<int, EquipData> dictUseEquips = roleEquipLogicData.GetCurRoleAllUseEquips();

		List<int> vecEquipIndex = new List<int>();
		for(int i = (int)EQUIP_TYPE.ARM_EQUIP; i < (int)EQUIP_TYPE.MAX; i++)
		{
			EquipData equipData = roleEquipLogicData.GetUseEquip(i);
			if(equipData != null)
			{
				vecEquipIndex.Add(equipData.tid);
			}
		}

		if(vecEquipIndex.Count >= 4)
		{
			foreach(KeyValuePair<int, DataRecord> v in DataCenter.mSetEquipConfig.GetAllRecord())
			{
				if(v.Value.get("SET_EQUIP_0") == vecEquipIndex[0]
				   && v.Value.get("SET_EQUIP_1") == vecEquipIndex[1]
				   && v.Value.get("SET_EQUIP_2") == vecEquipIndex[2]
				   && v.Value.get("SET_EQUIP_3") == vecEquipIndex[3])
				{
					return v.Key;
				}
			}
		}
		return 0;
	}

	public static int GetEquipAttachAttributeIndex(int iModelIndex, int iAttributeType)
	{
		int iGroupID = TableCommon.GetNumberFromRoleEquipConfig(iModelIndex, "ATTACHATTRIBUTE_GROUP");
		Dictionary<int, DataRecord> dictRecord = DataCenter.mEquipAttachAttributeConfig.GetAllRecord();
		foreach(KeyValuePair<int, DataRecord> pair in dictRecord)
		{
			if(pair.Value["ATTACHATTRIBUTE_GROUP"] == iGroupID
			   && pair.Value["TYPE"] == iAttributeType)
			{
				return pair.Key;
			}
		}
		return 0;
	}

	public static List<T> SortList<T>(List<T>mList, Comparison<T>mSort)
	{
		mList.Sort (mSort);
		return mList;
	}

	public static int Sort(int a, int b, bool isDescending)
	{
		if(a == b) return 0;
		if(isDescending) return  a > b ? -1 : 1;
		else return a > b ? 1 : -1;
	}
	
	public static int Sort(bool a, bool b, bool isDescengding)
	{
		if(a == b) return 0; 
		if(isDescengding) return a? -1 : 1;
		else return a? 1 : -1;
	}
	
	public static int Sort(bool bA, bool bB, bool bIsDescending, params SortListParam[] mParam)
	{
		if(Sort (bA, bB, bIsDescending) == 0)
		{
			return Sort (mParam);
//			if(Sort (iA1 , iB1, isDescending1) == 0)
//			{
//				if(Sort (iA2 , iB2, isDescending2) == 0) return(Sort (iA3 , iB3, isDescending3));
//				else return Sort (iA2 , iB2, isDescending2);
//			}
//			else return Sort (iA1 , iB1, isDescending1);
		}
		else return Sort (bA, bB, bIsDescending);
	}

	public static int Sort(params SortListParam[] mParam)
	{
		for(int i = 0; i < mParam.Length; i++)
		{
			if(Sort (mParam[i].mParam1, mParam[i].mParam2, mParam[i].bIsDescending) == 0)
			{
				if(i == mParam.Length - 2) return Sort (mParam[i+1].mParam1, mParam[i+1].mParam2, mParam[i+1].bIsDescending);
				else continue;
			}
			else return Sort (mParam[i].mParam1, mParam[i].mParam2, mParam[i].bIsDescending);
		}

		return Sort (mParam[0].mParam1, mParam[0].mParam2, mParam[0].bIsDescending);
	}

	public static void GetSortListParamFromPetData(SORT_TYPE type, PetData a, PetData b, out SortListParam sStarLevel, out SortListParam sLevel, out SortListParam sModelIndex, out SortListParam sElement)
	{
		sStarLevel = new SortListParam{mParam1 = a.starLevel, mParam2 = b.starLevel, bIsDescending = true};
		sLevel = new SortListParam{mParam1 = a.level, mParam2 = b.level, bIsDescending = true};
		sModelIndex = new SortListParam{mParam1 = a.tid, mParam2 = b.tid, bIsDescending = false};
		sElement = new SortListParam{mParam1 = TableCommon.GetNumberFromActiveCongfig(a.tid, "ELEMENT_INDEX"), 
			mParam2 = TableCommon.GetNumberFromActiveCongfig(b.tid, "ELEMENT_INDEX"), bIsDescending = false};

		if(type != null)
		{
			switch(type)
			{
			case SORT_TYPE.STAR_LEVEL :
				sStarLevel = new SortListParam{mParam1 = a.starLevel, mParam2 = b.starLevel, bIsDescending = DataCenter.Get ("DESCENDING_ORDER")};
				break;
			case SORT_TYPE.LEVEL :
				sLevel = new SortListParam{mParam1 = a.level, mParam2 = b.level, bIsDescending = DataCenter.Get ("DESCENDING_ORDER")};
				break;
			case SORT_TYPE.ELEMENT_INDEX:
				sElement = new SortListParam{mParam1 = TableCommon.GetNumberFromActiveCongfig(a.tid, "ELEMENT_INDEX"), 
					mParam2 = TableCommon.GetNumberFromActiveCongfig(b.tid, "ELEMENT_INDEX"), bIsDescending = !DataCenter.Get ("DESCENDING_ORDER")};
				break;
			}
		}
	}

	public static int SortPetDataByStarLevel(PetData a, PetData b)
	{
		SortListParam sStarLevel, sLevel, sModelIndex, sElement;
		GetSortListParamFromPetData (SORT_TYPE.STAR_LEVEL, a, b, out sStarLevel, out sLevel, out sModelIndex, out sElement);

		return Sort (sStarLevel, sLevel, sModelIndex, sElement);
	}

	public static int SortPetDataByLevel(PetData a, PetData b)
	{
		SortListParam sStarLevel, sLevel, sModelIndex, sElement;
		GetSortListParamFromPetData (SORT_TYPE.LEVEL, a, b, out sStarLevel, out sLevel, out sModelIndex, out sElement);
		
		return Sort (sLevel, sStarLevel, sModelIndex, sElement);
	}

	public static int SortPetDataByElement(PetData a, PetData b)
	{
		SortListParam sStarLevel, sLevel, sModelIndex, sElement;
		GetSortListParamFromPetData (SORT_TYPE.ELEMENT_INDEX, a, b, out sStarLevel, out sLevel, out sModelIndex, out sElement);
		
		return Sort (sElement, sStarLevel, sLevel, sModelIndex);
	}

	public static int SortAutoJoinPetDataList(PetData a, PetData b)
	{
		SortListParam sStarlevel, sLevel, sModelIndex, sElement, sStrengthenLevel;
		GetSortListParamFromPetData (SORT_TYPE.AUTO_JOIN, a, b, out sStarlevel, out sLevel, out sModelIndex, out sElement);
		sStrengthenLevel = new SortListParam{mParam1 = a.strengthenLevel, mParam2 = b.strengthenLevel, bIsDescending = true};
		
		return Sort (sStarlevel, sLevel, sStrengthenLevel, sModelIndex, sElement);
	}

	public static void GetSortListParamFromEquipData(SORT_TYPE type, EquipData a, EquipData b, out SortListParam sStarLevel, out SortListParam sStrengthenLevel, out SortListParam sModelIndex, out SortListParam sElement)
	{
		sStarLevel = new SortListParam{mParam1 = a.mStarLevel, mParam2 = b.mStarLevel, bIsDescending = true};
		sStrengthenLevel = new SortListParam{mParam1 = a.strengthenLevel, mParam2 = b.strengthenLevel, bIsDescending = true};
		sModelIndex = new SortListParam{mParam1 = a.tid, mParam2 = b.tid, bIsDescending = false};
		sElement = new SortListParam{mParam1 = (int)a.mElementType, mParam2 = (int)b.mElementType, bIsDescending = false};

		if(type != null)
		{
			switch(type)
			{
			case SORT_TYPE.STAR_LEVEL :
				sStarLevel = new SortListParam{mParam1 = a.mStarLevel, mParam2 = b.mStarLevel, bIsDescending = DataCenter.Get ("DESCENDING_ORDER")};
				break;
			case SORT_TYPE.STRENGTHEN_LEVEL :
				sStrengthenLevel = new SortListParam{mParam1 = a.strengthenLevel, mParam2 = b.strengthenLevel, bIsDescending = DataCenter.Get ("DESCENDING_ORDER")};
				break;
			case SORT_TYPE.ELEMENT_INDEX:
				sElement = new SortListParam{mParam1 = (int)a.mElementType, mParam2 = (int)b.mElementType, bIsDescending = !DataCenter.Get ("DESCENDING_ORDER")};
				break;
			}
		}
	}

	public static int SortEquipDataByStarLevel(EquipData a, EquipData b)
	{
		SortListParam sStarLevel, sStrengthenLevel, sModelIndex, sElement;
		GetSortListParamFromEquipData(SORT_TYPE.STAR_LEVEL, a, b, out sStarLevel, out sStrengthenLevel, out sModelIndex, out sElement);

		return Sort (sStarLevel, sStrengthenLevel, sModelIndex, sElement);
	}

	public static int SortEquipDataByStrengthenLevel(EquipData a, EquipData b)
	{
		SortListParam sStarLevel, sStrengthenLevel, sModelIndex, sElement;
		GetSortListParamFromEquipData(SORT_TYPE.STRENGTHEN_LEVEL ,a, b, out sStarLevel, out sStrengthenLevel, out sModelIndex, out sElement);
		
		return Sort (sStrengthenLevel, sStarLevel, sModelIndex, sElement);
	}

	public static int SortEquipDataByElement(EquipData a, EquipData b)
	{
		SortListParam sStarLevel, sStrengthenLevel, sModelIndex, sElement;
		GetSortListParamFromEquipData(SORT_TYPE.ELEMENT_INDEX, a, b, out sStarLevel, out sStrengthenLevel, out sModelIndex, out sElement);
		
		return Sort (sElement, sStarLevel, sStrengthenLevel, sModelIndex);
	}
	//fabaohecheng paixu 
	public static void GetSortListParamFromEquipsData(SORT_TYPE type, DataRecord a, DataRecord b, out SortListParam sStarLevel)
	{
		sStarLevel = new SortListParam{mParam1 = a["STAR_LEVEL"], mParam2 = b["STAR_LEVEL"], bIsDescending = true};
		if(type != null)
		{
			switch(type)
			{
			case SORT_TYPE.STAR_LEVEL :
				sStarLevel = new SortListParam{mParam1 = a["STAR_LEVEL"], mParam2 = b["STAR_LEVEL"], bIsDescending = DataCenter.Get ("DESCENDING_ORDER")};
				break;
			}
		}
	}

	public static int SortEquipsDataByStarLevel(DataRecord a, DataRecord b)
	{
		SortListParam sStarLevel;
		GetSortListParamFromEquipsData(SORT_TYPE.STAR_LEVEL, a, b, out sStarLevel);
		
		return Sort (sStarLevel);
	}
	//Material rank
	public static void GetSortListParamFromMaterialData(SORT_TYPE type, DataRecord a, DataRecord b, out SortListParam sStarLevel)
	{
		sStarLevel = new SortListParam{mParam1 = a["INDEX"], mParam2 = b["INDEX"], bIsDescending = true};
		
		if(type != null)
		{
			switch(type)
			{
			case SORT_TYPE.STAR_LEVEL :
				sStarLevel = new SortListParam{mParam1 = a["INDEX"], mParam2 = b["INDEX"], bIsDescending = DataCenter.Get ("DESCENDING_ORDER")};
				break;
			}
		}
	}

	public static int SortMaterialDataByStarLevel(DataRecord a, DataRecord b)
	{
		SortListParam sStarLevel;
		GetSortListParamFromMaterialData(SORT_TYPE.STAR_LEVEL, a, b, out sStarLevel);
		
		return Sort (sStarLevel);
	}
	
	public static void GetSortListParamFromPetFragmentData(PetFragmentData a, PetFragmentData b, out SortListParam sStarLevel, out SortListParam sCount, out SortListParam sModelIndex, out SortListParam sElement)
	{
		sStarLevel = new SortListParam{mParam1 = TableCommon.GetNumberFromActiveCongfig(a.mPetModelIndex, "STAR_LEVEL"), 
			mParam2 = TableCommon.GetNumberFromActiveCongfig(b.mPetModelIndex, "STAR_LEVEL"), bIsDescending = true};
		sCount = new SortListParam{mParam1 = a.itemNum, mParam2 = b.itemNum, bIsDescending = true};
		sModelIndex = new SortListParam{mParam1 = a.tid, mParam2 = b.tid, bIsDescending = false};
		sElement = new SortListParam{mParam1 = TableCommon.GetNumberFromActiveCongfig(a.mPetModelIndex, "ELEMENT_INDEX"), 
			mParam2 = TableCommon.GetNumberFromActiveCongfig(b.mPetModelIndex, "ELEMENT_INDEX"), bIsDescending = false};
	}

	public static int SortPetFragmentDataByStarLevel(PetFragmentData a, PetFragmentData b)
	{
		SortListParam sStarLevel, sCount, sModelIndex, sElement;
		GetSortListParamFromPetFragmentData(a, b, out sStarLevel, out sCount, out sModelIndex, out sElement);
		
		return Sort (sStarLevel, sCount, sModelIndex, sElement);
	}

	public static int SortPetFragmentDataByNumber(PetFragmentData a, PetFragmentData b)
	{
		SortListParam sStarLevel, sCount, sModelIndex, sElement;
		GetSortListParamFromPetFragmentData(a, b, out sStarLevel, out sCount, out sModelIndex, out sElement);
		
		return Sort (sCount, sStarLevel, sModelIndex, sElement);
	}

	public static int SortPetFragmentDataByElement(PetFragmentData a, PetFragmentData b)
	{
		SortListParam sStarLevel, sCount, sModelIndex, sElement;
		GetSortListParamFromPetFragmentData(a, b, out sStarLevel, out sCount, out sModelIndex, out sElement);
		
		return Sort (sElement, sStarLevel, sCount, sModelIndex);
	}

    public static void GetSortListParamFromMaterialData(MaterialData a, MaterialData b, out SortListParam sCount, out SortListParam sIndex)
    {
        sCount = new SortListParam { mParam1 = a.mCount, mParam2 = b.mCount, bIsDescending = true };
        sIndex = new SortListParam { mParam1 = a.mIndex, mParam2 = b.mIndex, bIsDescending = false };
    }

    public static int SortMaterialDataByNumber(MaterialData a, MaterialData b)
    {
        SortListParam sCount, sIndex;
        GetSortListParamFromMaterialData(a, b, out sCount, out sIndex);

        return Sort(sCount, sIndex);
    }

    public static void GetSortListParamFromMaterialFragmentData(MaterialFragmentData a, MaterialFragmentData b, out SortListParam sCount, out SortListParam sFragmentIndex)
    {
        sCount = new SortListParam { mParam1 = a.mCount, mParam2 = b.mCount, bIsDescending = true };
        sFragmentIndex = new SortListParam { mParam1 = a.mFragmentIndex, mParam2 = b.mFragmentIndex, bIsDescending = false };
    }

    public static int SortMaterialFragmentDataByNumber(MaterialFragmentData a, MaterialFragmentData b)
    {
        SortListParam sCount, sFragmentIndex;
        GetSortListParamFromMaterialFragmentData(a, b, out sCount, out sFragmentIndex);

        return Sort(sCount, sFragmentIndex);
    }

    public static CREATE_ROLE_TYPE GetCharacterTypeByModelIndex(int iModelIndex)
    {
        iModelIndex = ((iModelIndex / 1000) - (iModelIndex / 10000)* 10) % (int)CREATE_ROLE_TYPE.max;
        return (CREATE_ROLE_TYPE)iModelIndex;
    }

	public static void CleanALlChatLog()
	{
		DataCenter.SetData ("CHAT_WORLD_WINDOW", "CLEAN_ALL_CHAT", true);
		DataCenter.SetData ("CHAT_PRIVATE_WINDOW", "CLEAN_ALL_CHAT", true);
		DataCenter.SetData ("CHAT_UNION_WINDOW", "CLEAN_ALL_CHAT", true);
	}

	public static void SetPalyerIcon(UISprite icon, int roleIconIndex)
	{
		if(roleIconIndex < 1000) roleIconIndex = 1000;
		string spriteName = TableCommon.GetStringFromVipList (roleIconIndex, "VIPICON");
		if(roleIconIndex == 1000)
		{
			string [] names = spriteName.Split ('\\');
			if(RoleLogicData.Self.character.tid < 190999)
				spriteName = names[0];
			else 
				spriteName = names[1];
		}
		SetIcon (icon, TableCommon.GetStringFromVipList (roleIconIndex, "VIPALATS"), spriteName);
	}

	public static void PlayUIPlayTween(string text)
	{
		UIPlayTween uiPt = null;
		uiPt = GameObject.Find ("pet_group_sifting_box").GetComponent<UIPlayTween >();
		UILabel siftingButtonLabel = GameObject.Find ("pet_group_sifting_button").transform.Find ("label").GetComponent<UILabel >();
		if(text != null)
			siftingButtonLabel.text = text;
		uiPt.Play (true);
	}

	public static void CloseUIPlayTween(SORT_TYPE type)
	{
		if(GameObject.Find ("pet_group_sifting_box") != null && GameObject.Find ("pet_group_sifting_box").transform.localScale.y >= 0.5f)
		{
			UIPlayTween uiPt = GameObject.Find ("pet_group_sifting_box").GetComponent<UIPlayTween >();
			UILabel siftingButtonLabel = GameObject.Find ("pet_group_sifting_button").transform.Find ("label").GetComponent<UILabel >();
			string text = "星级";
			switch(type)
			{
			case SORT_TYPE.ELEMENT_INDEX:
				text = "属性";
				break;
			case SORT_TYPE.STAR_LEVEL:
				text = "星级";
				break;
			case SORT_TYPE.LEVEL:
				text = "等级";
				break;
			}
			siftingButtonLabel.text = text;
			uiPt.Play (true);
		}
	}

    public static int GetPetPassiveSkillIndex(PetData petData, int iIndex)
    {
        int iSkillIndex = 0;
        int iNum = 0;
        int iMaxNum = GetPassiveSkillMaxNum(petData);
        for (int i = 1; i <= 3; i++)
        {
            iSkillIndex = TableCommon.GetNumberFromActiveCongfig(petData.tid, "ATTACK_STATE_" + i.ToString());
            if (iSkillIndex <= 0)
                continue;

            iNum++;
            if (iIndex == iNum)
            {
                return iSkillIndex;
            }
        }

        for (int j = 1; j <= 3; j++)
        {
            iSkillIndex = TableCommon.GetNumberFromActiveCongfig(petData.tid, "AFFECT_BUFFER_" + j.ToString());
            if (iSkillIndex <= 0)
                continue;

            iNum++;

            if (iIndex == iNum)
            {
                return iSkillIndex;
            }
        }

        return 0;
    }

    public static int GetPassiveSkillMaxNum(PetData petData)
    {
        int iMaxNum = 0;
        if (petData != null)
        {
            if (petData.starLevel >= 3 && petData.starLevel < 5)
                iMaxNum = 1;
            else if (petData.starLevel == 5)
                iMaxNum = 2;
        }
        return iMaxNum;
    }

    public static int GetPassiveSkillNum(PetData petData)
    {
        if (petData == null)
            return 0;

        int iNum = 0;
        int iMaxNum = GameCommon.GetPassiveSkillMaxNum(petData);
        for (int i = 1; i <= 3 && iNum <= iMaxNum; i++)
        {
            int skillIndex = TableCommon.GetNumberFromActiveCongfig(petData.tid, "ATTACK_STATE_" + i.ToString());
            if (skillIndex <= 0)
                continue;

            iNum++;
        }

        for (int j = 1; j <= 3 && iNum <= iMaxNum; j++)
        {
            int skillIndex = TableCommon.GetNumberFromActiveCongfig(petData.tid, "AFFECT_BUFFER_" + j.ToString());
            if (skillIndex <= 0)
                continue;

            iNum++;
        }

        return iNum;
    }

    public static int GetNumberFromSkill(int iSkillIndex, string strIndexName)
    {
        string str = iSkillIndex.ToString().Substring(0, 1);
        if (str == "8")
        {
            return TableCommon.GetNumberFromSkillConfig(iSkillIndex, strIndexName);
        }
        else if (str == "9")
        {
            return TableCommon.GetNumberFromAffectBuffer(iSkillIndex, strIndexName);
        }

        return TableCommon.GetNumberFromAttackState(iSkillIndex, strIndexName);
    }

    public static string GetStringFromSkill(int iSkillIndex, string strIndexName)
    {
        string str = iSkillIndex.ToString().Substring(0, 1);
        if (str == "8")
        {
            return TableCommon.GetStringFromSkillConfig(iSkillIndex, strIndexName);
        }
        else if (str == "9")
        {
            return TableCommon.GetStringFromAffectBuffer(iSkillIndex, strIndexName);
        }

        return TableCommon.GetStringFromAttackState(iSkillIndex, strIndexName);
    }

    public static string GetSkillName(int iSkillIndex, string strIndexName)
    {
        string str = iSkillIndex.ToString().Substring(0, 1);
        if (str == "8")
        {
            return SkillGlobal.GetInfo(iSkillIndex).title;
        }
        else if (str == "9")
        {
            return BuffGlobal.GetInfo(iSkillIndex).title;
        }

        return TableCommon.GetStringFromAttackState(iSkillIndex, strIndexName);
    }

    public static string GetSkillDescription(int iSkillIndex, string strIndexName)
    {
        string str = iSkillIndex.ToString().Substring(0, 1);
        if (str == "8")
        {
            return SkillGlobal.GetInfo(iSkillIndex).describe;
        }
        else if (str == "9")
        {
            return BuffGlobal.GetInfo(iSkillIndex).describe;
        }

        return TableCommon.GetStringFromAttackState(iSkillIndex, strIndexName);
    }

    public static void SetPetSkillName(GameObject parentObj, int iSkillIndex, string objName)
    {
        // name
        string strName = GameCommon.GetSkillName(iSkillIndex, "NAME");
        string[] stringInfo = strName.Split(' ');
        stringInfo = stringInfo[0].Split('L');
        UILabel nameLabel = GameCommon.FindComponent<UILabel>(parentObj, "skill_name");
        if (nameLabel != null)
            nameLabel.text = stringInfo[0];
    }

    public static void SetPetSkillLevel(GameObject parentObj, int iSkillIndex, string objName, PetData petData)
    {
        if (petData == null) return;

        // level
        int iLevel = petData.GetSkillLevelByIndex(iSkillIndex);
        UILabel levelLabel = GameCommon.FindComponent<UILabel>(parentObj, "skill_level");
        if (levelLabel != null)
            levelLabel.text = "Lv." + iLevel.ToString();
    }

    public static int GetCurRoleEquipNum()
    {
        RoleEquipLogicData roleEquipLogicData = DataCenter.GetData("EQUIP_DATA") as RoleEquipLogicData;
        int iCount = roleEquipLogicData.mDicEquip.Count;
        int iCurUseID = RoleLogicData.GetMainRole().mIndex;

        foreach (KeyValuePair<int, Dictionary<int, EquipData>> pair in roleEquipLogicData.mDicRoleUseEquip)
        {
            if (iCurUseID == pair.Key)
                continue;
            iCount -= pair.Value.Count;
        }
        return iCount;
    }


	public static bool HaveEnoughCurrency(ITEM_TYPE type, int count)
	{
		switch(type)
		{
		case ITEM_TYPE.GOLD:
			if(RoleLogicData.Self.gold < count)
			{
				DataCenter.OpenMessageWindow (STRING_INDEX.ERROR_SHOP_NO_ENOUGH_GOLD);
				return false;
			}
			break;
		case ITEM_TYPE.YUANBAO:
			if(RoleLogicData.Self.diamond < count)
			{
				DataCenter.OpenMessageWindow (STRING_INDEX.ERROR_SHOP_NO_ENOUGH_DIAMOND);
				return false;
			}
			break;
		case ITEM_TYPE.SPIRIT:
			if(RoleLogicData.Self.spirit < count)
			{
				DataCenter.OpenMessageWindow (STRING_INDEX.ERROR_SHOP_NO_ENOUGH_FRIEND_POINT);
				return false;
			}
			break;
		case ITEM_TYPE.HONOR_POINT:
			if(RoleLogicData.Self.mHonorPoint < count)
			{
				DataCenter.OpenMessageWindow (STRING_INDEX.ERROR_SHOP_NO_ENOUGH_HONOR_POINT);
				return false;
			}
			break;
		}

		return true;
	}


	static public void SetIconData(GameObject icon, ItemData data)
	{
		//		UIButtonEvent evt = GameCommon.FindComponent<UIButtonEvent>("icon_tips_btn");
		//		NiceData buttonData = GameCommon.GetButtonData(icon , "icon_tips_btn");
		UIButtonEvent evt = GameCommon.FindObject (icon ,"icon_tips_btn").GetComponent<UIButtonEvent >();
		switch(data.mType)
		{
		case (int)ITEM_TYPE.GOLD:
			evt.mData.set ("INDEX", (int)ITEM_TYPE.GOLD);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_ITEMICON");
			break;
		case (int)ITEM_TYPE.POWER:
			evt.mData.set ("INDEX", (int)ITEM_TYPE.POWER);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_ITEMICON");
			break;
		case (int)ITEM_TYPE.YUANBAO:
			evt.mData.set ("INDEX", (int)ITEM_TYPE.YUANBAO);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_ITEMICON");
			break;
		case (int)ITEM_TYPE.SAODANG_POINT:
			evt.mData.set ("INDEX", (int)ITEM_TYPE.SAODANG_POINT);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_ITEMICON");
			break;
		case (int)ITEM_TYPE.SPIRIT:
			evt.mData.set ("INDEX", (int)ITEM_TYPE.SPIRIT);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_ITEMICON");
			break;
		case (int)ITEM_TYPE.EQUIP:
			evt.mData.set ("INDEX", data.mID);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_ROLE_EQUIP");
			break;
		case (int)ITEM_TYPE.PET:
			evt.mData.set ("INDEX", data.mID);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_PET");
			break;
		case (int)ITEM_TYPE.GEM:
			evt.mData.set ("INDEX", data.mID);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_STONE");
			break;
		case (int)ITEM_TYPE.MATERIAL:
			evt.mData.set ("INDEX", data.mID);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_ROLE_EQUIP_MATERIAL");
			break;
		case (int)ITEM_TYPE.MATERIAL_FRAGMENT:
			evt.mData.set ("INDEX", data.mID);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_ROLE_EQUIP_MATERIAL_FRAGMENT");
			break;
		case (int)ITEM_TYPE.PET_FRAGMENT:
			evt.mData.set ("INDEX", data.mID);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_PET_FRAGMENT");
			break;
		case (int)ITEM_TYPE.CONSUME_ITEM :
		case (int)ITEM_TYPE.LOCK_POINT :
		case (int)ITEM_TYPE.RESET_POINT :
			evt.mData.set ("INDEX", data.mID);
			evt.mData.set ("TABLE_NAME", "ITEM_TYPE_TOOLITEM");
			break;
		}
	}

    public static string GetNameByTid(int tid)
    {
        int iItemType = tid / 1000;
        switch ((ITEM_TYPE)iItemType)
        {
            case ITEM_TYPE.CHARACTER:
                return RoleLogicData.Self.name;
            case ITEM_TYPE.PET:
                return TableCommon.GetStringFromActiveCongfig(tid, "NAME");
            case ITEM_TYPE.EQUIP:
                break;
            case ITEM_TYPE.MAGIC:
                break;
        }
        return "";
    }

    public static void InitCard(GameObject cardObj, float fCardScale, GameObject parentPanelObj)
	{
		GameObject obj = GameCommon.LoadAndIntanciateUIPrefabs("card_group_window", cardObj.name);
		
		if(obj != null)
		{
			CardGroupUI uiScript = obj.GetComponent<CardGroupUI>();
            uiScript.InitPetInfo(cardObj.name, fCardScale, parentPanelObj);
		}
		
		cardObj.transform.localScale = cardObj.transform.localScale * fCardScale;
	}

    public static void InitCardWithoutBackground(GameObject cardObj, float fCardScale, GameObject parentPanelObj)
    {
        InitCard(cardObj, fCardScale, parentPanelObj);

        // hide
        HideUI(cardObj);
    }

    public static void HideUI(GameObject cardObj)
    {
        GameObject info = GameCommon.FindObject(cardObj, "info");
        GameObject infoCardBackground = GameCommon.FindObject(cardObj, "info_card_background");

        if (info != null)
            info.SetActive(false);

        if (infoCardBackground != null)
            infoCardBackground.SetActive(false);
    }

    public static ItemDataBase GetItemDataBase(ItemDataBase item)
    {
        if (item != null)
        {
            return GetItemDataBase(item.itemId, item.tid, item.itemNum);
        }

        return null;
    }

    public static ItemDataBase GetItemDataBase(int iItemId, int iTid, int iItemNum)
    {
        ItemDataBase item = new ItemDataBase();
        item.itemId = iItemId;
        item.tid = iTid;
        item.itemNum = iItemNum;

        return item;
    }

    public static int CheckShowUINum(int iNum)
    {
        if (iNum < 0)
        {
            iNum = 0;
        }
        return iNum;
    }

    public static string CheckShowUINum(int iCostNum, int iOwnNum)
    {
        iCostNum = CheckShowUINum(iCostNum);

        if (iCostNum > iOwnNum)
        {
            return "[ff0000]" + iCostNum;
        }
        return iCostNum.ToString();
    }
}

public static class CommonExtension
{
    //public static IEnumerable<T> GetItemList<T>(this int[] source,ITEM_TYPE itemType) where T:ItemDataBase
    //{
    //    T[] result
    //    for(int i=0;i<source.Length;i++)
    //    {

    //    }
    //}

}

//--------------------------------------------------------------------------------------------------
#endif