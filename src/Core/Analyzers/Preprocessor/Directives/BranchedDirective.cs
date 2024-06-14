using System.Collections.Immutable;
using Pdcl.Core.Text;
namespace Pdcl.Core.Preproc;

public abstract class BranchedDirective : IDirective 
{
    public readonly bool Result;
    public readonly TextPosition BodyPosition;
    public ImmutableList<IDirective> Children {get; init;}

#pragma warning disable CS8618
    public BranchedDirective(string name, TextPosition header, TextPosition bodyPos, bool res) : base(name, header)
    {
        BodyPosition = bodyPos;
        Result = res;
    }
#pragma warning restore
}