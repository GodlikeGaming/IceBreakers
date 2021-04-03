using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    SyncList<GameObject> lines = new SyncList<GameObject>();

    public GameObject prefab_lr_holder;
    public override void OnStartClient()
    {
        Debug.Log("Client started");
        foreach (var line in lines)
        {
            NetworkServer.Spawn(line);
        }
    }

}
