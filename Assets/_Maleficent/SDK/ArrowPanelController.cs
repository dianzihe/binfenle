using UnityEngine;
using System.Collections;

public class ArrowPanelController : MonoBehaviour 
{
	public GameObject m_shopPanel=null,m_manaScrollerPanel=null,m_propertyScrollPanel;
	public GameObject m_manaPanel=null,m_propertyPanel=null;
	public GameObject m_LeftArrow=null;
	public GameObject m_RightArrow=null;
	public float m_Manaleft,m_Manaright,m_propertyLeft,m_propertyRight;
	// Use this for initialization
	void Start () 
	{ 
		OrientationListener.Instance.OnOrientationChanged += OrientChanged;

		m_LeftArrow.SetActive (false);
		m_RightArrow.SetActive (false);
	}
	void OrientChanged(ScreenOrientation newOrientation)
	{
		if (newOrientation == ScreenOrientation.Landscape) 
		{
			gameObject.SetActive(false);
		}
		else if (newOrientation == ScreenOrientation.Portrait) 
		{
			gameObject.SetActive(true);
		}
	}
	// Update is called once per frame
	void Update () 
	{
		if(Screen.orientation!=ScreenOrientation.Landscape)
		{
			if(m_shopPanel.activeSelf==true)
			{
				if(m_manaPanel.activeSelf==true||m_propertyPanel.activeSelf==true)
				{
					if(m_manaPanel.activeSelf==true)
					{
						if(m_manaScrollerPanel.transform.localPosition.y<m_Manaleft)
						{
							m_LeftArrow.SetActive(false);
						//	MakeAlaphSameFreqence();
						}
						else
						{
							m_LeftArrow.SetActive(true);
						}

						if(m_manaScrollerPanel.transform.localPosition.y>m_Manaright)
						{
							m_RightArrow.SetActive(false);
						//	MakeAlaphSameFreqence();
						}
						else
						{
							m_RightArrow.SetActive(true);
						}
					}
					else
					{
						if(m_propertyScrollPanel.transform.localPosition.y<m_propertyLeft)
						{
							m_LeftArrow.SetActive(false);
							//	MakeAlaphSameFreqence();
						}
						else
						{
							m_LeftArrow.SetActive(true);
						}
						
						if(m_propertyScrollPanel.transform.localPosition.y>m_propertyRight)
						{
							m_RightArrow.SetActive(false);
							//	MakeAlaphSameFreqence();
						}
						else
						{
							m_RightArrow.SetActive(true);
						}
					}
				}
				else
				{
					m_LeftArrow.SetActive(false);
					m_RightArrow.SetActive(false);
				}
			}
			else
			{
				m_LeftArrow.SetActive(false);
				m_RightArrow.SetActive(false);
			}
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	void MakeAlaphSameFreqence()
	{
		if(m_LeftArrow!=null&&m_RightArrow!=null)
		{
			TweenAlpha alphaLeftInstance=m_LeftArrow.GetComponentInChildren<TweenAlpha>();
			 
			if(alphaLeftInstance!=null)
			{
				alphaLeftInstance.from=1;
				alphaLeftInstance.to=0;
				alphaLeftInstance.Play(true);
			}
			TweenAlpha alphaRightInstance=m_RightArrow.GetComponentInChildren<TweenAlpha>();
			 
			if(alphaRightInstance!=null)
			{
				alphaRightInstance.from=1;
				alphaRightInstance.to=0;
				alphaRightInstance.Play(true);
			}


		}
	}
}
