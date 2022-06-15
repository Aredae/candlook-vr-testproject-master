// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Author:      Troy Atkinson
// Publisher:   HelloWorld Studios
// Support:     supprt@helloworldstudios.co.uk
// Suggestions: troy@helloworldstudios.co.uk
// Feel free to edit any code to suit your needs, although you can not redistribute it.
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

using UnityEngine;
using UnityEditor;
using System.IO;
using PrettyConsole;
using PConsoleMethods;

[ExecuteInEditMode]
public class ConsoleWindow : EditorWindow
{
    // Holds the filestream string
    private string settingsFilePath;

    // String for 'settings file path' text field
    private string newSettingsFilePath;

    [MenuItem("Window/Pretty Console Configuration")]
    public static void ShowWindow()
    {
        // Create Menu Option
        GetWindow(typeof(ConsoleWindow), false, "Pretty Console");
    }

    private void OnEnable()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Create or load the file
        settingsFilePath = PConsole.GetFileStream();
        if (File.Exists(settingsFilePath))
        {
            PConsole.LoadSettingsFromFile(settingsFilePath);
        }
        else
        {
            PConsole.CreateSettingsFile(settingsFilePath);
        }

        // Set datapath
        string consoleSettingsPath = EditorPrefs.GetString("consoleSettingsPath");
        newSettingsFilePath = consoleSettingsPath != string.Empty ? consoleSettingsPath : "HelloWorld Tools/PrettyConsole";
    }

    private void OnGUI()
    {
        Space(2);
        DrawHeader();
        Space(2);

        #region Log Settings
        EditorGUILayout.BeginVertical();
        PConsole.ShowLogSettings = EditorGUILayout.Foldout(PConsole.ShowLogSettings, "Log Settings");
        if (PConsole.ShowLogSettings)
        {
            DrawLogSettings();
        }
        EditorGUILayout.EndVertical();
        #endregion

        Space(2);

        #region Prefix Settings
        EditorGUILayout.BeginVertical();
        PConsole.ShowPrefixSettings = EditorGUILayout.Foldout(PConsole.ShowPrefixSettings, "Prefix Settings");
        if (PConsole.ShowPrefixSettings)
        {
            DrawPrefixSettings();
        }
        EditorGUILayout.EndVertical();
        #endregion

        Space(2);

        #region Quote Settings
        EditorGUILayout.BeginVertical();
        PConsole.ShowQuoteSettings = EditorGUILayout.Foldout(PConsole.ShowQuoteSettings, "Quote Settings");
        if (PConsole.ShowQuoteSettings)
        {          
            DrawQuoteSettings();
        }
        EditorGUILayout.EndVertical();
        #endregion

        Space(2);

        #region Serialization Settings
        EditorGUILayout.BeginVertical();
        PConsole.ShowSerializationSettings = EditorGUILayout.Foldout(PConsole.ShowSerializationSettings, "Serialization Settings");
        if (PConsole.ShowSerializationSettings)
        {
            DrawSerializationSettings();
        }
        EditorGUILayout.EndVertical();
        #endregion

        Space(2);

        #region Infomation
        EditorGUILayout.BeginVertical();
        PConsole.ShowInfomation = EditorGUILayout.Foldout(PConsole.ShowInfomation, "Asset Infomation");
        if (PConsole.ShowInfomation)
        {
            DrawInfomation();
        }
        EditorGUILayout.EndVertical();
        #endregion
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Pretty Console Configuration", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(PConsole.version);
        EditorGUILayout.EndHorizontal();
    }

    private void DrawLogSettings()
    {
        #region Prefix Type
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Default Log Style", EditorStyles.label);
        PConsole.DefaultMessageStyle = (FontStyle)EditorGUILayout.EnumPopup(PConsole.DefaultMessageStyle);
        EditorGUILayout.EndVertical();
        #endregion

        #region Log Color
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Default Log Color", EditorStyles.label);
        PConsole.DefaultLogColor = EditorGUILayout.ColorField(PConsole.DefaultLogColor);
        EditorGUILayout.EndVertical();
        #endregion

        #region Log Error Color
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Default Log Error Color", EditorStyles.label);
        PConsole.DefaultLogErrorColor = EditorGUILayout.ColorField(PConsole.DefaultLogErrorColor);
        EditorGUILayout.EndVertical();
        #endregion

        #region Log Warning Color
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Default Log Warning Color", EditorStyles.label);
        PConsole.DefaultLogWarningColor = EditorGUILayout.ColorField(PConsole.DefaultLogWarningColor);
        EditorGUILayout.EndVertical();
        #endregion
    }

    private void DrawPrefixSettings()
    {
        #region Prefix Type
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Default Prefix Type", EditorStyles.label);
        PConsole.DefaultPrefixType = (PrefixType)EditorGUILayout.EnumPopup(PConsole.DefaultPrefixType);
        EditorGUILayout.EndVertical();
        #endregion

        #region Prefix Layout
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Prefix Layout", EditorStyles.label);
        PConsole.PrefixLayout = (PrefixLayout)EditorGUILayout.EnumPopup(PConsole.PrefixLayout);
        EditorGUILayout.EndVertical();
        #endregion

        #region Prefix Style
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Prefix Style", EditorStyles.label);
        PConsole.PrefixStyle = (FontStyle)EditorGUILayout.EnumPopup(PConsole.PrefixStyle);
        EditorGUILayout.EndVertical();
        #endregion

        #region Prefix Color Type
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Default Prefix Colors", EditorStyles.label);
        PConsole.PrefixColorType = (PrefixColorType)EditorGUILayout.EnumPopup(PConsole.PrefixColorType);
        if (PConsole.PrefixColorType == PrefixColorType.MultipleColors)
        {
            // Draw random colors
            if (PConsole.ShowRandomColors)
            {
                for (int i = 0; i < PConsole.RandomColors.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    PConsole.RandomColors[i] = EditorGUILayout.ColorField(PConsole.RandomColors[i]);
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        PConsole.RandomColors.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if(GUILayout.Button("Add Color"))
                {
                    PConsole.RandomColors.Add(Color.white);
                }
            }

            //  Show/Hide random colors
            if (GUILayout.Button(PConsole.ShowRandomColors ? "Hide Colors" : "Show Colors"))
            {
                PConsole.ShowRandomColors = !PConsole.ShowRandomColors;
            }
        }
        else
        {
            PConsole.DefaultPrefixColor = EditorGUILayout.ColorField(PConsole.DefaultPrefixColor);
            PConsole.ShowRandomColors = false;
        }
        EditorGUILayout.EndVertical();
        #endregion

        #region Prefix Color Type
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Custom Prefix Colors", EditorStyles.label);

        // Draw custom colors
        for (int i = 0; i < PConsole.CustomColorsPrefix.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            PConsole.CustomColorsPrefix[i] = EditorGUILayout.TextField(PConsole.CustomColorsPrefix[i]);
            PConsole.CustomColorsColor[i] = EditorGUILayout.ColorField(PConsole.CustomColorsColor[i]);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                PConsole.CustomColorsPrefix.RemoveAt(i);
                PConsole.CustomColorsColor.RemoveAt(i);
                Repaint();
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Custom Color"))
        {
            PConsole.CustomColorsPrefix.Add(string.Empty);
            PConsole.CustomColorsColor.Add(Color.white);
        }
        EditorGUILayout.EndVertical();
        #endregion 
    }

    private void DrawQuoteSettings()
    {
        #region Enable quotes
        EditorGUILayout.BeginVertical("box");
        PConsole.EnableQuotes = EditorGUILayout.Toggle("Enable Quotes", PConsole.EnableQuotes);
        EditorGUILayout.EndVertical();
        #endregion

        if (PConsole.EnableQuotes)
        {
            #region Quote Mark
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Quote Character", EditorStyles.label);
            PConsole.QuoteMark = EditorGUILayout.TextField(PConsole.QuoteMark);
            EditorGUILayout.EndVertical();
            #endregion

            #region Quote Color
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Quote Color", EditorStyles.label);
            PConsole.QuoteColor = EditorGUILayout.ColorField(PConsole.QuoteColor);
            EditorGUILayout.EndVertical();
            #endregion
        }
    }

    private void DrawSerializationSettings()
    {
        #region Settings Serialization bool
        EditorGUILayout.BeginVertical("box");
        PConsole.DisableSerialization = !EditorGUILayout.Toggle("Enable Serialization", !PConsole.DisableSerialization);
        EditorGUILayout.EndVertical();
        #endregion

        #region Settings Path
        if (!PConsole.DisableSerialization)
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Settings File Path (Assets/... /ConsoleSettings.json)", EditorStyles.label);
            newSettingsFilePath = EditorGUILayout.TextField(newSettingsFilePath);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Update"))
            {
                // Check if the user is just pressing the 'Update' button for no reason (it is tempting)
                if (newSettingsFilePath == EditorPrefs.GetString("consoleSettingsPath"))
                {
                    return;
                }

                // Update file path if it's valid
                if (IsValidPath(newSettingsFilePath))
                {
                    UpdateSettingsPath(newSettingsFilePath);
                }
            }
            if (GUILayout.Button("Default"))
            {
                // Check if the user is just pressing the 'Update' button for no reason (it is tempting)
                if (EditorPrefs.GetString("consoleSettingsPath") == "HelloWorld Tools/PrettyConsole")
                {
                    newSettingsFilePath = "HelloWorld Tools/PrettyConsole";
                    return;
                }

                // Update file path
                UpdateSettingsPath("HelloWorld Tools/PrettyConsole");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        #endregion
    }

    private void DrawInfomation()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Developed by Troy Atkinson", EditorStyles.label);
        GUILayout.Label("Published by HelloWorld Studios", EditorStyles.label);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Feature Suggestions: troy@helloworldstudios.co.uk", EditorStyles.label);
        GUILayout.Label("Support: support@helloworldstudios.co.uk", EditorStyles.label);
        EditorGUILayout.EndVertical();
    }

    private void Space(int spaces)
    {
        for (int i = 0; i < spaces; i++)
        {
            EditorGUILayout.Space();
        }
    }

    private void UpdateSettingsPath(string settingsPath)
    {
        Directory.CreateDirectory(Application.dataPath + "/" + settingsPath);
        EditorPrefs.SetString("consoleSettingsPath", settingsPath);
        PConsole.SaveSettings();
        LoadSettings();
        Console.Log("Console Settings file path has been updated to 'Assets/" + settingsPath + "/'", PrefixType.NoPrefix);
        Repaint();
    }

    private bool IsValidPath(string path)
    {
        // Check if either the first or last characters in the new file path are forward slashes
        char lastChar = path[path.Length - 1];
        if (path[0] == '/' || lastChar == '/')
        {
            Console.LogWarning("Console Settings file path was not set, you can't start or end the string with a forward slash.", PrefixType.NoPrefix);
            return false;
        }

        // Check if there is a space at the end of the string
        if (lastChar == ' ' || path.Contains(" /"))
        {
            Console.LogWarning("Console Settings file path was not set, you can't end a folder name with a space.", PrefixType.NoPrefix);
            return false;
        }

        // Check for invalid characters in the string
        char[] invalidChars = new char[8] { '<', '>', ':', '"', '\\', '|', '?', '*' };
        foreach (char character in path)
        {
            foreach (char invalidChar in invalidChars)
            {
                if (invalidChar == character)
                {
                    Console.LogWarning("Console Settings file path was not set, invalid character ( " + invalidChar + " ) in string.", PrefixType.NoPrefix);
                    return false;
                }
            }
        }

        return true;
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}