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
    object value = Visit(context.expression()); // Avalia a expressão (ex: i + 1)

    if (memory.ContainsKey(varName))
    {
        object currentValue = memory[varName]; // Obtém o valor atual da variável

        // 🔥 DEBUG: Antes da atualização
        Console.WriteLine($"DEBUG: Antes da atualização -> {varName} = {currentValue}");

        // Verifica o tipo da variável de controle e do valor a ser atribuído
        if (currentValue is float || value is float)
        {
            value = Convert.ToSingle(value); // Garante que o valor continue como float
        }
        else if (currentValue is int || value is int)
        {
            value = Convert.ToInt32(value); // Mantém como int se necessário
        }
        else if (currentValue is string || value is string)
        {
            value = Convert.ToString(value); // Armazena como string
        }
        else if (currentValue is char || value is char)
        {
            value = Convert.ToChar(value); // Armazena como char
        }

        memory[varName] = value; // Atualiza a variável na memória

        // 🔥 DEBUG: Depois da atualização
        Console.WriteLine($"DEBUG: Depois da atualização -> {varName} = {memory[varName]}");
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

    // 🔥 DEBUG: Antes da operação
    Console.WriteLine($"DEBUG: Soma antes -> {left} {op} {right}");

    // 🔹 Se left ou right forem variáveis (ID), pegue os valores da memória
    if (left is string leftVar && memory.ContainsKey(leftVar))
    {
        left = memory[leftVar];
    }

    if (right is string rightVar && memory.ContainsKey(rightVar))
    {
        right = memory[rightVar];
    }

    object result;

    if (left is float || right is float)
    {
        float l = Convert.ToSingle(left);
        float r = Convert.ToSingle(right);
        result = op == "+" ? l + r : l - r;
    }
    else
    {
        int l = Convert.ToInt32(left);
        int r = Convert.ToInt32(right);
        result = op == "+" ? l + r : l - r;
    }

    // 🔥 DEBUG: Após a operação
    Console.WriteLine($"DEBUG: Resultado da soma -> {result}");

    return result;
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
    else if (context.STRING() != null)
    {
        // Processa strings (remove as aspas)
        string stringValue = context.STRING().GetText();
        return stringValue.Substring(1, stringValue.Length - 2); // Remove as aspas
    }
    else if (context.CHAR() != null)
    {
        // Processa chars (remove as aspas simples)
        string charValue = context.CHAR().GetText();
        return charValue.Substring(1, charValue.Length - 2)[0]; // Retorna o caractere
    }
    else if (context.ID() != null)
    {
        // Variáveis (busca na memória)
        string varName = context.ID().GetText();
        if (memory.ContainsKey(varName))
            return memory[varName];

        throw new Exception($"Erro: Variável '{varName}' não declarada.");
    }
    else if (context.GetChild(0).GetText() == "!")
    {
        // Negação lógica
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
        // Obtém a string de formatação (primeiro argumento de printf)
        string formatString = context.expression(0).GetText();

        // Remove aspas da string (caso existam)
        if (formatString.StartsWith("\"") && formatString.EndsWith("\""))
        {
            formatString = formatString.Substring(1, formatString.Length - 2);
        }

        // Substituir escape sequences (\n, \t, etc.)
        formatString = formatString.Replace("\\n", "\n").Replace("\\t", "\t");

        List<object> args = new List<object>();

        // Captura os argumentos (ignorando o primeiro que é a string de formatação)
        for (int i = 1; i < context.expression().Length; i++)
        {
            args.Add(Visit(context.expression(i)));
        }

        string formattedString = formatString;
        int argIndex = 0;

        // Substituir %d e %f pelos valores fornecidos
        while ((formattedString.Contains("%d") || formattedString.Contains("%f")) && argIndex < args.Count)
        {
            int placeholderIndex = formattedString.IndexOf("%d");

            if (placeholderIndex == -1 || (formattedString.Contains("%f") && formattedString.IndexOf("%f") < placeholderIndex))
            {
                placeholderIndex = formattedString.IndexOf("%f");
                formattedString = formattedString.Substring(0, placeholderIndex) +
                                  string.Format("{0:F2}", args[argIndex]) +
                                  formattedString.Substring(placeholderIndex + 2);
            }
            else
            {
                formattedString = formattedString.Substring(0, placeholderIndex) +
                                  args[argIndex].ToString() +
                                  formattedString.Substring(placeholderIndex + 2);
            }
            argIndex++;
        }

        // 🔥 DEBUG: Verificar a string formatada antes de imprimir
        Console.WriteLine($"DEBUG: printf -> {formattedString}");

        // Imprime a string formatada corretamente
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
    string varName = null;

    // 1️⃣ Inicialização da variável (ex: int i = 0;)
    if (context.declaration() != null)
    {
        Visit(context.declaration());
        varName = context.declaration().ID().GetText();
    }
    else if (context.assignmentExpression(0) != null)
    {
        Visit(context.assignmentExpression(0));
        varName = context.assignmentExpression(0).ID().GetText();
    }

    if (varName == null || !memory.ContainsKey(varName))
    {
        throw new Exception($"Erro: Variável de controle '{varName}' não encontrada.");
    }

    Console.WriteLine($"DEBUG: Variável '{varName}' inicializada com {memory[varName]}");

    // 2️⃣ Executa o loop enquanto a condição for verdadeira
    while (context.expression() != null && Convert.ToBoolean(Visit(context.expression())))
    {
        Console.WriteLine($"DEBUG: Entrando no loop, {varName} = {memory[varName]}");

        Visit(context.block()); // Executa o bloco do loop

        // 3️⃣ Atualiza a variável de controle dentro da memória
        if (context.assignmentExpression(1) != null)
        {
            Visit(context.assignmentExpression(1));
            Console.WriteLine($"DEBUG: Variável '{varName}' atualizada para {memory[varName]}");
        }

        // 🔥 4️⃣ Reavalia a condição após cada iteração
        if (!Convert.ToBoolean(Visit(context.expression())))
        {
            Console.WriteLine($"DEBUG: Saindo do loop porque {varName} = {memory[varName]} não atende à condição");
            break;
        }
    }

    // 🔥 5️⃣ Se a variável foi declarada dentro do `for`, remova após o loop
    if (context.declaration() != null)
    {
        Console.WriteLine($"DEBUG: Removendo variável '{varName}' da memória após o loop.");
        memory.Remove(varName);
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
