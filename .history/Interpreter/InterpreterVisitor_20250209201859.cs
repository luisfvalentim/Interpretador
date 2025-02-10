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
    object value = null;

    if (context.expression() != null)
    {
        value = Visit(context.expression());

        if (type == "float" && value is int)
        {
            value = Convert.ToSingle(value);
        }
        else if (type == "char" && value is string s && s.Length == 1)
        {
            value = s[0]; // Transforma string de um caractere em `char`
        }
        else if (type == "string" && value is not string)
        {
            value = Convert.ToString(value);
        }
    }

    // Define valores padrão
    if (value == null)
    {
        value = type switch
        {
            "int" => 0,
            "float" => 0.0f,
            "char" => '\0',
            "string" => "",
            _ => throw new Exception($"Tipo desconhecido: {type}")
        };
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
        object currentValue = memory[varName];

        if (currentValue is float || value is float)
        {
            value = Convert.ToSingle(value);
        }
        else if (currentValue is int || value is int)
        {
            value = Convert.ToInt32(value);
        }
        else if (currentValue is string || value is string)
        {
            value = Convert.ToString(value);
        }
        else if (currentValue is char)
        {
            if (value is string s && s.Length == 1)
                value = s[0]; // Converte string de um caractere para `char`
            else
                throw new Exception($"Erro: valor inválido para char: {value}");
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

    if (left is string || right is string)
    {
        return left.ToString() + right.ToString(); // Concatena strings
    }

    if (left is float || right is float)
    {
        return op == "+" ? Convert.ToSingle(left) + Convert.ToSingle(right) :
                           Convert.ToSingle(left) - Convert.ToSingle(right);
    }

    return op == "+" ? Convert.ToInt32(left) + Convert.ToInt32(right) :
                       Convert.ToInt32(left) - Convert.ToInt32(right);
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
        return numberText.Contains(".") ? float.Parse(numberText, System.Globalization.CultureInfo.InvariantCulture)
                                        : int.Parse(numberText);
    }
    else if (context.STRING() != null)
    {
        return context.STRING().GetText().Trim('"'); // Remove aspas
    }
    else if (context.CHAR() != null)
    {
        string charText = context.CHAR().GetText().Trim('\'');
        if (charText.Length != 1)
        {
            throw new Exception($"Erro: Caractere inválido '{charText}'");
        }
        return charText[0]; // Retorna `char`
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
        string formatString = context.expression(0).GetText().Trim('"');
        formatString = formatString.Replace("\\n", "\n").Replace("\\t", "\t");

        List<object> args = new List<object>();
        for (int i = 1; i < context.expression().Length; i++)
        {
            args.Add(Visit(context.expression(i)));
        }

        int argIndex = 0;
        while ((formatString.Contains("%d") || formatString.Contains("%f") || formatString.Contains("%c") || formatString.Contains("%s"))
                && argIndex < args.Count)
        {
            if (formatString.Contains("%s") && args[argIndex] is string)
            {
                formatString = formatString.Replace("%s", args[argIndex].ToString(), 1);
            }
            else if (formatString.Contains("%c") && args[argIndex] is char)
            {
                formatString = formatString.Replace("%c", args[argIndex].ToString(), 1);
            }
            else if (formatString.Contains("%f"))
            {
                formatString = formatString.Replace("%f", string.Format("{0:F2}", args[argIndex]), 1);
            }
            else if (formatString.Contains("%d"))
            {
                formatString = formatString.Replace("%d", args[argIndex].ToString(), 1);
            }

            argIndex++;
        }

        Console.WriteLine(formatString);
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
    string varName = null;

    // 1️⃣ Inicialização da variável (ex: int i = 0;)
    if (context.declaration() != null)
    {
        Visit(context.declaration()); // Processa a declaração de variável
        varName = context.declaration().ID().GetText(); // Obtém o nome da variável
    }
    else if (context.assignmentExpression(0) != null)
    {
        Visit(context.assignmentExpression(0)); // Processa a atribuição inicial (i = 0)
        varName = context.assignmentExpression(0).ID().GetText(); // Obtém o nome da variável
    }

    if (varName == null || !memory.ContainsKey(varName))
    {
        throw new Exception($"Erro: Variável de controle '{varName}' não encontrada.");
    }

    // 2️⃣ Executa o loop enquanto a condição for verdadeira
    while (context.expression() != null && Convert.ToBoolean(Visit(context.expression())))
    {
        Visit(context.block()); // Executa o bloco do loop

        // 3️⃣ Atualiza a variável de controle dentro da memória
        if (context.assignmentExpression(1) != null)
        {
            Visit(context.assignmentExpression(1)); // Atualiza o valor de `i` na memória
        }

        // 🔥 4️⃣ Reavalia a condição após cada iteração
        if (!Convert.ToBoolean(Visit(context.expression())))
        {
            break;
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
