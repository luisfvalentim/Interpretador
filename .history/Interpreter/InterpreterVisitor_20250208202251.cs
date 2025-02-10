using Antlr4.Runtime.Misc;
using Interpretador.Generated;

namespace Interpretador.Interpreter // 🔹 Ajustado para corresponder ao namespace usado no Program.cs
{

public class InterpreterVisitor : CSubsetBaseVisitor<object>
{
    private readonly Dictionary<string, object> _variables = new();

    // Exemplo: Declaração de variável
    public override object VisitDeclaration(CSubsetParser.DeclarationContext context)
    {
        string id = context.ID().GetText();
        object value = context.expression() != null ? Visit(context.expression()) : null;
        _variables[id] = value;
        return null;
    }

    // Exemplo: Comando 'if'
    public override object VisitIfStatement(CSubsetParser.IfStatementContext context)
{
    Console.WriteLine("Visitando IfStatement");

    if (context == null)
    {
        Console.WriteLine("Erro: contexto nulo.");
        return null;
    }

    Console.WriteLine($"Expressão condicional: {context.expression().GetText()}");

    var condition = Visit(context.expression());

    if (condition is bool cond)
    {
        Console.WriteLine($"Condição avaliada como: {cond}");

        if (cond)
        {
            Console.WriteLine("Executando bloco if");
            return Visit(context.statement(0));
        }
        else if (context.statement(1) != null) 
        {
            Console.WriteLine("Executando bloco else");
            return Visit(context.statement(1));
        }
    }
    else
    {
        Console.WriteLine("Erro: A condição não é booleana.");
    }

    return null;
}




    // Exemplo: Função 'printf'
    public override object VisitFunctionCall(CSubsetParser.FunctionCallContext context)
    {
        if (context.ID().GetText() == "printf")
        {
            string text = Visit(context.expression(0)).ToString();
            Console.WriteLine(text.Replace("\"", "")); // Remove aspas
        }
        return null;
    }

    public override object VisitRelationalExpression(CSubsetParser.RelationalExpressionContext context)
{
    var left = Visit(context.additiveExpression(0));
    var right = Visit(context.additiveExpression(1));

    if (left is int leftInt && right is int rightInt)
    {
        switch (context.GetChild(1).GetText())
        {
            case "<": return leftInt < rightInt;
            case ">": return leftInt > rightInt;
            case "<=": return leftInt <= rightInt;
            case ">=": return leftInt >= rightInt;
            case "==": return leftInt == rightInt;
            case "!=": return leftInt != rightInt;
        }
    }
    else if (left is float leftFloat && right is float rightFloat)
    {
        switch (context.GetChild(1).GetText())
        {
            case "<": return leftFloat < rightFloat;
            case ">": return leftFloat > rightFloat;
            case "<=": return leftFloat <= rightFloat;
            case ">=": return leftFloat >= rightFloat;
            case "==": return leftFloat == rightFloat;
            case "!=": return leftFloat != rightFloat;
        }
    }

    throw new Exception("Erro: Expressão relacional inválida.");
}


}

}