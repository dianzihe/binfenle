using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;

public static partial class SerializerUtils {
	
	/// <summary>
	/// Inspects all public primitive and serializable (non-reference) fields in the given object and writes them
	/// to the given BinaryWriter.
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	/// <param name='writer'>
	/// Writer.
	/// </param>
	public static void WriteToBinaryStream(Object targetObj, BinaryWriter writer) {
		System.Type objType = targetObj.GetType();
		
		FieldInfo[] publicFields = objType.GetFields(BindingFlags.Instance | BindingFlags.Public);		
		
		for(int i = 0; i < publicFields.Length; i++) {
			System.Type fieldType = publicFields[i].FieldType;

			if ((fieldType.IsValueType || fieldType == typeof(string)) && 
				!System.Attribute.IsDefined(publicFields[i], typeof(System.NonSerializedAttribute)) ) {

				Debug.Log(targetObj.name + " -> " + publicFields[i].Name + " = " + publicFields[i].GetValue(targetObj));
			}
		}
		
		Debug.Log("Public properties:");
		PropertyInfo[] publicProp = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
		
		for(int i = 0; i < publicProp.Length; i++) {
			System.Type propType = publicProp[i].PropertyType;
			
			if (propType.IsValueType || propType == typeof(string)) {
				
				Debug.Log(targetObj.name + " -> " + publicProp[i].Name + " = " + publicProp[i].GetValue(targetObj, null));
				Debug.Log(publicProp[i].Name + " = " + propType.Name);
			}
		}
	}
}
