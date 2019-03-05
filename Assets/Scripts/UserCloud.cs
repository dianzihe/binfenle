using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;


public class UserCloud
{
	public const float USER_DATA_VERSION = 1.4f;
	private static UserCloud currentUser;
	public static UserCloud CurrentCloudUser;

	#region Serialized Info
	
	private int _UserNumPurchases;
	/// <summary>
	/// Gets or sets the number of purchases the user has done during the lifetime of the app.
	/// </summary>
	/// <value>
	/// The user purchase count.
	/// </value>
	public int UserNumPurchases {
		get { 
			return _UserNumPurchases; 
		}
		set 
		{ 
			_UserNumPurchases = value; 
			UpdateAt = DateTime.UtcNow;
		}
	}
	
	private int _UserGoldCoins;
	public int UserGoldCoins {
		get { return _UserGoldCoins; }
		set { _UserGoldCoins = value; 
				UpdateAt = DateTime.UtcNow;
		}
	}
	
	private int _UserSilverCoins;
	public int UserSilverCoins {
		get { return _UserSilverCoins; }
		set { _UserSilverCoins = value; 
			UpdateAt = DateTime.UtcNow;		
		}
	}
	
	public int _NumsLiveLeft;
	public int NumsLiveLeft
	{
		get { return _NumsLiveLeft; }
		set { _NumsLiveLeft = value;
			 UpdateAt = DateTime.UtcNow;
		}
	}
	
	private Dictionary<string, object> _FinishedLevels;
	public Dictionary<string, object> FinishedLevels
	{
		get { return _FinishedLevels;}
		set { _FinishedLevels = value;
			UpdateAt = DateTime.UtcNow;
		}
	}
	
	private int _LastFinishedLvl;
	public int LastFinishedLvl
	{
		get { return _LastFinishedLvl; }
		set {
			_LastFinishedLvl = value;
			UpdateAt = DateTime.UtcNow;
		}
	}
	private DateTime _UpadateAt = DateTime.MinValue;
	public DateTime UpdateAt {
		get	{ return _UpadateAt; }
		set { _UpadateAt = DateTime.UtcNow; }
	}

	#endregion
	
	#region Extra Info
		
	public Texture2D Avatar;
	
	public static UserCloud CurrentUser
	{
		get
		{
			// Always return currentUser
			if (currentUser != null)
				return currentUser;
				
			// Firstime, load userdata from disk. If exists, user is not new.
			currentUser = Deserialize();
			
			// If not exist, create a new User.
			if (currentUser == null)
			{
				// New User -> Save it into userdata file
				currentUser = new UserCloud();
				Serialize(UserManagerCloud.FILE_NAME_LOCAL);			
			}
			return currentUser;
		}
		set {
			currentUser = value;
		}
	}
	
	#endregion
		
	#region Constructor
	
	public UserCloud() 
	{	
		ResetUserInfo(null);
	}
	
		
	#endregion
	
	#region Public Methods
	public bool IsPayingUser
	{
		get {
			return UserNumPurchases > 0;
		}
	}
	
	public void ResetUserInfo(string filename)
	{
		
		LastFinishedLvl = 0;
		UserNumPurchases = 0;
		UserGoldCoins = 0;
		UserSilverCoins = 0;
		NumsLiveLeft = 3;	
		FinishedLevels = new Dictionary<string, object>();
		Dictionary<string, object> levelFinishedInfo = new Dictionary<string, object>();
		levelFinishedInfo["stars"] = 0;
		levelFinishedInfo["score"] = 0;
		FinishedLevels["0"] = levelFinishedInfo;
		UpdateAt = DateTime.MinValue;	
		
		Avatar = null;
		
		if ( !string.IsNullOrEmpty(filename) ) {
			Delete(filename);
			Serialize(UserManagerCloud.FILE_NAME_LOCAL);
		}
	}
	
	#endregion 
	
	#region Serialization
	
	public static string GetPath(string objectId)
	{	
		string 	path = UserManagerCloud.DataPath + objectId;
		
		DirectoryInfo dir 	= new DirectoryInfo(UserManagerCloud.DataPath);
		
		if (!dir.Exists)
			dir.Create();
		
		return path;
	}
	
	public static bool Serialize(string filename)
	{
	#if UNITY_WEBPLAYER
		Debug.LogWarning("Unimplemented method");
		return false;
	#else
		bool detailedLog = false;
		
		MemoryStream ms = null;
//		CryptoStream encStream = null;
		BinaryWriter writeStream = null;
//		FileStream file = null;
		
		try
		{
//			float timeStart = Time.realtimeSinceStartup;
					
			ms = new MemoryStream();
//			DESCryptoServiceProvider mDES = new DESCryptoServiceProvider();
//			mDES.Mode = CipherMode.ECB;
//			mDES.Key = System.Text.Encoding.UTF8.GetBytes(TweaksSystem.CRYPTO_KEY);
//			
//			encStream = new CryptoStream(ms, mDES.CreateEncryptor(), CryptoStreamMode.Write);
//			
//			using (writeStream = new BinaryWriter(encStream))
			using (writeStream = new BinaryWriter(ms))
			{
				if (detailedLog) 
				{
					Debug.Log("Serializing");
					Debug.Log("USER DATA VERSION " + USER_DATA_VERSION);
				}
				writeStream.Write(USER_DATA_VERSION);
				
				if (detailedLog) {
					Debug.Log("Gold coins " + CurrentUser.UserGoldCoins);
				}
				writeStream.Write(CurrentUser.UserGoldCoins);
				
//				if (detailedLog) {
//					Debug.Log("Gold coins " + CurrentUser.UserNumPurchases);
//				}
//				writeStream.Write(CurrentUser.UserNumPurchases);
				
				if (detailedLog) {
					Debug.Log("Silver coins " + CurrentUser.UserSilverCoins);
				}
				writeStream.Write(CurrentUser.UserSilverCoins);
				
				if (detailedLog) {
					Debug.Log("Lives left " + CurrentUser.NumsLiveLeft);
				}
				writeStream.Write(CurrentUser.NumsLiveLeft);
				
				if (detailedLog) {
					Debug.Log("Last finished level " + CurrentUser.LastFinishedLvl);
				}
				writeStream.Write(CurrentUser.LastFinishedLvl);
				
				if (detailedLog) {
					Debug.Log("Finished levels count " + CurrentUser.FinishedLevels.Count);
				}
				writeStream.Write(CurrentUser.FinishedLevels.Count);
				
				foreach (var level in CurrentUser.FinishedLevels)
				{
					if (detailedLog) {
						Debug.Log("Level key: " + level.Key);
					}
					writeStream.Write(level.Key);
					
					if (detailedLog) {
						Debug.Log("Level dict count: " + ((Dictionary<string, object>)level.Value).Count);
					}
					writeStream.Write(((Dictionary<string, object>)level.Value).Count);
					
					foreach (var levelInfo in ((Dictionary<string, object>)level.Value))
					{
						if (detailedLog) {
							Debug.Log("Level info key: " + levelInfo.Key);
						}
						writeStream.Write(levelInfo.Key);
					
						if (detailedLog) {
							Debug.Log("Level info value: " + Convert.ToInt32(levelInfo.Value));
						}
						writeStream.Write(Convert.ToInt32(levelInfo.Value));
					}
				}
				
				if (detailedLog) {
					Debug.Log("Ticks: " + CurrentUser.UpdateAt.Ticks);
				}
				writeStream.Write(CurrentUser.UpdateAt.Ticks);
			}
//			encStream.Close();
			writeStream.Close();
			
			ms.Close();
			
			System.IO.File.WriteAllBytes(GetPath(filename), ms.ToArray());
		
			if (detailedLog) {
				Debug.Log("[UserManagerCloud] Saved user data file locally: " + (new FileInfo(GetPath(filename))).Length);
			}
			
			return true;

		}
		catch (Exception ex)
		{
			Debug.LogWarning("Error in create or save user file data. Exception: " + ex.Message);
			Debug.LogWarning(ex.StackTrace);
			
			return false;
		}
		finally 
		{
			if (detailedLog) {
				Debug.Log("[Serialize] Closing possible open streams...");
			}
			
			if (writeStream != null)
			{
				if (detailedLog) {
					Debug.Log("Closing write stream...");
				}
				writeStream.Close();
			}
			
//			if (encStream != null)
//			{
//				Debug.Log("Closing encrypt stream...");
//				encStream.Close();
//			}
			
			if (ms != null)
			{
				if (detailedLog) {
					Debug.Log("Closing memory stream...");
				}
				ms.Close();
			}
			
		}
	#endif
	}
	
	public static UserCloud Deserialize(byte[] data = null)
	{
		bool detailedLog = false;
		
		Stream str = null;
//		CryptoStream crStream = null;
		BinaryReader readStream = null;
		
		try
		{
			UserCloud nUser = new UserCloud();
			
//			float timeStart = Time.realtimeSinceStartup;
					
			if (data == null)
			{
				string path = GetPath(UserManagerCloud.FILE_NAME_LOCAL);
				
				if (!File.Exists(path))
					return null;
				str = File.OpenRead(path);
			}
			else
			{
				str = new MemoryStream(data);
			}


//			DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
//			cryptic.Mode = CipherMode.ECB;
//			cryptic.Key = System.Text.Encoding.UTF8.GetBytes(TweaksSystem.CRYPTO_KEY);
//			
//			crStream = new CryptoStream(str, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
//					
//			using (readStream = new BinaryReader(crStream))
			using (readStream = new BinaryReader(str))
			{
				float versionFile = readStream.ReadSingle();
				if (detailedLog) {
					Debug.Log("File version " + versionFile);
				}
				
				if (versionFile < USER_DATA_VERSION)
				{
					Debug.LogWarning("[Deserialize] Trying to load older user data file!");
					
					if (data != null)
					{
						return nUser;
					}
					return null;
				}
										
				nUser.UserGoldCoins = readStream.ReadInt32();
				if (detailedLog) {
					Debug.Log("Gold coins " + nUser.UserGoldCoins);
				}
				
//				nUser.UserNumPurchases = readStream.ReadInt32();
//				if (detailedLog) {
//					Debug.Log("Num purchases " + nUser.UserNumPurchases);
//				}
				
				nUser.UserSilverCoins = readStream.ReadInt32();
				if (detailedLog) {
					Debug.Log("Silver coins " + nUser.UserSilverCoins);
				}
				
				nUser.NumsLiveLeft = readStream.ReadInt32();
				if (detailedLog) {
					Debug.Log("Lives left " + nUser.NumsLiveLeft);
				}
				
				nUser.LastFinishedLvl = readStream.ReadInt32();


				//nUser.LastFinishedLvl=LoadLevelButton.maxLevels;


				if (detailedLog) {
					Debug.Log("Last finished level " + nUser.LastFinishedLvl);
				}
					
				Dictionary<string, object> levels = new Dictionary<string, object>();
				int countLevels = readStream.ReadInt32();
				if (detailedLog) {
					Debug.Log("Finished levels count " + countLevels);
				}
				
				for (int i = 0; i < countLevels; i++)
				{
					string keyLevel = readStream.ReadString();
					if (detailedLog) {
						Debug.Log("Level key: " + keyLevel);
					}
					
					Dictionary<string, object> levelInfo = new Dictionary<string, object>();
					int countLevelInfo = readStream.ReadInt32();
					if (detailedLog) {
						Debug.Log("Level dict count: " + countLevelInfo);
					}
					
					for (int j = 0; j < countLevelInfo; j++)
					{
						string keyLevelInfo = readStream.ReadString();
						if (detailedLog) {
							Debug.Log("Level info key: " + keyLevelInfo);
						}
						
						int valueLevelInfo = readStream.ReadInt32();
						if (detailedLog) {
							Debug.Log("Level info value: " + valueLevelInfo);
						}
						
						levelInfo.Add(keyLevelInfo, valueLevelInfo);
					}
					
					levels.Add(keyLevel, levelInfo);
				}
				
				nUser.FinishedLevels = levels;
				nUser.UpdateAt = new DateTime(readStream.ReadInt64());
				if (detailedLog) {
					Debug.Log("Ticks: " + nUser.UpdateAt.Ticks);
				}
				
				readStream.Close();
			}
			
			str.Close();
//			float durationTime = Time.realtimeSinceStartup - timeStart;
//			Debug.Log(" =>Ecrypt Deserialization Time: " + durationTime * 1000);
			
			return nUser;
			
		}
		catch (Exception e)
		{
			Debug.LogWarning(e.Message + "\nUser Data saved in disk was corrupted. One or more fields were added.");
			Debug.LogWarning(e.StackTrace);
				
			return null;
		}
		finally 
		{
			if (detailedLog) {
				Debug.Log("[Deserialize] Closing possible open streams...");
			}
			
			if (readStream != null)
			{
				if (detailedLog) {
					Debug.Log("Closing readStream stream...");
				}
				readStream.Close();
			}
			
//			if (crStream != null)
//			{
//				Debug.Log("Closing crStream stream...");
//				crStream.Close();
//			}
			
			if (str != null)
			{
				if (detailedLog) {
					Debug.Log("Closing str stream...");
				}
				str.Close();
			}
			
		}
	}
	
	private void Delete(string filename)
	{
		string path = GetPath(filename);
		if (File.Exists(path))
			File.Delete(path);
	}
	
	
	#endregion
		
}

