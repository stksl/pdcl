namespace Pdcl.Core;

public sealed class ProjectManager 
{
    private readonly string projectPath;
    public ProjectManager(string projectPath_)
    {    
        projectPath = projectPath_;
    }
    // todo: maybe compile all .pdcl files using threadPool and at the end combine all Syntax Trees
    public void BuildAll() 
    {

    }
}

public struct SourceOptions 
{
    public enum OutputType 
    {
        Executable = 1,
        Library,
    }
    public readonly OutputType OutputType_;
    public readonly bool UsePreprocessor;

}