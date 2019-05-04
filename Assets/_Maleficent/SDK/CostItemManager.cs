using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System;
public class CostItemManager : MonoBehaviour 
{
	static private CostItemManager m_Instance=null;
	protected string saveFile;

	static public CostItemManager mInstance
	{
		get
		{
			return m_Instance;
		}
	}

	void Awake()
	{
		m_Instance = this;
		saveFile = Application.persistentDataPath + "/NewSystemValue_1.tmp";

		LoadItems ();
		CreateOrSaveFile ();

	}


	static private Dictionary<string,int> m_countDict=new Dictionary<string, int>()
	{
		{"Crow_Prop_Count",0},
		{"Crow2nd_Prop_Count",0},
		{"TheStaffCost_Prop_Count",0},
		{"GreenMagicCost_Prop_Count",0},
		{"WingWindCost_Prop_Count",0},
		{"YellowPixieDustCost_Prop_Count",0},
		{"WolfHowlCost_Prop_Count",0},
		{"ThorwnCost_Prop_Count",0}
	};

	static public Dictionary<string,int> getDict()
	{
		return m_countDict;
	}

	void AssignDefaultValues ()
	{
		m_countDict["Crow_Prop_Count"] = TweaksSystem.Instance.intValues["Crow_Prop_Count"];
		m_countDict ["Crow2nd_Prop_Count"] = TweaksSystem.Instance.intValues ["Crow2nd_Prop_Count"];

		m_countDict["TheStaffCost_Prop_Count"] = TweaksSystem.Instance.intValues["TheStaffCost_Prop_Count"];
		m_countDict ["GreenMagicCost_Prop_Count"] = TweaksSystem.Instance.intValues ["GreenMagicCost_Prop_Count"];

		m_countDict["WingWindCost_Prop_Count"] = TweaksSystem.Instance.intValues["WingWindCost_Prop_Count"];
		m_countDict ["YellowPixieDustCost_Prop_Count"] = TweaksSystem.Instance.intValues ["YellowPixieDustCost_Prop_Count"];

		m_countDict["WolfHowlCost_Prop_Count"] = TweaksSystem.Instance.intValues["WolfHowlCost_Prop_Count"];
		m_countDict ["ThorwnCost_Prop_Count"] = TweaksSystem.Instance.intValues ["ThorwnCost_Prop_Count"];
	}



	void LoadItems()
	{
		Stream readStream = null;
		BinaryReader reader = null;
		try 
		{
			if (PlayerPrefs.GetInt("Property_Data",-1)==1) 
			{
				//readStream = File.OpenRead(saveFile);
			}
			else 
			{
				PlayerPrefs.SetInt("Property_Data",1);
				PlayerPrefs.Save();
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
			//reader = new BinaryReader(readStream);
			ReadCostCount(reader);
//			if (reader != null) 
//			{
//
//				reader.Close();
//			}
//			readStream.Close();
		} 
		catch (Exception ex)
		{
			Debug.Log("Error in loading user file data. Exception: " + ex.Message);
		}
		finally
		{
//			if (reader != null) {
//				reader.Close();
//			}
//			if (readStream != null) {
//				readStream.Close();
//	//			readStream.Flush();
//			}
		}
	}

	void WriteCostCount(BinaryWriter wr)
	{
//		wr.Write (m_countDict["Crow_Prop_Count"]);
//		wr.Write (m_countDict ["Crow2nd_Prop_Count"]);
//		wr.Write (m_countDict["TheStaffCost_Prop_Count"]);
//		wr.Write (m_countDict ["GreenMagicCost_Prop_Count"]);
//		wr.Write (m_countDict["WingWindCost_Prop_Count"] );
//		wr.Write (m_countDict ["YellowPixieDustCost_Prop_Count"]);
//		wr.Write (m_countDict["WolfHowlCost_Prop_Count"]);
//		wr.Write (m_countDict ["ThorwnCost_Prop_Count"]);



		PlayerPrefs.SetInt("Crow_Prop_Count",m_countDict ["Crow_Prop_Count"]);
		PlayerPrefs.SetInt("Crow2nd_Prop_Count",m_countDict ["Crow2nd_Prop_Count"]);
		
		PlayerPrefs.SetInt("TheStaffCost_Prop_Count",m_countDict ["TheStaffCost_Prop_Count"]);
		PlayerPrefs.SetInt("GreenMagicCost_Prop_Count",m_countDict ["GreenMagicCost_Prop_Count"]);
		
		PlayerPrefs.SetInt("WingWindCost_Prop_Count",m_countDict ["WingWindCost_Prop_Count"]);
		PlayerPrefs.SetInt("YellowPixieDustCost_Prop_Count",m_countDict ["YellowPixieDustCost_Prop_Count"]);
		
		PlayerPrefs.SetInt("WolfHowlCost_Prop_Count",m_countDict ["WolfHowlCost_Prop_Count"]);
		PlayerPrefs.SetInt("ThorwnCost_Prop_Count",m_countDict ["ThorwnCost_Prop_Count"]);

		PlayerPrefs.Save ();
	}

	void ReadCostCount(BinaryReader br)
	{
//
//		m_countDict["Crow_Prop_Count"] = br.ReadInt32();
//		m_countDict ["Crow2nd_Prop_Count"] = br.ReadInt32();
//		
//		m_countDict["TheStaffCost_Prop_Count"] =br.ReadInt32();
//		m_countDict ["GreenMagicCost_Prop_Count"] = br.ReadInt32();
//		
//		m_countDict["WingWindCost_Prop_Count"] =br.ReadInt32();
//		m_countDict ["YellowPixieDustCost_Prop_Count"] = br.ReadInt32();
//		
//		m_countDict["WolfHowlCost_Prop_Count"] =br.ReadInt32();
//		m_countDict ["ThorwnCost_Prop_Count"] = br.ReadInt32();


		m_countDict ["Crow_Prop_Count"] = PlayerPrefs.GetInt ("Crow_Prop_Count");
		m_countDict ["Crow2nd_Prop_Count"] = PlayerPrefs.GetInt ("Crow2nd_Prop_Count");
		
		m_countDict["TheStaffCost_Prop_Count"] =PlayerPrefs.GetInt ("TheStaffCost_Prop_Count");
		m_countDict ["GreenMagicCost_Prop_Count"] = PlayerPrefs.GetInt ("GreenMagicCost_Prop_Count");
		
		m_countDict["WingWindCost_Prop_Count"] =PlayerPrefs.GetInt ("WingWindCost_Prop_Count");
		m_countDict ["YellowPixieDustCost_Prop_Count"] = PlayerPrefs.GetInt ("YellowPixieDustCost_Prop_Count");
		
		m_countDict["WolfHowlCost_Prop_Count"] =PlayerPrefs.GetInt ("WolfHowlCost_Prop_Count");
		m_countDict ["ThorwnCost_Prop_Count"] = PlayerPrefs.GetInt ("ThorwnCost_Prop_Count");




	}

	void CreateOrSaveFile()
	{
		Stream saveStream = null;
		BinaryWriter writer = null;
		try 
		{
//			if (PlayerPrefs.GetInt("Property_Data",-1)==1) 
//			{
//				File.Create(saveFile);
//			}
			//saveStream=File.OpenWrite(saveFile);
			
			//			DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
			//			cryptic.Mode = CipherMode.ECB;
			//			cryptic.Key = System.Text.Encoding.UTF8.GetBytes(TweaksSystem.CRYPTO_KEY);
			//			
			//			CryptoStream crStream = new CryptoStream(readStream, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
			//			
			//			BinaryReader reader = new BinaryReader(crStream);
			//writer = new BinaryWriter(saveStream);
			WriteCostCount(writer);
//			if (writer != null) 
//			{
//			
//				writer.Close();
//			}
//			saveStream.Close();
		} 
		catch (Exception ex)
		{
			Debug.Log("Error in loading user file data. Exception: " + ex.Message);
		}
		finally
		{
//			if (writer!= null)
//			{
//				writer.Close();
//			}
//			if (saveStream != null) 
//			{
//				saveStream.Close();
//	//			saveStream.Flush();
//			}
		}
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void AddCount(int _units,string m_strWhich)
	{
		if(m_countDict.ContainsKey(m_strWhich))
		{
			m_countDict[m_strWhich]+=_units;
		}
		CreateOrSaveFile ();
	}
	
	public void SubstractCount(int _units,string m_strWhich)
	{
		if(m_countDict.ContainsKey(m_strWhich))
		{
			m_countDict[m_strWhich]-=_units;
			if(m_countDict[m_strWhich]<=0)
			{
				m_countDict[m_strWhich]=0;
			}
		}
		CreateOrSaveFile ();

	}
}
