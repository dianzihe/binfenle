using UnityEngine;
using System.Collections;

///TODO: implement this extending PulseEngine's Block class.
public abstract class AbstractLoseCondition : MonoBehaviour, ICustomSerializable 
{	
	public delegate IEnumerator ConditionCheckedCoroutine();
	
	public event System.Action OnLoseChecked;
	public event System.Action OnNewMove;
	public event ConditionCheckedCoroutine OnNewMoveCoroutine;
	
	protected bool paused = true;
	
	public bool IsPaused {
		get {
			return paused;
		}
	}
	
	protected virtual void Awake () {

	}
	
	// Use this for initialization
	protected virtual void Start () {
	
	}	

	#region ICustomSerializable implementation
	public virtual void WriteToStream (System.IO.BinaryWriter writeStream) {
		throw new System.NotImplementedException ();
	}

	public virtual void ReadFromStream (int fileVersion, System.IO.BinaryReader readStream) {
		throw new System.NotImplementedException ();
	}
	#endregion
	
	/// <summary>
	/// Checks if this condition has been met.
	/// </summary>
	public abstract bool Check();
	
	/// <summary>
	/// Pause or resume this condition.
	/// </summary>
	/// <param name='pause'>
	/// Pause if true, otherwise resume.
	/// </param>
	public virtual void Pause(bool pause) {
		paused = pause;
	}
	
	/// <summary>
	/// Called when a new move is performed.
	/// </summary>
	public virtual void NewMove() {
		if (OnNewMove != null) {
			OnNewMove();
		}
		if(OnNewMoveCoroutine != null) {
			StartCoroutine(OnNewMoveCoroutine());
		}
	}
	
	public void RaiseOnLoseChecked()
	{
		if (OnLoseChecked != null) {
			OnLoseChecked();
		}
	}
	
	public virtual string GetString()
	{
		return "";
	}
	
	public virtual string GetStringUnit()
	{
		return "";
	}
	
	public virtual string GetStringValue()
	{
		return "";
	}
	
	public virtual string GetLoseString()
	{
		return "";
	}
		
	public virtual int GetOffer(int packIndex)
	{
		return 0;
	}
	
	public virtual void AcceptOffer(int packIndex){
	}
	
	public virtual void DoWin()
	{
//		LoadLevelButton.lastUnlockedLevel = Mathf.Max(LoadLevelButton.lastUnlockedLevel, MaleficentBlackboard.Instance.level + 1);
//		PlayerPrefs.SetInt("LastLevel", LoadLevelButton.lastUnlockedLevel);
	}
	
	/// <summary>
	/// Unity event.
	/// </summary>
	protected virtual void OnDestroy() { }
}
