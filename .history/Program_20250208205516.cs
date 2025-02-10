using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Interpretador.Generated;
using Interpretador.Interpreter;

class Program
{
    static void Main(string[] args)
    {
        // Código-fonte que será interpretado
        string code = @"
            int x = 10;
            x = x + 1;
            printf(""Valor atualizado de x: %d\n"", x);
        ";

        try
        {
            // Criando lexer e parser
            AntlrInputStream inputStream = new AntlrInputStream(code);
            CSubsetLexer lexer = new CSubsetLexer(inputStream);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            CSubsetParser parser = new CSubsetParser(tokenStream);

            // Construindo árvore sintática
            CSubsetParser.ProgramContext tree = parser.program();

            // Criando o interpretador e executando o código
            InterpreterVisitor visitor = new InterpreterVisitor();
            visitor.Visit(tree);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}
