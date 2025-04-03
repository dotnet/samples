using System.Linq.Expressions;

namespace ExpressionVisitor;

public abstract class Visitor
{
    private readonly Expression node;

    protected Visitor(Expression node) => this.node = node;

    public abstract void Visit(string prefix);

    public ExpressionType NodeType => node.NodeType;
    public static Visitor CreateFromExpression(Expression node) =>
        node.NodeType switch
        {
            ExpressionType.Constant => new ConstantVisitor((ConstantExpression)node),
            ExpressionType.Lambda => new LambdaVisitor((LambdaExpression)node),
            ExpressionType.Parameter => new ParameterVisitor((ParameterExpression)node),
            // Lots of Binary Expression Types => 
            ExpressionType.Add => new BinaryVisitor((BinaryExpression)node),
            ExpressionType.Equal => new BinaryVisitor((BinaryExpression)node),
            ExpressionType.Multiply => new BinaryVisitor((BinaryExpression)node),
            ExpressionType.GreaterThan => new BinaryVisitor((BinaryExpression)node),
            ExpressionType.Assign => new BinaryVisitor((BinaryExpression)node),
            ExpressionType.Conditional => new ConditionalVisitor((ConditionalExpression)node),
            ExpressionType.Call => new MethodCallVisitor((MethodCallExpression)node),
            ExpressionType.Block => new BlockVisitor((BlockExpression)node),
            ExpressionType.Loop => new LoopVisitor((LoopExpression)node),
            ExpressionType.PostDecrementAssign => new UnaryVisitor((UnaryExpression)node),
            ExpressionType.Goto => new GoToVisitor((GotoExpression)node),
            _ => throw new InvalidOperationException($"Node not processed yet: {node.NodeType}"),
        };
}
