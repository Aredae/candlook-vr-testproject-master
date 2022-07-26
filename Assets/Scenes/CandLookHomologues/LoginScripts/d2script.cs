using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Util;

public class d2script : MonoBehaviour
{
    //public SubjectInfo subjectinfo = practice.sinfo;
    private int fontsize;
    private int charSpacing = 1;
    private int dotHeight = 12;
    private int dotWidth = 3;
    private System.Tuple<int[], bool[]>[] series;
    //Random rnd = new Random();
    private int n_rep_row=0;
    private int[] row;
    private bool[] validation;
    public GameObject canvas;
    private int numcorrect;
    private int currentRow=0;
    private Vector3 initpos;
    public GameObject button1;
    public bool fail = false;
    public GameObject retry;
    public GameObject continuebutton;
    public GameObject finishedtext;
    public int currentSeries;
    public GameObject seriestext;
    public GameObject rowtext;
    private bool finishedseries;
    private float timer;
    private float roundtimer;
    public GameObject roundtimertext;
    private bool waitrunning;
    private bool done;
    public GameObject startButton;
    private bool gameStarted;
    private Scorer scorer;
    private float rowtimer;
    private bool allseriesfinished;
    private System.Action<object> _createSaveCallback;
    public GameObject ScoreResultPage;
    private bool alldone;
    public int numrows;
    public int numletters;
    public int waitduration;
    private ResultEntity resultsobject = new ResultEntity();
    private System.DateTime starttime;
    WebRequest webrequest = new WebRequest();
    //public GameObject xrrig;

    // Start is called before the first frame update
    void Start()
    {
        //currentSeries = 10;
        alldone = false;
        hideAllCanvasElements();
        startButton.SetActive(true);
        initpos = button1.transform.position;
        finishedseries = false;
        scorer = new Scorer();
        scorer.nextSeries();
        //startNewSeries();
        rowtimer = 0;
        numrows = 14;
        numletters = 9;
        waitduration = 0;
        finishedtext.SetActive(false);
        retry.SetActive(false);
        ScoreResultPage.SetActive(false);
        allseriesfinished = false;

        _createSaveCallback = (jsonArray) => {
            Debug.Log(jsonArray);
            Debug.Log("I Think this was saved?");
        };
        //SimpleSmoothMouseLook mouseController = xrrig.AddComponent<SimpleSmoothMouseLook>();
        //if (!UnityEngine.XR.XRSettings.isDeviceActive)
        //{
        //    SimpleSmoothMouseLook mouseController = xrrig.AddComponent<SimpleSmoothMouseLook>();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Escape) && (xrrig.GetComponent<SimpleSmoothMouseLook>() != null))
        //{
        //    Destroy(xrrig.GetComponent<SimpleSmoothMouseLook>());
        //}
        if (gameStarted && !alldone)
        {


            seriestext.GetComponent<Text>().text = "Series: " + currentSeries + "/12";
            rowtext.GetComponent<Text>().text = "Row: " + (currentRow + 1) + "/" + numrows;

            if (finishedseries)
            {
                finishedseries = false;
                startNewSeries();
                scorer.nextSeries();
                //startNewSeries();
                if (allseriesfinished)
                {
                    ScoreResult result = scorer.summarise(canvas);
                    showScore(result);
                    allseriesfinished = false;
                    alldone = true;
                }
            }
            else
            {
                roundtimertext.GetComponent<Text>().text = "Current Series Time: " + (int)roundtimer;
                if (roundtimer >= 30 && !waitrunning)
                {
                    scorer.ScoreRow(series[currentRow].Item1, rowtimer, canvas);
                    rowtimer = 0;

                    startNewSeries();
                    scorer.nextSeries();
                    roundtimer = 0;
                }
            }

            if (waitrunning)
            {
                timer += Time.deltaTime;
                int time = (int)System.Math.Floor(timer);
                finishedtext.GetComponent<Text>().text = "Waiting for series " + (currentSeries + 1) + "/12 :" + time;
            }
            if (!waitrunning)
            {
                rowtimer += Time.deltaTime;
                roundtimer += Time.deltaTime;
            }

            if (allseriesfinished)
            {
                ScoreResult result = scorer.summarise(canvas);
                showScore(result);
                allseriesfinished = false;
                alldone = true;
            }
        }

        /*
        var inputname = ScoreResultPage.transform.GetChild(13).GetComponent<InputField>();
        var se_name = new InputField.SubmitEvent();
        se_name.AddListener(SubmitName);
        inputname.onEndEdit = se_name;

        var inputnote = ScoreResultPage.transform.GetChild(13).GetComponent<InputField>();
        var se_note = new InputField.SubmitEvent();
        se_note.AddListener(SubmitNote);
        inputnote.onEndEdit = se_note;
        */

        //change what renders
    }

    public void SaveScore()
    {
        ScoreResult result = scorer.summarise(canvas);
        ScoreResultDBType resultdb = convertToCorrectType(result);

        resultsobject.subject_id = Subjectinfo.instance.GetId();
        resultsobject.notes = Subjectinfo.instance.GetNotes();
        resultsobject.start_date = starttime;
        resultsobject.tn = result.metrics.TN;
        resultsobject.e = result.metrics.E;
        resultsobject.tn_e = result.metrics.TNminusE;
        resultsobject.e1 = result.metrics.E1;
        resultsobject.e2 = result.metrics.E2;
        resultsobject.e_percent = result.metrics.Epercent;
        resultsobject.cp = result.metrics.CP;
        resultsobject.fr = result.metrics.FR;
        resultsobject.ed = result.metrics.ED;
        resultsobject.d2 = result.metrics.D2;
        resultsobject.rawseriesdata = resultdb.rawSeriesData;
        string jsontosave = JsonConvert.SerializeObject(resultsobject);
        Debug.Log(JsonConvert.SerializeObject(resultsobject));

        SaveResults(resultsobject);
        /*
        string jsonrawSeriesData = JsonConvert.SerializeObject(resultdb.rawSeriesData);
        Debug.Log(jsonrawSeriesData);
        */
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveResults(ResultEntity resultsobject)
    {
        StartCoroutine(webrequest.InsertD2Results("http://localhost/SaveNewD2Score.php", resultsobject, _createSaveCallback));
    }

    public void hideAllCanvasElements()
    {
        for (int i = 1; i < canvas.transform.childCount; i++)
        {
            canvas.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void showAllCanvasElements()
    {
        for (int i = 1; i < canvas.transform.childCount; i++)
        {
            canvas.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void showScore(ScoreResult result)
    {
        ScoreResultPage.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text += result.metrics.TN;
        ScoreResultPage.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text += result.metrics.D2;
        ScoreResultPage.transform.GetChild(4).transform.GetChild(0).GetComponent<Text>().text += result.metrics.E;
        ScoreResultPage.transform.GetChild(5).transform.GetChild(0).GetComponent<Text>().text += result.metrics.TNminusE;
        ScoreResultPage.transform.GetChild(6).transform.GetChild(0).GetComponent<Text>().text += result.metrics.E1;
        ScoreResultPage.transform.GetChild(7).transform.GetChild(0).GetComponent<Text>().text += result.metrics.E2;
        ScoreResultPage.transform.GetChild(8).transform.GetChild(0).GetComponent<Text>().text += result.metrics.Epercent;
        ScoreResultPage.transform.GetChild(9).transform.GetChild(0).GetComponent<Text>().text += result.metrics.CP;
        ScoreResultPage.transform.GetChild(10).transform.GetChild(0).GetComponent<Text>().text += result.metrics.FR;
        ScoreResultPage.transform.GetChild(11).transform.GetChild(0).GetComponent<Text>().text += result.metrics.ED;
        ScoreResultPage.transform.GetChild(13).GetComponent<Text>().text += "Are"; //System.Environment.NewLine + Subjectinfo.instance.GetName();
        //ScoreResultPage.transform.GetChild(14).GetComponent<Text>().text += "Note"; // System.Environment.NewLine + Subjectinfo.instance.GetNotes();
        ScoreResultPage.SetActive(true);
        canvas.SetActive(false);

        //
        //ScoreResultPage.transform.GetChild(3)
    }


    public void start()
    {
        gameStarted = true;
        starttime = System.DateTime.Now;
        showAllCanvasElements();
        finishedtext.SetActive(false);
        retry.SetActive(false);
        startButton.SetActive(false);
        startNewSeries();
    }

    IEnumerator wait()
    {
        waitrunning = true;
        if(currentSeries != 0)
        {
            yield return new WaitForSeconds(waitduration);
        }
        else
        {
            yield return new WaitForSeconds(0);
        }
        currentSeries++;
        series = generateSeries();
        row = series[currentRow].Item1;
        validation = series[currentRow].Item2;
        numcorrect = 0;
        n_rep_row = 0;
        fail = false;
        timer = 0f;
        continuebutton.SetActive(true);
        finishedtext.SetActive(false);
        roundtimer = 0;
        roundtimertext.SetActive(true);
        for (int i = 0; i<numletters; i++) {
            if (row.Length > i)
            {
                int x = row[i];
                addDotsAndChangeLetter(x, i);
            }
        }
        showAllLetters();
        waitrunning = false;
    }

    void startNewSeries()
    {
        roundtimertext.SetActive(false);
        currentRow = 0;
        finishedtext.SetActive(true);
        continuebutton.SetActive(false);
        hideAllLetters();
        if (!waitrunning && currentSeries != 12)
        {
            StartCoroutine(wait());
        }
        else
        {
            hideAllLetters();
            continuebutton.SetActive(false);
            roundtimertext.SetActive(false);
            seriestext.SetActive(false);
            rowtext.SetActive(false);
            finishedtext.GetComponent<Text>().text = "D2 Test completed, good work!";
            finishedtext.SetActive(true);
            currentRow--;
            allseriesfinished = true;
        }
    }

    public void switchAllLetters()
    {
        for(int i =0; i<numletters; i++)
        {
            canvas.transform.GetChild(i + 1).gameObject.SetActive(!canvas.transform.GetChild(i+1).gameObject.activeInHierarchy);
        }
    }

    public void hideAllLetters()
    {
        for (int i = 0; i < numletters; i++)
        {
            canvas.transform.GetChild(i + 1).gameObject.SetActive(false);
        }
    }

    public void showAllLetters()
    {
        for (int i = 0; i < numletters; i++)
        {
            canvas.transform.GetChild(i + 1).gameObject.SetActive(true);
        }
    }

    public void next()
    {
        //if(numcorrect == validation.Count(bl => bl == true) && !fail)
        //{
            scorer.ScoreRow(series[currentRow].Item1, rowtimer, canvas);
            currentRow++;
            rowtimer = 0;
            if (currentRow < series.Length)
            {
                row = series[currentRow].Item1;
                validation = series[currentRow].Item2;
                for (int i = 0; i < numletters; i++)
                {
                    if (row.Length > i)
                    {
                        int x = row[i];
                        addDotsAndChangeLetter(x, i);
                    }
                }
                numcorrect = 0;
                n_rep_row = 0;
                fail = false;
            }
            else
            {
                finishedseries = true;
            }
            
        //}
        //else
       // {
          //  retry.SetActive(true);
          //  continuebutton.SetActive(false);
        //}
    }

    public void click()
    {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        int x = button.GetComponent<ButtonValue>().value;
        bool toggle = button.GetComponent<ButtonValue>().toggle;
        int nbot = (x >> 4) & 0xF;
        int ntop = (x >> 8) & 0xF;
        int letter = x & 1;
        if (toggle)
        {
            button.transform.GetChild(3).gameObject.SetActive(false);
            if (nbot + ntop == 2 && letter == 1)
            {
                button.GetComponent<ButtonValue>().error = true;
                numcorrect--;
            }
            else
            {
                button.GetComponent<ButtonValue>().error = false;
                fail = false;
            }
        }
        else
        {
            button.transform.GetChild(3).gameObject.SetActive(true);
            if (nbot + ntop == 2 && letter == 1)
            {
                numcorrect++;
                button.GetComponent<ButtonValue>().error = false;
            }
            else
            {
                button.GetComponent<ButtonValue>().error = true;
                fail = true;
            }

        }
        button.GetComponent<ButtonValue>().toggle = !button.GetComponent<ButtonValue>().toggle;
    }

    public void restart()
    {
        SceneManager.LoadScene("D2Test");
    }

    void addDotsAndChangeLetter(int x, int n_rep_row)
    {

        GameObject button = canvas.transform.GetChild(n_rep_row+1).gameObject;
        if (currentRow == 0 && currentSeries == 0)
        {
            button.transform.position = new Vector3(button.transform.position.x + n_rep_row * 0.02f, button.transform.position.y, button.transform.position.z);
        }
        button.transform.GetChild(3).gameObject.SetActive(false);
        button.GetComponent<ButtonValue>().toggle = false;
        button.GetComponent<ButtonValue>().value = x;
        Text texttop = button.transform.GetChild(1).GetComponent<Text>();
        texttop.text = "";
        Text textbot = button.transform.GetChild(2).GetComponent<Text>();
        textbot.text = "";
        int ntop = (x >> 8) & 0xF;
        for(int i=0; i<ntop; i++)
        {
            texttop.text = texttop.text + "|\u200a\u200a";
        }
        int nbot = (x >> 4) & 0xF;
        for (int i = 0; i < nbot; i++){
            textbot.text = textbot.text + "|\u200a\u200a";
        }
        int letter = x & 1;
        if(letter == 1)
        {
            Text textchar = button.transform.GetChild(0).GetComponent<Text>();
            textchar.text = "d";
        }
        else
        {
            Text textchar = button.transform.GetChild(0).GetComponent<Text>();
            textchar.text = "p";
        }

        if(letter ==1 && nbot+ntop == 2)
        {
            button.GetComponent<ButtonValue>().error = true;
        }
        else
        {
            button.GetComponent<ButtonValue>().error = false;
        }

        GameObject toptext = button.transform.GetChild(1).gameObject;
        GameObject bottext = button.transform.GetChild(2).gameObject;
        toptext.transform.position = new Vector3(button.transform.position.x+0.0125f, button.transform.position.y + 0.25f, button.transform.position.z);
        bottext.transform.position = new Vector3(button.transform.position.x+0.0125f, button.transform.position.y - 0.25f, button.transform.position.z);
        //button.GetComponentInChildren<Text>().text = "d";
        //if(x)
        

    }

    public void CreateButton(Transform panel, Vector3 position, Vector2 size, UnityEngine.Events.UnityAction method)
    {
        GameObject button = new GameObject();
        button.transform.parent = panel;
        button.AddComponent<RectTransform>();
        button.AddComponent<Button>();
        button.transform.position = position;
        button.GetComponent<RectTransform>().sizeDelta = size;
        button.GetComponent<Button>().onClick.AddListener(method);
    }

    public System.Tuple<int[], bool[]> generateRow(int target)
    {
        int[] row = new int[numletters];
        bool[] expectedAnswer = new bool[numletters];

        for(int i = 0; i<numletters; i++)
        {
            int ntop;
            int nbot;
            int chr;

            do
            {
                ntop = Random.Range(0, 3);
                nbot = Random.Range(0, 3);
                chr = Random.Range(0, 2);
            } while (ntop + nbot == 2 && chr == 1);
            row[i] = (ntop << 8) | (nbot << 4) | chr;
        }
        for(int i =0; i<target; i++)
        {
            int[] d2s = { 0x21, 0x111, 0x0201 };
            row[i] = d2s[Random.Range(0, 3)];
            expectedAnswer[i] = true;
        }

        //shuffle
        for(int i=row.Length; i>0; i--)
        {
            System.Random random = new System.Random();
            int j = (int)System.Math.Floor(random.NextDouble() * i);
            int tmpRow = row[i - 1];
            bool tmpAns = expectedAnswer[i - 1];
            row[i - 1] = row[j];
            expectedAnswer[i - 1] = expectedAnswer[j];
            row[j] = tmpRow;
            expectedAnswer[j] = tmpAns;
        }
        return System.Tuple.Create(row, expectedAnswer);
    }

    public System.Tuple<int[], bool[]>[] generateSeries()
    {
        int[] targets = new int[numrows];
        int rn = Random.Range(-2, 3);
        targets[0] = 4 + rn; // 1,2 = 4 + 4
        targets[1] = 4 + rn; // 1,2 = 4 + 4
        targets[2] = 4 - rn; // 3,4 = 4 + 4
        targets[3] = 4 - rn; // 3,4 = 4 + 4
        targets[4] = 2; // 5,6 = 2 + 4
        targets[5] = 4;
        rn = Random.Range(-1, 2);
        targets[6] = 5+rn;
        targets[7] = 5 - rn;
        targets[8] = 4 + rn; // 9,10 = 4 + 4
        targets[9] = 4 - rn; // 9,10 = 4 + 4
        rn = Random.Range(-1, 2);
        targets[8] += 2 + rn;
        targets[9] += 2 - rn;
        targets[10] = 4 + rn + 2; // 11,12 = 4 + 2
        targets[11] = 4 - rn + 2; // 11,12 = 4 + 2
        targets[12] = 4; // 13,14 = 4 + 5
        targets[13] = 9; // 13,14 = 4 + 5
        System.Tuple<int[], bool[]>[] results;
        results = targets.Select(generateRow).ToArray();
        return results;
    }

    public int[] getError()
    {
        int[] errors = new int[numletters];
        for (int i = 0; i < numletters; i++)
        {
            GameObject button = canvas.transform.GetChild(i + 1).gameObject;
            errors[i] = button.GetComponent<ButtonValue>().error ? 1 : 0;
        }
        return errors;
    }

    public void showError()
    {
        for (int i = 0; i < numletters; i++)
        {
            GameObject button = canvas.transform.GetChild(i + 1).gameObject;
            if (button.GetComponent<ButtonValue>().error)
            {
                Image errorbox = button.transform.GetChild(0).gameObject.GetComponent<Image>();
                var tempColor = errorbox.color;
                tempColor.a = 1f;
                errorbox.color = tempColor;
            }
        }
    }

    public ScoreResultDBType convertToCorrectType(ScoreResult result)
    {
        List<RawSeriesDataDBHelper> rawSeriesDataList = new List<RawSeriesDataDBHelper>();

        foreach(RawSeriesData rs in result.rawSeriesData)
        {
            List<RowDataDBHelper> rowdatalist = new List<RowDataDBHelper>();
            foreach(RowData rd in rs.rowData)
            {
                int[] row = new int[rd.RawRow.Length];
                for(int i=0; i<rd.RawRow.Length; i++)
                {
                    row[i] = rd.RawRow[i].Item1;
                }
                RowDataDBHelper rowdata = new RowDataDBHelper(row, rd.WasCorrect, rd.Duration);
                rowdatalist.Add(rowdata);
            }
            RawSeriesDataDBHelper rsddbh = new RawSeriesDataDBHelper(rowdatalist);
            rawSeriesDataList.Add(rsddbh);
        }

        return new ScoreResultDBType(rawSeriesDataList, result.metrics);
    }
    
}

public class PerSeriesType{

    public int TN { get; set; }

    public int D2 { get; set; }

    public int E { get; set; }

    public int E1 { get; set; }

    public int E2 { get; set; }

    public PerSeriesType(int tn, int d2, int e, int e1, int e2)
    {
        this.TN = tn;
        this.D2 = d2;
        this.E = e;
        this.E1 = e1;
        this.E2 = e2;
    }

}


public class Scorer
{
    //GameObject canvas;
    List<RawSeriesData> seriesData;

    public Scorer()
    {
        //this.canvas = canvas;
        this.seriesData = new List<RawSeriesData>();
    }
    public void Push<T>(T[] source, T value)
    {
        var index = System.Array.IndexOf(source, default(T));

        if (index != -1)
        {
            source[index] = value;
        }
    }

    public void ScoreRow(int[] row, float duration, GameObject canvas)
    {
        bool[] marked = new bool[9];
        bool[] correct = new bool[9];
        for (int i = 0; i < 9; i++)
        {
            GameObject button = canvas.transform.GetChild(i + 1).gameObject;
            correct[i] = !button.GetComponent<ButtonValue>().error;
            marked[i] = button.GetComponent<ButtonValue>().toggle;
            
        }
        System.Tuple<int, bool>[] rowwithmark = new System.Tuple<int, bool>[9];
        for (int i = 0; i<9; i++)
        {
            rowwithmark[i] = System.Tuple.Create(row[i], marked[i]);
        }
        //System.Tuple<int, bool>[] rowwithmark;
        //int len = seriesData.Length;
        RawSeriesData rd = this.seriesData[seriesData.Count-1];
        List<RowData> rowdata = rd.rowData;
        rowdata.Add(new RowData(rowwithmark, correct, duration));

        //Push(seriesData[seriesData.Length - 1].RowData, new RowData(row, correct, duration));
    }

    public void nextSeries()
    {
        this.seriesData.Add(new RawSeriesData(new List<RowData>()));
        //Push(seriesData, new RawSeriesData(new RowData[0]));
    }

    public ScoreResult summarise(GameObject canvas)
    {
        List<PerSeriesType> perSeries = new List<PerSeriesType>();
        foreach (var series in this.seriesData)
        {
            var tn = 0;
            var d2 = 0;
            var e1 = 0;
            var e2 = 0;
            foreach (var row in series.rowData)
            {
                tn += row.RawRow.Length;
                foreach (var x in row.RawRow)
                {
                    int nbot = (x.Item1 >> 4) & 0xF;
                    int ntop = (x.Item1 >> 8) & 0xF;
                    int letter = x.Item1 & 1;

                    bool marked = x.Item2;
                    //find out if marked, TRY TO FIND A WAY TO NOT SAVE THE MARK IN THE RAW ROW

                    if (nbot + ntop == 2 && letter == 1) // find out if d2
                    {
                        d2++;
                        if (!marked) // if d2 and not marked
                        {
                            e1 ++;
                        }
                    } else if (marked) // if not d2 and marked
                    {
                        e2 ++;
                    }
                }
            }
            perSeries.Add(new PerSeriesType(tn, d2, (e1 + e2), e1, e2));

        }

        int tn2 = perSeries.Aggregate(0, (s, x) => s + x.TN);
        int d22 = perSeries.Aggregate(0, (s, x) => s + x.D2);
        int e22 = perSeries.Aggregate(0, (s, x) => s + x.E);
        int tne2 = 0; //placeholder
        int e12 = perSeries.Aggregate(0, (s, x) => s + x.E1);
        int e222 = perSeries.Aggregate(0, (s, x) => s + x.E2);
        float ep = 0; //placeholder
        int cp = 0; //placeholder
        int fr = perSeries.Aggregate(0, (s, x) => System.Math.Max(s, x.TN)) - perSeries.Aggregate(int.MaxValue, (s, x) => System.Math.Min(s, x.TN));
        float ed = 0; //placeholder


        Metrics metrics = new Metrics(tn2, d22, e22, tne2, e12, e222, ep, cp, fr, ed);

        metrics.TNminusE = metrics.TN - metrics.E;
        metrics.Epercent = 100f * ((float)metrics.E / (float)metrics.TN);
        metrics.CP = metrics.D2 - metrics.E;
        for (int i = 0; i < 4; i++)
        {
            metrics.ED += perSeries[perSeries.Count - i - 1].E - perSeries[i].E;
        }
        metrics.ED /= 4;

        return new ScoreResult(this.seriesData, metrics);
    }
}

public class Metrics
{
    public int TN { get; set; } // total number of letters seen
    public int D2 { get; set; }  // total number of d2 seen
    public int E { get; set; }  // total number of errors
    public int TNminusE { get; set; }   // TN minus E, i.e. total corect
    public int E1 { get; set; }  // number of d2 not marked
    public int E2 { get; set; }  // number of non-d2 marked
    public System.Single Epercent { get; set; }  // E / TN as percentage
    public int CP { get; set; }  // concentration performance: total number of correctly marked d2 minus E2
    public int FR { get; set; }  // fluctuation rate: max. per-series TN minus min. per-series TN
    public System.Single ED { get; set; }  // avg. E in last 4 series minus avg. E in first 4 series

    public Metrics(int tn, int d2, int e, int tnminuse, int e1, int e2, System.Single epercent, int cp, int fr, System.Single ed)
    {
        this.TN = tn;
        this.D2 = d2;
        this.E = e;
        this.TNminusE = tnminuse;
        this.E1 = e1;
        this.E2 = e2;
        this.Epercent = epercent;
        this.CP = cp;
        this.FR = fr;
        this.ED = ed;
    }
}

public class ScoreResult
{
    public List<RawSeriesData> rawSeriesData { get; set; }
    public Metrics metrics { get; set; }

    public ScoreResult( List<RawSeriesData> rsd, Metrics m)
    {
        this.rawSeriesData = rsd;
        this.metrics = m;
    }

}

public class RawSeriesData
{
    public List<RowData> rowData { get; set; }
    public RawSeriesData(List<RowData> data)
    {
        this.rowData = data;
    }

    //public List<RowData> RowData { get; set; }
}

public class RowData
{
    public System.Tuple<int, bool>[] RawRow { get; set; }

    public bool[] WasCorrect { get; set; }

    public float Duration { get; set; }

    public RowData(System.Tuple<int, bool>[] row, bool[] correct, float duration)
    {
        this.RawRow = row;
        this.WasCorrect = correct;
        this.Duration = duration;
    }
}

public class ScoreResultDBType
{
    public List<RawSeriesDataDBHelper> rawSeriesData { get; set; }
    public Metrics metrics { get; set; }

    public ScoreResultDBType(List<RawSeriesDataDBHelper> rsd, Metrics m)
    {
        this.rawSeriesData = rsd;
        this.metrics = m;
    }

}

public class RawSeriesDataDBHelper
{
    public List<RowDataDBHelper> rowData { get; set; }
    public RawSeriesDataDBHelper(List<RowDataDBHelper> data)
    {
        this.rowData = data;
    }

    //public List<RowData> RowData { get; set; }
}
public class RowDataDBHelper
{
    public int[] RawRow { get; set; }

    public bool[] WasCorrect { get; set; }

    public float Duration { get; set; }

    public RowDataDBHelper(int[] row, bool[] correct, float duration)
    {
        this.RawRow = row;
        this.WasCorrect = correct;
        this.Duration = duration;
    }
}