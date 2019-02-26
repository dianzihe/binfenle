#define disableGoogleCloud

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;


public enum ResultSave 
{
	NotSaved,
	Disk,
	Cloud
}

public class UserManagerCloud: MonoBehaviour 
{
	public static string FILE_NAME_LOCAL = "user.dat";
	public static string FILE_NAME_CLOUD = "savegame";
	
	protected static UserManagerCloud instance;
	public static string DataPath = "";
//	public static bool IsUserInit = false;
	protected static bool DisableICloud = false;
	
	private string googleCloudData;
	private bool googleCloudTryingToLogin;
	// Indicates if the google cloud failed to login the last time it tried.
	private bool googleCloudLoginFailed = false;
			
	// Indicates if there was internet connectivity the last time CanUseGoogleCloud() method was called.
	private bool wasInternetReachable = false;
	
	
	public static UserManagerCloud Instance {
		get {
			if (instance == null && !dontReload) {				
				GameObject container = GameObject.Instantiate(Resources.Load("UserManagerCloud") as GameObject) as GameObject;
				container.name ="UserManagerCloud";
				container.AddComponent<Animation>();
				
				//instance = container.AddComponent<UserManager>();
				DontDestroyOnLoad(container);
			}
			
			return instance;
		}
	}
	
	protected static bool dontReload = false;

	public UserCloud CurrentUser { 
		get {return UserCloud.CurrentUser; }
		private set { UserCloud.CurrentUser = value; }
	}
	public UserCloud CurrentCloudUser { 
		get {return UserCloud.CurrentCloudUser; }
		private set { UserCloud.CurrentCloudUser = value; }
	}
	/// <summary>
	/// Gets the stars for level.
	/// </summary>
	/// <returns>
	/// The stars for level for CurrentUser. -1 if Player doesn't that level
	/// </returns>
	/// <param name='level'>
	/// Level.
	/// </param>
	public int GetStarsForLevel(int level)
	{
		return this.GetStarsForLevel(CurrentUser, level);
	}
	
	/// <summary>
	/// Gets the stars for level.
	/// </summary>
	/// <returns>
	/// The stars for level. -1 if Player doesn't that level
	/// </returns>
	/// <param name='user'>
	/// User to set the stars.
	/// </param>
	/// <param name='level'>
	/// Level.
	/// </param>
	public int GetStarsForLevel(UserCloud user, int level)
	{
		//Debug.Log("Geting stars" + user.FinishedLevels);
		if (!user.FinishedLevels.ContainsKey(level.ToString()))
			return -1;
		Dictionary<string, object> infoLevel = (Dictionary<string, object>)user.FinishedLevels[level.ToString()];
		
		return Convert.ToInt32(infoLevel["stars"]);
	}
		/// <summary>
	/// Gets the score for level.
	/// </summary>
	/// <returns>
	/// The score for level for CurrentUser. -1 if Player doesn't that level
	/// </returns>
	/// <param name='level'>
	/// Level.
	/// </param>
	public int GetScoreForLevel(int level)
	{
		return this.GetScoreForLevel(CurrentUser, level);
	}
	
	/// <summary>
	/// Gets the score for level.
	/// </summary>
	/// <returns>
	/// The score for level. -1 if Player doesn't that level
	/// </returns>
	/// <param name='user'>
	/// User to set the score
	/// </param>
	/// <param name='level'>
	/// Level.
	/// </param>
	public int GetScoreForLevel(UserCloud user, int level)
	{
		//Debug.Log("Geting score" + user.FinishedLevels);
		if (!user.FinishedLevels.ContainsKey(level.ToString()))
			return 0;
		Dictionary<string, object> infoLevel = (Dictionary<string, object>)user.FinishedLevels[level.ToString()];
		//Debug.Log("GetScore: " + Convert.ToInt32(infoLevel["score"]));
		return Convert.ToInt32(infoLevel["score"]);
	}

		/* 
	#region Events
	
	public delegate void UserHasBeenDownloadedFromCloudDelegate(object sender, UserCloudDownloadDelegateEventArgs e);
	public event UserHasBeenDownloadedFromCloudDelegate UserHasBeenDownloadedFromCloud;
	private void RaiseUserHasBeenDownloaded(UserCloudDownloadDelegateEventArgs e, System.Action<UserCloudDownloadDelegateEventArgs> onResult)
	{
		if (onResult != null)
			onResult(e);
		
		if (UserHasBeenDownloadedFromCloud != null)
			UserHasBeenDownloadedFromCloud(this, e);
		
	}
	
	public delegate void UserHasBeenUploadedToCloudDelegate(object sender, UserCloudUploadedDelegateEventArgs e);
	public event UserHasBeenUploadedToCloudDelegate UserHasBeenUploadedToCloud;
	private void RaiseUserHasBeenUploaded(UserCloudUploadedDelegateEventArgs e, System.Action<UserCloudUploadedDelegateEventArgs> onResult)
	{
		if (onResult != null)
			onResult(e);
		
		if (UserHasBeenUploadedToCloud != null)
			UserHasBeenUploadedToCloud(this, e);
		
	}
	
	/// <summary>
	/// User has reached new record.
	/// </summary>
	public delegate void UserHasReachedNewRecordDelegate(object sender, UserHasReachedNewRecordDelegateEventArgs e);
	
	
	/// <summary>
	/// User has reached new record.
	/// </summary>
	private UserHasReachedNewRecordDelegate _UserHasReachedNewRecord;
	public event UserHasReachedNewRecordDelegate UserHasReachedNewRecord
	{
		add { _UserHasReachedNewRecord += value; }
		remove {_UserHasReachedNewRecord -= value;}
	}
	private void RaiseUserHasReachedNewRecord(UserHasReachedNewRecordDelegateEventArgs e, System.Action<UserHasReachedNewRecordDelegateEventArgs> onResultNewRecord)
	{
		if (_UserHasReachedNewRecord != null)
			_UserHasReachedNewRecord(this, e);
		
		if (onResultNewRecord != null)
			onResultNewRecord(e);
	}

	
	#endregion
	
	#region Monobehaviour Methods
	
	void Awake ()
	{
		// Disable the google cloud logic for android
		#if UNITY_ANDROID && disableGoogleCloud
		DisableICloud = true;
		#endif

		instance = this;
		DontDestroyOnLoad(gameObject);
		
		#if UNITY_ANDROID && !disableGoogleCloud
		
		//Subscribe to events
		GPGManager.loadCloudDataForKeyFailedEvent 		+= loadCloudDataForKeyFailedEvent;
		GPGManager.loadCloudDataForKeySucceededEvent 	+= loadCloudDataForKeySucceededEvent;
		GPGManager.authenticationFailedEvent 			+= loginCloudAutenticationFailedEvent;
		GPGManager.authenticationSucceededEvent 		+= loginCloudAutenticationSucceededEvent;
		GPGManager.userSignedOutEvent					+= OnGoogleCloudSignedOut;
		
		//Authenticate player
		if ( CanUseGoogleCloud() )
		{
			StartGoogleCloudLogin();
		}
		
		#endif
		
	#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		DataPath = Application.persistentDataPath + "/";
	#elif UNITY_EDITOR
		DataPath = "StoredData/";
	#endif
		CurrentUser.GetType();
	}
	
	#endregion
	
	#region Utility methods
	
	#if UNITY_ANDROID && !disableGoogleCloud
	public bool CanUseGoogleCloud()
	{
		bool isInternetReachable = Application.internetReachability != NetworkReachability.NotReachable;
		// Reset the google cloud login failed only if internet connection is back or we're in the Home scene
		if ( !wasInternetReachable && isInternetReachable )
		{
			// Reset the login failed flag because the internet availability status changed to true
			googleCloudLoginFailed = false;
			
			// Re-try google authenticate so it can sync user progress with the google cloud after the internet is back on during the game.
			StartGoogleCloudLogin();
			
//			NativeMessagesSystem.Instance.ShowMessage("Debug", "googleCloudLoginFailed reset to false! we have internet!", "Ok");
		}
		wasInternetReachable = isInternetReachable;
		
		if (Application.loadedLevelName != "Home" && googleCloudLoginFailed)
		{
//			Debug.Log("CanUseGoogleCloud() failed: loadedLevel = " + Application.loadedLevelName + " / googleCloudLoginFailed = " + googleCloudLoginFailed);
			
			return false;
		}
		
		return true;
	}
	#endif
	
	#endregion
	
	#region Test Methods
	
	
	public void ModifyUser(System.Action<UserHasReachedNewRecordDelegateEventArgs> onResult = null)
	{
		Debug.Log("Modifying user with level: " + CurrentUser.LastFinishedLvl);
		int currentScore = this.GetScoreForLevel(4) + 1;
		int level = CurrentUser.LastFinishedLvl + 1;
		this.SetScoreForLevel(level, currentScore, 3, onResult, true);
	}
	
	
	#endregion
	
	#region Public Methods
	
	/// <summary>
	/// Init the user from iCloud. 
	/// </summary>
	/// <param name='onResult'>
	/// On result.
	/// </param>
	public void InitUser(System.Action<UserCloudDownloadDelegateEventArgs> onResult = null)
	{
		this.LoadUserFromCloud(onResult);
	}

	public void DeleteUserFromCloud()
	{
		Debug.Log("[UserManagerCloud] DeleteUserFromCloud...");
#if UNITY_IPHONE || UNITY_EDITOR
		StartCoroutine(DeleteFromCloud());
#elif UNITY_ADNROID
		if (PlayGameServices.isSignedIn())
			PlayGameServices.deleteCloudDataForKey(0, false);
#endif
	}
	
#if UNITY_ANDROID && !disableGoogleCloud
	
	public void StartGoogleCloudLogin()
	{
		Debug.Log("StartGoogleCloudLogin() called");
		
		if ( !PlayGameServices.isSignedIn() && !googleCloudTryingToLogin ) 
		{
			Debug.Log("StartGoogleCloudLogin() authenticate called");
			PlayGameServices.authenticate();
			googleCloudTryingToLogin = true;
		}
	}
#endif
	
	public void LoadUserFromCloud(System.Action<UserCloudDownloadDelegateEventArgs> onResult = null)
	{		
		StartCoroutine(DownloadFromCloudByCheckingConflicts(onResult));
	}
	
	public static void LogCurrentStackTrace()
	{
		System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
		
		Debug.Log("LogCurrentStackTrace(): ");
		for(int i = 1; i < st.FrameCount; i++)
		{
			System.Diagnostics.StackFrame sf = st.GetFrame(i);
            Debug.Log(string.Format("Method: {0} (Line: {1})", sf.GetMethod(), sf.GetFileLineNumber()));
		}
	}
	
	/// <summary>
	/// Saves the data in cloud.
	/// </summary>
	/// <param name='onResultSave'>
	/// Info with result of Save.
	/// </param>
	public void SaveUserInCloud(System.Action<UserCloudUploadedDelegateEventArgs> onResultSave = null)
	{
		if (!DisableICloud) {
			StartCoroutine(SaveInCloud(onResultSave));
		}
	}
	
	/// <summary>
	/// Saves the data to disk.
	/// </summary>
	/// <param name='onResultSave'>
	/// Info with result of Save.
	/// </param>
	public bool SaveDataToDisk(System.Action<ResultSave> onResultSave = null)
	{
		bool result = UserCloud.Serialize(FILE_NAME_LOCAL);
		
		if (result && onResultSave != null)
			onResultSave(ResultSave.Disk);
		else if (!result && onResultSave != null)
			onResultSave(ResultSave.NotSaved);
		
		return result;
	}
	
	/// <summary>
	/// Resets the local user. Deletes the file with user data
	/// </summary>
	public void ResetLocalUser()
	{
		CurrentUser.ResetUserInfo(FILE_NAME_LOCAL);
	}
	
	
	#region Level Management
	

	public int GetTotalStarsWon()
	{
		int totalStars = 0;
		int levelIdx = 0;
		foreach(KeyValuePair<string, object> pair in CurrentUser.FinishedLevels)
		{
			totalStars += Convert.ToInt32((pair.Value as Dictionary<string,object>)["stars"]); 
			levelIdx++;
		}

		return totalStars;
	}
	

	/// <summary>
	/// Gets the score and stars for level.
	/// </summary>
	/// <param name='user'>
	/// User.
	/// </param>
	/// <param name='level'>
	/// Level.
	/// </param>
	/// <param name='score'>
	/// Score.
	/// </param>
	/// <param name='stars'>
	/// Stars.
	/// </param>
	public void GetScoreAndStarsForLevel(UserCloud user, int level, out int score, out int stars)
	{
		score = -1;
		stars = -1;
		if (!user.FinishedLevels.ContainsKey(level.ToString()))
			return;
		
		Dictionary<string, object> infoLevel = (Dictionary<string, object>)user.FinishedLevels[level.ToString()];
		//Debug.Log("GetScore And Stars: " + Convert.ToInt32(infoLevel["score"]));
		score = Convert.ToInt32(infoLevel["score"]);
		stars = Convert.ToInt32(infoLevel["stars"]);
	}
	
	/// <summary>
	/// Sets the score for level for CurrentUser.
	/// </summary>
	/// <param name='level'>
	/// Level.
	/// </param>
	/// <param name='scoreToSubmit'>
	/// Score to submit. -1 if you dont save/override the current score
	/// </param>
	/// <param name='starsToSubmit'>
	/// Stars to submit. -1 if you dont save/override the current stars
	/// </param>
	/// <param name='onResultNewRecord'>
	/// On result new record.
	/// </param>
	public void SetScoreForLevel(int level, int scoreToSubmit, int starsToSubmit, System.Action<UserHasReachedNewRecordDelegateEventArgs> onResultNewRecord = null, bool forceSaveInCloud = true)
	{
		this.SetScoreForLevel(CurrentUser, level, scoreToSubmit, starsToSubmit, onResultNewRecord, forceSaveInCloud);
	}
	
	/// <summary>
	/// Sets the score for level.
	/// </summary>
	/// <param name='user'>
	/// User to set the new information.
	/// </param>
	/// <param name='level'>
	/// Level.
	/// </param>
	/// <param name='scoreToSubmit'>
	/// Score to submit. -1 if you dont save/overwrite the current score
	/// </param>
	/// <param name='starsToSubmit'>
	/// Stars to submit. -1 if you dont save/overwrite the current stars
	/// </param>
	/// <param name='onResultNewRecord'>
	/// On result new record.
	/// </param>
	public void SetScoreForLevel(UserCloud user, int level, int scoreToSubmit, int starsToSubmit, System.Action<UserHasReachedNewRecordDelegateEventArgs> onResultNewRecord = null, bool forceSaveInCloud = true)
	{
		if (DisableICloud)
		{
			ContinueWithSetScoreForLevel(user, level, scoreToSubmit, starsToSubmit, onResultNewRecord, forceSaveInCloud);
			return;
		}
		
		ContinueWithSetScoreForLevel(user, level, scoreToSubmit, starsToSubmit, null, false);
		
		// First at all, merge user info or download from Cloud
		if (CurrentCloudUser != null)
		{
			this.MergeUsers( result =>
			{
				ContinueWithSetScoreForLevel(user, level, scoreToSubmit, starsToSubmit, onResultNewRecord, forceSaveInCloud);
			});
		}
		else
		{
			// Download User from cloud and Merge data with local user
			this.LoadUserFromCloud( result =>
			{
				ContinueWithSetScoreForLevel(user, level, scoreToSubmit, starsToSubmit, onResultNewRecord, forceSaveInCloud);
			});
		}
	}
	
	private void  ContinueWithSetScoreForLevel(UserCloud user, int level, int scoreToSubmit, int starsToSubmit, System.Action<UserHasReachedNewRecordDelegateEventArgs> onResultNewRecord = null, bool forceSaveInCloud = true, bool saveToDisk = true)
	{
		Dictionary<string, object> infoLevel;
		int currentScore = this.GetScoreForLevel(user, level);
		int currentStars = this.GetStarsForLevel(user, level);
		bool isNeededUpdateScore = false;
		bool isNeededUpdateStars = false;
		
		if (currentScore < scoreToSubmit) 
		{
			isNeededUpdateScore = true;
		}
		if (currentStars < starsToSubmit) 
		{
			isNeededUpdateStars = true;
		}
		
		//Save the score
		if (scoreToSubmit > -1 && isNeededUpdateScore)
		{
			if (user.FinishedLevels.ContainsKey(level.ToString()))
			{
				infoLevel = (Dictionary<string, object>)user.FinishedLevels[level.ToString()];
				infoLevel["score"] = scoreToSubmit;
				
			}
			else
			{
				infoLevel = new Dictionary<string, object>();
				infoLevel["score"] = scoreToSubmit;
				infoLevel["stars"] = 0;
				user.FinishedLevels.Add(level.ToString(), infoLevel);
				
			}
		}
		//Save the stars
		if (starsToSubmit > -1 && isNeededUpdateStars)
		{
			if (user.FinishedLevels.ContainsKey(level.ToString()))
			{
				infoLevel = (Dictionary<string, object>)user.FinishedLevels[level.ToString()];
				infoLevel["stars"] = starsToSubmit;
			}
			else
			{
				infoLevel = new Dictionary<string, object>();
				infoLevel["score"] = 0;
				infoLevel["stars"] = starsToSubmit;
				user.FinishedLevels.Add(level.ToString(), infoLevel);
				
			}
		}
		
		if (level > CurrentUser.LastFinishedLvl)
			CurrentUser.LastFinishedLvl = level;	

	 	//CurrentUser.LastFinishedLvl = LoadLevelButton.maxLevels;
		
		if (saveToDisk) {
			//user.SaveInDisk();
			Debug.Log("Saving scores to disk");
			bool isPossibleToSaveInCloud = true;
			isPossibleToSaveInCloud = this.SaveDataToDisk();
	
			Debug.Log("forceSave " + forceSaveInCloud);
			Debug.Log("DisableICloud " + DisableICloud);
			Debug.Log("isPossibleToSaveInCloud " + isPossibleToSaveInCloud);
			if (forceSaveInCloud && !DisableICloud && isPossibleToSaveInCloud) {
				this.SaveUserInCloud();
			}
		}

		UserHasReachedNewRecordDelegateEventArgs e = new UserHasReachedNewRecordDelegateEventArgs();
		if (isNeededUpdateScore || isNeededUpdateStars)
		{
			e.Level = level;
			e.Score = scoreToSubmit;
			e.Result = RecordType.Stars;
			if (isNeededUpdateScore && isNeededUpdateStars)
				e.Result = RecordType.ScoreAndStars;
			else if (isNeededUpdateScore)
				e.Result = RecordType.Score;
			e.Message = "User has reached a new record";
		}
		else
		{
			e.Level = level;
			e.Score = scoreToSubmit;
			e.Result = RecordType.None;
			e.Message = "User has not reached a new record";
		}
		RaiseUserHasReachedNewRecord(e, onResultNewRecord);
		
		//Debug.Log("SetScoreForLevel -> Score: " + scoreToSubmit + " for level: " + level);
	}
	
	#endregion
	
	#endregion
	
	#region iCloud Courutines
	
	private IEnumerator DeleteFromCloud()
	{
		JCloudDocument.FileDelete(FILE_NAME_CLOUD);
		yield break;
	}
	
	private IEnumerator SaveInCloud (System.Action<UserCloudUploadedDelegateEventArgs> onResultSave = null)
	{
		Debug.Log("Saving in the Cloud");
		UserCloudUploadedDelegateEventArgs args = new UserCloudUploadedDelegateEventArgs();
		bool isPossibleSaveInCloud = true;
		byte[] bytes = null;
		string errorReadingLocalData = string.Empty;
	
		try
		{
			#if UNITY_WEBPLAYER
				Debug.LogWarning("Unimplemented method");
				throw new Exception();
			#else
				bytes = System.IO.File.ReadAllBytes(UserCloud.GetPath(FILE_NAME_LOCAL));
				Debug.Log("[UserManagerCloud] Loaded local file user data: " + UserCloud.GetPath(FILE_NAME_LOCAL));
			#endif
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[SaveInCloud] Error reading local user data file...");
			isPossibleSaveInCloud = false;
			errorReadingLocalData = ex.Message;
		}
		
		if (isPossibleSaveInCloud == false)
		{
			Debug.LogWarning("Exception reading local user before save it: " + errorReadingLocalData);
			args.Error = errorReadingLocalData;
			args.Message = "There was an error saving to iCloud";
			args.Result = false;
			args.SaveIn = ResultSave.NotSaved;
			RaiseUserHasBeenUploaded(args, onResultSave);
			
			yield break;
		}
		
#if UNITY_IPHONE || UNITY_EDITOR
		JCloudDocumentOperation op = JCloudDocument.FileWriteAllBytes(FILE_NAME_CLOUD, bytes);
		
		while (op.finished == false)
		{
			Debug.Log("[SaveInCloud] Writting user data to cloud: " + op.progress);
				yield return null;
		}
		if (op.error.HasValue)
		{
			string res = "";
			switch (op.error.Value) {
				
				case JCloudDocumentError.PluginError:
				case JCloudDocumentError.NativeError:
				case JCloudDocumentError.InvalidArguments: // Look out for this one as it means you passed invalid path
					// Offer the user to retry
					res = "An error ocurred while saving text file. please retry.";
					Debug.LogWarning("[SaveInCloud] " + res);
					break;
				case JCloudDocumentError.DocumentNotFound:
					res = "There is no file present on this device. Create a new text file.";
					Debug.LogWarning("[SaveInCloud] " + res);
					break;
				case JCloudDocumentError.DownloadTimeout:
					// Offer the user to retry
					res = "Could not download user cloud file. Please retry.";
					Debug.LogWarning("[SaveInCloud] " + res);
					break;
				default: // We should never get there
					break;
			}

			Debug.LogWarning("[SaveInCloud] " + op.error.Value);
			args.Error = res;
			args.Message = "There was an error saving to iCloud";
			args.Result = false;
			args.ErrorType = op.error.Value;
			args.SaveIn = ResultSave.NotSaved;
			RaiseUserHasBeenUploaded(args, onResultSave);
		}
		else
		{
			// OK
			Debug.Log("[SaveInCloud] Saved user file in cloud " + bytes.Length + " bytes");
			args.Error = string.Empty;
			args.Message = "Saved in iCloud";
			args.Result = true;
			args.SaveIn = ResultSave.Cloud;
			
			Debug.Log("[SaveInCloud] - SaveUser. Level: " + CurrentUser.LastFinishedLvl);
			
			RaiseUserHasBeenUploaded(args, onResultSave);
		}

		yield break;	
		
//		byte[] bytes = System.IO.File.ReadAllBytes(UserCloud.GetPath(FILE_NAME_LOCAL));
//		JCloudDocument.FileWriteAllBytes(FILE_NAME_CLOUD, bytes);
//		
////		UserCloudUploadedDelegateEventArgs args = new UserCloudUploadedDelegateEventArgs();
////		
////		if (onResultSave != null)
////			onResultSave(ResultSave.Cloud);
//
//		Debug.Log("Save File in Cloud " + bytes.Length);
//		yield break;
		
#elif UNITY_ANDROID && !disableGoogleCloud
		if ( CanUseGoogleCloud() )
		{				
			while (!PlayGameServices.isSignedIn() && googleCloudTryingToLogin)
				yield return null;
			
			if ( PlayGameServices.isSignedIn() && !googleCloudTryingToLogin ) 
			{
				string strUserData = null;
				try
				{
					strUserData = System.Convert.ToBase64String(bytes);
					Debug.Log("[UserManagerCloud] Saving to Google Cloud services: " + bytes.Length + " bytes");
				}
				catch(Exception)
				{
					Debug.LogWarning("[UserManagerCloud] Failed to encode user raw bytes to string. Size: " + bytes.Length + " bytes");
				}
				
				if (strUserData != null) {
					PlayGameServices.setStateData(strUserData, 0);
				}
			}
		}
		
		yield break;
#endif
	}
	
	private IEnumerator DownloadFromCloudByCheckingConflicts(System.Action<UserCloudDownloadDelegateEventArgs> onResul = null)
	{
#if UNITY_IPHONE || UNITY_EDITOR
		JCloudDocumentOperation operation;

		// Let's check file for conflict first
		operation = JCloudDocument.FileHasConflictVersions(FILE_NAME_CLOUD);
		
		while (operation.finished == false)
			yield return null; // Wait for 1 frame
	
		// Look for error -- if any, handle & stop coroutine here -- ignore cloud unvailable as it simply means conflicts are not handled
		if (operation.error.HasValue && operation.error.Value != JCloudDocumentError.CloudUnavailable) {
			HandleDocumentError(operation.error.Value, onResul);
			yield break;
		}
	
		// Did looking for conflict trigger a result?
		if (((bool?)operation.result).HasValue && (bool)operation.result == true) {
			// We have conflicts -- sort them out
			operation = JCloudDocument.FileFetchAllVersions(FILE_NAME_CLOUD);
			while (operation.finished == false) 
				yield return null;
	
			// Look for error -- if any, handle & stop coroutine here
			if (operation.error.HasValue) {
				HandleDocumentError(operation.error.Value, onResul);
				yield break;
			}
	
			// Get conflict versions
			JCloudDocumentVersions versions = (JCloudDocumentVersions)operation.result;
	
			// We will now attempt to pick the "best" version (e.g. most progress) -- you could offer UI
			UserCloud temporalUser;
			int lastLevelTemporalUser = -1;
			byte[] bestVersionIdentifier = null;
			foreach (JCloudDocumentVersionMetadata metadata in versions.versionsMetadata) {
				// Read version bytes
				operation = JCloudDocument.FileReadVersionBytes(FILE_NAME_CLOUD, metadata.uniqueIdentifier);
				while (operation.finished == false) 
					yield return null;
	
				// Look for error -- if any, handle & stop coroutine here
				if (operation.error.HasValue) {
					HandleDocumentError(operation.error.Value, onResul);
					yield break;
				}
	
				// Pick best version
				temporalUser = UserCloud.Deserialize((byte[])operation.result);
				
				if (temporalUser == null) {
					continue;
				}
				
				if (temporalUser.LastFinishedLvl > lastLevelTemporalUser)
				{
					lastLevelTemporalUser = temporalUser.LastFinishedLvl;
					bestVersionIdentifier = metadata.uniqueIdentifier;
				}
			}
	
			// At that point we should have the best version
			if (bestVersionIdentifier != null) {
				// Pick it as current version
				operation = JCloudDocument.FilePickVersion(FILE_NAME_CLOUD, bestVersionIdentifier, versions.versionsHash);
				while (operation.finished == false) 
					yield return null;
	
				// Look for error -- if any, handle & stop coroutine here
				if (operation.error.HasValue) {
					HandleDocumentError(operation.error.Value, onResul);
					yield break;
				}
			}
		}
	
		// At that point conflicts have been cleared -- wait for file reading
		operation = JCloudDocument.FileReadAllBytes(FILE_NAME_CLOUD);
		while (operation.finished == false) 
			yield return null; // Wait for 1 frame
		
		// Look for error -- if any, handle & stop coroutine here
		if (operation.error.HasValue) {
			HandleDocumentError(operation.error.Value, onResul);
			yield break;
		}

		byte[] bytes = operation.result as byte[];
		if (bytes != null)
		{
			//Save iCloud info in Memory. Then, merge information with local user data
			Debug.Log("- LoadUserFromCloud OK");
			CurrentCloudUser = UserCloud.Deserialize(bytes);
			this.MergeUsers(onResul);
		}
#endif
		
#if UNITY_ANDROID && !disableGoogleCloud
		byte[] googleCloudDataBytes = null;
		
		if ( CanUseGoogleCloud() )
		{
			while (!PlayGameServices.isSignedIn() && googleCloudTryingToLogin)
			{
				yield return null; // Wait for n frames until googleCloudTryingToLogin was setted to FALSE
			}
			
			if(!PlayGameServices.isSignedIn() && !googleCloudTryingToLogin)
			{
				HandleDocumentError(JCloudDocumentError.CloudUnavailable, onResul);
				yield break;
			}
			
			Debug.Log("[UserManagerCloud] DownloadFromCloudByCheckingConflicts(): Downloading user data from cloud...");
			PlayGameServices.loadCloudDataForKey(0, false);
			
			// Wait a maximum of 6 seconds to retrieve google cloud data
			float timeLeftToWait = 6f;
			float lastTime = Time.realtimeSinceStartup;
			while (googleCloudData == null && timeLeftToWait > 0f) 
			{
				timeLeftToWait -= Time.realtimeSinceStartup - lastTime;
				lastTime = Time.realtimeSinceStartup;
				
				yield return null; // Wait for n frames until googleCloudData was setted in loadCloudDataForKeyFailedEvent or loadCloudDataForKeySucceededEvent
			}
	
			if (googleCloudData == "ERROR" || googleCloudData == null)
			{
				Debug.Log("ANDROID CLOUD SERVICES RETURNED ERROR DOWNLOADING");
				HandleDocumentError(JCloudDocumentError.DocumentNotFound, onResul);
				yield break;
			}
	
			Debug.Log("[UserManagerCloud] There is google cloud data : " + googleCloudData.Length);
			
			try 
			{
				googleCloudDataBytes = System.Convert.FromBase64String(googleCloudData);
				Debug.Log("[UserManagerCloud] Cloud data size: " + googleCloudDataBytes.Length);
			}
			catch(Exception)
			{
				Debug.LogWarning("[UserManagerCloud] Failed to decode android cloud data: " + googleCloudData);
			}
			
			
		}
		CurrentCloudUser = UserCloud.Deserialize(googleCloudDataBytes);
		MergeUsers(onResul);

		//Free memory
		googleCloudData = null;
#endif
		yield break;
	}
		
	// Single method for error handling
	private void HandleDocumentError(JCloudDocumentError documentError, System.Action<UserCloudDownloadDelegateEventArgs> onResul) {
		string error = "";
		switch (documentError) {
			case JCloudDocumentError.InvalidPlatform: // Web player -- no file access. Do not use JCloudDocument.
				error = "No file access allowed on this platform.";
				break;
			case JCloudDocumentError.PluginError:
			case JCloudDocumentError.NativeError:
			case JCloudDocumentError.InvalidArguments: // Look out for this one as it means you passed invalid path
				// Offer the user to retry
				error = "An error ocurred while loading game data. Please retry.";
				break;
			case JCloudDocumentError.DocumentNotFound:
				error = "There is no saved game present on this device. Start a new game.";
				break;
			case JCloudDocumentError.DownloadTimeout:
				// Offer the user to retry
				error = "Could not download the save game data. Please retry.";
				break;
			case JCloudDocumentError.InvalidVersionIdentifier:
			case JCloudDocumentError.InvalidVersionsHash:
				// Offer the user to retry
				error = "An error occured while handling conflict versions of your save game data. Please retry.";
				break;
			default: // We should never get there
				break;
		}
		UserCloudDownloadDelegateEventArgs args = new UserCloudDownloadDelegateEventArgs();
		
		Debug.Log(error);
			
		args.Error = error;
		args.Message = "There was an error downloading from iCloud by checking conflicts";
		args.Result = false;
		args.ErrorType = documentError;
		
		RaiseUserHasBeenDownloaded(args, onResul);
	}
	
	#endregion
	
	/// <summary>
	/// Merges the users and save the info in Local File and Cloud File. Previously, User from Cloud must be initialized
	/// </summary>
	/// <param name='onResult'>
	/// On result.
	/// </param>
	/// <param name='args'>
	/// Arguments.
	/// </param>
	private void MergeUsers(System.Action<UserCloudDownloadDelegateEventArgs> onResult = null)
	{
		bool isDirty = false;
		
		Debug.Log("- MergeUser 1");
		UserCloudDownloadDelegateEventArgs args = new UserCloudDownloadDelegateEventArgs();
		
		if (CurrentCloudUser == null)
		{
			Debug.Log("- MergeUser 2");
			args.Result = false;
			args.Message = "MergeUser: User from cloud must be initialized before the Merge";
			args.Error = "User from Cloud was not initialized";
			this.SaveUserInCloud();
			RaiseUserHasBeenDownloaded(args, onResult);
			return;
		}
		
		//Updating Levels
		int levelsInCloudUser = CurrentCloudUser.LastFinishedLvl;
		int levelsInLocalUser = CurrentUser.LastFinishedLvl;
		Debug.Log("- MergeUser - UserLocal had Level: " + levelsInLocalUser);
		Debug.Log("- MergeUser - UserCloud had Level: " + levelsInCloudUser);
		if (levelsInCloudUser > levelsInLocalUser)
		{
			CurrentUser.LastFinishedLvl = levelsInCloudUser;
			isDirty = true;

	 		//CurrentUser.LastFinishedLvl = LoadLevelButton.maxLevels;
		}
		
		for (int i = 1; i <= CurrentUser.LastFinishedLvl; i++)
		{
			int scoreForLocalUser;
			int scoreForCloudUser;
			
			int starsForLocalUser;
			int starsForCloudUser;
			this.GetScoreAndStarsForLevel(CurrentCloudUser, i, out scoreForCloudUser, out starsForCloudUser);
			this.GetScoreAndStarsForLevel(CurrentUser, i, out scoreForLocalUser, out starsForLocalUser);
			
			int winnerScore = scoreForCloudUser > scoreForLocalUser ? scoreForCloudUser : scoreForLocalUser;
			int winnerStars = starsForCloudUser > starsForLocalUser ? starsForCloudUser : starsForLocalUser;
			
			if (winnerScore != scoreForLocalUser || winnerStars != starsForLocalUser)
			{
				this.ContinueWithSetScoreForLevel(CurrentUser, i, winnerScore, winnerStars, null ,false, i == CurrentUser.LastFinishedLvl);
			}
			
			if (scoreForCloudUser < scoreForLocalUser || starsForCloudUser < starsForLocalUser) {
				isDirty = true;
			}
		}
		Debug.Log("- MergeUser 4");
		//Updating Coins
		if (CurrentCloudUser.UserGoldCoins > CurrentUser.UserGoldCoins)
		{
			CurrentUser.UserGoldCoins =  CurrentCloudUser.UserGoldCoins;
		}
		else if (CurrentCloudUser.UserGoldCoins < CurrentUser.UserGoldCoins) {
			isDirty = true;
		}
		
		if (CurrentCloudUser.UserSilverCoins > CurrentUser.UserSilverCoins)
		{
			CurrentUser.UserSilverCoins = CurrentCloudUser.UserSilverCoins;
		}
		else if (CurrentCloudUser.UserSilverCoins < CurrentUser.UserSilverCoins) {
			isDirty = true;
		}
		
		//Updating Lives
//		if (CurrentCloudUser.NumsLiveLeft > CurrentUser.NumsLiveLeft)
//		{
//			isDirty = true;
//			CurrentUser.NumsLiveLeft = CurrentCloudUser.NumsLiveLeft;
//		}
		
		//Once finished Merging, both instance must be the same
		CurrentCloudUser = CurrentUser;
		
		//Raise the event
		args.Error = string.Empty;
		args.Message = "MergeUser: User has been downloaded";
		args.Result = true;
		RaiseUserHasBeenDownloaded(args, onResult);
		
		if (onResult != null)
			onResult(args);
		
//		IsUserInit = true;
		
		Debug.Log(" - MergeUser. Level: " + CurrentUser.LastFinishedLvl);
		
		bool isPossibleToSaveInCloud = this.SaveDataToDisk();
		if (isDirty && isPossibleToSaveInCloud)
			this.SaveUserInCloud();
		
	}
	
#if UNITY_ANDROID
//	//TODO: comment this after finished testing functionality.
//	void OnGUI()
//	{
//		GUILayout.BeginVertical();
//		{
//			if ( GUILayout.Button("Delete Cloud Data", GUILayout.Height(50f)) )
//			{
//				DeleteUserFromCloud();
//			}	
//		}
//		GUILayout.EndVertical();
//	}
#endif
	
	void OnDestroy()
	{
		dontReload = true;

#if UNITY_ANDROID && !disableGoogleCloud
		GPGManager.loadCloudDataForKeyFailedEvent 		-= loadCloudDataForKeyFailedEvent;
		GPGManager.loadCloudDataForKeySucceededEvent 	-= loadCloudDataForKeySucceededEvent;
		GPGManager.authenticationFailedEvent 			-= loginCloudAutenticationFailedEvent;
		GPGManager.authenticationSucceededEvent 		-= loginCloudAutenticationSucceededEvent;
		GPGManager.userSignedOutEvent					-= OnGoogleCloudSignedOut;
#endif
	}

#region Android helper methods

#if UNITY_ANDROID && !disableGoogleCloud
	private void loadCloudDataForKeyFailedEvent(string _error)
	{
		Debug.Log("[UserManagerCloud] Failed to load Android Cloud server data: " + _error	);
		googleCloudData = "ERROR";
		
		if (_error == "2002") //The is no key in the server
		{
			Debug.Log("[UserManagerCloud] There is no key in the Android Cloud service, creating a new one");
			SaveUserInCloud();
		}
	}
	
	private void loadCloudDataForKeySucceededEvent(int _key, string _data)
	{
		Debug.Log("[UserManagerCloud] Successfull loaded Android Cloud server data.  Key : " + _key);
		googleCloudData = _data;
	}
	
	private void loginCloudAutenticationFailedEvent(string _error)
	{
		Debug.Log("[UserManagerCloud] Android cloud can't login ERROR: " + _error);

		googleCloudTryingToLogin = false;
		googleCloudLoginFailed = true;

//		NativeMessagesSystem.OnButtonPressed += OnLoginFailedMsgBtnPressed;
//		NativeMessagesSystem.Instance.ShowMessage("Google Cloud", "Google Cloud login failed. Please check your internet settings.",
//			"Cancel", "Retry");
	}
	
	private void loginCloudAutenticationSucceededEvent(string _userId)
	{
		Debug.Log("[UserManagerCloud] Android Cloud user id: " + _userId);
		googleCloudTryingToLogin = false;
		googleCloudLoginFailed = false;

		LoadUserFromCloud();
	}
	
	protected void OnGoogleCloudSignedOut()
	{
		Debug.Log("[UserManagerCloud] Google cloud signed out...");	
	}
	
//	protected void OnLoginFailedMsgBtnPressed(int buttonIdx)
//	{
//		NativeMessagesSystem.OnButtonPressed -= OnLoginFailedMsgBtnPressed;
//		if (buttonIdx == 0)
//		{
//			Debug.Log("OnLoginFailedMsgBtnPressed(): Retry button pressed...");
//			
//			// Reset this flag to allow retry of the google cloud connection.
//			hasGoogleCloudTriedLogin = false;
//			userCanceledGoogleCloud = false;
//			NativeMessagesSystem.OnButtonPressed += OnLoginFailedMsgBtnPressed;
//			StartGoogleCloudLogin();
//		}
//		else {
//			Debug.Log("OnLoginFailedMsgBtnPressed(): Cancel button pressed...");
//			userCanceledGoogleCloud = true;
//		}
//	}
	
#endif
	
#endregion	
*/
}
