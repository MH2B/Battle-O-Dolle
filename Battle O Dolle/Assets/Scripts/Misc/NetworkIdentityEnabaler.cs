using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkIdentityEnabaler : MonoBehaviour
{

    [SerializeField] private List<GameObject> objectsWithNetworkIdentity = new List<GameObject>();

	private void Start()
	{
		Invoke("Enable", 1f);
	}

	private void Enable()
	{
		objectsWithNetworkIdentity.ForEach(x => x.SetActive(true));
		Destroy(gameObject);
	}
}
