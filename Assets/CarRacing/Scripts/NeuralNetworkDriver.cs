using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class NeuralNetworkDriver : MonoBehaviour {

    private ANN ann;
    [SerializeField] private float raycastDistance = 50;
    [SerializeField] private int epochs = 1000;
    [SerializeField] private LayerMask mask;

    private bool loadWeightsFromFile = true;
    private bool finishedTraining = false;
    private float trainingProgress = 0;
    private double sse = 0;
    private double lastSSE = 1; 
    private float translationInput;
    private float rotationInput;
    private string weightPath;

    public void SetWeightFile(TextAsset weights)
    {
        weightPath = Application.dataPath + "/CarRacing/SavedWeights/" + weights.name + ".txt";
        loadWeightsFromFile = true;
    }

    private void Start ()
    {
		ann = new ANN(5,2,1,10,0.5);
		if(loadWeightsFromFile)
        {
			LoadWeightsFromFile();
			finishedTraining = true;
        }
        else
        	StartCoroutine(LoadTrainingSet());
	}

    public void InitLearning()
    {
        loadWeightsFromFile = false;
    }

    private void LoadWeightsFromFile()
    {
        if (string.IsNullOrEmpty(weightPath))
        {
            weightPath = Application.dataPath + "/CarRacing/weights.txt";
        }
        StreamReader streamReader = File.OpenText(weightPath);

        if (File.Exists(weightPath))
        {
            string line = streamReader.ReadLine();
            ann.LoadWeights(line);
        }
    }


    private IEnumerator LoadTrainingSet()
    {
        string path = Application.dataPath + "/CarRacing/trainingData.txt";
        string line;
        if(File.Exists(path))
        {
            int lineCount = File.ReadAllLines(path).Length;
            StreamReader streamReader = File.OpenText(path);
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            for(int i = 0; i < epochs; i++)
            {
                sse = 0;
                streamReader.BaseStream.Position = 0;
                string currentWeights = ann.PrintWeights();
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] data = line.Split(',');
                    //if nothing to be learned ignore this line
                    float thisError = 0;
                    if (System.Convert.ToDouble(data[5]) != 0 && System.Convert.ToDouble(data[6]) != 0)
                    {
                        inputs.Clear();
                        outputs.Clear();
                        FillInputs(inputs, data);
                        FillOutputs(outputs, data);

                        List<double> calcOutputs = ann.Train(inputs, outputs);
                        thisError = ((Mathf.Pow((float)(outputs[0] - calcOutputs[0]), 2) +
                            Mathf.Pow((float)(outputs[1] - calcOutputs[1]), 2))) / 2.0f;
                    }
                    sse += thisError;
                }
                trainingProgress = (float)i / (float)epochs;
                sse /= lineCount;

                CorrectTraining(currentWeights);

                yield return null;
            }

        }
        finishedTraining = true;
        SaveWeightsToFile();
    }

    private static void FillInputs(List<double> inputs, string[] data)
    {
        inputs.Add(System.Convert.ToDouble(data[0]));
        inputs.Add(System.Convert.ToDouble(data[1]));
        inputs.Add(System.Convert.ToDouble(data[2]));
        inputs.Add(System.Convert.ToDouble(data[3]));
        inputs.Add(System.Convert.ToDouble(data[4]));
    }

    private void FillOutputs(List<double> outputs, string[] data)
    {
        double o1 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[5]));
        outputs.Add(o1);
        double o2 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[6]));
        outputs.Add(o2);
    }

    private void CorrectTraining(string currentWeights)
    {
        if (lastSSE < sse)
        {
            ann.LoadWeights(currentWeights);
            ann.alpha = Mathf.Clamp((float)ann.alpha - 0.001f, 0.01f, 0.9f);
        }
        else
        {
            ann.alpha = Mathf.Clamp((float)ann.alpha + 0.001f, 0.01f, 0.9f);
            lastSSE = sse;
        }
    }

    private void SaveWeightsToFile()
    {
        string path = Application.dataPath + "/CarRacing/weights.txt";
        StreamWriter streamWriter = File.CreateText(path);
        streamWriter.WriteLine (ann.PrintWeights());
        streamWriter.Close();
    }

    private void Update()
    {
        if (!finishedTraining)
            return;

        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

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

        inputs.Add(fDist);
        inputs.Add(rDist);
        inputs.Add(lDist);
        inputs.Add(r45Dist);
        inputs.Add(l45Dist);
        List<double> calcOutputs = ann.CalcOutput(inputs);
        translationInput = Map(-1, 1, 0, 1, (float)calcOutputs[0]);
        rotationInput = Map(-1, 1, 0, 1, (float)calcOutputs[1]);
    }


    private float Map (float newfrom, float newto, float origfrom,float origto, float value) 
    {
    	if (value <= origfrom)
        	return newfrom;
    	else if (value >= origto)
        	return newto;
    	return (newto - newfrom) * ((value - origfrom) / (origto - origfrom)) + newfrom;
	}

    private float Round(float value) 
    {   
        return (float)System.Math.Round(value, System.MidpointRounding.AwayFromZero) / 2.0f;
    }

    private void Raycast(Vector3 direction, ref float distance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, raycastDistance, mask))
        {
            distance = 1 - Round(hit.distance / raycastDistance);
        }
    }

    public float GetHorizontalInput()
    {
        return rotationInput;
    }

    public float GetVerticalInput()
    {
        return translationInput;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + lastSSE);
        GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + ann.alpha);
        GUI.Label(new Rect(25, 55, 250, 30), "Trained: " + trainingProgress);
    }
}
