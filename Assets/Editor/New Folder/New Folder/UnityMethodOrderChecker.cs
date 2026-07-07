using System.Collections.Generic;
using System.IO;

public static class UnityMethodOrderChecker
{
    public static List<UnityMethodOrderViolation> CheckFolder(string folder)
    {
        var result = new List<UnityMethodOrderViolation>();

        var files = Directory.GetFiles(
            folder,
            "*.cs",
            SearchOption.AllDirectories);

        foreach (var file in files)
        {
            CheckFile(file, result);
        }

        return result;
    }

    private static readonly HashSet<string> UnityMethods = new()
{
    // Initialization
    "Reset",
    "OnValidate",
    "Awake",
    "OnEnable",
    "Start",

    // Update
    "FixedUpdate",
    "Update",
    "LateUpdate",

    // Physics 3D
    "OnCollisionEnter",
    "OnCollisionStay",
    "OnCollisionExit",
    "OnTriggerEnter",
    "OnTriggerStay",
    "OnTriggerExit",

    // Physics 2D
    "OnCollisionEnter2D",
    "OnCollisionStay2D",
    "OnCollisionExit2D",
    "OnTriggerEnter2D",
    "OnTriggerStay2D",
    "OnTriggerExit2D",

    // Rendering
    "OnBecameVisible",
    "OnBecameInvisible",
    "OnWillRenderObject",
    "OnPreCull",
    "OnPreRender",
    "OnPostRender",
    "OnRenderObject",
    "OnRenderImage",
    "OnDrawGizmos",
    "OnDrawGizmosSelected",

    // Animation
    "OnAnimatorMove",
    "OnAnimatorIK",

    // UI / IMGUI
    "OnGUI",

    // Mouse
    "OnMouseDown",
    "OnMouseUp",
    "OnMouseUpAsButton",
    "OnMouseEnter",
    "OnMouseExit",
    "OnMouseOver",
    "OnMouseDrag",

    // Application
    "OnApplicationFocus",
    "OnApplicationPause",
    "OnApplicationQuit",

    // Disable / Destroy
    "OnDisable",
    "OnDestroy"
};

    private static void CheckFile(
     string file,
     List<UnityMethodOrderViolation> result)
    {
        string[] lines = File.ReadAllLines(file);

        bool insideClass = false;
        bool classBraceFound = false;
        int braceDepth = 0;

        bool regularMethodFound = false;
        string lastRegularMethod = "";

        bool inBlockComment = false;

        foreach (string original in lines)
        {
            string line = original;

            // Удаление многострочных комментариев
            if (inBlockComment)
            {
                int endComment = line.IndexOf("*/");

                if (endComment == -1)
                    continue;

                line = line.Substring(endComment + 2);
                inBlockComment = false;
            }

            while (true)
            {
                int startComment = line.IndexOf("/*");

                if (startComment == -1)
                    break;

                int endComment = line.IndexOf("*/", startComment + 2);

                if (endComment == -1)
                {
                    line = line.Substring(0, startComment);
                    inBlockComment = true;
                    break;
                }

                line = line.Remove(startComment, endComment - startComment + 2);
            }

            int singleComment = line.IndexOf("//");

            if (singleComment >= 0)
                line = line.Substring(0, singleComment);

            line = line.Trim();

            if (line.Length == 0)
                continue;

            // Нашли объявление класса
            if (!insideClass)
            {
                if (line.Contains("class "))
                {
                    insideClass = true;
                }

                continue;
            }

            // Ждем открывающую фигурную скобку класса
            if (!classBraceFound)
            {
                if (line.Contains("{"))
                {
                    classBraceFound = true;
                    braceDepth = 1;
                }

                continue;
            }

            foreach (char c in line)
            {
                if (c == '{')
                    braceDepth++;

                if (c == '}')
                {
                    braceDepth--;

                    if (braceDepth == 0)
                    {
                        insideClass = false;
                        classBraceFound = false;
                        regularMethodFound = false;
                        lastRegularMethod = "";
                    }
                }
            }

            // Ищем только методы первого уровня класса
            if (braceDepth != 1)
                continue;

            if (!line.Contains("(") || !line.Contains(")"))
                continue;

            if (line.StartsWith("if"))
                continue;

            if (line.StartsWith("for"))
                continue;

            if (line.StartsWith("foreach"))
                continue;

            if (line.StartsWith("while"))
                continue;

            if (line.StartsWith("switch"))
                continue;

            if (line.StartsWith("catch"))
                continue;

            int paren = line.IndexOf('(');

            int end = paren - 1;

            while (end >= 0 && char.IsWhiteSpace(line[end]))
                end--;

            int start = end;

            while (start >= 0 &&
                   (char.IsLetterOrDigit(line[start]) || line[start] == '_'))
            {
                start--;
            }

            if (end <= start)
                continue;

            string methodName = line.Substring(start + 1, end - start);

            if (UnityMethods.Contains(methodName))
            {
                if (regularMethodFound)
                {
                    result.Add(new UnityMethodOrderViolation
                    {
                        AssetPath = file.Replace("\\", "/"),
                        Description =
                            $"Unity-метод '{methodName}' расположен после пользовательского метода '{lastRegularMethod}'."
                    });

                    return;
                }
            }
            else
            {
                lastRegularMethod = methodName;
                regularMethodFound = true;
            }
        }
    }
}