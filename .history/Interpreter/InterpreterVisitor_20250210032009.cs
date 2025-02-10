using System;
using System.Collections.Generic;
using Interpretador.Generated;
using Antlr4.Runtime;


namespace Interpretador.Interpreter
{
    public class InterpreterVisitor : CSubsetBaseVisitor<object>
    {
        private Dictionary<string, object> memory = new Dictionary<string, object>(); // Armazena variáveis
        private Dictionary<string, CSubsetParser.FunctionContext> functions = new Dictionary<string, CSubsetParser.FunctionContext>();

        private Dictionary<string, Dictionary<string, object>> structs = new Dictionary<string, Dictionary<string, object>>();
        private Dictionary<string, Dictionary<string, object>> unions = new Dictionary<string, Dictionary<string, object>>();

        private Dictionary<string, object> macros = new Dictionary<string, object>();
        // Processa declarações de variáveis (ex: float x = 10;)
public override object VisitDeclaration(CSubsetParser.DeclarationContext context)
{
    string varName = context.ID().GetText();
    string type = context.type().GetText();
    object? value = null; // ✅ Variáveis começam como `null` para verificar inicialização

    // 🔹 Se for um array, inicializa corretamente
    if (context.ChildCount > 3 && context.GetChild(1).GetText() == "[") // ✅ Corrige a verificação de colchetes
 // ✅ Usa `LBRACKET()` para verificar arrays corretamente
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

        // ✅ Garante que `int` continue `int` e não `float`
        if (type == "int" && value is float floatValue && floatValue == (int)floatValue)
        {
            value = (int)floatValue;
        }
        else if (type == "int" && value is float)
        {
            throw new Exception($"Erro: Tentativa de atribuir Float a variável int '{varName}'.");
        }
    }

    memory[varName] = value; // ✅ Variáveis sem valor inicial permanecem `null`
    return null;
}







        // Processa atribuições (ex: x = x + 1;)
public override object VisitAssignmentExpression(CSubsetParser.AssignmentExpressionContext context)
{
    string varName = context.ID().GetText();
    object value = Visit(context.expression(0));

    if (!memory.ContainsKey(varName))
    {
        throw new Exception($"Erro: Variável '{varName}' não declarada.");
    }

    // ✅ Verifica se é uma atribuição a um array `arr[index] = valor;`
    if (context.expression().Length > 1) // ✅ Confirma que há um índice no array
    {
        object indexObj = Visit(context.expression(0)); // ✅ Obtém o índice corretamente

        if (!(indexObj is int index))
        {
            throw new Exception($"Erro: Índice do array '{varName}' deve ser um inteiro.");
        }

        if (memory[varName] is object[] array)
        {
            if (index < 0 || index >= array.Length)
            {
                throw new Exception($"Erro: Índice {index} fora dos limites do array '{varName}'.");
            }

            // ✅ Atribui corretamente o valor ao índice do array
            array[index] = value;
            return null;
        }
        else
        {
            throw new Exception($"Erro: '{varName}' não é um array.");
        }
    }

    // ✅ Marca a variável como inicializada
    memory[varName] = value;

    CheckInitialization(varName);
    CheckType(varName, value);

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

    // ✅ Se um dos operandos for `string`, concatena strings
    if (left is string && right is string)
    {
        return left.ToString() + right.ToString();
    }

    // ❌ Se um dos operandos for string e o outro não, gera erro
    if (left is string || right is string)
    {
        throw new Exception($"Erro: Operação inválida entre {left.GetType().Name} e {right.GetType().Name}.");
    }

    // ❌ Se um dos operandos for um array ou char, gera erro
    if (left is object[] || right is object[] || left is char || right is char)
    {
        throw new Exception($"Erro: Operação inválida entre {left.GetType().Name} e {right.GetType().Name}.");
    }

    // ✅ Se houver um float, a operação será em ponto flutuante
    if (left is float || right is float)
    {
        return op == "+" ? Convert.ToSingle(left) + Convert.ToSingle(right) :
                           Convert.ToSingle(left) - Convert.ToSingle(right);
    }

    // ✅ Caso contrário, opera como inteiros
    if (left is int && right is int)
    {
        return op == "+" ? (int)left + (int)right : (int)left - (int)right;
    }

    throw new Exception($"Erro: Operação inválida entre {left?.GetType().Name} e {right?.GetType().Name}.");
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

        // ✅ Diferencia inteiros e floats corretamente
        if (numberText.Contains("."))
        {
            return float.Parse(numberText, System.Globalization.CultureInfo.InvariantCulture);
        }
        return int.Parse(numberText);
    }
    else if (context.STRING() != null)
    {
        return context.STRING().GetText().Trim('"'); // ✅ Remove aspas corretamente
    }
    else if (context.CHAR() != null)
    {
        string charText = context.CHAR().GetText().Trim('\'');
        if (charText.Length != 1)
        {
            throw new Exception($"Erro: Caractere inválido '{charText}'.");
        }
        return charText[0]; // ✅ Retorna corretamente um `char`
    }
    else if (context.ID() != null)
    {
        string varName = context.ID().GetText();

        // ✅ Suporte a macros definidas com `#define`
        if (macros.ContainsKey(varName))
        {
            return macros[varName];
        }

        CheckInitialization(varName); // ✅ Verifica inicialização antes de acessar

        // 🔹 Verifica se estamos acessando um índice do array `arr[i]`
        if (context.ChildCount > 3 && context.GetChild(1).GetText() == "[") 
        {
            object indexObj = Visit(context.expression());

            if (!(indexObj is int index))
            {
                throw new Exception($"Erro: Índice do array '{varName}' deve ser um inteiro, mas recebeu {indexObj?.GetType().Name ?? "null"}.");
            }

            if (!memory.ContainsKey(varName))
            {
                throw new Exception($"Erro: Variável '{varName}' não declarada.");
            }

            if (!(memory[varName] is object[] array))
            {
                throw new Exception($"Erro: '{varName}' não é um array.");
            }

            if (index < 0 || index >= array.Length)
            {
                throw new Exception($"Erro: Índice {index} fora dos limites do array '{varName}'.");
            }

            if (array[index] == null)
            {
                throw new Exception($"Erro: Elemento '{varName}[{index}]' usado antes da inicialização.");
            }

            return array[index]; // ✅ Retorna o valor correto do array
        }

        // 🔹 Verifica se estamos acessando um membro de `struct` (ex: `pessoa.idade`)
        if (context.ChildCount == 3 && context.GetChild(1).GetText() == ".")
        {
            string member = context.GetChild(2).GetText();

            if (!memory.ContainsKey(varName))
            {
                throw new Exception($"Erro: Struct '{varName}' não declarada.");
            }

            if (!(memory[varName] is Dictionary<string, object> structData))
            {
                throw new Exception($"Erro: '{varName}' não é uma struct.");
            }

            if (!structData.ContainsKey(member))
            {
                throw new Exception($"Erro: Membro '{member}' não encontrado na struct '{varName}'.");
            }

            return structData[member]; // ✅ Retorna o valor do membro da struct
        }

        // 🔹 Se for uma variável normal, retorna seu valor
        if (memory.ContainsKey(varName))
        {
            return memory[varName];
        }

        throw new Exception($"Erro: Variável '{varName}' não declarada.");
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

    // ✅ `printf` - Formatação e saída formatada
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

        while ((formattedString.Contains("%d") || formattedString.Contains("%f") || formattedString.Contains("%s") || formattedString.Contains("%c")) && argIndex < args.Count)
        {
            if (formattedString.Contains("%d"))
            {
                formattedString = formattedString.Replace("%d", args[argIndex].ToString());
            }
            else if (formattedString.Contains("%f"))
            {
                formattedString = formattedString.Replace("%f", string.Format("{0:F2}", args[argIndex]));
            }
            else if (formattedString.Contains("%s"))
            {
                formattedString = formattedString.Replace("%s", args[argIndex].ToString());
            }
            else if (formattedString.Contains("%c"))
            {
                formattedString = formattedString.Replace("%c", args[argIndex].ToString());
            }
            argIndex++;
        }

        Console.WriteLine(formattedString);
        return null;
    }

    // ✅ `scanf` - Lê entrada do usuário e armazena na variável
    if (functionName == "scanf")
    {
        if (context.expression().Length < 2)
        {
            throw new Exception("Erro: scanf requer pelo menos dois argumentos (formato e variável).");
        }

        string format = context.expression(0).GetText().Trim('"'); // Obtém o formato
        string varName = context.expression(1).GetText(); // Obtém o nome da variável

        if (!memory.ContainsKey(varName))
        {
            throw new Exception($"Erro: Variável '{varName}' não declarada.");
        }

        Console.Write($"Entrada para {varName}: ");
        string input = Console.ReadLine();

        if (format.Contains("%d")) // Inteiro
        {
            memory[varName] = int.TryParse(input, out int intValue) ? intValue : throw new Exception($"Erro: Entrada inválida para inteiro em '{varName}'.");
        }
        else if (format.Contains("%f")) // Float
        {
            memory[varName] = float.TryParse(input, System.Globalization.CultureInfo.InvariantCulture, out float floatValue) ? floatValue : throw new Exception($"Erro: Entrada inválida para float em '{varName}'.");
        }
        else if (format.Contains("%c")) // Caractere
        {
            memory[varName] = input.Length == 1 ? input[0] : throw new Exception($"Erro: Entrada inválida para char em '{varName}'.");
        }
        else if (format.Contains("%s")) // String
        {
            memory[varName] = input;
        }
        else
        {
            throw new Exception("Erro: Formato de `scanf` não reconhecido.");
        }

        return null;
    }

    // ✅ `gets` - Lê uma linha inteira do usuário e armazena na variável
    if (functionName == "gets")
    {
        if (context.expression().Length != 1)
        {
            throw new Exception("Erro: gets requer exatamente um argumento (variável).");
        }

        string varName = context.expression(0).GetText();

        if (!memory.ContainsKey(varName))
        {
            throw new Exception($"Erro: Variável '{varName}' não declarada.");
        }

        Console.Write($"Entrada para {varName}: ");
        memory[varName] = Console.ReadLine();

        return null;
    }

    // ✅ `puts` - Imprime uma string na tela
    if (functionName == "puts")
    {
        if (context.expression().Length != 1)
        {
            throw new Exception("Erro: puts requer exatamente um argumento (string).");
        }

        object value = Visit(context.expression(0));

        if (value is string strValue)
        {
            Console.WriteLine(strValue);
        }
        else
        {
            throw new Exception("Erro: puts só pode imprimir strings.");
        }

        return null;
    }

    // ✅ `atoi` - Converte string para int
    if (functionName == "atoi")
    {
        if (context.expression().Length != 1)
        {
            throw new Exception("Erro: atoi requer exatamente um argumento (string).");
        }

        object value = Visit(context.expression(0));

        return value is string strValue && int.TryParse(strValue, out int result) ? result : throw new Exception("Erro: atoi não pôde converter a string para inteiro.");
    }

    // ✅ `atof` - Converte string para float
    if (functionName == "atof")
    {
        if (context.expression().Length != 1)
        {
            throw new Exception("Erro: atof requer exatamente um argumento (string).");
        }

        object value = Visit(context.expression(0));

        return value is string strValue && float.TryParse(strValue, System.Globalization.CultureInfo.InvariantCulture, out float result) ? result : throw new Exception("Erro: atof não pôde converter a string para float.");
    }

    // ✅ `itoa` - Converte int para string
    if (functionName == "itoa")
{
    if (context.expression().Length != 1)
    {
        throw new Exception("Erro: itoa requer exatamente um argumento (inteiro).");
    }

    object value = Visit(context.expression(0));

    // ✅ Se o valor for `float`, mas sem parte decimal, converte para `int`
    if (value is float floatValue && floatValue == (int)floatValue)
    {
        return ((int)floatValue).ToString(); // 🔹 Converte `42.0` para `"42"`
    }

    // ❌ Se for um `float` real, gera erro
    if (value is float)
    {
        throw new Exception($"Erro: itoa só pode converter inteiros para string, mas recebeu Float ({value}). Use `ftoa` para converter floats.");
    }

    // ✅ Se já for `int`, converte normalmente
    if (value is int intValue)
    {
        return intValue.ToString();
    }

    throw new Exception($"Erro: itoa só pode converter inteiros para string, mas recebeu {value?.GetType().Name ?? "null"}.");
}






    if (functionName == "ftoa")
{
    if (context.expression().Length != 1)
    {
        throw new Exception("Erro: ftoa requer exatamente um argumento (float).");
    }

    object value = Visit(context.expression(0));

    // ✅ Se o valor for `int`, mas deveria ser `float`, lança erro correto
    if (value is int intValue)
    {
        throw new Exception($"Erro: ftoa só pode converter floats para string, mas recebeu Int ({intValue}). Use `itoa` para converter inteiros.");
    }

    // ✅ Se o valor for realmente um `float`, converte corretamente
    if (value is float floatValue)
    {
        return floatValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    throw new Exception($"Erro: ftoa só pode converter floats para string, mas recebeu {value?.GetType().Name ?? "null"}.");
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

    Dictionary<string, object> previousMemory = new Dictionary<string, object>(memory);
    memory = new Dictionary<string, object>(previousMemory);

    for (int i = 0; i < parameters?.Length; i++)
    {
        string paramName = parameters[i].GetText();
        object argValue = Visit(arguments[i]);
        memory[paramName] = argValue;
    }

    object returnValue = null;

    try
    {
        Visit(functionContext.block());
    }
    catch (ReturnException ex)
    {
        returnValue = ex.ReturnValue;
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

// ✅ Exceção personalizada para controle de `break`
public class BreakException : Exception
{
    public BreakException() : base("Break statement executed.") { }
}


public override object VisitBreakStatement(CSubsetParser.BreakStatementContext context)
{
    throw new BreakException();
}

public override object VisitWhileStatement(CSubsetParser.WhileStatementContext context)
{
    while (Convert.ToBoolean(Visit(context.expression())))
    {
        try
        {
            Visit(context.block());
        }
        catch (BreakException)
        {
            break; // ✅ Captura e interrompe o `while` corretamente
        }
    }
    return null;
}

public override object VisitForStatement(CSubsetParser.ForStatementContext context)
{
    List<string> varNames = new List<string>();

    // 1️⃣ Processa inicialização (Declaração OU Atribuição)
    if (context.declaration() != null) 
    {
        var decl = context.declaration();
        Visit(decl); // ✅ Processa a declaração de variável
        varNames.Add(decl.ID().GetText()); // ✅ Obtém o nome da variável declarada
    }
    else if (context.assignmentExpression(0) != null)
    {
        Visit(context.assignmentExpression(0)); // ✅ Processa a atribuição inicial
        varNames.Add(context.assignmentExpression(0).ID().GetText());
    }

    // 2️⃣ Verifica se a variável de controle foi corretamente declarada
    foreach (var varName in varNames)
    {
        if (!memory.ContainsKey(varName))
        {
            throw new Exception($"Erro: Variável de controle '{varName}' não encontrada.");
        }
    }

    // 3️⃣ Executa o loop enquanto a condição for verdadeira
    while (context.expression() == null || Convert.ToBoolean(Visit(context.expression())))
    {
        try
        {
            Visit(context.block()); // ✅ Executa o bloco do loop

            // 4️⃣ Atualiza a variável de controle
            if (context.assignmentExpression(1) != null)
            {
                Visit(context.assignmentExpression(1)); // ✅ Atualiza `i` ou outra variável de controle
            }
        }
        catch (BreakException)
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

public override object VisitStructDeclaration(CSubsetParser.StructDeclarationContext context)
{
    string structName = context.ID().GetText();
    Dictionary<string, object> members = new Dictionary<string, object>();

    foreach (var member in context.structMember())
    {
        string memberName = member.ID().GetText();
        string memberType = member.type().GetText();

        object defaultValue = memberType switch
        {
            "int" => 0,
            "float" => 0.0f,
            "char" => '\0',
            "string" => "",
            _ => throw new Exception($"Erro: Tipo desconhecido '{memberType}'.")
        };

        members[memberName] = defaultValue;
    }

    structs[structName] = members;
    return null;
}

public override object VisitStructInstance(CSubsetParser.StructInstanceContext context)
{
    string structName = context.GetChild(0).GetText(); // Nome da struct (ex: "Pessoa")
    string instanceName = context.GetChild(1).GetText(); // Nome da variável (ex: "p")

    // ✅ Verifica se a `struct` foi declarada antes de ser usada
    if (!structs.ContainsKey(structName))
    {
        throw new Exception($"Erro: Struct '{structName}' não foi declarada.");
    }

    // ✅ Cria uma nova instância com os membros da `struct`
    var newInstance = new Dictionary<string, object>(structs[structName]);

    memory[instanceName] = newInstance; // ✅ Armazena a instância na memória
    return null;
}


public override object VisitUnionDeclaration(CSubsetParser.UnionDeclarationContext context)
{
    string unionName = context.ID().GetText();
    Dictionary<string, object> members = new Dictionary<string, object>();

    foreach (var member in context.structMember())
    {
        string memberName = member.ID().GetText();
        string memberType = member.type().GetText();

        object defaultValue = memberType switch
        {
            "int" => 0,
            "float" => 0.0f,
            "char" => '\0',
            "string" => "",
            _ => throw new Exception($"Erro: Tipo desconhecido '{memberType}'.")
        };

        members[memberName] = defaultValue;
    }

    unions[unionName] = members;
    return null;
}


public override object VisitExpression(CSubsetParser.ExpressionContext context)
{
    // ✅ Trata `structInstance.membro`
    if (context.ChildCount == 3 && context.GetChild(1).GetText() == ".")
    {
        string structVar = context.GetChild(0).GetText();
        string member = context.GetChild(2).GetText();

        // ✅ Verifica se a variável é uma `struct`
        if (!memory.ContainsKey(structVar))
        {
            throw new Exception($"Erro: Variável '{structVar}' não declarada.");
        }

        if (!(memory[structVar] is Dictionary<string, object> structData))
        {
            throw new Exception($"Erro: '{structVar}' não é uma struct.");
        }

        // ✅ Verifica se o membro existe
        if (!structData.ContainsKey(member))
        {
            throw new Exception($"Erro: Membro '{member}' não existe na struct '{structVar}'.");
        }

        return structData[member]; // ✅ Retorna o valor do membro
    }

    return base.VisitExpression(context);
}

public override object VisitPreprocessorDirective(CSubsetParser.PreprocessorDirectiveContext context)
{
    string directive = context.GetChild(1).GetText(); // Obtém `include`, `define` ou `error`
    
    switch (directive)
    {
        case "include":
            string fileName = "";

            if (context.STRING() != null) // `#include "arquivo.h"`
            {
                fileName = context.STRING().GetText().Trim('"');
            }
            else if (context.ID(0) != null && context.ID(1) != null) // `#include <stdio.h>`
            {
                fileName = context.ID(0).GetText() + "." + context.ID(1).GetText();
            }
            else
            {
                throw new Exception($"❌ Erro: Diretiva `#include` mal formada.");
            }

            // 🔹 Define os diretórios onde buscará os arquivos incluídos
            string[] searchPaths = { ".", "./includes", "/usr/include", "/usr/local/include" };
            string filePath = searchPaths.Select(path => Path.Combine(path, fileName))
                                         .FirstOrDefault(File.Exists);

            if (filePath == null)
            {
                throw new Exception($"❌ Erro: Arquivo incluído '{fileName}' não encontrado.\n" +
                                    $"Caminhos verificados:\n{string.Join("\n", searchPaths.Select(path => $"- {Path.Combine(path, fileName)}"))}");
            }

            Console.WriteLine($"🔹 Incluindo arquivo: {filePath}");
            string fileContent = File.ReadAllText(filePath);

            // ✅ Agora processamos o conteúdo do arquivo incluído!
            Antlr4.Runtime.ICharStream inputStream = CharStreams.fromString(fileContent);
            CSubsetLexer lexer = new CSubsetLexer(inputStream);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            CSubsetParser parser = new CSubsetParser(tokenStream);
            CSubsetParser.ProgramContext includedTree = parser.program();

            // ✅ Visita a árvore do arquivo incluído!
            Visit(includedTree);
            break;

        case "define":
            string macroName = context.ID(0).GetText();
            object macroValue = Visit(context.expression());

            if (!macros.ContainsKey(macroName))
            {
                macros[macroName] = macroValue;
                Console.WriteLine($"✅ Macro '{macroName}' definida como: {macroValue}");
            }
            else
            {
                throw new Exception($"Erro: Macro '{macroName}' já foi definida anteriormente.");
            }
            break;
            
        default:
            throw new Exception($"Erro: Diretiva '{directive}' não reconhecida.");
    }

    return null;
}








    }
}
