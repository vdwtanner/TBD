using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random : System.Random {
	///<summary>
	///A random float in the range [0..1]
	///</summary>
	public float value { get { return (float)Sample(); } private set { } }

	public Random() : base(){ }

	public Random(int seed) : base(seed) { }

	/// <summary>
	/// Returns a random float in range [min..max]. Max must be greater than min.
	/// </summary>
	/// <param name="min">The minimum value</param>
	/// <param name="max">The maximum value. Must be greater than min</param>
	/// <returns>Returns a random float in range [min..max]. Max must be greater than min.</returns>
	public float Range(float min, float max)
	{
		Debug.Assert(max > min, "min value was greater than max value");
		float dif = max - min;
		return (float)Sample() * dif + min;
	}

}
