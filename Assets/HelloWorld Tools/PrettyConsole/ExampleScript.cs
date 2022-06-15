// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Author:      Troy Atkinson
// Publisher:   HelloWorld Studios
// Support:     supprt@helloworldstudios.co.uk
// Suggestions: troy@helloworldstudios.co.uk
// This is an example script that will show you some of the methods that come with Pretty Console. Put this script on an active game object and run the scene.
// Feel free to edit any code to suit your needs, although you can not redistribute it.
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

#if UNITY_EDITOR

using UnityEngine;
using PrettyConsole; // <--- Use this namespace to access all of the Pretty Console methods.

public class ExampleScript : MonoBehaviour
{
    private void Start()
    {

        Console.Log("This is a normal log.");

        Console.Log("Normal color 'quote color' normal color.");


        Console.Log("This is a color log.", Color.cyan);
        Console.Log("This is a font style log.", FontStyle.Italic);
        Console.Log("And this is a custom prefix type log.", PrefixType.MethodName);

        Console.LogWarning("Warning log.", PrefixType.NoPrefix);
        Console.LogError("Error log.", FontStyle.BoldAndItalic);

        Debug.Log(Console.String("Normal string log."));
        Debug.LogWarning(Console.StringWarning("Warning string log."));


        Console.Log("Bold magenta log", Color.magenta, FontStyle.Bold, PrefixType.NoPrefix);

        // Normal log
        Console.Log("This is a normal log.");

        // One parameter log
        Console.Log("You can change the color of the log with a parameter.", Color.cyan);
        Console.Log("And change the font style of the log.", FontStyle.Italic);
        Console.Log("You can also change the prefix type (to show either the class, method or nothing).", PrefixType.MethodName);

        // Threee parameter log
        Console.Log("Or you can do all 3.", Color.magenta, FontStyle.Bold, PrefixType.NoPrefix);

        // Warning & error logs
        Console.LogWarning("You can also log warnings.");
        Console.LogError("And errors.");

        // Quote
        Console.Log("You can change the color of text inside the log by using quotes 'like this' and make them stand out.");

        // Console.String logs
        Debug.Log(Console.String("If you want to click on the log and go to the correct section in the code. Use Console.String instead of Console.Log."));
        Debug.LogWarning(Console.StringWarning("This also works for Console.StringWarning and Console.StringError."));

        // Reminder
        Console.Log("Don't forget to open the Pretty Console Configuration window in the menu 'Window/Pretty Console Configuration'.");
    }
}
#endif