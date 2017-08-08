using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager : MonoBehaviour {
    public int seed = 0;

    protected BiomeGenerator m_biomeGenerator;
    protected CityStateGenerator m_cityStateGenerator;
    protected TerrainGenerator m_terrainGenerator;

    private static GenerationManager s_instance;

    void Awake() {
        s_instance = this;
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    static GenerationManager GetInstance() {
        return s_instance;
    }
}
