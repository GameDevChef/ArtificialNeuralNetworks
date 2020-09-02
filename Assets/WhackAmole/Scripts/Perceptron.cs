using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainingData
{
	public double[] input;
	public double output;
}

public class Perceptron : MonoBehaviour
{
	List<TrainingData> trainingSet = new List<TrainingData>();
	double[] weights = { 0, 0 };
	double bias = 0;

	public Hammer agent;


	private void Start()
	{
		InitialiseWeights();
	}

	private void InitialiseWeights()
	{
		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = Random.Range(-1.0f, 1.0f);
		}
		bias = Random.Range(-1.0f, 1.0f);
	}

	public void SendData(double input1, double input2, double output, Vector3 position)
	{
		Train();
		ReactToSentData(input1, input2, position);
		AddNewData(input1, input2, output);
	}

	private void ReactToSentData(double input1, double input2, Vector3 position)
	{
		double result = CalcOutput(new double[] { input1, input2 });
		agent.MoveObject(position, result == 0);
	}

	private void AddNewData(double input1, double input2, double output)
	{
		var set = new TrainingData
		{
			input = new double[2] { input1, input2 },
			output = output
		};
		trainingSet.Add(set);
	}

	private double DotProductBias(double[] v1, double[] v2)
	{
		if (v1 == null || v2 == null)
			return -1;

		if (v1.Length != v2.Length)
			return -1;

		double d = 0;
		for (int x = 0; x < v1.Length; x++)
		{
			d += v1[x] * v2[x];
		}

		d += bias;

		return d;
	}

	private void Train()
	{
		for (int i = 0; i < trainingSet.Count; i++)
		{
			UpdateWeights(trainingSet[i]);
		}
	}

	private void UpdateWeights(TrainingData trainingData)
	{
		double error = trainingData.output - CalcOutput(trainingData.input);
		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = weights[i] + error * trainingData.input[i];
		}
		bias += error;
	}

	private double CalcOutput(double[] input)
	{
		return (ActivationFunction(DotProductBias(weights, input)));
	}

	private double ActivationFunction(double dp)
	{
		if (dp > 0)
			return 1;
		return 0;
	}
}