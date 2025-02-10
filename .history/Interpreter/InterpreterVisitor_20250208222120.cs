using System;
using System.Collections.Generic;
using Interpretador.Generated;

namespace Interpretador.Interpreter
{
    public class InterpreterVisitor : CSubsetBaseVisitor<object>
    {
        private Dictionary<string, object> memory = new Dictionary<string, object>(); // Armazena variáveis

        // Processa declarações de variáveis (ex: float x = 10;)
        public override object VisitDeclaration(CSubsetParser.DeclarationContext context)
        {
            string varName = context.ID().GetText();
            string type = context.type().GetText();
            object value = (type == "float") ? 0.0f : 0; // Mantém float corretamente

            if (context.expression() != null)
            {
                value = Visit(context.expression());

                // Converte inteiros para float caso a variável seja do tipo float
                if (type == "float" && value is int)
                {
                    value = Convert.ToSingle(value);
                }
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
                if (memory[varName] is float || value is float)
                {
                    value = Convert.ToSingle(value); // Garante que o valor continue como float
                }
                else
                {
                    value = Convert.ToInt32(value); // Mantém como int se necessário
                }

                memory[varName] = value;
            }
            else
            {
                throw new Exception($"Erro: Variável '{varName}' não declarada.");
            }

            return null;
        }

        // Processa expressões matemáticas (ex: x + 1, x * 2)
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

        public override object VisitMultiplicativeExpression(CSubsetParser.MultiplicativeExpressionContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.GetChild(0));
            }

            var left = Visit(context.GetChild(0));
            var op = context.GetChild(1).GetText();
            var right = Visit(context.GetChild(2));

            if (left is float || right is float) // Garante que a operação preserve float
            {
                float l = Convert.ToSingle(left);
                float r = Convert.ToSingle(right);

                return op == "*" ? l * r :
                       op == "/" ? l / r :
                       l % r;
            }
            else // Caso contrário, opera como inteiro
            {
                int l = Convert.ToInt32(left);
                int r = Convert.ToInt32(right);

                return op == "*" ? l * r :
                       op == "/" ? l / r :
                       l % r;
            }
        }

        // Processa expressões lógicas (&&, ||)
        public override object VisitLogicalExpression(CSubsetParser.LogicalExpressionContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.GetChild(0));
            }

            var left = Convert.ToBoolean(Visit(context.GetChild(0)));
            var op = context.GetChild(1).GetText();
            var right = Convert.ToBoolean(Visit(context.GetChild(2)));

            return op == "&&" ? left && right : left || right;
        }

        // Processa expressões de igualdade (==, !=)
        public override object VisitEqualityExpression(CSubsetParser.EqualityExpressionContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.GetChild(0));
            }

            var left = Visit(context.GetChild(0));
            var op = context.GetChild(1).GetText();
            var right = Visit(context.GetChild(2));

            return op == "==" ? left.Equals(right) : !left.Equals(right);
        }

        // Processa expressões relacionais (<, >, <=, >=)
        public override object VisitRelationalExpression(CSubsetParser.RelationalExpressionContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.GetChild(0));
            }

            var left = Convert.ToDouble(Visit(context.GetChild(0)));
            var op = context.GetChild(1).GetText();
            var right = Convert.ToDouble(Visit(context.GetChild(2)));

            return op == ">" ? left > right :
                   op == "<" ? left < right :
                   op == ">=" ? left >= right :
                   left <= right;
        }

        // Processa números, variáveis e chamadas de funções
        public override object VisitPrimary(CSubsetParser.PrimaryContext context)
        {
            if (context.NUMBER() != null)
            {
                string numberText = context.NUMBER().GetText();
                if (numberText.Contains("."))
                {
                    return float.Parse(numberText, System.Globalization.CultureInfo.InvariantCulture);
                }
                return int.Parse(numberText);
            }
            else if (context.ID() != null)
            {
                string varName = context.ID().GetText();
                if (memory.ContainsKey(varName))
                    return memory[varName];

                throw new Exception($"Erro: Variável '{varName}' não declarada.");
            }
            else if (context.GetChild(0).GetText() == "!")
            {
                return !Convert.ToBoolean(Visit(context.GetChild(1)));
            }

            return base.VisitPrimary(context);
        }

        // Processa chamadas de função (ex: printf)
        public override object VisitFunctionCall(CSubsetParser.FunctionCallContext context)
        {
            string functionName = context.ID().GetText();

            if (functionName == "printf")
            {
                string formatString = context.expression(0).GetText();

                if (formatString.StartsWith("\"") && formatString.EndsWith("\""))
                {
                    formatString = formatString.Substring(1, formatString.Length - 2);
                }

                List<object> args = new List<object>();

                for (int i = 1; i < context.expression().Length; i++)
                {
                    args.Add(Visit(context.expression(i)));
                }

                string formattedString = formatString;
                int argIndex = 0;

                while ((formattedString.Contains("%d") || formattedString.Contains("%f")) && argIndex < args.Count)
                {
                    if (formattedString.Contains("%d") && args[argIndex] is int)
                    {
                        formattedString = formattedString.Replace("%d", args[argIndex].ToString());
                    }
                    else if (formattedString.Contains("%f") && args[argIndex] is float)
                    {
                        formattedString = formattedString.Replace("%f", ((float)args[argIndex]).ToString("F2"));
                    }
                    argIndex++;
                }

                formattedString = formattedString.Replace("%f", "0.00");
                Console.WriteLine(formattedString);
                return null;
            }

            throw new Exception($"Erro: Função '{functionName}' não reconhecida.");
        }

        public override object VisitIfStatement(CSubsetParser.IfStatementContext context)
{
    bool condition = Convert.ToBoolean(Visit(context.expression()));

    if (condition)
    {
        Visit(context.block(0)); // Executa o bloco do `if`
    }
    else if (context.block().Length > 1) // Se houver `else`, executa o segundo bloco
    {
        Visit(context.block(1));
    }

    return null;
}

public override object VisitWhileStatement(CSubsetParser.WhileStatementContext context)
{
    while (Convert.ToBoolean(Visit(context.expression())))
    {
        Visit(context.block());
    }
    return null;
}

public override object VisitForStatement(CSubsetParser.ForStatementContext context)
{
    // 1️⃣ Inicialização (ex: int i = 0;)
    if (context.declaration() != null)
    {
        Visit(context.declaration()); // Executa a declaração de variável
    }
    else if (context.assignmentExpression(0) != null)
    {
        Visit(context.assignmentExpression(0)); // Executa a atribuição inicial
    }

    // 2️⃣ Executa o loop enquanto a condição for verdadeira
    while (context.expression() != null && Convert.ToBoolean(Visit(context.expression())))
    {
        Visit(context.block()); // Executa o bloco do loop

        // 3️⃣ Incremento do for (ex: i = i + 1;)
        if (context.assignmentExpression(1) != null)
        {
            Visit(context.assignmentExpression(1));
        }

        // 4️⃣ 🔥 REAVALIA A CONDIÇÃO PARA SAIR DO LOOP
        if (!Convert.ToBoolean(Visit(context.expression())))
        {
            break; // Sai do loop se a condição não for mais verdadeira
        }
    }

    return null;
}



public override object VisitDoWhileStatement(CSubsetParser.DoWhileStatementContext context)
{
    do
    {
        Visit(context.block());
    } while (Convert.ToBoolean(Visit(context.expression())));

    return null;
}

public override object VisitSwitchStatement(CSubsetParser.SwitchStatementContext context)
{
    var switchValue = Visit(context.expression());

    foreach (var caseStmt in context.caseStatement())
    {
        var caseValue = int.Parse(caseStmt.NUMBER().GetText());

        if (switchValue.Equals(caseValue))
        {
            foreach (var stmt in caseStmt.statement())
            {
                Visit(stmt);
            }
            return null; // Sai após encontrar o case correto
        }
    }

    if (context.defaultStatement() != null)
    {
        foreach (var stmt in context.defaultStatement().statement())
        {
            Visit(stmt);
        }
    }

    return null;
}

    }
}
