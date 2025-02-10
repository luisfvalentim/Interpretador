using Antlr4.Runtime;
using Interpretador.Generated;
using Interpretador.Interpreter;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Uso: dotnet run <arquivo.c>");
            return;
        }

        string code = File.ReadAllText(args[0]);
        var inputStream = new AntlrInputStream(code);
        var lexer = new CSubsetLexer(inputStream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new CSubsetParser(tokens);
        var tree = parser.program();

        new InterpreterVisitor().Visit(tree);
    }
}