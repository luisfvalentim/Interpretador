using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Interpretador.Generated;
using Interpretador.Interpreter;

class Program
{
    static void Main(string[] args)
    {
        string filePath = "test.c"; // Caminho do arquivo a ser lido

        try
        {
            // Verifica se o arquivo existe
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Erro: Arquivo '{filePath}' não encontrado.");
                return;
            }

            // Lendo o conteúdo do arquivo test.c
            string code = File.ReadAllText(filePath);

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
