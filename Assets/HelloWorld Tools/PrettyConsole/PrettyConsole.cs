// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Author:      Troy Atkinson
// Publisher:   HelloWorld Studios
// Support:     supprt@helloworldstudios.co.uk
// Suggestions: troy@helloworldstudios.co.uk
// Feel free to edit any code to suit your needs, although you can not redistribute it.
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

# if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Collections.Generic;
using PrettyConsole;
using PConsoleMethods;

namespace PrettyConsole
{
    // Enumerations
    public enum PrefixType { ClassName, MethodName, NoPrefix }
    public enum PrefixLayout { Brackets, Parentheses, Braces, AngleBrackets, Hyphen, Tilde, JustName }
    public enum PrefixColorType { MultipleColors, SingleColor }

    public static class Console
    {
        #region Default Logs
        /// <summary>
        /// Log a message to the console.
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            Debug.Log(PConsole.GetLog(LogType.Log, message, null, null, null));
        }

        /// <summary>
        /// Log a message to the console with a specific color.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        public static void Log(string message, Color messageColor)
        {
            Debug.Log(PConsole.GetLog(LogType.Log, message, null, null, messageColor));
        }

        /// <summary>
        /// Log a message to the console with a specific font style.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageStyle"></param>
        public static void Log(string message, FontStyle messageStyle)
        {
            Debug.Log(PConsole.GetLog(LogType.Log, message, null, messageStyle, null));
        }

        /// <summary>
        /// Log a message to the console with a specific prefix type.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefixType"></param>
        public static void Log(string message, PrefixType prefixType)
        {
            Debug.Log(PConsole.GetLog(LogType.Log, message, prefixType, null, null));
        }

        /// <summary>
        /// Log a message to the console with a specific color, font style and prefix type. Set parameters as null for defaults.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        /// <param name="messageStyle"></param>
        /// <param name="prefixType"></param>
        public static void Log(string message, Color? messageColor, FontStyle? messageStyle, PrefixType? prefixType)
        {
            Debug.Log(PConsole.GetLog(LogType.Log, message, prefixType, messageStyle, messageColor));
        }
        #endregion

        #region Warning Logs
        /// <summary>
        /// Log a warning message to the console.
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarning(string message)
        {
            Debug.LogWarning(PConsole.GetLog(LogType.Warning, message, null, null, null));
        }

        /// <summary>
        /// Log a warning message to the console with a specific color.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        public static void LogWarning(string message, Color messageColor)
        {
            Debug.LogWarning(PConsole.GetLog(LogType.Warning, message, null, null, messageColor));
        }

        /// <summary>
        /// Log a warning message to the console with a specific font style.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageStyle"></param>
        public static void LogWarning(string message, FontStyle messageStyle)
        {
            Debug.LogWarning(PConsole.GetLog(LogType.Warning, message, null, messageStyle, null));
        }

        /// <summary>
        /// Log a warning message to the console with a specific prefix type.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefixType"></param>
        public static void LogWarning(string message, PrefixType prefixType)
        {
            Debug.LogWarning(PConsole.GetLog(LogType.Warning, message, prefixType, null, null));
        }

        /// <summary>
        /// Log a warning message to the console with a specific color, font style and prefix type. Set parameters as null for defaults.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        /// <param name="messageStyle"></param>
        /// <param name="prefixType"></param>
        public static void LogWarning(string message, Color? messageColor, FontStyle? messageStyle, PrefixType? prefixType)
        {
            Debug.LogWarning(PConsole.GetLog(LogType.Warning, message, prefixType, messageStyle, messageColor));
        }
        #endregion

        #region Error Logs
        /// <summary>
        /// Log a error message to the console.
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(string message)
        {
            Debug.LogError(PConsole.GetLog(LogType.Error, message, null, null, null));
        }

        /// <summary>
        /// Log a error message to the console with a specific color.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        public static void LogError(string message, Color messageColor)
        {
            Debug.LogError(PConsole.GetLog(LogType.Error, message, null, null, messageColor));
        }

        /// <summary>
        /// Log a error message to the console with a specific font style.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageStyle"></param>
        public static void LogError(string message, FontStyle messageStyle)
        {
            Debug.LogError(PConsole.GetLog(LogType.Error, message, null, messageStyle, null));
        }

        /// <summary>
        /// Log a error message to the console with a specific prefix type.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefixType"></param>
        public static void LogError(string message, PrefixType prefixType)
        {
            Debug.LogError(PConsole.GetLog(LogType.Error, message, prefixType, null, null));
        }

        /// <summary>
        /// Log a error message to the console with a specific color, font style and prefix type. Set parameters as null for defaults.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        /// <param name="messageStyle"></param>
        /// <param name="prefixType"></param>
        public static void LogError(string message, Color? messageColor, FontStyle? messageStyle, PrefixType? prefixType)
        {
            Debug.LogError(PConsole.GetLog(LogType.Warning, message, prefixType, messageStyle, messageColor));
        }
        #endregion

        #region Default Strings
        /// <summary>
        /// Returns a string ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string String(string message)
        {
            return PConsole.GetLog(LogType.Log, message, null, null, null);
        }

        /// <summary>
        /// Returns a string with a specific color, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        /// <returns></returns>
        public static string String(string message, Color messageColor)
        {
            return PConsole.GetLog(LogType.Log, message, null, null, messageColor);
        }

        /// <summary>
        /// Returns a string with a specific font style, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageStyle"></param>
        /// <returns></returns>
        public static string String(string message, FontStyle messageStyle)
        {
            return PConsole.GetLog(LogType.Log, message, null, messageStyle, null);
        }

        /// <summary>
        /// Returns a string with a specific prefix type, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefixType"></param>
        /// <returns></returns>
        public static string String(string message, PrefixType prefixType)
        {
            return PConsole.GetLog(LogType.Log, message, prefixType, null, null);
        }

        /// <summary>
        /// Returns a string with a specific color, font style and prefix type, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        /// <param name="messageStyle"></param>
        /// <param name="prefixType"></param>
        /// <returns></returns>
        public static string String(string message, Color? messageColor, FontStyle? messageStyle, PrefixType? prefixType)
        {
            return PConsole.GetLog(LogType.Log, message, prefixType, messageStyle, messageColor);
        }
        #endregion

        #region Warning Strings
        /// <summary>
        /// Returns a warning string ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string StringWarning(string message)
        {
            return PConsole.GetLog(LogType.Warning, message, null, null, null);
        }

        /// <summary>
        /// Returns a warning string with a specific color, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        /// <returns></returns>
        public static string StringWarning(string message, Color messageColor)
        {
            return PConsole.GetLog(LogType.Warning, message, null, null, messageColor);
        }

        /// <summary>
        /// Returns a warning string with a specific font style, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageStyle"></param>
        /// <returns></returns>
        public static string StringWarning(string message, FontStyle messageStyle)
        {
            return PConsole.GetLog(LogType.Warning, message, null, messageStyle, null);
        }

        /// <summary>
        /// Returns a warning string with a specific prefix type, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefixType"></param>
        /// <returns></returns>
        public static string StringWarning(string message, PrefixType prefixType)
        {
            return PConsole.GetLog(LogType.Warning, message, prefixType, null, null);
        }

        /// <summary>
        /// Returns a warning string with a specific color, font style and prefix type, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        /// <param name="messageStyle"></param>
        /// <param name="prefixType"></param>
        /// <returns></returns>
        public static string StringWarning(string message, Color? messageColor, FontStyle? messageStyle, PrefixType? prefixType)
        {
            return PConsole.GetLog(LogType.Warning, message, prefixType, messageStyle, messageColor);
        }
        #endregion

        #region Error Strings
        /// <summary>
        /// Returns a error string ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string StringError(string message)
        {
            return PConsole.GetLog(LogType.Error, message, null, null, null);
        }

        /// <summary>
        /// Returns a error string with a specific color, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        /// <returns></returns>
        public static string StringError(string message, Color messageColor)
        {
            return PConsole.GetLog(LogType.Error, message, null, null, messageColor);
        }

        /// <summary>
        /// Returns a error string with a specific font style, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageStyle"></param>
        /// <returns></returns>
        public static string StringError(string message, FontStyle messageStyle)
        {
            return PConsole.GetLog(LogType.Error, message, null, messageStyle, null);
        }

        /// <summary>
        /// Returns a error string with a specific prefix type, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="prefixType"></param>
        /// <returns></returns>
        public static string StringError(string message, PrefixType prefixType)
        {
            return PConsole.GetLog(LogType.Error, message, prefixType, null, null);
        }

        /// <summary>
        /// Returns a error string with a specific color, font style and prefix type, ready to be logged to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        /// <param name="messageStyle"></param>
        /// <param name="prefixType"></param>
        /// <returns></returns>
        public static string StringError(string message, Color? messageColor, FontStyle? messageStyle, PrefixType? prefixType)
        {
            return PConsole.GetLog(LogType.Error, message, prefixType, messageStyle, messageColor);
        }
        #endregion
    }
}

namespace PConsoleMethods
{
    // Console Settings
    [SerializeField]
    public class PrettyConsoleSettings
    {
        // Log Settings
        public Color defaultLogColor = Color.black;
        public Color defaultLogErrorColor = Color.red;
        public Color defaultLogWarningColor = Color.yellow;
        public FontStyle defaultMessageStyle = FontStyle.Normal;

        // Prefix Settings
        public PrefixType defaultPrefixType = PrefixType.ClassName;
        public FontStyle prefixStyle = FontStyle.Bold;
        public PrefixLayout prefixLayout = PrefixLayout.Brackets;
        public PrefixColorType prefixColorType = PrefixColorType.MultipleColors;
        public Color defaultPrefixColor = Color.green;
        public List<Color> randomColors = new List<Color>()
        {
        Color.green,
        Color.magenta,
        Color.yellow,
        Color.red,
        Color.cyan,
        new Color32(135,206,250,255), // Light Sky Blue
        new Color32(152,251,152,255), // Pale Green
        new Color32(173,255,47,255), // Green Yellow
        new Color32(255,105,180,255), // Hot Pink
        new Color32(218,112,214,255) // Orchid      
        };
        public List<string> customColorsPrefix = new List<string>();
        public List<Color> customColorsColor = new List<Color>();

        // Quote settings
        public bool enableQuotes = true;
        public string quoteMark = "'";
        public Color quoteColor = Color.blue;

        // Window settings
        public bool showLogSettings = false;
        public bool showPrefixSettings = false;
        public bool showQuoteSettings = false;
        public bool showSerializationSettings = false;
        public bool showInfomation = false;
        public bool showRandomColors = false;
    }

    public static class PConsole
    {
        // Asset version
        public static string version = "1.0.2";

        // Settings class
        private static PrettyConsoleSettings settings;

        // Log properties
        public static Color DefaultLogColor { get { return settings.defaultLogColor; } set { settings.defaultLogColor = value; SaveSettings(); } }
        public static Color DefaultLogErrorColor { get { return settings.defaultLogErrorColor; } set { settings.defaultLogErrorColor = value; SaveSettings(); } }
        public static Color DefaultLogWarningColor { get { return settings.defaultLogWarningColor; } set { settings.defaultLogWarningColor = value; SaveSettings(); } }
        public static FontStyle DefaultMessageStyle { get { return settings.defaultMessageStyle; } set { settings.defaultMessageStyle = value; SaveSettings(); } }

        // Prefix properties
        public static PrefixType DefaultPrefixType { get { return settings.defaultPrefixType; } set { settings.defaultPrefixType = value; SaveSettings(); } }
        public static FontStyle PrefixStyle { get { return settings.prefixStyle; } set { settings.prefixStyle = value; SaveSettings(); } }
        public static PrefixLayout PrefixLayout { get { return settings.prefixLayout; } set { settings.prefixLayout = value; SaveSettings(); } }
        public static PrefixColorType PrefixColorType { get { return settings.prefixColorType; } set { settings.prefixColorType = value; SaveSettings(); } }
        public static Color DefaultPrefixColor { get { return settings.defaultPrefixColor; } set { settings.defaultPrefixColor = value; SaveSettings(); } }
        public static List<Color> RandomColors { get { return settings.randomColors; } set { settings.randomColors = value; SaveSettings(); } }
        public static List<string> CustomColorsPrefix { get { return settings.customColorsPrefix; } set { settings.customColorsPrefix = value; SaveSettings(); } }
        public static List<Color> CustomColorsColor { get { return settings.customColorsColor; } set { settings.customColorsColor = value; SaveSettings(); } }

        // Quote properties
        public static bool EnableQuotes { get { return settings.enableQuotes; } set { settings.enableQuotes = value; SaveSettings(); } }
        public static Color QuoteColor { get { return settings.quoteColor; } set { settings.quoteColor = value; SaveSettings(); } }
        public static string QuoteMark { get { return settings.quoteMark; }
            set
            {
                // Make sure the user is only inputting one character
                if(value.Length > 1 || value.Length == 0)
                {
                    return;
                }

                settings.quoteMark = value;
                SaveSettings();
            }
        }

        // Serialization properties
        public static bool DisableSerialization
        {
            get
            {
                return EditorPrefs.GetBool("disableSerialization");
            }
            set
            {
                EditorPrefs.SetBool("disableSerialization", value);
            }
        }

        // Window Properties
        public static bool ShowLogSettings { get { return settings.showLogSettings; } set { settings.showLogSettings = value; SaveSettings(); } }
        public static bool ShowPrefixSettings { get { return settings.showPrefixSettings; } set { settings.showPrefixSettings = value; SaveSettings(); } }
        public static bool ShowQuoteSettings { get { return settings.showQuoteSettings; } set { settings.showQuoteSettings = value; SaveSettings(); } }
        public static bool ShowSerializationSettings { get { return settings.showSerializationSettings; } set { settings.showSerializationSettings = value; SaveSettings(); } }
        public static bool ShowInfomation { get { return settings.showInfomation; } set { settings.showInfomation = value; SaveSettings(); } }
        public static bool ShowRandomColors { get { return settings.showRandomColors; } set { settings.showRandomColors = value; SaveSettings(); } }

        // Name of the prefix and what color has been assigned to it (in the random prefix colors)
        private static Dictionary<string, Color> prefixColors = new Dictionary<string, Color>();

        // List of colors to be used from the random colors
        private static List<Color> unusedColors = new List<Color>();

        // All the colors used by a random prefix
        private static List<Color> usedColors = new List<Color>();

        // Contructs log and sends it
        public static string GetLog(LogType logType, string message, PrefixType? prefixType, FontStyle? fontStyle, Color? color)
        {
            // Check to make sure settings isn't null
            if (settings == null)
            {
                string fileStream = GetFileStream();
                if (File.Exists(fileStream))
                {
                    LoadSettingsFromFile(fileStream);
                }
                else
                {
                    CreateSettingsFile(fileStream);
                }
            }

            // Prefix
            string prefix = string.Empty;
            if ((prefixType ?? settings.defaultPrefixType) != PrefixType.NoPrefix)
            {
                // Give the prefix the correct text, and color and style
                string perfixText = (prefixType ?? settings.defaultPrefixType) == PrefixType.ClassName ? GetClassName() : GetMethodName();
                prefix = string.Format("{0}{1}{2} ", PrefixLayoutToStartString(settings.prefixLayout), perfixText, PrefixLayoutToEndString(settings.prefixLayout));
                prefix = SetColor(prefix, GetPrefixColor(perfixText));
                prefix = SetFontStyle(prefix, settings.prefixStyle);
            }

            // Quote
            if (settings.enableQuotes)
            {
                // Find at least one quote mark
                if (Regex.IsMatch(message, settings.quoteMark))
                {
                    // Look through each character in the message
                    bool lookingForEndingQuoteMark = true;
                    int secondQuoteMark = 0;
                    for (int i = message.Length - 1; i > -1; i--)
                    {
                        // Find either the start or the head of the quote, make sure one of the chars either side are spaces
                        if (message[i].ToString() == settings.quoteMark && FoundEmptyAroundChar(message, i))
                        {
                            // Save the second quote
                            if (lookingForEndingQuoteMark)
                            {
                                secondQuoteMark = i;
                            }
                            else
                            {
                                // Send the quote to the correct color
                                message = SetColorAt(message, i + 1, secondQuoteMark + 1, settings.quoteColor, color, logType);
                            }
                            lookingForEndingQuoteMark = !lookingForEndingQuoteMark;
                        }
                    }
                }
            }

            // Message
            // Give the message the correct color and style
            message = SetColor(message, color ?? DefaultColor(logType));
            message = SetFontStyle(message, fontStyle ?? settings.defaultMessageStyle);

            // Return the completed string
            return prefix + message;
        }

        // Returns the name of the class that called Log()
        private static string GetClassName()
        {
            string callingMethod = Environment.StackTrace.Split('\n')[4];
            return callingMethod.Substring(6, callingMethod.IndexOf('.') - 6);
        }

        // Returns the name of the method that called Log()
        private static string GetMethodName()
        {
            string callingMethod = Environment.StackTrace.Split('\n')[4];
            int indexOfPeriod = callingMethod.IndexOf('.') + 1;
            return callingMethod.Substring(indexOfPeriod, callingMethod.IndexOf('(') - indexOfPeriod);
        }

        // Returns the correct color for the given script or assigns a color if one doesn't excist
        private static Color GetPrefixColor(string prefixText)
        {
            // Check if the given prefix has been assgined a custom color
            int i = settings.customColorsPrefix.IndexOf(prefixText);
            if(i != -1)
            {
                return settings.customColorsColor[i];
            }

            // Assign a new color if there are no assigned colors
            if (prefixColors.Count == 0)
            {
                return AssignColor(prefixText);
            }
            else
            {
                // Assign a new color if there is no assigned color to the prefix name
                if (!prefixColors.ContainsKey(prefixText))
                {
                    return AssignColor(prefixText);
                }
                else
                {
                    return prefixColors[prefixText];
                }
            }
        }

        // Gets a random color from the database or get the single prefix color
        private static Color AssignColor(string prefixText)
        {
            // If it's set to a single color, get that
            if(settings.prefixColorType == PrefixColorType.SingleColor)
            {
                return settings.defaultPrefixColor;
            }

            // If there are no more un-used colors, reset the list
            if (unusedColors.Count == 0)
            {
                unusedColors = new List<Color>(settings.randomColors);
                usedColors = new List<Color>();
            }

            // Get a random color, assign it to that prefix name then remove it from the bank of colors
            Color colorToUse = unusedColors[UnityEngine.Random.Range(0, unusedColors.Count - 1)];
            unusedColors.Remove(colorToUse);
            prefixColors.Add(prefixText, colorToUse);
            usedColors.Add(colorToUse);

            return colorToUse;
        }

        // Changes the color of the string to the color given
        private static string SetColor(string message, Color color)
        {
            return string.Format("<color={0}>{1}</color>", HexColor(color), message);
        }

        // Changes the color of the substring given while the rest stays the stringColor
        private static string SetColorAt(string message, int startColor, int endColor, Color substringColor, Color? stringColor, LogType logType)
        {
            return string.Format("{0}{1}{2}", message.Substring(0, startColor) + "</color>", SetColor(message.Substring(startColor, endColor - startColor - 1), settings.quoteColor), string.Format("<color={0}>", HexColor(stringColor ?? DefaultColor(logType))) + message.Substring(endColor - 1));
        }

        // Changes the style of the string to the style given
        private static string SetFontStyle(string message, FontStyle fontStyle)
        {
            return string.Format("{0}{1}{2}", StyleToStartTag(fontStyle), message, StyleToEndTag(fontStyle));
        }

        // Returns the default log color for the log type given
        private static Color DefaultColor(LogType logType)
        {
            switch (logType)
            {
                case LogType.Log:
                default:
                    return settings.defaultLogColor;
                case LogType.Error:
                    return settings.defaultLogErrorColor;
                case LogType.Warning:
                    return settings.defaultLogWarningColor;
            }
        }

        // Returns the given color in hex format
        private static string HexColor(Color32 color)
        {
            return string.Format("#{0}{1}{2}", color.r.ToString("x2"), color.g.ToString("X2"), color.b.ToString("X2"));
        }

        // Returns the tag to start the fontstyle given
        private static string StyleToStartTag(FontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case FontStyle.Normal:
                default:
                    return string.Empty;
                case FontStyle.Bold:
                    return "<b>";
                case FontStyle.Italic:
                    return "<i>";
                case FontStyle.BoldAndItalic:
                    return "<b><i>";
            }
        }

        // Returns the tag to end the fontstyle given
        private static string StyleToEndTag(FontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case FontStyle.Normal:
                default:
                    return string.Empty;
                case FontStyle.Bold:
                    return "</b>";
                case FontStyle.Italic:
                    return "</i>";
                case FontStyle.BoldAndItalic:
                    return "</i></b>";
            }
        }

        // Returns what is before the prefix text for that prefix layout
        private static string PrefixLayoutToStartString(PrefixLayout prefixLayout)
        {
            switch (prefixLayout)
            {
                case PrefixLayout.Brackets:
                default:
                    return "[";
                case PrefixLayout.Parentheses:
                    return "(";
                case PrefixLayout.Braces:
                    return "{";
                case PrefixLayout.AngleBrackets:
                    return "<";
                case PrefixLayout.Hyphen:
                case PrefixLayout.Tilde:
                case PrefixLayout.JustName:
                    return string.Empty;
            }
        }

        // Returns what is after the prefix text for that prefix layout
        private static string PrefixLayoutToEndString(PrefixLayout prefixLayout)
        {
            switch (prefixLayout)
            {
                case PrefixLayout.Brackets:
                default:
                    return "]";
                case PrefixLayout.Parentheses:
                    return ")";
                case PrefixLayout.Braces:
                    return "}";
                case PrefixLayout.AngleBrackets:
                    return ">";
                case PrefixLayout.Hyphen:
                    return " -";
                case PrefixLayout.Tilde:
                    return " ~";
                case PrefixLayout.JustName:
                    return " ";
            }
        }

        // Check either side of a char in a string for a space, fullstop or commar
        private static bool FoundEmptyAroundChar(string stringToCheck, int index)
        {
            // If the index is at the start or the end of the string return true
            if (index + 1 >= stringToCheck.Length || index == 0)
            {
                return true;
            }
            else
            {
                // If it's surrounded by any of the chars below return true
                string[] valuesToFinds = { " ", ",", ".", "-", "=", "<", ">" };
                foreach (string value in valuesToFinds)
                {
                    if (stringToCheck[index + 1].ToString() == value || stringToCheck[index - 1].ToString() == value)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static string GetFileStream()
        {
            string consoleSettingsPath = EditorPrefs.GetString("consoleSettingsPath");
            string settingspath = consoleSettingsPath != string.Empty ? consoleSettingsPath : "HelloWorld Tools/PrettyConsole";
            return string.Format("{0}/{1}/ConsoleSettings.json", Application.dataPath, settingspath);
        }

        public static void CreateSettingsFile(string dataFileLocation)
        {
            PrettyConsoleSettings newSettings = new PrettyConsoleSettings();
            if (!DisableSerialization)
            {
                File.WriteAllText(dataFileLocation, JsonUtility.ToJson(newSettings));
            }
            settings = newSettings;
        }

        public static void SaveSettings()
        {
            if (!DisableSerialization)
            {
                string consoleSettingsPath = EditorPrefs.GetString("consoleSettingsPath");
                string settingspath = consoleSettingsPath != string.Empty ? consoleSettingsPath : "HelloWorld Tools/PrettyConsole";
                if (!Directory.Exists(settingspath))
                {
                    Directory.CreateDirectory(Application.dataPath + "/" + settingspath);
                }

                File.WriteAllText(GetFileStream(), JsonUtility.ToJson(settings));
            }
        }

        public static void LoadSettingsFromFile(string dataFileLocation)
        {
            if (!DisableSerialization)
            {
                settings = JsonUtility.FromJson<PrettyConsoleSettings>(File.ReadAllText(dataFileLocation));
            }
            else
            {
                settings = new PrettyConsoleSettings();
            }
        }
    }
}

#endif