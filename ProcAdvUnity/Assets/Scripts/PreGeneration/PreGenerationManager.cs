using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGenerationManager : MonoBehaviour {
    public int seed = 0;
	[Range(10, 25)]
	public int numResourceNodesPerCell = 10;
	//Width and height in generationCells
	[Range(1, 10)]
	public int size = 1;

	protected ResourceGenerator m_resourceGenerator;
    protected CityStateGenerator m_cityStateGenerator;

    public static PreGenerationManager Instance { get { return s_instance; } }

	protected static PreGenerationManager s_instance;

	void Awake() {
		s_instance = this;
		m_resourceGenerator = new ResourceGenerator(numResourceNodesPerCell);
		
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    static PreGenerationManager GetInstance() {
        return Instance;
    }

	void Generate()
	{
		m_resourceGenerator.Generate();
	}
}