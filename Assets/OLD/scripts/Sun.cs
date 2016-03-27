using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sun : MonoBehaviour {
	public float speed;
	public float maxIntensity;
	public float nightAmbientLight;
	public Color nightAmbientColor;
	public float dayAmbientLight;
	public Color dayAmbientColor;
	public int dayCounter=0;
	Light sun;
	public int time;
	float timer;
	int secondsInDay = 86400;
	private List<Light> ambientLighting;

	// Use this for initialization
	void Start () {
		ambientLighting = new List<Light>();
		sun = gameObject.GetComponent<Light>();
		GameObject[] gos = GameObject.FindGameObjectsWithTag("AmbientLighting");
		foreach(GameObject go in gos){
			Light light = go.GetComponent<Light>();
			if(light != null){
				ambientLighting.Add(light);
			}
		}
		timer=0f;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(speed*Time.deltaTime, 0, 0));
		timer+=speed*Time.deltaTime;
		if(timer>=360){
			timer-=360;
			dayCounter++;
		}
		if(timer==0){
			time = 0;
		}else{
			time = Mathf.RoundToInt((float)secondsInDay*(timer/360.0f));
		}
		if(timer > 270 || timer < 90){//9PM to 6AM
			sun.intensity=0;
		}else if(timer > 270){

		}else{
			sun.intensity = maxIntensity;
		}
		if(timer > 50 && timer < 115){
			RenderSettings.ambientIntensity = Mathf.Lerp(nightAmbientLight, dayAmbientLight, (timer-50)/65.0f);
			RenderSettings.ambientLight = Color.Lerp(nightAmbientColor, dayAmbientColor, (timer-50)/65.0f);
			/*foreach(Light light in ambientLighting){
				light.intensity = Mathf.Lerp(nightAmbientLight, dayAmbientLight, (timer-50)/65.0f);
				light.color = Color.Lerp(nightAmbientColor, dayAmbientColor, (timer-50)/65.0f);
			}*/

		}else if(timer >= 115 && timer < 115){

		}else if(timer > 245 && timer < 310){
			RenderSettings.ambientIntensity = Mathf.Lerp(dayAmbientLight, nightAmbientLight, (timer-250)/65.0f);
			RenderSettings.ambientLight = Color.Lerp(dayAmbientColor, nightAmbientColor, (timer-250)/65.0f);
			/*foreach(Light light in ambientLighting){
				light.intensity = Mathf.Lerp(dayAmbientLight, nightAmbientLight, (timer-250)/65.0f);
				light.color = Color.Lerp(dayAmbientColor, nightAmbientColor, (timer-250)/65.0f);
			}*/
		}
	}

	void OnGUI(){
		GUI.Box(new Rect(10,10,220,80), "");
		GUI.Label(new Rect(20,20,200,80),("Time: "+stringTime()));
		GUI.Label(new Rect(20,60,200,80), transform.eulerAngles.x.ToString());
		GUI.Label(new Rect(20,40,200,80), timer.ToString());
	}

	string stringTime(){
		int seconds = time;
		int min = time/(60);
		int hours = time/(60*60);
		return hours%24+":"+min%60+":"+seconds%60;
	}
}
