﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantType : MonoBehaviour {

	public int plantType;
	
	public void setType(int a)
	{
		plantType = a;
	}

	public int getPlantType()
	{
		return plantType;
	}
		


}