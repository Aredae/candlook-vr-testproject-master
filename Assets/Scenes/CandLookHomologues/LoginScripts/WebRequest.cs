using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequest
{
    void Start()
    {
        // A correct website page.
        //StartCoroutine(GetGroups("http://localhost/server.php"));
    }

    public IEnumerator GetGroups(string uri, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    string jsonArray = webRequest.downloadHandler.text;
                    Debug.Log(jsonArray);
                    callback(jsonArray);
                    //call callback function to pass results
                    break;
            }
        }
    }
    
    public IEnumerator GetSubjectsFromGroup(string uri, int group_id, System.Action<string> callback)
    {
        WWWForm myform = new WWWForm();
        myform.AddField("group_id", group_id);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, myform))
        {
            // Request and wait for the desired page.
            webRequest.useHttpContinue = false;
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    string jsonArray = webRequest.downloadHandler.text;
                    Debug.Log(jsonArray);
                    callback(jsonArray);
                    //call callback function to pass results
                    break;
            }
        }
    }
    public IEnumerator InsertD2Results(string uri, ResultEntity resultsobject, System.Action<string> callback)
    {
        WWWForm myform = new WWWForm();
        myform.AddField("subject_id", resultsobject.subject_id);
        myform.AddField("notes", resultsobject.notes);
        myform.AddField("start_date", resultsobject.start_date.ToString());
        myform.AddField("tn", resultsobject.tn);
        myform.AddField("d2", resultsobject.d2);
        myform.AddField("e", resultsobject.e);
        myform.AddField("tn_e", resultsobject.tn_e);
        myform.AddField("e1", resultsobject.e1);
        myform.AddField("e2", resultsobject.e2);
        myform.AddField("e_percent", (int)resultsobject.e_percent);
        myform.AddField("cp", resultsobject.cp);
        myform.AddField("fr", resultsobject.fr);
        myform.AddField("ed", (int)resultsobject.ed);
        myform.AddField("raw_series_data", JsonConvert.SerializeObject(resultsobject.rawseriesdata));
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, myform))
        {
            // Request and wait for the desired page.
            webRequest.useHttpContinue = false;
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    string jsonArray = webRequest.downloadHandler.text;
                    Debug.Log(jsonArray);
                    callback(jsonArray);
                    //call callback function to pass results
                    break;
            }
        }
    }

    public IEnumerator getRecordingsFromUser(string uri, int userid, System.Action<string> callback)
    {
        WWWForm myform = new WWWForm();
        myform.AddField("subject_id", userid);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, myform))
        {
            // Request and wait for the desired page.
            webRequest.useHttpContinue = false;
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    string jsonArray = webRequest.downloadHandler.text;
                    Debug.Log(jsonArray);
                    callback(jsonArray);
                    //call callback function to pass results
                    break;
            }
        }
    }

    public IEnumerator getGazeDataForRecording(string uri, int userid, string timestamp, System.Action<string> callback)
    {
        WWWForm myform = new WWWForm();
        myform.AddField("subject_id", userid);
        myform.AddField("recordingtime", timestamp);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, myform))
        {
            // Request and wait for the desired page.
            webRequest.useHttpContinue = false;
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    string jsonArray = webRequest.downloadHandler.text;
                    Debug.Log(jsonArray);
                    callback(jsonArray);
                    //call callback function to pass results
                    break;
            }
        }
    }

}
