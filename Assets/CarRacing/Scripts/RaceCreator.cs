using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceCreator : MonoBehaviour
{
    [SerializeField] private Transform[] startPositions;
    [SerializeField] private TextAsset[] driverData;
    [SerializeField] private NeuralNetworkDriver enemyCarPrefab;
    [SerializeField] private int numOfEnemies;

    public void InitRace()
    {
        numOfEnemies = Mathf.Min(numOfEnemies, startPositions.Length);
        for (int i = 0; i < numOfEnemies; i++)
        {
            NeuralNetworkDriver enemy = Instantiate(enemyCarPrefab, startPositions[i].position, startPositions[i].rotation);
            enemy.SetWeightFile(driverData[i % driverData.Length]);
        }
    }

    public Transform GetRandomPosition()
    {
        return startPositions[UnityEngine.Random.Range(0, startPositions.Length)];
    }
}
