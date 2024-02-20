using System.Collections.Immutable;
using Pdcl.Core.Text;
namespace Pdcl.Core.Preproc;

public abstract class BranchedDirective : ComplexDirective 
{
    public bool Result {get; protected set;}
    public readonly TextPosition BodyPosition;

    private ImmutableList<IDirective> children;
    public BranchedDirective(string name, TextPosition header, IList<IDirective> children, TextPosition bodyPos) : base(name, header)
    {
        this.children = children.ToImmutableList();

        BodyPosition = bodyPos;
    }

    public override ImmutableList<IDirective> GetChildren() => children;
}