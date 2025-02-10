using System;
using System.Collections.Generic;
using Interpretador.Generated;

namespace Interpretador.Interpreter
{
    public class InterpreterVisitor : CSubsetBaseVisitor<object>
    {
        private Dictionary<string, object> memory = new Dictionary<string, object>(); // Armazena variáveis
        private Dictionary<string, CSubsetParser.FunctionContext> functions = new Dictionary<string, CSubsetParser.FunctionContext>();

        // Processa declarações de variáveis (ex: float x = 10;)
public override object VisitDeclaration(CSubsetParser.DeclarationContext context)
{
    string varName = context.ID().GetText();
    string type = context.type().GetText();
    object? value = null; // ✅ Variáveis começam como `null` para verificar inicialização

    // 🔹 Se for um array, inicializa corretamente
    if (context.children.Count > 3 && context.GetChild(2).GetText() == "[") 
    {
        int size = int.Parse(context.NUMBER().GetText());
        object[] array = new object[size];

        for (int i = 0; i < size; i++)
        {
            array[i] = type switch
            {
                "int" => 0,
                "float" => 0.0f,
                "char" => '\0',
                "string" => "",
                _ => throw new Exception($"Erro: Tipo desconhecido '{type}'.")
            };
        }

        memory[varName] = array;
        return null;
    }

    // Se a variável tem uma atribuição na declaração, avalia a expressão
    if (context.expression() != null)
    {
        value = Visit(context.expression());
    }

    memory[varName] = value; // ✅ Variáveis sem valor inicial permanecem `null`
    return null;
}







        // Processa atribuições (ex: x = x + 1;)
public override object VisitAssignmentExpression(CSubsetParser.AssignmentExpressionContext context)
{
    string varName = context.ID().GetText();
    object value = Visit(context.expression(0));

    // ✅ Verifica se a variável existe antes de usá-la
    if (!memory.ContainsKey(varName))
    {
        throw new Exception($"Erro: Variável '{varName}' não declarada.");
    }

    // ✅ Verifica se a variável foi inicializada corretamente
    CheckInitialization(varName);

    // ✅ Verifica se o tipo é compatível antes de armazenar o valor
    CheckType(varName, value);

    // 🔹 Verifica se é um array (ex: arr[2] = 10;)
    if (context.expression() != null)
    {
        int index = Convert.ToInt32(Visit(context.expression(0)));

        if (memory.ContainsKey(varName) && memory[varName] is object[] array)
        {
            if (index < 0 || index >= array.Length)
                throw new Exception($"Erro: Índice {index} fora dos limites do array '{varName}'.");

            array[index] = value;
            return null;
        }
        else
        {
            throw new Exception($"Erro: '{varName}' não é um array.");
        }
    }

    memory[varName] = value;
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

        // ✅ Verifica se a variável foi inicializada antes de acessá-la
        CheckInitialization(varName);

        return memory[varName];
    }

    return base.VisitPrimary(context);
}


public override object VisitFunction(CSubsetParser.FunctionContext context)
{
    string functionName = context.ID().GetText();
    functions[functionName] = context; // ✅ Armazena a função na memória
    return null; // A função não executa na definição, só quando chamada
}

// Criamos uma exceção especial para retorno antecipado da função
public class ReturnException : Exception
{
    public object ReturnValue { get; }
    
    public ReturnException(object returnValue)
    {
        ReturnValue = returnValue;
    }
}

public override object VisitReturnStatement(CSubsetParser.ReturnStatementContext context)
{
    object returnValue = null;

    if (context.expression() != null)
    {
        returnValue = Visit(context.expression());
    }

    throw new ReturnException(returnValue);
}




        // Processa chamadas de função (ex: printf)
public override object VisitFunctionCall(CSubsetParser.FunctionCallContext context)
{
    string functionName = context.ID().GetText();

    // ✅ Se for `printf`, continua funcionando normalmente
    if (functionName == "printf")
    {
        string formatString = Visit(context.expression(0)).ToString();

        if (formatString.StartsWith("\"") && formatString.EndsWith("\""))
        {
            formatString = formatString.Substring(1, formatString.Length - 2);
        }

        formatString = formatString.Replace("\\n", "\n").Replace("\\t", "\t");

        List<object> args = new List<object>();

        for (int i = 1; i < context.expression().Length; i++)
        {
            args.Add(Visit(context.expression(i)));
        }

        string formattedString = formatString;
        int argIndex = 0;

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

        Console.WriteLine(formattedString);
        return null;
    }

    // ✅ Verifica se a função foi definida
    if (!functions.ContainsKey(functionName))
    {
        throw new Exception($"Erro: Função '{functionName}' não foi definida.");
    }

    CSubsetParser.FunctionContext functionContext = functions[functionName];

    // ✅ Captura os parâmetros da função
    var parameters = functionContext.parameters()?.ID();
    var arguments = context.expression();

    if (parameters != null && parameters.Length != arguments.Length)
    {
        throw new Exception($"Erro: A função '{functionName}' esperava {parameters.Length} argumentos, mas recebeu {arguments.Length}.");
    }

    // ✅ Criar um escopo local para armazenar os parâmetros da função
    Dictionary<string, object> previousMemory = new Dictionary<string, object>(memory);
    memory = new Dictionary<string, object>(previousMemory); // Criar um novo escopo

    // ✅ Armazena os argumentos como variáveis locais
    for (int i = 0; i < parameters?.Length; i++)
    {
        string paramName = parameters[i].GetText();
        object argValue = Visit(arguments[i]);

        //Console.WriteLine($"DEBUG: Armazenando parâmetro '{paramName}' com valor {argValue}");

        memory[paramName] = argValue;
    }

    object returnValue = null;

    try
    {
        // ✅ Agora, a execução da função pode ser interrompida por um `return`
        Visit(functionContext.block());
    }
    catch (ReturnException ex)
    {
        returnValue = ex.ReturnValue; // ✅ Captura o valor de retorno correto
    }

    memory = previousMemory;

    return returnValue;
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

private void CheckType(string varName, object value)
{
    if (!memory.ContainsKey(varName))
    {
        throw new Exception($"Erro: Variável '{varName}' não declarada.");
    }

    object expectedType = memory[varName];

    // 🔹 Garante que a atribuição respeita o tipo original da variável
    if (expectedType is int && !(value is int))
    {
        throw new Exception($"Erro: '{varName}' esperava um inteiro, mas recebeu {value.GetType().Name}.");
    }
    if (expectedType is float && !(value is float || value is int))
    {
        throw new Exception($"Erro: '{varName}' esperava um float, mas recebeu {value.GetType().Name}.");
    }
    if (expectedType is string && !(value is string))
    {
        throw new Exception($"Erro: '{varName}' esperava uma string, mas recebeu {value.GetType().Name}.");
    }
    if (expectedType is char && !(value is char))
    {
        throw new Exception($"Erro: '{varName}' esperava um char, mas recebeu {value.GetType().Name}.");
    }
}

private void CheckInitialization(string varName)
{
    if (!memory.ContainsKey(varName))
    {
        throw new Exception($"Erro: Variável '{varName}' não declarada.");
    }

    // ✅ Agora verifica se a variável foi declarada, mas nunca inicializada
    if (memory[varName] == null)
    {
        throw new Exception($"Erro: Variável '{varName}' usada antes da inicialização.");
    }
}





    }
}
