using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RaceCreator))]
public class GameManager : MonoBehaviour
{
    public enum InitType { Learn, Race, Record}

    [SerializeField] private InitType initType;
    [SerializeField] private DriveDataRecorder playerDataRecorder;
    [SerializeField] private NeuralNetworkDriver annDriver;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] float simulationScale;
    private RaceCreator raceCreator;

    private void Awake()
    {
        Time.timeScale = simulationScale;
        raceCreator = GetComponent<RaceCreator>();
        switch (initType)
        {
            case InitType.Learn:
                StartLearning();
                break;
            case InitType.Race:
                StartRace();
                break;
            case InitType.Record:
                StartDataRecording();
                break;
            default:
                break;
        }
    }

    private void StartRace()
    {
        raceCreator.InitRace();
        playerDataRecorder.StopRecording();
        playerDataRecorder.GetComponent<CarController>().DecreaseSpeed(0.5f);
    }

    private void StartDataRecording()
    {
        playerDataRecorder.StartRecording();
    }

    private void StartLearning()
    {
        Transform spawn = raceCreator.GetRandomPosition();
        NeuralNetworkDriver driver = Instantiate(annDriver, spawn.position, spawn.rotation);
        driver.InitLearning();
        cameraFollow.SetTarget(driver.transform);
        playerDataRecorder.gameObject.SetActive(false);
    }
}
