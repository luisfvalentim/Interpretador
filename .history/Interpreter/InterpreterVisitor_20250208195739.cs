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
            object value = context.expression() != null ? Visit(context.expression()) : 0; // 🔹 Padrão 0 para evitar null
            _variables[id] = value;
            Console.WriteLine($"Variável '{id}' declarada com valor: {value}");
            return null;
        }

        // ✅ Comando 'if'
        public override object VisitIfStatement(CSubsetParser.IfStatementContext context)
        {
            if (context == null) 
            {
                throw new ArgumentNullException(nameof(context), "Erro: IfStatementContext está nulo.");
            }
            if (context.expression() == null) 
            {
                Console.WriteLine("Aviso: expressão dentro do if está nula.");
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

        // ✅ Operação matemática simples (exemplo)
        public override object VisitExpression(CSubsetParser.ExpressionContext context)
        {
            if (context.NUMBER() != null)
            {
                return int.Parse(context.NUMBER().GetText());
            }
            if (context.ID() != null && _variables.ContainsKey(context.ID().GetText()))
            {
                return _variables[context.ID().GetText()];
            }
            return base.VisitExpression(context);
        }
    }
}
