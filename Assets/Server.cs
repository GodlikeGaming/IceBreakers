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

    public void AddSnowball(Vector2 pos, Vector2 dir, GameObject player)
    {
        Debug.Log(spawnPrefabs[1].name);
        GameObject snowball = (GameObject)Instantiate(spawnPrefabs[1], pos, Quaternion.identity);

        var movement = snowball.GetComponent<SnowballMovement>();
        movement.ignorePlayer = player;
        movement.Velocity = dir;
        movement.Position = pos;

       // var conn = NetworkServer.connections[(int) player_id];
        

       
        NetworkServer.Spawn(snowball);
        //IgnoreCol(snowball, player);
    }

    

   


}
