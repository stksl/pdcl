namespace Pdcl.Core.Assembly;

public sealed class AssemblyManifest 
{
    public readonly string AssemblyName;
    /// <summary>
    /// Represents major, minor, build and revision versions
    /// </summary>
    public readonly (int, int, int, int) Version;
    public readonly byte[] PublicKey;
    public AssemblyManifest(string asmName, (int, int, int, int) versions, byte[] pubKey)
    {
        AssemblyName = asmName;

        Version = versions;

        PublicKey = pubKey;
    }
}