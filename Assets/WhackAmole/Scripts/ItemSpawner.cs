using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
	[SerializeField] private Transform[] spawnTransforms;
	[SerializeField] private WhackItem whackItem;
	[SerializeField] private Mesh[] avaliableMeshes;
	[SerializeField] private Material[] avaliableMaterials;

	[SerializeField] private float minInterval = 1f;
	[SerializeField] private float maxInterval = 2f;
	[SerializeField] private float lifeTime;

	private List<Transform> freeTransforms;
	private Perceptron perceprton;

	private float currentInterval = 0f;
	private float currentWaitTime = 0f;

	private void Awake()
	{
		perceprton = GetComponent<Perceptron>();
		freeTransforms = new List<Transform>(spawnTransforms);
	}

	public void FreeSpace(Transform occupiedTransform)
	{
		if (!freeTransforms.Contains(occupiedTransform))
			freeTransforms.Add(occupiedTransform);
	}

	private void Start()
	{
		ResetWaitInterval();
	}

	private void ResetWaitInterval()
	{
		currentInterval = UnityEngine.Random.Range(minInterval, maxInterval);
	}

	private void Update()
	{
		currentWaitTime += Time.deltaTime;
		if (currentWaitTime >= currentInterval)
		{
			currentWaitTime = 0f;
			ResetWaitInterval();
			SpawnRandomItem();
		}
	}

	private void SpawnRandomItem()
	{
		Transform transformToOccupy = GetRandomSpawnPosition();
		if (transformToOccupy != null)
			SpawnItem(whackItem, transformToOccupy);
	}

	private void SpawnItem(WhackItem item, Transform transformToOccupy)
	{
		int materialIndex = UnityEngine.Random.Range(0, 2);
		int meshIndex = UnityEngine.Random.Range(0, 2);

		WhackItem spawnedItem = Instantiate(item, transformToOccupy.position, Quaternion.identity);
		spawnedItem.Init(this, transformToOccupy, avaliableMaterials[materialIndex], avaliableMeshes[meshIndex]);

		int outcome = (materialIndex == 0 && meshIndex == 0) ? 0 : 1;
		perceprton.SendData(materialIndex, meshIndex, outcome, transformToOccupy.position);

		Destroy(spawnedItem.gameObject, lifeTime);
		if (freeTransforms.Contains(transformToOccupy))
			freeTransforms.Remove(transformToOccupy);
	}

	private Transform GetRandomSpawnPosition()
	{
		if (freeTransforms.Count == 0)
			return null;
		return freeTransforms[UnityEngine.Random.Range(0, freeTransforms.Count)];
	}
}