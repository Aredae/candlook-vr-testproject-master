using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Management;

public class InitalLoaderState : MonoBehaviour
{
    // Start is called before the first frame update
    public static InitalLoaderState instance;

    XRGeneralSettings InitialLoaderSettings;
    bool returning;
    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public XRGeneralSettings getSettings()
    {
        return this.InitialLoaderSettings;
    }

    public void setSettings()
    {
        this.InitialLoaderSettings = XRGeneralSettings.Instance;
    }

    public bool getReturning()
    {
        return this.returning;
    }

    public void setReturning(bool b)
    {
        this.returning = b;
    }
}
