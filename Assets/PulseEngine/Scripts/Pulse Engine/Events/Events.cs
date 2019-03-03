using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//namespace PulseEngine {
	
/// <summary>
/// Base class for storing animation events. Will be extended by specific classes with various animation events.
/// </summary>
public class Events : MonoBehaviour
{
	private Transform cachedTransform;
	
	/// <summary>
	/// Gets the cached transform. Sets it if it is null.
	/// </summary>
	/// <value>
	/// The cached transform.
	/// </value>
	public Transform CachedTransform 
	{
		get {
			if (cachedTransform == null) {
				cachedTransform = transform;
			}
			
			return cachedTransform;
		}
	}
	
	/// <summary>
	/// Gets the target Component of type T and with the given name.
	/// </summary>
	/// <returns>
	/// The target Component.
	/// </returns>
	/// <param name='targetName'>
	/// Target name.
	/// </param>
	/// <typeparam name='T'>
	/// The type of the target component.
	/// </typeparam>
	public T GetTarget<T>(string targetName) where T : Component
	{
		T[] components = CachedTransform.root.GetComponentsInChildren<T>(true);
		
		foreach (T component in components) 
		{
			if (component.name == targetName) {
				return component;
			}
		}
		
		return null;
	}
	
	/// <summary>
	/// Gets the target Component of type T and values dictionary by parsing the given parameters string. 
	/// The target name is the first of the arguments.
	/// </summary>
	/// <returns>
	/// The target Component and arguments.
	/// </returns>
	/// <param name='paramsString'>
	/// Parameters string. The parameters are separated by ", " and each parameter is a "key=value" string.
	/// </param>
	/// <param name='values'>
	/// The resulting values dictionary.
	/// </param>
	/// <typeparam name='T'>
	/// The type of the target Component.
	/// </typeparam>
	public T GetTargetAndValues<T>(string paramsString, out Dictionary<string, string> values) where T : Component
	{
		string[] args = paramsString.Split(new string[]{", "}, System.StringSplitOptions.None);
		values = CreateDictionary(args);
		
		if (values.ContainsKey("target")) {
			return GetTarget<T>(values["target"]);
		}
		
		return null;
	}
	
	/// <summary>
	/// Creates the values dictionary using the provided arguments. Each argument is a "key=value" string.
	/// </summary>
	/// <returns>
	/// The dictionary.
	/// </returns>
	/// <param name='args'>
	/// Arguments. Each argument is a "key=value" string.
	/// </param>
	public Dictionary<string, string> CreateDictionary(string[] args)
	{
		Dictionary<string, string> values = new Dictionary<string, string>();
		
		foreach (string arg in args)
		{
			string[] pair = arg.Split(new char[]{'='}, System.StringSplitOptions.None);
			
			if (pair.Length != 2) {
				Debug.LogWarning("Bad formatting for pair: " + arg + ". Ignoring pair.");
			}
			
			values.Add(pair[0].Trim(), pair[1].Trim());
		}
		
		return values;
	}
}

//}
