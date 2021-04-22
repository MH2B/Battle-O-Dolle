using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Threading.Tasks;

public class MirrorSpawner : NetworkBehaviour
{

	[SerializeField] public Dictionary<int, GameObject> spawnedFounders = new Dictionary<int, GameObject>();
	private int spawnedIndex = 0;

    public static MirrorSpawner instance = null;
	private GameObject newSpawnedGameObjec = null;

	public PlayersInGameSettings playerInGameSettings;

	private bool hasSettedThePlayerInGameSettings = false;


	public override void OnStartClient()
	{
		if (isLocalPlayer)
		{
			if (instance == null)
			{
				instance = this;
			}
		}
	}


	public int SpawnGameObjectindex(GameObject spawnable, Vector3 _spawnPos , Quaternion _rotation)
	{
		// Set the player settings
		if (!hasSettedThePlayerInGameSettings)
		{
			playerInGameSettings = MirrorPlayer.localPlayer.thisPlayerInGameSettings;
			hasSettedThePlayerInGameSettings = true;
		}
		int index = spawnedIndex;
		spawnedIndex++;
		CmdSpawn(spawnable.name, _spawnPos , _rotation , gameObject, playerInGameSettings, index);
		return index;
	}

	[Command(ignoreAuthority = true)]
	private void CmdSpawn(string spawnable , Vector3 _spawnPos, Quaternion _rotation , GameObject owner, PlayersInGameSettings _playerInGameSettings , int _spawnedIndex)
	{
		GameObject spawnableobject = Resources.Load(spawnable, typeof(GameObject)) as GameObject;
		// Can make this courotine to make wait for rpc
		print("Spawning in-game stuffs");
		GameObject player = Instantiate(spawnableobject, _spawnPos, _rotation);
		player.GetComponent<NetworkMatchChecker>().matchId = _playerInGameSettings.matchID.ToGuid();
		NetworkServer.Spawn(player, owner);
		TRPCSpawnedGameObjectNetId(player, _spawnedIndex);
	}

	[TargetRpc]
	private void TRPCSpawnedGameObjectNetId(GameObject newSpawned , int _spawnedIndex)
	{
		newSpawnedGameObjec = newSpawned;
		spawnedFounders.Add(_spawnedIndex, newSpawned);
	}


	public void DestoyGameObject(GameObject destroyable)
	{
		CmdDestroy(destroyable);
	}

	[Command(ignoreAuthority = true)]
	private void CmdDestroy(GameObject destroyable)
	{
		RPCDestroy(destroyable);
	}

	[ClientRpc]
	private void RPCDestroy(GameObject destroyable)
	{
		Destroy(destroyable);
	}



	///// help
	IEnumerator AfterSpawninGrenade(int _index, float waitingTime)
	{
		yield return new WaitForSeconds(waitingTime);
		if (!MirrorSpawner.instance.spawnedFounders.ContainsKey(_index))
		{
			StartCoroutine(AfterSpawninGrenade(_index, waitingTime));
		}
		if (MirrorSpawner.instance.spawnedFounders[_index] == null)
		{
			print("error");
		}
		else
		{
			print("found");
		}
		GameObject newTrapDetector = MirrorSpawner.instance.spawnedFounders[_index];
		//StartCoroutine(AfterSpawninGrenade(index, 0.5f));
	}

}
