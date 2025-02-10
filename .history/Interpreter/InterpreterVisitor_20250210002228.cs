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

    // 🔹 Verifica se é um array
    if (context.children.Count > 3 && context.GetChild(2).GetText() == "[") 
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

        // ✅ Depuração: Verifica se o array foi armazenado corretamente
        //Console.WriteLine($"DEBUG: '{varName}' foi armazenado como {memory[varName].GetType()}");

        return null;
    }

    // Se for variável normal
    object value = null;
    if (context.expression() != null)
    {
        value = Visit(context.expression());
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

    memory[varName] = value;

    // ✅ Depuração: Verifica se a variável foi armazenada corretamente
    //Console.WriteLine($"DEBUG: '{varName}' foi armazenado como {memory[varName].GetType()}");

    return null;
}





        // Processa atribuições (ex: x = x + 1;)
public override object VisitAssignmentExpression(CSubsetParser.AssignmentExpressionContext context)
{
    string varName = context.ID().GetText();
    object value = Visit(context.expression(0)); // ✅ Obtém o valor certo

    // ✅ Depuração: Verifica se a variável já existe antes da modificação
    if (memory.ContainsKey(varName))
    {
        Console.WriteLine($"DEBUG: Antes da atribuição, '{varName}' é {memory[varName].GetType()}");
    }

    // 🔹 Verifica se é um array (ex: arr[2] = 10;)
    if (context.expression() != null)
    {
        int index = Convert.ToInt32(Visit(context.expression(0)));

        if (memory.ContainsKey(varName) && memory[varName] is object[] array)
        {
            if (index < 0 || index >= array.Length)
                throw new Exception($"Erro: Índice {index} fora dos limites do array '{varName}'.");

            array[index] = value;

            // ✅ Depuração: Verifica a modificação correta
            Console.WriteLine($"DEBUG: Modificação bem-sucedida de '{varName}[{index}]' para {value}");
            return null;
        }
        else
        {
            throw new Exception($"Erro: '{varName}' não é um array.");
        }
    }

    memory[varName] = value;

    // ✅ Depuração: Após a atribuição, verifica o tipo armazenado
    Console.WriteLine($"DEBUG: Após a atribuição, '{varName}' agora é {memory[varName].GetType()}");

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

        // ✅ Depuração: Verifica o tipo da variável antes de acessá-la
        // if (memory.ContainsKey(varName))
        // {
        //     Console.WriteLine($"DEBUG: Verificando acesso a '{varName}', tipo armazenado: {memory[varName].GetType()}");
        // }

        // ✅ Verifica se a variável é um array ANTES de tentar acessar índices
        if (context.expression() != null)  
        {
            if (!memory.ContainsKey(varName))
                throw new Exception($"Erro: Variável '{varName}' não declarada.");

            // ✅ Se a variável NÃO for um array, não pode usar colchetes `[]`
            if (!(memory[varName] is object[]))
                throw new Exception($"Erro: '{varName}' não é um array, então não pode ser acessado como 'arr[i]'.");

            int index = Convert.ToInt32(Visit(context.expression())); // ✅ Obtém o índice corretamente

            if (index < 0 || index >= ((object[])memory[varName]).Length)
                throw new Exception($"Erro: Índice {index} fora dos limites do array '{varName}'.");

            return ((object[])memory[varName])[index]; // ✅ Retorna corretamente o valor do array
        }

        // ✅ Se `varName` for uma variável normal, retorna diretamente
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

    }
}
