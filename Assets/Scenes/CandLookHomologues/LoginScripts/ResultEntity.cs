using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ResultEntity
{
    public int subject_id { get; set; }
    public string notes { get; set; }
    public DateTime start_date { get; set; }
    public int tn { get; set; }
    public int e { get; set; }
    public int tn_e { get; set; }
    public int e1 { get; set; }
    public int e2 { get; set; }
    public Single e_percent { get; set; }
    public int cp { get; set; }
    public int fr { get; set; }
    public Single ed { get; set; }
    public int d2 { get; set; }
    public List<RawSeriesDataDBHelper> rawseriesdata {get; set;}
}
