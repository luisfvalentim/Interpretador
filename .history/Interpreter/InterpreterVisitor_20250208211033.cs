using System;
using System.Collections.Generic;
using Interpretador.Generated;

namespace Interpretador.Interpreter
{
    public class InterpreterVisitor : CSubsetBaseVisitor<object>
    {
        private Dictionary<string, object> memory = new Dictionary<string, object>(); // Armazena variáveis

        // Processa declarações de variáveis (ex: int x = 10;)
        public override object VisitDeclaration(CSubsetParser.DeclarationContext context)
{
    string varName = context.ID().GetText();
    string type = context.type().GetText();
    object value = (type == "float") ? 0.0f : 0; // Mantém float corretamente

    if (context.expression() != null)
    {
        value = Visit(context.expression());
    }

    memory[varName] = value;
    return null;
}



        // Processa atribuições (ex: x = x + 1;)
        public override object VisitAssignmentExpression(CSubsetParser.AssignmentExpressionContext context)
{
    string varName = context.ID().GetText();
    object value = Visit(context.expression());

    if (memory.ContainsKey(varName))
    {
        if (memory[varName] is float)
        {
            value = Convert.ToSingle(value); // Converte para float
        }
        else if (memory[varName] is int)
        {
            value = Convert.ToInt32(value); // Mantém inteiro
        }

        memory[varName] = value;
    }
    else
    {
        throw new Exception($"Erro: Variável '{varName}' não declarada.");
    }

    return null;
}


        // Processa expressões matemáticas (ex: x + 1)
        public override object VisitAdditiveExpression(CSubsetParser.AdditiveExpressionContext context)
{
    if (context.ChildCount == 1)
    {
        return Visit(context.GetChild(0));
    }

    var left = Visit(context.GetChild(0));
    var op = context.GetChild(1).GetText();
    var right = Visit(context.GetChild(2));

    if (left is float || right is float)
    {
        float l = Convert.ToSingle(left);
        float r = Convert.ToSingle(right);
        return op == "+" ? l + r : l - r;
    }
    else
    {
        int l = Convert.ToInt32(left);
        int r = Convert.ToInt32(right);
        return op == "+" ? l + r : l - r;
    }
}



        // Processa números, variáveis e chamadas de funções
        public override object VisitPrimary(CSubsetParser.PrimaryContext context)
        {
            if (context.NUMBER() != null)
            {
                return int.Parse(context.NUMBER().GetText());
            }
            else if (context.ID() != null)
            {
                string varName = context.ID().GetText();
                if (memory.ContainsKey(varName))
                    return memory[varName];

                throw new Exception($"Erro: Variável '{varName}' não declarada.");
            }
            return base.VisitPrimary(context);
        }

        // Processa chamadas de função (ex: printf)
        public override object VisitFunctionCall(CSubsetParser.FunctionCallContext context)
{
    string functionName = context.ID().GetText();

    if (functionName == "printf")
    {
        // Obtém a string de formatação (primeiro argumento de printf)
        string formatString = context.expression(0).GetText();

        // Remove aspas da string (caso existam)
        if (formatString.StartsWith("\"") && formatString.EndsWith("\""))
        {
            formatString = formatString.Substring(1, formatString.Length - 2);
        }

        List<object> args = new List<object>();

        // Captura os argumentos (ignorando o primeiro que é a string de formatação)
        for (int i = 1; i < context.expression().Length; i++)
        {
            args.Add(Visit(context.expression(i)));
        }

        // Substituir %d e %f corretamente pelos valores fornecidos
        string formattedString = formatString;
        int argIndex = 0;

        while ((formattedString.Contains("%d") || formattedString.Contains("%f")) && argIndex < args.Count)
        {
            if (formattedString.Contains("%d") && args[argIndex] is int)
            {
                formattedString = formattedString.Replace("%d", args[argIndex].ToString(), StringComparison.Ordinal);
            }
            else if (formattedString.Contains("%f") && args[argIndex] is float)
            {
                formattedString = formattedString.Replace("%f", ((float)args[argIndex]).ToString("F2"), StringComparison.Ordinal);
            }
            argIndex++;
        }

        // Imprime a string formatada corretamente
        Console.WriteLine(formattedString);
        return null;
    }

    throw new Exception($"Erro: Função '{functionName}' não reconhecida.");
}




    }
}
