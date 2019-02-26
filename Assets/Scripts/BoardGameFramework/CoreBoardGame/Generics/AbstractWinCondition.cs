using UnityEngine;
using System.Collections;

/// <summary>
/// WinCondition.
/// Encapsulates the logic for testing the win condition for the current game.
/// It will also look for child WinCondition components 
/// </summary>
public abstract class AbstractWinCondition : MonoBehaviour, ICustomSerializable 
{
	public delegate void ConditionChecked();
	
	public bool isOptional = false;

	/// <summary>
	/// When this component initializes it checks if its GameObject has any other child AbstractWinConditions to evaluate.
	/// </summary>
//	public AbstractWinCondition[] childWinConditions;
	
	public event ConditionChecked OnWinChecked;
	
	protected bool paused = true;
		
	/// <summary>
	/// The cached remaining moves/seconds at the moment the player achieved the minimum requirements to pass the level.
	/// -1 means this is not set yet.
	/// </summary>
	protected int cachedRemainingPlayAmount = -1;
	
	
	public bool IsPaused {
		get {
			return paused;
		}
	}
	
	protected virtual void Awake() {
	}
	
	protected virtual void Start() {
	}
	
	/// <summary>
	/// Calculates and returns the objective completion percentage (a value from 0f to 1f).
	/// The result represents the percentage reached until the level can be completed with the minimum requirements.
	/// Each extending class should calculate this progress accordingly.
	/// </summary>
	/// <value>
	/// The objective progress. (0f - 1f)
	/// </value>
	public abstract float CalculateObjectiveProgress();
	
//	public abstract bool EvaluateWinCondition(BoardGameLogic gameLogic);

	#region ICustomSerializable implementation
	public virtual void WriteToStream (System.IO.BinaryWriter writeStream) {
		throw new System.NotImplementedException ();
	}

	public virtual void ReadFromStream (int fileVersion, System.IO.BinaryReader readStream) {
		throw new System.NotImplementedException ();
	}
	#endregion
	
	/// <summary>
	/// Updates the minimum win requirement conditions. This is where <see cref="CachedRemainingConditions"/>
	/// This method should be called inside the Check() method.
	/// </summary>
	protected abstract void UpdateMinimumWinRequirement();
	
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
	
	public void RaiseOnWinChecked()
	{
		if (OnWinChecked != null) {
			OnWinChecked();
		}
	}
			
	public int CachedRemainingPlayAmount
	{
		get {
			return cachedRemainingPlayAmount;
		}
		protected set {
			cachedRemainingPlayAmount = value;
		}
	}
	
	public virtual string GetWinString()
	{
		//return Language.Get("CONGRATULATIONS");
		return "Congratulations!";
	}
	
	public virtual string GetLoseReason()
	{
		return "";
	}
	
	public virtual string GetObjectiveString()
	{
		return "";
	}
	
	public virtual string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		return "";
	}
	
	/// <summary>
	/// Gets a short string describing the type of the level. 
	/// Mainly used for the Analytics events.
	/// </summary>
	/// <value>
	/// The type of the level.
	/// </value>
	public abstract string GetLevelType(AbstractLoseCondition loseConditions);
}
