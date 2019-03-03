/// <summary>
/// Name this class DebugOn when you want to enable logs in Unity.
/// Name it Debug when you want to disable all Debug.Log calls.
/// </summary>
public class Debug {
	// Conditional compiling attribute to remove this method call while there is no define for "DEBUG_ON"
//	[System.Diagnostics.Conditional("DEBUG_ON")]

	public static bool isDebugBuild{
		get {
			return UnityEngine.Debug.isDebugBuild;
		}
	}

	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void Log(object msg) { 
		UnityEngine.Debug.Log(msg);
	}
	
	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void LogError(object msg) { 
		UnityEngine.Debug.LogError(msg);
	}
	
	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void LogError(object msg, object sender) { 
		UnityEngine.Debug.LogError(msg);
	}
	
	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void LogWarning(object msg) { 
		UnityEngine.Debug.LogWarning(msg);
	}
	
	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void LogWarning(object msg, object sender) { 
		UnityEngine.Debug.LogWarning(msg);
	}
	
	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void DebugBreak() { 
		UnityEngine.Debug.DebugBreak();
	}
	
	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void Break() { 
		UnityEngine.Debug.Break();
	}	
	
	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void DrawLine(UnityEngine.Vector3 a, UnityEngine.Vector3 b) {
		UnityEngine.Debug.DrawLine(a, b);
	}
	
	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void DrawRay(UnityEngine.Vector3 origin, UnityEngine.Vector3 direction) {
		UnityEngine.Debug.DrawRay(origin, direction);
	}
	
	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void DrawLine(UnityEngine.Vector3 a, UnityEngine.Vector3 b, UnityEngine.Color color, float duration = 0f, bool depthTest = true) {
		UnityEngine.Debug.DrawLine(a, b, color, duration, depthTest);
	}
	
	[System.Diagnostics.Conditional("DEBUG_ON")]
	public static void DrawRay(UnityEngine.Vector3 origin, UnityEngine.Vector3 direction, UnityEngine.Color color, float duration = 0f, bool depthTest = true) {
		UnityEngine.Debug.DrawRay(origin, direction, color, duration, depthTest);
	}
	
//	[System.Diagnostics.Conditional("DEBUG_ON")]
//	public static void DrawRay(UnityEngine.Vector3 origin, UnityEngine.Vector3 direction, UnityEngine.Color color, float duration, bool depthTest) {
//		UnityEngine.Debug.DrawRay(origin, direction, color, duration, depthTest);
//	}
}