using UnityEngine;

public static class PathUtil
{
    private const string PREFIX_RES = "Assets/Res/";
    private const string PREFIX_RESTEMP = "Assets/ResTemp/";
    private const string PREFIX_RESOURCE = "Assets/Resource/";

    public static string GetBundlePath(string path)
    {
        if (path.StartsWith(PREFIX_RES))
        {
            return path.Substring(PREFIX_RES.Length);
        }
        else if (path.StartsWith(PREFIX_RESTEMP))
        {
            return path.Substring(PREFIX_RESTEMP.Length);
        }
        return path;
    }
    
}
