using System;
using System.Collections.Generic;
using Interpretador.Generated;

namespace Interpretador.Interpreter
{
    public class InterpreterVisitor : CSubsetBaseVisitor<object>
    {
        private readonly Dictionary<string, object> _variables = new(); // Tabela de símbolos

        // 📌 Tratamento de Declarações de Variáveis
        public override object VisitDeclaration(CSubsetParser.DeclarationContext context)
        {
            string id = context.ID().GetText();
            object value = 0; // Define um valor padrão (int = 0, float = 0.0)

            if (context.expression() != null)
            {
                value = Visit(context.expression());
            }

            Console.WriteLine($"🔹 Declarando variável: {id} = {value}");
            _variables[id] = value; // Armazena na tabela de símbolos

            return null;
        }

        // 📌 Tratamento de Expressões Primárias (ID, números, strings)
        public override object VisitPrimary(CSubsetParser.PrimaryContext context)
        {
            if (context.ID() != null)
            {
                string varName = context.ID().GetText();

                if (_variables.ContainsKey(varName))
                {
                    Console.WriteLine($"🔹 Variável encontrada: {varName} = {_variables[varName]}");
                    return _variables[varName]; // Retorna o valor armazenado
                }
                else
                {
                    throw new Exception($"❌ Erro: Variável '{varName}' não foi declarada antes do uso.");
                }
            }
            else if (context.NUMBER() != null)
            {
                return int.Parse(context.NUMBER().GetText()); // Converte números inteiros
            }
            else if (context.STRING() != null)
            {
                return context.STRING().GetText().Trim('"'); // Remove aspas de strings
            }
            else if (context.expression() != null)
            {
                return Visit(context.expression()); // Avalia expressões dentro de parênteses
            }

            throw new Exception("❌ Erro: Expressão primária inválida.");
        }

        // 📌 Tratamento de Expressões Relacionais (>, <, >=, etc.)
        public override object VisitRelationalExpression(CSubsetParser.RelationalExpressionContext context)
        {
            var left = Visit(context.additiveExpression(0));
            var right = Visit(context.additiveExpression(1));

            Console.WriteLine($"🔍 Visitando Expressão Relacional: {context.GetText()}");
            Console.WriteLine($"   ➤ Left: {left?.ToString() ?? "null"}, Right: {right?.ToString() ?? "null"}");

            if (left == null || right == null)
            {
                throw new Exception($"❌ Erro: Um dos operandos da expressão relacional é nulo. Left: {left}, Right: {right}");
            }

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

            throw new Exception($"❌ Erro: Expressão relacional inválida. Tipos inesperados {left?.GetType()} e {right?.GetType()}");
        }

        // 📌 Tratamento da Estrutura Condicional 'if'
        public override object VisitIfStatement(CSubsetParser.IfStatementContext context)
        {
            Console.WriteLine("🔍 Visitando IfStatement");

            if (context == null)
            {
                Console.WriteLine("❌ Erro: contexto nulo.");
                return null;
            }

            Console.WriteLine($"   ➤ Expressão condicional: {context.expression().GetText()}");

            var condition = Visit(context.expression());

            Console.WriteLine($"   ➤ Condição avaliada como: {condition}");

            if (condition is bool cond)
            {
                Console.WriteLine($"   ➤ Condição é booleana: {cond}");

                if (cond)
                {
                    Console.WriteLine("   ✅ Executando bloco if");
                    return Visit(context.statement(0));
                }
                else if (context.statement(1) != null)
                {
                    Console.WriteLine("   ✅ Executando bloco else");
                    return Visit(context.statement(1));
                }
            }
            else
            {
                Console.WriteLine($"❌ Erro: A condição não é booleana. Tipo recebido: {condition?.GetType()}");
            }

            return null;
        }

        // 📌 Tratamento de Chamadas de Função (exemplo: printf)
        public override object VisitFunctionCall(CSubsetParser.FunctionCallContext context)
        {
            if (context.ID().GetText() == "printf")
            {
                string text = Visit(context.expression(0)).ToString();
                Console.WriteLine(text.Replace("\"", "")); // Remove aspas
            }
            return null;
        }
    }
}
