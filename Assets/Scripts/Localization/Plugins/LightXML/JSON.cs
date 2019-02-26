using UnityEngine;
using System.Collections;

public class JSON : MonoBehaviour {
/*
 * UnityScript JSON Parser
 * by Fraser McCormick (unityscripts@roguishness.com)
 * http://twitter.com/flimgoblin
 * http://www.roguishness.com/unity/
 *
 * You may use this script under the terms of either the MIT License 
 * or the Gnu Lesser General Public License (LGPL) Version 3. 
 * See:
 * http://www.roguishness.com/unity/lgpl-3.0f-standalone.html
 * http://www.roguishness.com/unity/gpl-3.0f-standalone.html
 * or
 * http://www.roguishness.com/unity/MIT-license.txt
 */

public static Hashtable ParseJSON (string json){
	Hashtable hash=new Hashtable();
	ArrayList parents=new ArrayList();
	ArrayList array = new ArrayList();
	char quote="\""[0];
		bool quoted=false;
		bool haveValue=false;
		string quotedString="";
		//bool isKey=true;
		string lastKey="";
		bool inArray=false;
		for(int i=1;i<json.Length-1;i++){
			char c=json[i];
			if(c==quote){
				if(quoted){
					// end of an element
					quoted=false;
					haveValue=true;
				}else{
					// start of an element
					quoted=true;
					quotedString="";
				}
			}else{
			
				if(quoted){
					quotedString+=c;
				}else{
					if(c=="{"[0]){
						if(inArray){
							// hash in an array
							hash=new Hashtable();
							array[array.Count]=hash;
							parents.Add(array);
							inArray=false;
						}else{
							// open a new hash
							hash[lastKey]=new Hashtable();
							parents.Add(hash);
							hash=(Hashtable)hash[lastKey];
							lastKey="";
						}
					}else if(c=="["[0]){
						// start of an array
					
						if(inArray){
							// array in an array
							array[array.Count]=new ArrayList();
							parents.Add(array);
							array=(ArrayList)array[array.Count-1];
						}else{
							// array in a hash
							array=new ArrayList();
							parents.Add(hash);
							hash[lastKey]=array;
							lastKey="";
						}
						inArray=true;				
					}else if(c=="]"[0] || c=="}"[0]){
						// end of array
						
						if(haveValue || quotedString!=null){
							if(inArray){
								array.Add(quotedString);
							}else{
								hash[lastKey]=quotedString;
								lastKey="";
							}
							haveValue=false;
							quotedString="";
						}

                        object par = parents[(int)(parents.Count - 1)];
                        parents.RemoveAt((parents.Count - 1)); 
                        if (par is Hashtable)
                        {
							hash=(Hashtable)par;
							inArray=false;
						}else{
							array=(ArrayList)par;
							inArray=true;
						}
						
					}else if(c==":"[0]){
						lastKey=quotedString;
						haveValue=false;
						quotedString="";
					}else if(c==","[0]){
						// end of value
						
						if(haveValue || quotedString!=null){
							if(inArray){
								array.Add(quotedString);
							}else{
								hash[lastKey]=quotedString;
								lastKey="";
							}
							haveValue=false;
							quotedString="";
						}
					}else{
						quotedString+=c;
					}
				}
			}
		}
		if(haveValue || quotedString!=null){
			if(inArray){
				array.Add(quotedString);
			}else{
				hash[lastKey]=quotedString;
				lastKey="";
			}
			haveValue=false;
		}
	
	return hash;
}
}