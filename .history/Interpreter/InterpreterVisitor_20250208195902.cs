using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Interpretador.Generated;

namespace Interpretador.Interpreter
{
    public class InterpreterVisitor : CSubsetBaseVisitor<object>
    {
        private readonly Dictionary<string, object> _variables = new();

        // ✅ Declaração de variável
        public override object VisitDeclaration(CSubsetParser.DeclarationContext context)
        {
            string id = context.ID().GetText();
            object value = context.expression() != null ? Visit(context.expression()) : 0;
            _variables[id] = value;
            Console.WriteLine($"Variável '{id}' declarada com valor: {value}");
            return null;
        }

        // ✅ Comando 'if'
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

        // ✅ Função 'printf'
        public override object VisitFunctionCall(CSubsetParser.FunctionCallContext context)
        {
            if (context.ID().GetText() == "printf")
            {
                if (context.expression().Length == 0) 
                {
                    Console.WriteLine("Erro: printf chamado sem argumentos.");
                    return null;
                }

                object exprValue = Visit(context.expression(0));
                string text = exprValue != null ? exprValue.ToString() : "null";
                Console.WriteLine(text.Replace("\"", "")); 
            }
            return null;
        }

        // ✅ Avaliação de expressões (corrigido)
        public override object VisitExpression(CSubsetParser.ExpressionContext context)
        {
            if (context.GetChildCount() == 1) // Expressão simples (número ou variável)
            {
                var child = context.GetChild(0);

                if (child is CSubsetParser.NumberContext numberContext) // 🔹 Verifica se é um número
                {
                    return int.Parse(numberContext.GetText());
                }

                if (child is CSubsetParser.IdentifierContext identifierContext) // 🔹 Verifica se é uma variável
                {
                    string varName = identifierContext.GetText();
                    if (_variables.ContainsKey(varName))
                    {
                        return _variables[varName];
                    }
                    Console.WriteLine($"Erro: Variável '{varName}' não declarada.");
                    return 0;
                }
            }
            
            // 🔹 Implementação para operações matemáticas (Ex: a + b)
            if (context.GetChildCount() == 3) 
            {
                var left = Visit(context.GetChild(0));
                var op = context.GetChild(1).GetText();
                var right = Visit(context.GetChild(2));

                if (left is int l && right is int r)
                {
                    return op switch
                    {
                        "+" => l + r,
                        "-" => l - r,
                        "*" => l * r,
                        "/" => r != 0 ? l / r : throw new DivideByZeroException("Erro: Divisão por zero."),
                        _ => throw new Exception($"Operador desconhecido: {op}")
                    };
                }
            }

            return base.VisitExpression(context);
        }
    }
}
