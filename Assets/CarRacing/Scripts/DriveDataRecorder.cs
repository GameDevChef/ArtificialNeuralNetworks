using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[RequireComponent(typeof(CarController))]
public class DriveDataRecorder : MonoBehaviour {

    [SerializeField] private bool recordData;
    [SerializeField] private float raycastDistance = 50.0f;
    [SerializeField] private LayerMask mask;
    private List<string> collectedTrainingData = new List<string>();
    private StreamWriter streamWriter;
    private CarController carController;

    private void Awake()
    {
        carController = GetComponent<CarController>();
    }

    private void Start()
    {
        if (!recordData)
            return;
        string path = Application.dataPath + "/CarRacing/trainingData.txt";
    	streamWriter = File.CreateText(path);
    }

    private void Update()
    {
        if (!recordData)
            return;
        float translationInput = carController.GetVerticalInput();
        float rotationInput = carController.GetHorizontalInput();

        DrawDebugRay();

        float fDist = 0;
        float rDist = 0;
        float lDist = 0;
        float r45Dist = 0;
        float l45Dist = 0;

        
        Raycast(this.transform.forward, ref fDist);
        Raycast(this.transform.right, ref rDist);
        Raycast(-this.transform.right, ref lDist);
        Raycast(Quaternion.AngleAxis(-45, Vector3.up) * this.transform.right, ref r45Dist);
        Raycast(Quaternion.AngleAxis(45, Vector3.up) * -this.transform.right, ref l45Dist);

        string traningData = fDist + "," + rDist + "," + lDist + "," +
                      r45Dist + "," + l45Dist + "," +
                      Round(translationInput) + "," + Round(rotationInput);

        if (translationInput != 0 && rotationInput != 0)
        {
            if (!collectedTrainingData.Contains(traningData))
            {
                collectedTrainingData.Add(traningData);
            }
        }
    }

	private void Raycast(Vector3 direction, ref float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, raycastDistance, mask))
        {
            distance = 1 - Round(hit.distance / raycastDistance);
        }
    }

    private void DrawDebugRay()
    {
        Debug.DrawRay(transform.position, this.transform.forward * raycastDistance, Color.red);
        Debug.DrawRay(transform.position, this.transform.right * raycastDistance, Color.red);
        Debug.DrawRay(transform.position, -this.transform.right * raycastDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(45, Vector3.up) * -this.transform.right * raycastDistance, Color.green);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * this.transform.right * raycastDistance, Color.green);
    }

    private void OnApplicationQuit()
    {
        if (!recordData)
            return;
        foreach (string td in collectedTrainingData)
        {
            streamWriter.WriteLine(td);
        }
        streamWriter.Close();
    }

    private float Round(float value)
    {
        return (float)System.Math.Round(value, System.MidpointRounding.AwayFromZero) / 2.0f;
    }

    public void StartRecording()
    {
        recordData = true;
    }

    public void StopRecording()
    {
        recordData = false;
    }
}
