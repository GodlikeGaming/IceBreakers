using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Server : NetworkManager
{

    
    public override void OnServerAddPlayer(NetworkConnection conn)
    {

        var polygon = FindObjectOfType<MeshGenerator>().edge_points;
        var position = Geometry.PointNearEdgeOfPolygon(polygon);
        
        GameObject player = (GameObject)Instantiate(playerPrefab, position, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
