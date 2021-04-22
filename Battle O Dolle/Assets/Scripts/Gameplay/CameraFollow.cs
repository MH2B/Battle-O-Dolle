using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
	private CinemachineVirtualCamera virtualCamera = null;

	private void Awake()
	{
		virtualCamera = GetComponent<CinemachineVirtualCamera>();
	}

	public void SettheFollowTarget(Transform player)
	{
		virtualCamera.Follow = player;
	}
}
