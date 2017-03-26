using UnityEngine;
using System.Collections;

public class AutoIntensity : MonoBehaviour {
    public Gradient nightDayColor;
    public float maxIntensity = 3f;
    public float minIntensity = 0f;
    public float minPoint = -0.2f;
    public float maxAmbient = 1f;
    public float minAmbient = 0f;
    public float minAmbientPoint = -0.2f;

    public Gradient nightDayFogColor;
    public AnimationCurve fogDensityCurve;
    public float fogScale = 1f;

    public float dayAtmosphereThickness = 0.4f;
    public float nightAtmosphereThickness = 0.87f;

    public Vector3 dayRotateSpeed;
    //public Vector3 nightRotateSpeed;
    
    //private float time;
    private int days;
    Light mainLight;
    Skybox sky;
    Material skyMat;

	// Use this for initialization
	void Start () {
        
        mainLight = GetComponent<Light>();
        skyMat = RenderSettings.skybox;
        //time = 7.0F * 60F * 60F;

    }
	
	// Update is called once per frame
	void Update () {
        float tRange = 1 - minPoint;
        float dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minPoint) / tRange);
        float i = ((maxIntensity - minIntensity) * dot) + minIntensity;
        mainLight.intensity = i;

        tRange = 1 - minAmbientPoint;
        dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
        i = ((maxAmbient - minAmbient) * dot) + minAmbient;
        RenderSettings.ambientIntensity = i;

        mainLight.color = nightDayColor.Evaluate(dot);
        RenderSettings.ambientLight = mainLight.color;

        RenderSettings.fogColor = nightDayFogColor.Evaluate(dot);
        RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;

        i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
        skyMat.SetFloat("_AtmosphereThickness", i);

        //time += Time.deltaTime * 40f;
       

        //if (dot > 0)
        //print(transform.rotation.x);

        //dayRotateSpeed.x * Time.deltaTime
        Quaternion target = Quaternion.Euler(new Vector3((GameObject.Find("UI").GetComponent<UIScript>().clockTime - 21600)/86400*360,0,0));
        transform.rotation = target;

        //print(target);
        //transform.rotation = dayRotateSpeed * Time.deltaTime * skySpeed;
        //transform.Rotate(dayRotateSpeed * Time.deltaTime * skySpeed);
        //else
        //    transform.Rotate(nightRotateSpeed * Time.deltaTime * skySpeed);
    }
}
