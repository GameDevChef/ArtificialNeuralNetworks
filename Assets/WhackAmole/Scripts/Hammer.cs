using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
	[SerializeField] private Vector3 offsetToObject;

	private bool isWhacking;

	[SerializeField] private Animator animator;

	public void MoveObject(Vector3 position, bool hit)
	{
		if (hit && !isWhacking)
		{
			transform.position = position + offsetToObject;
			StartCoroutine(WhackCO(position, transform.position));
		}
	}

	private IEnumerator WhackCO(Vector3 position, Vector3 returnPosition)
	{
		isWhacking = true;
		animator.SetTrigger("Whack");
		yield return new WaitForSeconds(.6f);
		isWhacking = false;

	}

	private void OnTriggerEnter(Collider other)
	{

		if (other.CompareTag("Item"))
		{
			WhackItem item = other.GetComponentInParent<WhackItem>();
			if (item)
			{
				Debug.Log("whack");
				other.transform.root.gameObject.SetActive(false);
				item.FreeSpace();
			}
		}

	}
}
