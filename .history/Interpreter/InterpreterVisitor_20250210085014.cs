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
        public override object VisitDeclaration(CSubsetParser.DeclarationContext context)
        {
            string varName = context.ID().GetText();
            string type = context.type().GetText();
            object? value = null;

            if (context.ChildCount > 3 && context.GetChild(1).GetText() == "[")
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

            if (context.expression() != null)
            {
                value = Visit(context.expression());

                if (type == "int" && value is float floatValue && floatValue == (int)floatValue)
                {
                    value = (int)floatValue;
                }
                else if (type == "int" && value is float)
                {
                    throw new Exception($"Erro: Tentativa de atribuir Float a variável int '{varName}'.");
                }
            }

            memory[varName] = value;
            return null;
        }

        public override object VisitAssignmentExpression(CSubsetParser.AssignmentExpressionContext context)
        {
            string varName = context.ID().GetText();
            object value = Visit(context.expression(0));

            if (!memory.ContainsKey(varName))
            {
                throw new Exception($"Erro: Variável '{varName}' não declarada.");
            }

            if (context.expression().Length > 1)
            {
                object indexObj = Visit(context.expression(0));

                if (!(indexObj is int index))
                {
                    throw new Exception($"Erro: Índice do array '{varName}' deve ser um inteiro.");
                }

                if (!(memory[varName] is object[] array))
                {
                    throw new Exception($"Erro: '{varName}' não é um array.");
                }

                if (index < 0 || index >= array.Length)
                {
                    throw new Exception($"Erro: Índice {index} fora dos limites do array '{varName}'.");
                }

                CheckType(varName, value);
                array[index] = value;
                return null;
            }

            CheckType(varName, value);
            memory[varName] = value;

            return null;
        }


        public override object VisitAdditiveExpression(CSubsetParser.AdditiveExpressionContext context)
        {
            if (context.ChildCount == 1)
            {
                return Visit(context.GetChild(0));
            }

            var left = Visit(context.GetChild(0));
            var op = context.GetChild(1).GetText();
            var right = Visit(context.GetChild(2));

            if (left is string && right is string)
            {
                return left.ToString() + right.ToString();
            }

            if (left is string || right is string)
            {
                throw new Exception($"Erro: Operação inválida entre {left.GetType().Name} e {right.GetType().Name}.");
            }

            if (left is object[] || right is object[] || left is char || right is char)
            {
                throw new Exception($"Erro: Operação inválida entre {left.GetType().Name} e {right.GetType().Name}.");
            }

            if (left is float || right is float)
            {
                return op == "+" ? Convert.ToSingle(left) + Convert.ToSingle(right) :
                                   Convert.ToSingle(left) - Convert.ToSingle(right);
            }

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

            if (left is float || right is float)
            {
                float l = Convert.ToSingle(left);
                float r = Convert.ToSingle(right);

                return op == "*" ? l * r :
                       op == "/" ? l / r :
                       l % r;
            }
            else
            {
                int l = Convert.ToInt32(left);
                int r = Convert.ToInt32(right);

                return op == "*" ? l * r :
                       op == "/" ? l / r :
                       l % r;
            }
        }

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
                return context.STRING().GetText().Trim('"');
            }
            else if (context.CHAR() != null)
            {
                string charText = context.CHAR().GetText().Trim('\'');
                if (charText.Length != 1)
                {
                    throw new Exception($"Erro: Caractere inválido '{charText}'.");
                }
                return charText[0];
            }
            else if (context.ID() != null)
            {
                string varName = context.ID().GetText();

                if (macros.ContainsKey(varName))
                {
                    return macros[varName];
                }

                CheckInitialization(varName);

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

                    return array[index];
                }

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

                    return structData[member];
                }

                if (memory.ContainsKey(varName))
                {
                    return memory[varName];
                }

                throw new Exception($"Erro: Variável '{varName}' não declarada.");
            }

            return base.VisitPrimary(context);
        }



        public override object VisitProgram(CSubsetParser.ProgramContext context)
        {
            VisitChildren(context);

            if (!functions.ContainsKey("main"))
            {
                throw new Exception("Erro: Função `main` não encontrada.");
            }

            Visit(functions["main"].block());

            return null;
        }




        public override object VisitFunction(CSubsetParser.FunctionContext context)
        {
            string functionName = context.ID().GetText();

            functions[functionName] = context;

            return null;
        }

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

        public override object VisitFunctionCall(CSubsetParser.FunctionCallContext context)
        {
            string functionName = context.ID().GetText();

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

            if (functionName == "scanf")
            {
                if (context.expression().Length < 2)
                {
                    throw new Exception("Erro: scanf requer pelo menos dois argumentos (formato e variável).");
                }

                string format = context.expression(0).GetText().Trim('"');
                string varName = context.expression(1).GetText();

                if (!memory.ContainsKey(varName))
                {
                    throw new Exception($"Erro: Variável '{varName}' não declarada.");
                }

                Console.Write($"Entrada para {varName}: ");
                string input = Console.ReadLine();

                if (format.Contains("%d"))
                {
                    memory[varName] = int.TryParse(input, out int intValue) ? intValue : throw new Exception($"Erro: Entrada inválida para inteiro em '{varName}'.");
                }
                else if (format.Contains("%f"))
                {
                    memory[varName] = float.TryParse(input, System.Globalization.CultureInfo.InvariantCulture, out float floatValue) ? floatValue : throw new Exception($"Erro: Entrada inválida para float em '{varName}'.");
                }
                else if (format.Contains("%c"))
                {
                    memory[varName] = input.Length == 1 ? input[0] : throw new Exception($"Erro: Entrada inválida para char em '{varName}'.");
                }
                else if (format.Contains("%s"))
                {
                    memory[varName] = input;
                }
                else
                {
                    throw new Exception("Erro: Formato de `scanf` não reconhecido.");
                }

                return null;
            }

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

            if (functionName == "atoi")
            {
                if (context.expression().Length != 1)
                {
                    throw new Exception("Erro: atoi requer exatamente um argumento (string).");
                }

                object value = Visit(context.expression(0));

                return value is string strValue && int.TryParse(strValue, out int result) ? result : throw new Exception("Erro: atoi não pôde converter a string para inteiro.");
            }

            if (functionName == "atof")
            {
                if (context.expression().Length != 1)
                {
                    throw new Exception("Erro: atof requer exatamente um argumento (string).");
                }

                object value = Visit(context.expression(0));

                return value is string strValue && float.TryParse(strValue, System.Globalization.CultureInfo.InvariantCulture, out float result) ? result : throw new Exception("Erro: atof não pôde converter a string para float.");
            }

            if (functionName == "itoa")
            {
                if (context.expression().Length != 1)
                {
                    throw new Exception("Erro: itoa requer exatamente um argumento (inteiro).");
                }

                object value = Visit(context.expression(0));

                if (value is float floatValue && floatValue == (int)floatValue)
                {
                    return ((int)floatValue).ToString();
                }

                if (value is float)
                {
                    throw new Exception($"Erro: itoa só pode converter inteiros para string, mas recebeu Float ({value}). Use `ftoa` para converter floats.");
                }

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

                if (value is int intValue)
                {
                    throw new Exception($"Erro: ftoa só pode converter floats para string, mas recebeu Int ({intValue}). Use `itoa` para converter inteiros.");
                }

                if (value is float floatValue)
                {
                    return floatValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }

                throw new Exception($"Erro: ftoa só pode converter floats para string, mas recebeu {value?.GetType().Name ?? "null"}.");
            }

            if (!functions.ContainsKey(functionName))
            {
                throw new Exception($"Erro: Função '{functionName}' não foi definida.");
            }

            CSubsetParser.FunctionContext functionContext = functions[functionName];

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
    bool condition = Convert.ToBoolean(Visit(context.expression(0)));

    if (condition)
    {
        Visit(context.block(0));
        return null;
    }

    for (int i = 0; i < context.children.Count; i++)
    {
        if (context.GetChild(i).GetText() == "else" && context.GetChild(i + 1).GetText() == "if")
        {
            bool elseIfCondition = Convert.ToBoolean(Visit(context.expression(i / 2 + 1)));

            if (elseIfCondition)
            {
                Visit(context.block(i / 2 + 1));
                return null;
            }
        }
    }

    if (context.block().Length > 1 && context.GetChild(context.ChildCount - 2).GetText() == "else")
    {
        Visit(context.block(context.block().Length - 1));
    }

    return null;
}





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
                    break;
                }
            }
            return null;
        }

        public override object VisitBlock(CSubsetParser.BlockContext context)
        {
            foreach (var statement in context.statement())
            {
                Visit(statement);
            }

            return null;
        }


        public override object VisitForStatement(CSubsetParser.ForStatementContext context)
        {
            List<string> varNames = new List<string>();

            if (context.declaration() != null)
            {
                var decl = context.declaration();
                Visit(decl);
                varNames.Add(decl.ID().GetText());
            }
            else if (context.assignmentExpression(0) != null)
            {
                Visit(context.assignmentExpression(0));
                varNames.Add(context.assignmentExpression(0).ID().GetText());
            }

            foreach (var varName in varNames)
            {
                if (!memory.ContainsKey(varName))
                {
                    throw new Exception($"Erro: Variável de controle '{varName}' não encontrada.");
                }
            }

            while (context.expression() == null || Convert.ToBoolean(Visit(context.expression())))
            {
                try
                {
                    Visit(context.block());

                    if (context.assignmentExpression(1) != null)
                    {
                        Visit(context.assignmentExpression(1));
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

    try
    {
        foreach (var caseStmt in context.caseStatement())
        {
            var caseValue = int.Parse(caseStmt.NUMBER().GetText());

            if (switchValue.Equals(caseValue))
            {
                foreach (var stmt in caseStmt.statement())
                {
                    Visit(stmt);
                }
                return null; // ✅ Sai do switch ao encontrar o `case`
            }
        }

        if (context.defaultStatement() != null)
        {
            foreach (var stmt in context.defaultStatement().statement())
            {
                Visit(stmt);
            }
        }
    }
    catch (BreakException) // ✅ Captura `break` dentro do `switch`
    {
        return null; // ✅ Impede a exceção de quebrar o programa
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
            string structName = context.GetChild(0).GetText();
            string instanceName = context.GetChild(1).GetText();

            if (!structs.ContainsKey(structName))
            {
                throw new Exception($"Erro: Struct '{structName}' não foi declarada.");
            }

            var newInstance = new Dictionary<string, object>(structs[structName]);

            memory[instanceName] = newInstance;
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
            if (context.ChildCount == 3 && context.GetChild(1).GetText() == ".")
            {
                string structVar = context.GetChild(0).GetText();
                string member = context.GetChild(2).GetText();

                if (!memory.ContainsKey(structVar))
                {
                    throw new Exception($"Erro: Variável '{structVar}' não declarada.");
                }

                if (!(memory[structVar] is Dictionary<string, object> structData))
                {
                    throw new Exception($"Erro: '{structVar}' não é uma struct.");
                }

                if (!structData.ContainsKey(member))
                {
                    throw new Exception($"Erro: Membro '{member}' não existe na struct '{structVar}'.");
                }

                return structData[member];
            }

            return base.VisitExpression(context);
        }

        private HashSet<string> includedLibraries = new HashSet<string>();

        public override object VisitPreprocessorDirective(CSubsetParser.PreprocessorDirectiveContext context)
        {
            string directive = context.GetChild(1).GetText();

            switch (directive)
            {
                case "include":
                    string fileName;

                    if (context.GetChild(2).GetText().StartsWith("\""))
                    {
                        fileName = context.GetChild(2).GetText().Trim('"');
                    }
                    else if (context.GetChild(2).GetText() == "<")
                    {
                        fileName = context.GetChild(3).GetText().Trim('>');
                    }
                    else
                    {
                        throw new Exception($"Erro: Sintaxe inválida no `#include`. Esperado `#include <arquivo.h>` ou `#include \"arquivo.h\"`.");
                    }

                    if (includedLibraries.Contains(fileName))
                    {
                        return null;
                    }

                    includedLibraries.Add(fileName);

                    string filePath = Path.Combine("includes", fileName);

                    if (!File.Exists(filePath))
                    {
                        throw new Exception($"Erro: Arquivo incluído '{fileName}' não encontrado no diretório 'includes/'.");
                    }

                    string fileContent = File.ReadAllText(filePath);

                    AntlrInputStream inputStream = new AntlrInputStream(fileContent);
                    CSubsetLexer lexer = new CSubsetLexer(inputStream);
                    CommonTokenStream tokenStream = new CommonTokenStream(lexer);
                    CSubsetParser parser = new CSubsetParser(tokenStream);
                    CSubsetParser.ProgramContext includedTree = parser.program();

                    Visit(includedTree);

                    if (fileName == "stdio.h")
                    {
                        RegisterStdioFunctions();
                    }

                    if (fileName == "stdlib.h")
                    {
                        RegisterStdlibFunctions();
                    }
                    break;

                case "define":
                    if (context.ChildCount < 4)
                    {
                        throw new Exception("Erro: `#define` deve ter um nome e um valor.");
                    }

                    string macroName = context.GetChild(2).GetText();
                    string macroValue = string.Join(" ", context.GetText().Split(' ').Skip(3));

                    if (!macros.ContainsKey(macroName))
                    {
                        macros[macroName] = macroValue;
                    }
                    else
                    {
                        throw new Exception($"Erro: Macro '{macroName}' já foi definida anteriormente.");
                    }
                    break;

                default:
                    throw new Exception($"Erro: Diretiva `{directive}` não reconhecida.");
            }

            return null;
        }



        private void RegisterStdioFunctions()
        {
            functions["printf"] = new CSubsetParser.FunctionContext(null, 0);
            functions["scanf"] = new CSubsetParser.FunctionContext(null, 0);
            functions["puts"] = new CSubsetParser.FunctionContext(null, 0);
            functions["gets"] = new CSubsetParser.FunctionContext(null, 0);
        }


        private void RegisterStdlibFunctions()
        {
            functions["atoi"] = null;
            functions["atof"] = null;
            functions["itoa"] = null;
            functions["ftoa"] = null;
        }





    }
}
