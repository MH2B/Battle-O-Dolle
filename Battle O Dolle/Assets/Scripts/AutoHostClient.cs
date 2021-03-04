using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class AutoHostClient : NetworkManager
{


    #region Methods

    public override void Start()
    {
        base.Start();
        if (!Application.isBatchMode)
        { //Headless build
            Debug.Log($"=== Client Build ===");
            StartClient();
        }
        else
        {
            Debug.Log($"=== Server Build ===");
        }
    }

    #endregion


    #region Call Backs

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        StartHost();
    }

    #endregion



}