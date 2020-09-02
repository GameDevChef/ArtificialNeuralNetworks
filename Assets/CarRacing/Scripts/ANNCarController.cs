using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANNCarController : CarController
{
	private NeuralNetworkDriver ann;

	protected override void Awake()
	{
		base.Awake();
		ann = GetComponent<NeuralNetworkDriver>();
	}

	protected override void GetInput()
	{
		verticalInput = ann.GetVerticalInput();
		horizontalInput = ann.GetHorizontalInput();
	}
}
