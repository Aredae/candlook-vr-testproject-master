using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayController : MonoBehaviour
{
    WebRequest webrequest = new WebRequest();
    public GameObject usertext;
    private System.Action<object> _createReplayCallback;
    public GameObject initbutton;
    // Start is called before the first frame update
    void Start()
    {
        usertext.transform.GetComponent<Text>().text += " " + Subjectinfo.instance.GetName();
        

        _createReplayCallback = (jsonArray) => {
            //What to do when the data has been retrieved here

            //Separate entries and store in array
            
            //Generate button for each entry
            //if(gamename == something, create button for that game and send data with that button)
        };

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetETResults(int userid, string gamename, int version, System.DateTime starttime)
    {
        //StartCoroutine(webrequest.GetResultsFromSubject("http://158.37.193.176/SaveNewD2Score.php", resultsobject, _createSaveCallback));
    }

    public void CreateButton(Transform panel, Vector3 position, Vector2 size, UnityEngine.Events.UnityAction method)
    {
        GameObject button = new GameObject();
        button.transform.parent = panel;
        button.AddComponent<RectTransform>();
        button.AddComponent<Button>();
        button.transform.position = position;
        button.GetComponent<RectTransform>().sizeDelta.Set(size.x, size.y);
        button.GetComponent<Button>().onClick.AddListener(method);
    }

}
