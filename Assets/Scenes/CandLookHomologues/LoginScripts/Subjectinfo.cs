using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subjectinfo : MonoBehaviour
{
    public static Subjectinfo instance;

    Util.Subject s;
    string notes;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void SetSubjectInInfo(Util.Subject s)
    {
        this.s = s;
    }

    public void SetNotes(string n)
    {
        this.notes = n;
    }

    public string GetName()
    {
        return this.s.name;
    }

    public string GetNotes()
    {
        return this.notes;
    }

    public int GetId()
    {
        return this.s.subject_id;
    }
}
