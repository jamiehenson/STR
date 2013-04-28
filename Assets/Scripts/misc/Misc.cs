using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Misc
{
	public static void CleanStatics()
	{
		// The list of classes which rely on statics - they should be refactored
		Commander.SetupStatics();
	}

	public static List<int> listOfUniqueNumbers(int size, int max) {
		List<int> list = new List<int>();

		for (int i=0; i<size; i++) {
			int newValue;

			do {
				newValue = UnityEngine.Random.Range(0,max);
			} while (!list.Contains(newValue));

			list.Add (newValue);
		}

		return list;
	}

	public static bool ArrayContains(object[] arr, object val) {
		foreach (object obj in arr)
			if (obj == val)
				return true;

		return false;
	}
}

