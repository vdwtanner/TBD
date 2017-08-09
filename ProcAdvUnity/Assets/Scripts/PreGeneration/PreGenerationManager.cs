using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGenerationManager : MonoBehaviour {
    public int seed = 0;

    protected BiomeGenerator m_biomeGenerator;
    protected CityStateGenerator m_cityStateGenerator;

    private static PreGenerationManager s_instance;

    void Awake() {
        s_instance = this;
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    static PreGenerationManager GetInstance() {
        return s_instance;
    }
}
