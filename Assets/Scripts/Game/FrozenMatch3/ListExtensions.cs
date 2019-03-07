using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListExtensions
{
	public static void Shuffle<T>(this IList<T> list)  
	{  
	    int i = list.Count; 
		int j;
		
	    while (i > 1)
		{ 
	        i--;  
	        j = Random.Range(0, i);
			
	        T swap = list[i];  
	        list[i] = list[j];  
	        list[j] = swap;
	    }  
	}
}
