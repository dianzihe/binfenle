using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
public class DeviceDependentAssets : MonoBehaviour {
	public enum DeviceAssetsType
	{
		LowRes = 0,
		HighRes,
		
		Count,
		
		Unassigned,
	}
	
#if UNITY_EDITOR
	public DeviceAssetsType forceAssetsTypeInEditor = DeviceAssetsType.Unassigned;
#endif
	
	public DeviceAssetsType defaultAssetsType = DeviceAssetsType.HighRes;
		
	public bool autoLoadAssetsOnAwake = true;
	public bool autoInstantiateLoadedAssets = true;
	public bool autoDestroy = true;
	
	//public iPhoneGeneration[] lowResDevices;
	//public iPhoneGeneration[] highResDevices;

	public string[] lowResAssetsPaths;
	public string[] highResAssetsPaths;
	
	public DeviceAssetsType selectedAssetsType = DeviceAssetsType.Unassigned;
	
	private Object[] loadedAssets;

	protected string[][] mappedAssetsPaths;

	
	void Awake()
	{
		if (!enabled) {
			return;
		}
		
		// Map the assets paths arrays to make it easier to work with multiple different asset category types.
		mappedAssetsPaths = new string[(int)DeviceAssetsType.Count][];
		
		mappedAssetsPaths[(int)DeviceAssetsType.LowRes] = lowResAssetsPaths;
		mappedAssetsPaths[(int)DeviceAssetsType.HighRes] = highResAssetsPaths;
		
		if (autoLoadAssetsOnAwake)
		{
			LoadAllAssetsForCurrentDevice();
		}
		
		if (autoInstantiateLoadedAssets)
		{
			InstantiateAllLoadedAssets();
		}
	}
		
	void Start () 
	{
		if (autoDestroy) {
			Destroy(gameObject);
		}
	}
	
	public Object[] LoadedAssets {
		get {
			return loadedAssets;
		}
		set {
			loadedAssets = value;
		}
	}

	protected DeviceAssetsType GetAssetsTypeForIOS(/* iPhoneGeneration deviceGen*/)
	{
		/* 
		// Search through configured low res devices.
		for(int i = 0; i < lowResDevices.Length; i++)
		{
			if (lowResDevices[i] == deviceGen) {
				return DeviceAssetsType.LowRes;
			}
		}

		// Search through configured high res devices.
		for(int i = 0; i < highResDevices.Length; i++)
		{
			if (highResDevices[i] == deviceGen) {
				return DeviceAssetsType.HighRes;
			}
		}
		*/
		return defaultAssetsType;
	}
	
	public string[] GetAssetsPathsOfType(DeviceAssetsType assetsType)
	{
		if ( (int)assetsType < (int)DeviceAssetsType.Count ) {
			return mappedAssetsPaths[(int)assetsType];
		}
		
		return null;
	}
	
	public string[] GetAssetsPathsForCurrentDevice()
	{
#if UNITY_EDITOR
		if ((int)forceAssetsTypeInEditor < (int)DeviceAssetsType.Count)
		{
			selectedAssetsType = forceAssetsTypeInEditor;
			return GetAssetsPathsOfType(selectedAssetsType);
		}
#endif

#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) 
		{
			selectedAssetsType = GetAssetsTypeForIOS(iPhone.generation);
		}
		else
		{
			//TODO: find a way to configure Android setups for low/high res.
			// Here it's a little bit more complicated. We should use a custom struct to embedd required and optional settings like
			// CPU freq, total RAM, screen resolution, GPU Model, into account etc.
			// Use HighRes by default for now.
			selectedAssetsType = DeviceAssetsType.HighRes;
		}
#endif
		
		return GetAssetsPathsOfType(selectedAssetsType);
	}
	
	public void LoadAllAssetsForCurrentDevice()
	{
		string[] selectedAssetsPaths = GetAssetsPathsForCurrentDevice();
		
		if (selectedAssetsPaths != null) {
			loadedAssets = new Object[selectedAssetsPaths.Length];
			
			for(int i = 0; i < selectedAssetsPaths.Length; i++)
			{
				loadedAssets[i] = Resources.Load(selectedAssetsPaths[i]);
			}
		}
	}
	
	public void InstantiateAllLoadedAssets()
	{
		if (loadedAssets == null) {
			return;
		}
		
		for(int i = 0; i < loadedAssets.Length; i++)
		{
			Instantiate(loadedAssets[i]);
		}	
	}
		
}
#endif
