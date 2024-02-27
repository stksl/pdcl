namespace Pdcl.Core.Syntax;

internal sealed class BinaryExpressionVisitor : IVisitor<BinaryExpression> 
{
    public static BinaryExpressionVisitor Instance => _instance;
    private static BinaryExpressionVisitor _instance = new BinaryExpressionVisitor();
    private BinaryExpressionVisitor() {}
    public async Task<BinaryExpression> VisitAsync(Parser parser) 
    {
        return null!;
    }
}