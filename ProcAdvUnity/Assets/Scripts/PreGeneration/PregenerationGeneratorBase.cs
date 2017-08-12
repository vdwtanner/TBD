using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PregenerationGeneratorBase {
	protected Random m_random;

    public abstract void Generate();
}
