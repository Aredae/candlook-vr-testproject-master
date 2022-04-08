using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayIcon : MonoBehaviour
{
    public Sprite icon;
    public Vector2 pos;
    public Camera cam;

    public static GameObject spawn(string icon_resource)
    {
        return spawn(icon_resource, new Vector2(0, -2f), Camera.main);
    }
    public static GameObject spawn(string icon_resource, Vector2 pos)
    {

        return spawn(icon_resource, pos, Camera.main);
    }
    public static GameObject spawn(string icon_resource, Camera cam)
    {

        return spawn(icon_resource, new Vector2(0, -2f), cam);
    }
    public static GameObject spawn(string icon_resource, Vector2 pos, Camera cam)
    {
        GameObject obj = new GameObject("Icon");
        DisplayIcon icon = obj.AddComponent<DisplayIcon>();
        icon.icon = Resources.Load<Sprite>(icon_resource);
        icon.pos = pos;
        icon.cam = cam;
        return obj;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.transform.SetParent(cam.transform);
        this.transform.localPosition = new Vector3(pos.x, pos.y, 5f);
        this.transform.localRotation = Quaternion.identity;
        SpriteRenderer sp = this.gameObject.AddComponent<SpriteRenderer>();
        sp.sprite = this.icon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
