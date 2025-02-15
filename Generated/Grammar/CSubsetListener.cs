//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Grammar/CSubset.g4 by ANTLR 4.13.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Interpretador.Generated {
using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="CSubsetParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.2")]
[System.CLSCompliant(false)]
public interface ICSubsetListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProgram([NotNull] CSubsetParser.ProgramContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.program"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProgram([NotNull] CSubsetParser.ProgramContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.preprocessorDirective"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPreprocessorDirective([NotNull] CSubsetParser.PreprocessorDirectiveContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.preprocessorDirective"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPreprocessorDirective([NotNull] CSubsetParser.PreprocessorDirectiveContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] CSubsetParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] CSubsetParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.whileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatement([NotNull] CSubsetParser.WhileStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.whileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatement([NotNull] CSubsetParser.WhileStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.forStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterForStatement([NotNull] CSubsetParser.ForStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.forStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitForStatement([NotNull] CSubsetParser.ForStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.doWhileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDoWhileStatement([NotNull] CSubsetParser.DoWhileStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.doWhileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDoWhileStatement([NotNull] CSubsetParser.DoWhileStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.switchStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitchStatement([NotNull] CSubsetParser.SwitchStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.switchStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitchStatement([NotNull] CSubsetParser.SwitchStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.caseStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCaseStatement([NotNull] CSubsetParser.CaseStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.caseStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCaseStatement([NotNull] CSubsetParser.CaseStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.defaultStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDefaultStatement([NotNull] CSubsetParser.DefaultStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.defaultStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDefaultStatement([NotNull] CSubsetParser.DefaultStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.ifStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfStatement([NotNull] CSubsetParser.IfStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.ifStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfStatement([NotNull] CSubsetParser.IfStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlock([NotNull] CSubsetParser.BlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlock([NotNull] CSubsetParser.BlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.logicalExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogicalExpression([NotNull] CSubsetParser.LogicalExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.logicalExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogicalExpression([NotNull] CSubsetParser.LogicalExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.equalityExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEqualityExpression([NotNull] CSubsetParser.EqualityExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.equalityExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEqualityExpression([NotNull] CSubsetParser.EqualityExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.relationalExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRelationalExpression([NotNull] CSubsetParser.RelationalExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.relationalExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRelationalExpression([NotNull] CSubsetParser.RelationalExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.additiveExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAdditiveExpression([NotNull] CSubsetParser.AdditiveExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.additiveExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAdditiveExpression([NotNull] CSubsetParser.AdditiveExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.multiplicativeExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultiplicativeExpression([NotNull] CSubsetParser.MultiplicativeExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.multiplicativeExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultiplicativeExpression([NotNull] CSubsetParser.MultiplicativeExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.unaryExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryExpression([NotNull] CSubsetParser.UnaryExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.unaryExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryExpression([NotNull] CSubsetParser.UnaryExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.primary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrimary([NotNull] CSubsetParser.PrimaryContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.primary"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrimary([NotNull] CSubsetParser.PrimaryContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.structDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStructDeclaration([NotNull] CSubsetParser.StructDeclarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.structDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStructDeclaration([NotNull] CSubsetParser.StructDeclarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.structMember"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStructMember([NotNull] CSubsetParser.StructMemberContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.structMember"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStructMember([NotNull] CSubsetParser.StructMemberContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.unionDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnionDeclaration([NotNull] CSubsetParser.UnionDeclarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.unionDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnionDeclaration([NotNull] CSubsetParser.UnionDeclarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.structInstance"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStructInstance([NotNull] CSubsetParser.StructInstanceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.structInstance"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStructInstance([NotNull] CSubsetParser.StructInstanceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.unionInstance"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnionInstance([NotNull] CSubsetParser.UnionInstanceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.unionInstance"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnionInstance([NotNull] CSubsetParser.UnionInstanceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression([NotNull] CSubsetParser.ExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression([NotNull] CSubsetParser.ExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.functionCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionCall([NotNull] CSubsetParser.FunctionCallContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.functionCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionCall([NotNull] CSubsetParser.FunctionCallContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType([NotNull] CSubsetParser.TypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType([NotNull] CSubsetParser.TypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturnStatement([NotNull] CSubsetParser.ReturnStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturnStatement([NotNull] CSubsetParser.ReturnStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunction([NotNull] CSubsetParser.FunctionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.function"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunction([NotNull] CSubsetParser.FunctionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.parameters"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParameters([NotNull] CSubsetParser.ParametersContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.parameters"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParameters([NotNull] CSubsetParser.ParametersContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDeclaration([NotNull] CSubsetParser.DeclarationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.declaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDeclaration([NotNull] CSubsetParser.DeclarationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.assignmentExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignmentExpression([NotNull] CSubsetParser.AssignmentExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.assignmentExpression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignmentExpression([NotNull] CSubsetParser.AssignmentExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSubsetParser.breakStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBreakStatement([NotNull] CSubsetParser.BreakStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSubsetParser.breakStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBreakStatement([NotNull] CSubsetParser.BreakStatementContext context);
}
} // namespace Interpretador.Generated
