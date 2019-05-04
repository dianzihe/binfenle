using UnityEngine;
using System.Collections;
using System;
public class RandomNumberGenerator
{

	// Use this for initialization

	private static RandomNumberGenerator m_Instance = null;
	static public RandomNumberGenerator GetInstance()
	{
		if(m_Instance==null)
		{
			m_Instance=new RandomNumberGenerator();
		}
		return m_Instance;
	}
	public string GetRandomKey 
	{
		get 
		{
			string str = "acbdefghijklmnopqrstuvwxyzACBDEFGHIJKLMNOPQRSTUVWXYZ0123456789,.[]";
			char[] ch = new char[12];
			System.Random r = new System.Random((int)System.DateTime.Now.Ticks);
			for (int i = 0; i < ch.Length; i++) 
			{
				ch[i] = str[r.Next(str.Length)];
			}
			return new string(ch);
		}
	}



	private RandomNumberGenerator()
	{

	}
 
}
