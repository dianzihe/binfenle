using UnityEngine;
using System.Collections;

public class GrowingThornBoardPiece : Match3BoardPiece {

	public static event System.Action<GrowingThornBoardPiece> OnGrowingThornBoardPieceInit;
	const string spritesPath = "Game/Materials/Pieces/thorn_pieces";

	private bool burned = false;
	protected SpriteRenderer backModelRenderer;
	protected Animation emberAnimation;
	protected Sprite[] sprites;
	public GameObject burnEffectPrefab;
	public string burnSound;

	public bool Burned {
		get {
			return burned;
		}
		set {
			burned = value;
			int spriteIdx = burned?1:0;
			backModelRenderer.sprite = sprites[spriteIdx];

			if (burned){
				SoundManager.Instance.Play("thorn_burn_sfx");

				Transform effectInstance = (Instantiate(burnEffectPrefab) as GameObject).transform;
				effectInstance.position = transform.position;
				DestroyEffect destroyEffect = effectInstance.GetComponent<DestroyEffect>();
				Destroy(effectInstance.gameObject, destroyEffect.lifeTime);

				emberAnimation.Play();
			}
		}
	}

	public override void Awake ()
	{
		base.Awake ();
		sprites = Resources.LoadAll<Sprite>(spritesPath);
		backModelRenderer = transform.Find("Model_back").GetComponent<Renderer>() as SpriteRenderer;
		emberAnimation = transform.Find("Model_front").GetComponent<Animation>();
		Burned = false;
	}

	public override void InitComponent (AbstractBoardRenderer _boardRenderer)
	{
		base.InitComponent(_boardRenderer);
		if (OnGrowingThornBoardPieceInit != null) {
			OnGrowingThornBoardPieceInit(this);
		}
	}
	
}
