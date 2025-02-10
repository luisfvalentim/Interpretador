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
    if (context == null || context.expression() == null)
    {
        Console.WriteLine("Erro: IfStatementContext inválido.");
        return null;
    }

    var condition = Visit(context.expression());
    if (condition is bool condicao && condicao)
    {
        if (context.statement(0) != null) 
            Visit(context.statement(0));
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

    // Adicione métodos para 'while', operadores, etc.
}

}