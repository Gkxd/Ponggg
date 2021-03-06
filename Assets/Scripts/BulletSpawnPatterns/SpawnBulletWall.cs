﻿using UnityEngine;
using System.Collections;

public class SpawnBulletWall : _BulletSpawner
{
	[Tooltip("The bullet that you want to spawn. Must have a rigidbody or this will crash.")]
	public GameObject bullet;

	[Tooltip("The number of bullets in each wall")]
	public int wallAmount;
	[Tooltip("The width of the wall of bullets")]
	public float wallRange;

	[Tooltip("The number of walls that you will shoot")]
	public int wallLayers;
	[Tooltip("The speed of the slowest layer")]
	public float minSpeed;
	[Tooltip("The speed of the fastest layer")]
	public float maxSpeed;

	void Start()
	{
		// Spawn the bullets
		for (int i = 0; i < wallLayers; i++)
		{
			float layerSpeed = Mathf.Lerp(minSpeed, maxSpeed, wallLayers == 1 ? 1 : i / (wallLayers - 1f));
			for (int j = 0; j < wallAmount; j++)
			{
				float x = transform.position.x;
				float y = transform.position.y;
				float z = transform.position.z;
				float offset = j * (wallRange / (wallAmount-1)) - wallRange/2;

				GameObject b = (GameObject)Instantiate(bullet, new Vector3(x, y + offset, z), Quaternion.identity);
                b.transform.right = transform.right;
				b.GetComponent<Rigidbody>().velocity = layerSpeed * transform.right;
				b.GetComponent<BulletId>().playerId = playerId;
			}
		}

		Destroy(gameObject); // Destroy the gameObject that created the bullets
	}
}
