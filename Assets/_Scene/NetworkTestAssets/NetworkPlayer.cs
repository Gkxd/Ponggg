using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour {

    public GameObject bulletPrefab;

    // Update is called once per frame
    void Update() {
        if (!isLocalPlayer)
            return;

        if (Input.GetKey(KeyCode.W)) {
            this.transform.position += Vector3.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position += Vector3.down;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
    }


    // This [Command] code is called on the Client …
    // … but it is run on the Server!
    [Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
                                            bulletPrefab,
                                            this.transform.position,
                                            this.transform.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

//Use this to initialize the local player to something
public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }
}
