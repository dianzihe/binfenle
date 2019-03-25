using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Security.Cryptography;

public class TokensSystem : MonoBehaviour
{
	public static event System.Action<int> OnManaModified;
	protected static TokensSystem instance;
	
	public static TokensSystem Instance {
		get {
			if (instance == null) {
				instance = (new GameObject("TokensSystem")).AddComponent<TokensSystem>();
			}
			
			return instance;
		}
	}
	
	public int icePicks = 0;
	public int snowballs = 0;
	public int hourglasses = 0;
	public int itemTokens = 0;
	private int manaPoints = 0;
	
	protected string saveFile;

	//self

	
	void Awake()
	{
		instance = this;
		DontDestroyOnLoad(gameObject);
		
		saveFile = Application.persistentDataPath + "/Local.tmp";
		
		LoadItems();
	}

	public int ManaPoints 
	{
		get {
			return manaPoints;
		}
	}
	
	public void SaveItems()
	{
		MemoryStream ms = null;
		BinaryWriter writer = null;
//		CryptoStream encStream = null;
//		FileStream file = null;
		
		try 
		{
			ms = new MemoryStream();
			
//			DESCryptoServiceProvider mDES = new DESCryptoServiceProvider();
//			mDES.Mode = CipherMode.ECB;
//			mDES.Key = System.Text.Encoding.UTF8.GetBytes(TweaksSystem.CRYPTO_KEY);
//			
//			CryptoStream encStream = new CryptoStream(ms, mDES.CreateEncryptor(), CryptoStreamMode.Write);
			
//			BinaryWriter writer = new BinaryWriter(encStream);
			writer = new BinaryWriter(ms);
			
			if (writer != null) {
				writer.Write(UserCloud.USER_DATA_VERSION);
				writer.Write(manaPoints);
				
				writer.Close();
			}
			
//			encStream.Close();
			
			ms.Close();
			
			#if UNITY_WEBPLAYER
				Debug.LogWarning("Unimplemented method");
				throw new Exception();
			#else
				File.WriteAllBytes(saveFile, ms.ToArray());
			#endif
		}
		catch (Exception ex)
		{
			Debug.Log("Error in create or save user file data. Exception: " + ex.Message);
		}
		finally 
		{
			if (writer != null) {
				writer.Close();
			}
			if (ms != null) {
				ms.Close();
			}
		}
	}
	
	void LoadItems()
	{
		Stream readStream = null;
		BinaryReader reader = null;
		
		try 
		{
			if (File.Exists(saveFile)) {
				readStream = File.OpenRead(saveFile);
			}
			else {
				AssignDefaultValues();
				return;
			}
			
//			DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
//			cryptic.Mode = CipherMode.ECB;
//			cryptic.Key = System.Text.Encoding.UTF8.GetBytes(TweaksSystem.CRYPTO_KEY);
//			
//			CryptoStream crStream = new CryptoStream(readStream, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
//			
//			BinaryReader reader = new BinaryReader(crStream);
			reader = new BinaryReader(readStream);
			
			if (reader != null) 
			{
				reader.ReadSingle(); // test this in future updates
				manaPoints = reader.ReadInt32();
				
				reader.Close();
			}
					
//			crStream.Close();
			readStream.Close();
		} 
		catch (Exception ex)
		{
			Debug.Log("Error in loading user file data. Exception: " + ex.Message);
		}
		finally
		{
			if (reader != null) {
				reader.Close();
			}
			if (readStream != null) {
				readStream.Close();
			}
		}
	}
	
	void AssignDefaultValues ()
	{
		manaPoints = TweaksSystem.Instance.intValues["InitialMana"];
	}

	public void Reset ()
	{
		if (File.Exists(saveFile)) {
			File.Delete(saveFile);
			LoadItems();
		}
	}

	public void AddMana(int _units)
	{
		manaPoints += _units;
		SaveItems();
		if (OnManaModified != null) {
			OnManaModified(_units);
		}
	}

	public void SubstractMana(int _units)
	{
		manaPoints -= _units;
		manaPoints = Math.Max(0, manaPoints);
		SaveItems();
		if (OnManaModified != null) {
			OnManaModified(-_units);
		}
	}

}

