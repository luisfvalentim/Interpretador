using Antlr4.Runtime.Misc;

namespace Interpretador.Interpreter;

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
        bool condition = (bool)Visit(context.expression());
        if (condition)
            Visit(context.statement(0)); // Bloco 'if'
        else if (context.Else() != null)
            Visit(context.statement(1)); // Bloco 'else'
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

    // Adicione métodos para 'while', operadores, etc.
}