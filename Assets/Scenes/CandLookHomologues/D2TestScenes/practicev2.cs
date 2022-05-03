using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Util;
using TMPro;
using Varjo.XR;
//using Npgsql;

public class practicev2 : MonoBehaviour
{
    public GameObject xrrig;
    private int fontsize;
    private int charSpacing = 1;
    private int dotHeight = 12;
    private int dotWidth = 3;
    private System.Tuple<int[], bool[]>[] series;
    //Random rnd = new Random();
    private bool completed;
    private int n_rep_row = 0;
    private int n_rep_tutorial_row = 0;
    private int[] row;
    private bool[] validation;
    public GameObject canvas;
    private int numcorrect;
    private int currentseries = 1;
    private Vector3 initpos;
    public GameObject button1;
    public bool fail = false;
    public GameObject retry;
    public GameObject continuebutton;
    public GameObject finishedtext;
    private int rowCount =0;
    private int maxRowCount = 6;
    private float errorRate;
    private float maxErrorRate =1.5f;
    private int minRowCount = 1;
    private System.Tuple<int[], bool[]> row1;
    private System.Tuple<int[], bool[]> tutorialRow;
    private int numerrors;
    //public GameObject xrrig;
    public GameObject goToTest;
    private bool tutorialfinished;
    public GameObject tutorialTextTop;
    public GameObject tutorialTextBottom;
    private int tutorialStep;
    private bool currentTutorialStepDone;
    private bool fadingin;
    private bool fadingout;
    private bool showtutorialline;
    private bool currentdone;
    private bool fadingindone;
    private bool fadingoutdone;
    private bool waitdone;
    private bool buttonsAvailable;
    private int numletters;
    private int waitduration;
    private int numrows;
    //private List<Group> grouplist;

    // Start is called before the first frame update
    void Start()
    {
        numletters = 9;
        numrows = 14;
        tutorialfinished = false;
        initpos = button1.transform.position;
        completed = false;
        series = generateSeries();
        row = series[currentseries].Item1;
        validation = series[currentseries].Item2;
        row1 = generateRow(4 + Random.Range(-2, 3));
        currentTutorialStepDone = false;
        tutorialStep = 1;
        tutorialTextTop.GetComponent<Text>().color = new Color(tutorialTextTop.GetComponent<Text>().color.r, tutorialTextTop.GetComponent<Text>().color.g, tutorialTextTop.GetComponent<Text>().color.b, 0f);
        tutorialTextBottom.GetComponent<Text>().color = new Color(tutorialTextBottom.GetComponent<Text>().color.r, tutorialTextBottom.GetComponent<Text>().color.g, tutorialTextBottom.GetComponent<Text>().color.b, 0f);
        fadingin = false;
        fadingindone = false;
        fadingout = false;
        fadingoutdone = false;
        finishedtext.SetActive(false);
        retry.SetActive(false);
        goToTest.SetActive(false);
        hideAllCanvasElements();
        tutorialTextTop.SetActive(true);
        tutorialTextBottom.SetActive(true);
        tutorialRow = generateRow(4 + Random.Range(-2, 3));
        buttonsAvailable = false;
        //canvas.SetActive(false);
        //clear default options

        if (!UnityEngine.XR.XRSettings.isDeviceActive)
        {
            SimpleSmoothMouseLook mouseController = xrrig.AddComponent<SimpleSmoothMouseLook>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Escape) && (xrrig.GetComponent<SimpleSmoothMouseLook>() != null))
        //{
        //    Destroy(xrrig.GetComponent<SimpleSmoothMouseLook>());
        //}
            if (tutorialfinished)
            {
                if (row1.Item1.Length > n_rep_row)
                {
                    int x = row1.Item1[n_rep_row];
                    addDotsAndChangeLetter(x, n_rep_row);
                    n_rep_row++;

                }
            }
            else
            {
                tutorial();
                if (showtutorialline && tutorialRow.Item1.Length > n_rep_tutorial_row)
                {
                    int y = tutorialRow.Item1[n_rep_tutorial_row];
                    addDotsAndChangeLetter(y, n_rep_tutorial_row);
                    n_rep_tutorial_row++;
                }
            }
    }

    public IEnumerator waitDuringTutorial(int time)
    {
        yield return new WaitForSeconds(time);
        waitdone = true;
    }

    public void tutorial() {
        switch (tutorialStep)
        {
            case 1:
                tutorialTextTop.GetComponent<Text>().text = "Welcome!";
                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextTop.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        StartCoroutine(waitDuringTutorial(2));
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextTop.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    break;
                }
                break;
            //text at the top
            case 2:
                tutorialTextTop.GetComponent<Text>().text = "This is a D2 Test";
                showtutorialline = true;
                showAllLetters();
                if (!fadingin && !fadingindone)
                {
                    StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextTop.GetComponent<Text>()));
                }
                if(fadingindone && !waitdone)
                {
                    StartCoroutine(waitDuringTutorial(3));
                }
                if (!fadingin && currentdone && waitdone)
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    currentdone = false;
                    waitdone = false;
                    tutorialStep++;
                    break;
                }
                break;
            //change text at top to this
            //Show a row of the d2 test
            case 3:
                tutorialTextBottom.GetComponent<Text>().text = "We will use it to test your attention.";
                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        StartCoroutine(waitDuringTutorial(3));
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    break;
                }
                break;
            //show below d2 row
            case 4:
                tutorialTextBottom.GetComponent<Text>().text = "Let me explain how it works.";
                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        StartCoroutine(waitDuringTutorial(3));
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    break;
                }
                break;
            //below d2 row
            case 5:
                tutorialTextBottom.GetComponent<Text>().text = "Every letter is either a p or a d.";
                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        StartCoroutine(waitDuringTutorial(3));
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    break;
                }
                break;
            //below d2 row
            case 6:
                tutorialTextBottom.GetComponent<Text>().text = "Each letter has at most two dots above it, and at most two below it.";
                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        StartCoroutine(waitDuringTutorial(3));
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    break;
                }
                break;
            //below d2 row
            case 7:
                tutorialTextBottom.GetComponent<Text>().text = "These are your targets.";
                showError();
                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        StartCoroutine(waitDuringTutorial(3));
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    break;
                }
            break;
            //below d2 row
            //show the current errors
            case 8:
                tutorialTextBottom.GetComponent<Text>().text = "They are the letters d with two dots in total.";
                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        StartCoroutine(waitDuringTutorial(3));
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    break;
                }

                break;
            //below d2 row 
            case 9:
                tutorialTextBottom.GetComponent<Text>().text = "In other words, your targets are the letters d with either:";
                tutorialTextBottom.transform.GetChild(0).gameObject.GetComponent<Text>().text = " - Two dots above";
                tutorialTextBottom.transform.GetChild(1).gameObject.GetComponent<Text>().text = " - One dot above and one dot below";
                tutorialTextBottom.transform.GetChild(2).gameObject.GetComponent<Text>().text = " - Two dots below";


                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.GetComponent<Text>()));
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.transform.GetChild(0).gameObject.GetComponent<Text>()));
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.transform.GetChild(1).gameObject.GetComponent<Text>()));
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.transform.GetChild(2).gameObject.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        StartCoroutine(waitDuringTutorial(7));
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.GetComponent<Text>()));
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.transform.GetChild(0).gameObject.GetComponent<Text>()));
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.transform.GetChild(1).gameObject.GetComponent<Text>()));
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.transform.GetChild(2).gameObject.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    break;
                }
                break;
            //below d2 row
            case 10:
                tutorialTextBottom.GetComponent<Text>().text = "Your task is to click on every such letter, but without clicking any other letter.";

                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        StartCoroutine(waitDuringTutorial(3));
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    break;
                }

                break;
            //below d2 row
            case 11:
                tutorialTextBottom.GetComponent<Text>().text = "Give it a try!";
                hideErrors();
                showError();
                int[] errors = getError();
                buttonsAvailable = true;
                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        if (!errors.Contains(1))
                        {
                            waitdone = true;
                        }
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    buttonsAvailable = false;
                    break;
                }
            break;
            //below d2 row
            //wait for user to click errors
            //update errors every frame here and wait for there to be none
            //when none move to next step
            case 12:
                tutorialTextBottom.GetComponent<Text>().text = "You are now ready for a practice round.";

                if (!currentdone)
                {
                    if (!fadingin && !fadingindone)
                    {
                        StartCoroutine(FadeTextToFullAlpha(1f, tutorialTextBottom.GetComponent<Text>()));
                    }
                    if (fadingindone && !waitdone)
                    {
                        StartCoroutine(waitDuringTutorial(3));
                    }

                    if (!fadingout && !fadingoutdone && fadingindone && waitdone)
                    {
                        StartCoroutine(FadeTextToZeroAlpha(2f, tutorialTextBottom.GetComponent<Text>()));
                    }
                }
                else
                {
                    StopAllCoroutines();
                    fadingindone = false;
                    fadingoutdone = false;
                    tutorialStep++;
                    currentdone = false;
                    waitdone = false;
                    break;
                }


                break;
            //below d2 row
            case 13:
                showAllCanvasElements();
                finishedtext.SetActive(false);
                retry.SetActive(false);
                goToTest.SetActive(false);
                tutorialTextBottom.SetActive(false);
                tutorialTextTop.SetActive(false);
                tutorialfinished = true; 
                buttonsAvailable = true;
            break;
        }
        //tutorialTextTop.GetComponent<Text>().text = "Welcome!";
        
    
    
    }

    public IEnumerator FadeTextToFullAlpha(float t, Text i)
    {
        fadingin = true;
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        fadingin = false;
        fadingindone = true;
        if(tutorialStep == 2)
        {
            currentdone = true;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, Text i)
    {
        fadingout = true;
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        if(i.color.a <= 0.0f)
        {
            fadingout = false;
            fadingoutdone = true;
            currentdone = true;
        }
    }


    public int[] getError()
    {
        int[] errors = new int[numletters];
        for(int i =0; i<numletters; i++)
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

    public void hideErrors()
    {
        for (int i = 0; i < numletters; i++)
        {
            GameObject button = canvas.transform.GetChild(i + 1).gameObject;
            if (!button.GetComponent<ButtonValue>().error)
            {
                Image errorbox = button.transform.GetChild(0).gameObject.GetComponent<Image>();
                var tempColor = errorbox.color;
                tempColor.a = 0f;
                errorbox.color = tempColor;
            }
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

    public void hideAllCanvasElements() { 
        for(int i = 1; i<canvas.transform.childCount; i++)
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

    public void next()
    {
        int[] errors = getError();
        hideErrors();

        int errCount = errors.Aggregate(0,(s, x) => s + x);
        errorRate = 0.66f * errorRate + errCount;

        if(errCount > 0)
        {
            showError();
            errCount = errors.Aggregate(0, (s, x) => s + x);
        }
        else
        {
                
                completed = true;
                rowCount++;
            //currentseries++;
            if (rowCount < maxRowCount && (errorRate > maxErrorRate || rowCount < minRowCount))
                {
                    
                    row1 = generateRow(8 + Random.Range(-2, 3));
                    //validation = series[currentseries].Item2;
                    numcorrect = 0;
                    n_rep_row = 0;
                    
                    fail = false;
                    completed = false;
                }
                else
                {
                    continuebutton.SetActive(false);
                    hideAllLetters();
                    finishedtext.SetActive(true);
                    goToTest.SetActive(true);
                }
            //else
            //{
            //    retry.SetActive(true);
            //    continuebutton.SetActive(false);
            //}
        }
    }

    public void click()
    {
        if (buttonsAvailable)
        {
            GameObject button = EventSystem.current.currentSelectedGameObject;
            int x = button.GetComponent<ButtonValue>().value;
            bool toggle = button.GetComponent<ButtonValue>().toggle;
            int nbot = (x >> 4) & 0xF;
            int ntop = (x >> 8) & 0xF;
            int letter = x & 1;
            if (toggle)
            {
                button.transform.GetChild(4).gameObject.SetActive(false);
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
                button.transform.GetChild(4).gameObject.SetActive(true);
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
    }

    public void restart()
    {
        SceneManager.LoadScene("Practice");
    }

    void addDotsAndChangeLetter(int x, int n_rep_row)
    {
        GameObject button = canvas.transform.GetChild(n_rep_row + 1).gameObject;
        if (currentseries == 0)
        {
            button.transform.position = new Vector3(button.transform.position.x + n_rep_row * 0.02f, button.transform.position.y, button.transform.position.z);
        }
        button.transform.GetChild(4).gameObject.SetActive(false);
        button.GetComponent<ButtonValue>().toggle = false;
        button.GetComponent<ButtonValue>().value = x;
        Text texttop = button.transform.GetChild(2).GetComponent<Text>();
        texttop.text = "";
        Text textbot = button.transform.GetChild(3).GetComponent<Text>();
        textbot.text = "";
        int ntop = (x >> 8) & 0xF;
        for (int i = 0; i < ntop; i++)
        {
            texttop.text = texttop.text + "|\u200a\u200a";
        }
        int nbot = (x >> 4) & 0xF;
        for (int i = 0; i < nbot; i++)
        {
            textbot.text = textbot.text + "|\u200a\u200a";
        }
        int letter = x & 1;
        if (letter == 1)
        {
            Text textchar = button.transform.GetChild(1).GetComponent<Text>();
            textchar.text = "d";
        }
        else
        {
            Text textchar = button.transform.GetChild(1).GetComponent<Text>();
            textchar.text = "p";
        }

        if (nbot + ntop == 2 && letter == 1)
        {
            button.GetComponent<ButtonValue>().error = true;
        }
        else
        {
            button.GetComponent<ButtonValue>().error = false;
        }

        GameObject toptext = button.transform.GetChild(2).gameObject;
        GameObject bottext = button.transform.GetChild(3).gameObject;
        toptext.transform.position = new Vector3(button.transform.position.x + 0.0125f, button.transform.position.y + 0.25f, button.transform.position.z);
        bottext.transform.position = new Vector3(button.transform.position.x + 0.0125f, button.transform.position.y - 0.25f, button.transform.position.z);
        //button.transform.GetChild(1).gameObject.transform.position = button.transform.position;
        


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

        for (int i = 0; i < numletters; i++)
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
        for (int i = 0; i < target; i++)
        {
            int[] d2s = { 0x21, 0x111, 0x0201 };
            row[i] = d2s[Random.Range(0, 3)];
            expectedAnswer[i] = true;
        }

        //shuffle
        for (int i = row.Length; i > 0; i--)
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
        targets[6] = 5 + rn;
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

}

