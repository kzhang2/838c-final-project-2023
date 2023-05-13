using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opencloseStallDoor : MonoBehaviour
{

	public Animator openandclose;
	public bool open;
	public Transform Player;

	void Start()
	{
		open = false;
	}

	public void changeState()
	{
		if(open)
		{
			StartCoroutine(closing());
		}
		else
		{
			StartCoroutine(opening());
		}
	}

	IEnumerator opening()
	{
		print("you are opening the door");
		openandclose.Play("OpeningStall");
		open = true;
		yield return new WaitForSeconds(.5f);
	}

	IEnumerator closing()
	{
		print("you are closing the door");
		openandclose.Play("ClosingStall");
		open = false;
		yield return new WaitForSeconds(.5f);
	}


}