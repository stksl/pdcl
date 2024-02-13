using System.Collections.Immutable;
using Pdcl.Core.Text;
namespace Pdcl.Core.Preproc;

public abstract class BranchedDirective : ComplexDirective 
{
    public bool Result;
    private ImmutableList<IDirective> children;
    public BranchedDirective(string name, TextPosition header, IList<IDirective> children) : base(name, header)
    {
        this.children = children.ToImmutableList();
    }

    public override ImmutableList<IDirective> GetChildren() => children;
}