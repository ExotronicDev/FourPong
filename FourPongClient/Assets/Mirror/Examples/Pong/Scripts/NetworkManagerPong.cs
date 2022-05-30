using UnityEngine;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

namespace Mirror.Examples.Pong
{
    // Custom NetworkManager that simply assigns the correct racket positions when
    // spawning players. The built in RoundRobin spawn method wouldn't work after
    // someone reconnects (both players would be on the same side).
    [AddComponentMenu("")]
    public class NetworkManagerPong : NetworkManager
    {
        public Transform backLeftRacketSpawn;
        public Transform backRightRacketSpawn;
        public Transform frontLeftRacketSpawn;
        public Transform frontRightRacketSpawn;
        GameObject ball;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // add player at correct spawn position
            //Transform start = numPlayers == 0 ? upRacketSpawn : downRacketSpawn;
            Transform start;
            if (numPlayers == 0) {
                start = backLeftRacketSpawn;
            } else if (numPlayers == 1) {
                start = backRightRacketSpawn;
            } else if (numPlayers == 2) {
                start = frontLeftRacketSpawn;
            } else {
                start = frontRightRacketSpawn;
            }

            Debug.Log(playerPrefab);

            GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);

            // spawn ball if two players
            if (numPlayers == 2)
            {
                ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
                NetworkServer.Spawn(ball);
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // destroy ball
            if (ball != null)
                NetworkServer.Destroy(ball);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }
    }
}
