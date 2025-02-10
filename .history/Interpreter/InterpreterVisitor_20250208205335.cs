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
            object value = 0; // Valor padrão

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
            if (context.additiveExpression() != null)
            {
                var left = (int)Visit(context.additiveExpression());
                var right = (int)Visit(context.multiplicativeExpression());

                return context.GetChild(1).GetText() == "+" ? left + right : left - right;
            }
            return Visit(context.multiplicativeExpression());
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
                string formatString = context.expression(0).GetText().Trim('"');
                List<object> args = new List<object>();

                for (int i = 1; i < context.expression().Length; i++)
                {
                    args.Add(Visit(context.expression(i)));
                }

                Console.WriteLine(string.Format(formatString, args.ToArray()));
                return null;
            }

            throw new Exception($"Erro: Função '{functionName}' não reconhecida.");
        }
    }
}
