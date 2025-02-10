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

    // 🔹 Verifica se é um array (ex: int arr[3];)
    if (context.GetChild(2).GetText() == "[")
    {
        int size = int.Parse(context.NUMBER().GetText()); // Obtém o tamanho do array
        object[] array = new object[size];

        // Inicializa com valores padrão
        for (int i = 0; i < size; i++)
        {
            array[i] = type switch
            {
                "int" => 0,
                "float" => 0.0f,
                "char" => '\0',
                "string" => "",
                _ => throw new Exception($"Tipo desconhecido: {type}")
            };
        }

        memory[varName] = array; // ✅ Agora armazena corretamente um array
        return null;
    }

    // 🔹 Se for uma variável normal, processa como antes
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

    memory[varName] = value; // ✅ Agora armazena corretamente variáveis normais
    return null;
}



        // Processa atribuições (ex: x = x + 1;)
public override object VisitAssignmentExpression(CSubsetParser.AssignmentExpressionContext context)
{
    string varName = context.ID().GetText();
    object value = Visit(context.expression(0)); // Pegamos a primeira expressão (evita erro)

    // 🔹 Verifica se é um array (ex: arr[2] = 10;)
    if (context.children.Count == 4)  
    {
        int index = Convert.ToInt32(Visit(context.expression(0))); // Obtém o índice

        if (memory.ContainsKey(varName) && memory[varName] is object[] array)
        {
            if (index < 0 || index >= array.Length)
                throw new Exception($"Erro: Índice {index} fora dos limites do array '{varName}'.");

            array[index] = value; // Define o valor no array
            return null;
        }
        else
        {
            throw new Exception($"Erro: '{varName}' não é um array.");
        }
    }

    // 🔹 Se for uma variável normal
    if (memory.ContainsKey(varName))
    {
        memory[varName] = value;
        return null;
    }

    throw new Exception($"Erro: Variável '{varName}' não declarada.");
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
        return context.STRING().GetText().Trim('"');
    }
    else if (context.CHAR() != null)
    {
        string charText = context.CHAR().GetText().Trim('\'');
        if (charText.Length != 1)
        {
            throw new Exception($"Erro: Caractere inválido '{charText}'");
        }
        return charText[0];
    }
    else if (context.ID() != null)
    {
        string varName = context.ID().GetText();

        // 🔹 Verifica se é um acesso a array (ex: arr[0])
        if (context.children.Count == 4)  
        {
            int index = Convert.ToInt32(Visit(context.expression(0))); // ✅ Obtém o índice corretamente

            if (memory.ContainsKey(varName) && memory[varName] is object[] array)
            {
                if (index < 0 || index >= array.Length)
                    throw new Exception($"Erro: Índice {index} fora dos limites do array '{varName}'.");

                return array[index]; // ✅ Retorna corretamente o valor do array
            }
            else
            {
                throw new Exception($"Erro: '{varName}' não é um array.");
            }
        }

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
