using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MyExcel
{
    class ExpTreeVisitor : ExpGrammarBaseVisitor<decimal>
    {
        public static Dictionary<string, Cell> tableIdentifier = new Dictionary<string, Cell>();
        public override decimal VisitCompileUnit(ExpGrammarParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }
        public override decimal VisitParenthesizedExpr(ExpGrammarParser.ParenthesizedExprContext context)
        {
            return Visit(context.expression());
        }
        public override decimal VisitNumberExpr(ExpGrammarParser.NumberExprContext context)
        {
            var result = decimal.Parse(context.GetText());
            return result;
        }
        public override decimal VisitIdentifierExpr(ExpGrammarParser.IdentifierExprContext context)
        {
            string address = context.GetText();
            Cell cell;
            if (MainForm.CellDict.ContainsKey(address))
                cell = MainForm.CellDict[address];
            else
                cell = new Cell(address);
            
            return Convert.ToDecimal(Calculator.Evaluate(cell.Exp));
        }
        public override decimal VisitMultiplicativeExpr(ExpGrammarParser.MultiplicativeExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (context.operatorToken.Type == ExpGrammarLexer.MULTIPLY)
                return left * right;
            else //ExpGrammarLexer.DIVIDE
                return left / right;
        }
        public override decimal VisitAdditiveExpr(ExpGrammarParser.AdditiveExprContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);
            if (context.operatorToken.Type == ExpGrammarLexer.ADD)
                return left + right;
            else //ExpGrammarLexer.SUBTRACT
                return left - right;
        }
        public override decimal VisitIncdecExpr(ExpGrammarParser.IncdecExprContext context)
        {
            var left = WalkLeft(context);
            if (context.operatorToken.Type == ExpGrammarLexer.INC)
                return ++left;
            else //ExpGrammarLexer.SUBTRACT
                return --left;
        }
        public override decimal VisitUnaryExpr(ExpGrammarParser.UnaryExprContext context)
        {
            var result = WalkLeft(context);
            if (context.operatorToken.Type == ExpGrammarLexer.SUBTRACT)
                return -result;
            else //ExpGrammarLexer.ADD
                return result;
        }
        public override decimal VisitExponentialExpr(ExpGrammarParser.ExponentialExprContext context)
        {
            var left = Convert.ToDouble(WalkLeft(context));
            var right = Convert.ToDouble(WalkRight(context));
            return Convert.ToDecimal(System.Math.Pow(left, right));
        }
        private decimal WalkLeft(ExpGrammarParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<ExpGrammarParser.ExpressionContext>(0));
        }
        private decimal WalkRight(ExpGrammarParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<ExpGrammarParser.ExpressionContext>(1));
        }
    }
}
