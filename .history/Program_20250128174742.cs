
using System;
using Antlr4.Runtime;

class Program
{
    static void Main(string[] args)
    {
        // Código-fonte para interpretar
        string sourceCode = @"
            int a = 5;
            int b = a + 3;
            write b;
        ";

        // Criar o lexer e parser
        AntlrInputStream inputStream = new AntlrInputStream(sourceCode);
        ProdyCLexer lexer = new ProdyCLexer(inputStream);
        CommonTokenStream tokens = new CommonTokenStream(lexer);
        ProdyCParser parser = new ProdyCParser(tokens);

        // Criar a AST
        var tree = parser.prog();

        // Criar o Visitor e interpretar
        ProdyCVisitorImpl visitor = new ProdyCVisitorImpl();
        visitor.Visit(tree);
    }
}
