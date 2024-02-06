namespace Pdcl.Test;

public static class RelativePath 
{
    public static string GetRelativePath() 
    {
        string[] splitPath = Path.GetFullPath(".").Split('/');
        string path = string.Join('/', splitPath.Take(splitPath.Length - 3)) + "/";

        return path;
    }
}