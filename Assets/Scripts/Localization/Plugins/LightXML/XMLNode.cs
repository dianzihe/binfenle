// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class XMLNode : Hashtable {
    public XMLNodeList GetNodeList(string path)
    {
		return (XMLNodeList)GetObject(path);
	}

    public XMLNode GetNode(string path)
    {
		return (XMLNode)GetObject(path);
	}

    public string GetValue(string path)
    {
		return GetObject(path) as string;
	}
	
	private object  GetObject ( string path  ){
		string[] bits=path.Split(">"[0]);
		XMLNode currentNode=this;
		XMLNodeList currentNodeList=null;
		bool  listMode=false;
		object ob;
		for(int i=0;i<bits.Length;i++){
			 if(listMode){
				ob=currentNode=(XMLNode)currentNodeList[int.Parse(bits[i])];
				listMode=false;
			 }else{
				ob=currentNode[bits[i]];
				if(ob is ArrayList){
					currentNodeList=(XMLNodeList)ob ;
					listMode=true;
				}else{
					// reached a leaf node/attribute
					if(i!=(bits.Length-1)){
						// unexpected leaf node
						string actualPath="";
						for(int j=0;j<=i;j++){
							actualPath=actualPath+">"+bits[j];
						}
						Debug.Log("xml path search truncated. Wanted: "+path+" got: "+actualPath);
					}
					return ob;
				}
			 }
		}
		if(listMode) return currentNodeList;
		else return currentNode;
	}
}