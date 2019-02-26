using UnityEngine;
using System.Collections;

public class BasicItem : MonoBehaviour
{
	public delegate void UsingItemEvent(BasicItem item);
	
	public static event UsingItemEvent OnActuallyUsingAnyItem;
	public static event UsingItemEvent OnStartUsingAnyItem;
	
	public event UsingItemEvent OnFinishUsingItem;
	public event UsingItemEvent OnActuallyUsingItem;
	
	public GameObject effectPrefab;
	public Transform effectPosition;
	public Vector3 effectOffset;
	
	public string nameSingularKey;
	public string namePluralKey;
	
	public string iconName;
	
	protected Match3BoardGameLogic boardLogic;
	
	protected DestroyEffect destroyEffect;
	
	/// <summary>
	/// True if this basic item is executing its effect, False otherwise.
	/// </summary>
	private bool isRunning;
	
	protected virtual void Awake() { }
	protected virtual void Start() { }
	
	public string NameSingular {
		get {
			return nameSingularKey;
		}
	}
	
	public virtual string ItemName 
	{
		get {
			return "";
		}
	}
	
	public string NamePlural {
		get {
			return Language.Get(namePluralKey);
		}
	}
	
	public virtual void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
		gameLogic.unstableLock++;
		gameLogic.TryCheckStableBoard();

		boardLogic = _boardLogic;
		
		if (effectPrefab) {
			destroyEffect = effectPrefab.GetComponent<DestroyEffect>();
		}
		
		if (OnStartUsingAnyItem != null)
		{
			OnStartUsingAnyItem(this);
		}
	}
	
	/// <summary>
	/// Gets or set the isRunning flag that indicates if this item is executing its effect. 
	/// </summary>
	/// <value>
	/// <c>true</c> if this item's effect is running; otherwise, <c>false</c>.
	/// </value>
	public bool IsRunning 
	{
		get {
			return isRunning;
		}
		protected set {
			isRunning = value;
		}
	}

	protected virtual void FinishUsingItem()
	{
		IsRunning = false;
		
		boardLogic.unstableLock--;
		
		if (OnFinishUsingItem != null)
		{
			OnFinishUsingItem(this);
		}
	}
	
	public virtual void ActuallyUsingItem()
	{
		IsRunning = true;
		
		boardLogic.unstableLock++;
		

		
		if (OnActuallyUsingItem != null)
		{
			OnActuallyUsingItem(this);
		}
		
		if (OnActuallyUsingAnyItem != null)
		{
			OnActuallyUsingAnyItem(this);
		}
	}
	
	public virtual bool CanBeUsed()
	{
		return true;
	}
	
	public virtual void CancelUsingItem()
	{
		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
		gameLogic.unstableLock--;
		gameLogic.TryCheckStableBoard();

		DoDestroy();
	}
	
	public virtual void StartItemEffects()
	{
		ActuallyUsingItem();
		
		if (effectPrefab && effectPosition) {
			SpawnEffect(effectPosition.position, effectPrefab);
		}
		
		if (destroyEffect) {
			Invoke("DoItem", destroyEffect.destroyTileTime);
			Invoke("DoDestroy", destroyEffect.lifeTime);
		}
		else {
			DoItem();
			DoDestroy();
		}
	}
	
	protected Transform SpawnEffect(Vector3 initPosition, GameObject prefab) 
	{
		Transform effectInstance = (Instantiate(prefab) as GameObject).transform;
		effectInstance.position = initPosition + effectOffset;
		
		Destroy(effectInstance.gameObject, destroyEffect.lifeTime);
		
		return effectInstance;
	}
	
	protected virtual void DoItem()
	{
		Match3BoardGameLogic gameLogic = Match3BoardGameLogic.Instance;
		gameLogic.unstableLock--;
		gameLogic.TryCheckStableBoard();

		FinishUsingItem();
		boardLogic.IsBoardStable = false;
		boardLogic.TryCheckStableBoard();
	}
	
	protected virtual void DoDestroy()
	{	
		Destroy(gameObject);
	}
	
	protected virtual void OnDestroy() {
		
	}
}

