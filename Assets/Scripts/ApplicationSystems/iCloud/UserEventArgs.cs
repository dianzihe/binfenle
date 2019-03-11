using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#region Delegates Event Args

public class UserCloudDownloadDelegateEventArgs : EventArgs
{
	public bool Result {get; set; }
	public string Message {get; set;}
	public string Error {get; set;}
	//public JCloudDocumentError ErrorType {get; set;}
}

public class UserCloudUploadedDelegateEventArgs : EventArgs
{
	public bool Result {get; set; }
	public string Message {get; set;}
	public string Error {get; set;}
	//public JCloudDocumentError ErrorType {get; set;}
	public ResultSave SaveIn {get; set;}
}

public class UserLoginEventDelegateEventArgs : EventArgs
{
	public bool Result {get; set; }
	public string Message {get; set;}
	public string Error {get; set;}
}

public class UserFriendsDelegateEventArgs : EventArgs
{
	public bool Result {get; set; }
	public string Message {get; set;}
	public string Error {get; set;}
	public List<object> FriendsList {get; set;}
}

public class FriendsPictureProfileDelegateEventArgs : EventArgs
{
	public bool Result {get; set;}
	public string Message {get; set;}
	public int StartIndexOfCollection {get; set;}
	public int EndIndexOfCollection {get; set;}
	public int CurrentPage {get; set;}
}

public class LevelRankingDelegateEventArgs : EventArgs
{
	public bool Result {get; set;}
	public string Message {get; set;}
	public int Level;
	public List<object> Ranking;
	public int PositionOfUser;
}

public class PictureProfileDelegateEventArgs : EventArgs
{
	public bool Result {get; set;}
	public string Message {get; set;}
	public Texture2D Image {get; set;}
}

public enum RecordType
{
	Score,
	Stars,
	ScoreAndStars,
	None
}

public class UserHasReachedNewRecordDelegateEventArgs : EventArgs
{
	public RecordType Result {get; set;}
	public string Message {get; set;}
	public int Level {get; set;}
	public int Score {get; set;}
	public int Stars {get; set;}
}

#endregion

