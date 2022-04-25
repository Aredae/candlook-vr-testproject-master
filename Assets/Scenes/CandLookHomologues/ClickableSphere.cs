using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableSphere : MonoBehaviour
{

    public GameObject sphere4;
    public double currentarcsecs = 90 * 3600;
    public double currentradius = 0.25;
    public double arcsecsscaleronwin = 2;
    public double arcsecsscaleronL = 2;
    public double arcsecmin = 20;
    public double arcsecmax = 3600;
    public const double startingarcsecs= 150000;

    // Start is called before the first frame update
    void Start()
    {
        currentradius = MoveAndScale(startingarcsecs, currentarcsecs, currentradius, sphere4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        currentradius =MoveAndScale(currentarcsecs/2, currentarcsecs, currentradius, sphere4);
    }

    public double MoveAndScale(double nextangle_arcsecs, double currentangle_arcsecs, double currentradius, GameObject spheretochange)
    {
        double nextanglerad = (System.Math.PI / 180) * (nextangle_arcsecs / 3600);
        double currentanglerad = (System.Math.PI / 180) * (currentangle_arcsecs / 3600);

        double scalefactor = System.Math.Sin(currentanglerad) / System.Math.Sin(nextanglerad);
        Debug.Log(scalefactor);
        double nextpos = (currentradius * scalefactor) - System.Math.Sin(nextanglerad);
        Debug.Log(nextpos);
        Vector3 pos = spheretochange.transform.position;
        pos.z = (float)nextpos;
        spheretochange.transform.position = pos;

        Vector3 scale = spheretochange.transform.localScale;
        scale *= (float)scalefactor;
        spheretochange.transform.localScale = scale;

        currentarcsecs = nextangle_arcsecs;

        return currentradius * scalefactor;
    }
}
