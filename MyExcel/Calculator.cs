using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MyExcel
{
    public static class Calculator
    {
        public static decimal Evaluate(string expression)
        {
            var lexer = new ExpGrammarLexer(new AntlrInputStream(expression));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowExceptionErrorListener());
            var tokens = new CommonTokenStream(lexer);
            var parser = new ExpGrammarParser(tokens);
            var tree = parser.compileUnit();
            var visitor = new ExpTreeVisitor();
            return visitor.Visit(tree);
        }
        
        public static void GetDepCells(string expression, string adr, out List<string> nextList)
        {
            try
            {
                nextList = new List<string>();
                var lexer = new ExpGrammarLexer(new AntlrInputStream(expression));
                lexer.RemoveErrorListeners();
                lexer.AddErrorListener(new ThrowExceptionErrorListener());
                var tokens = new CommonTokenStream(lexer);
                var parser = new ExpGrammarParser(tokens);
                var tree = parser.compileUnit();

                for (int i = 0; i < tokens.Size - 1; i++)
                {
                    var token = tokens.Get(i);
                    string id = token.Text;
                    if (token.Type == ExpGrammarParser.IDENTIFIER && !(nextList.Contains(id)))
                        nextList.Add(id);  
                }
                return;
            }
            finally
            {
            }
        }
    }
}
