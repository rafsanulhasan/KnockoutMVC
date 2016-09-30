namespace Microsoft.AspNetCore.Mvc.Knockout.Utilities
{
#if netcoreapp16
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public class KnockoutExpressionConverter
	{
		KnockoutExpressionData _data;

		readonly ICollection<string> _lambdaFrom = new List<string>();

		public static string Convert(Expression expression, KnockoutExpressionData convertData = null)
		{
			var converter = new KnockoutExpressionConverter();
			return converter.LocalConvert(expression, convertData);
		}

		string LocalConvert(Expression expression, KnockoutExpressionData convertData = null)
		{
			_lambdaFrom.Clear();
			_data = convertData ?? new KnockoutExpressionData();
			_data = _data.Clone();
			if ( _data.InstanceNames == null ||
			     _data.InstanceNames.Count == 0 )
				_data.InstanceNames = new [] { "" };
			return GetterSetterCorrecting(Visit(expression));
		}

		//TODO: rewrite
		public string GetterSetterCorrecting(string expr)
		{
			var cnt = expr.Count(c => char.IsLetterOrDigit(c) || c == '(' || c == ')' || c == '_' || c == '.' || c == '$');
			if ( cnt == expr.Length &&
			     expr.EndsWith("()") &&
			     !_data.NeedBracketsForAllMembers )
				expr = expr.Substring(0, expr.Length - 2);
			return expr;
		}

		protected virtual string Visit(Expression exp)
		{
			if ( exp == null )
				throw new ArgumentNullException();

			switch ( exp.NodeType )
			{
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
					return VisitUnary((UnaryExpression) exp, "-");
				case ExpressionType.Not:
					return VisitUnary((UnaryExpression) exp, "!");
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					return VisitUnary((UnaryExpression) exp, "");
				case ExpressionType.ArrayLength:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
					throw new NotSupportedException();
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
					return VisitBinary((BinaryExpression) exp, "+");
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
					return VisitBinary((BinaryExpression) exp, "-");
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
					return VisitBinary((BinaryExpression) exp, "*");
				case ExpressionType.Divide:
					return VisitBinary((BinaryExpression) exp, "/");
				case ExpressionType.Modulo:
					return VisitBinary((BinaryExpression) exp, "%");
				case ExpressionType.And:
					return VisitBinary((BinaryExpression) exp, "&");
				case ExpressionType.AndAlso:
					return VisitBinary((BinaryExpression) exp, "&&");
				case ExpressionType.Or:
					return VisitBinary((BinaryExpression) exp, "|");
				case ExpressionType.OrElse:
					return VisitBinary((BinaryExpression) exp, "||");
				case ExpressionType.LessThan:
					return VisitBinary((BinaryExpression) exp, "<");
				case ExpressionType.LessThanOrEqual:
					return VisitBinary((BinaryExpression) exp, "<=");
				case ExpressionType.GreaterThan:
					return VisitBinary((BinaryExpression) exp, ">");
				case ExpressionType.GreaterThanOrEqual:
					return VisitBinary((BinaryExpression) exp, ">=");
				case ExpressionType.Equal:
					return VisitBinary((BinaryExpression) exp, "==");
				case ExpressionType.NotEqual:
					return VisitBinary((BinaryExpression) exp, "!=");
				case ExpressionType.Coalesce:
					throw new NotSupportedException();
				case ExpressionType.ArrayIndex:
					return VisitArrayIndex((BinaryExpression) exp);
				case ExpressionType.RightShift:
					return VisitBinary((BinaryExpression) exp, ">>");
				case ExpressionType.LeftShift:
					return VisitBinary((BinaryExpression) exp, "<<");
				case ExpressionType.ExclusiveOr:
					return VisitBinary((BinaryExpression) exp, "^");
				case ExpressionType.TypeIs:
					return VisitTypeIs((TypeBinaryExpression) exp);
				case ExpressionType.Conditional:
					return VisitConditional((ConditionalExpression) exp);
				case ExpressionType.Constant:
					return VisitConstant((ConstantExpression) exp);
				case ExpressionType.Parameter:
					return VisitParameter((ParameterExpression) exp);
				case ExpressionType.MemberAccess:
					return VisitMemberAccess((MemberExpression) exp);
				case ExpressionType.Call:
					return VisitMethodCall((MethodCallExpression) exp);
				case ExpressionType.Lambda:
					return VisitLambda((LambdaExpression) exp);
				case ExpressionType.New:
					return VisitNew((NewExpression) exp);
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					return VisitNewArray((NewArrayExpression) exp);
				case ExpressionType.Invoke:
					return VisitInvocation((InvocationExpression) exp);
				case ExpressionType.MemberInit:
					return VisitMemberInit((MemberInitExpression) exp);
				case ExpressionType.ListInit:
					return VisitListInit((ListInitExpression) exp);
				default:
					throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
			}
		}

		protected virtual string VisitMemberAccess(MemberExpression m)
		{
			return VisitMemberAccess(m?.Expression, m?.Member.Name);
		}

		//TODO: rewrite
		string VisitMemberAccess(Expression obj, string member)
		{
			if ( typeof( IKnockoutContext ).IsAssignableFrom(obj.Type) )
			{
				var lambda = Expression.Lambda<Func<IKnockoutContext>>(obj);
				var context = lambda.Compile()();
				if ( member == "Model" )
					return context.GetInstanceName();
			}

			var own = Visit(obj);
			if ( _data.Aliases.ContainsKey(own) )
				own = _data.Aliases[own];
			if ( _lambdaFrom.Contains(own) )
				own = _data.InstanceNames.SingleOrDefault(i => i == own);
			if ( ( member == "Length" || member == "Count" ) &&
			     !_data.InstanceNames.Contains(own) )
				member = "length";
			var prefix = own == "" ? "" : own + ".";
			var suffix = member == "length" ? "" : "()";
			var result = prefix + member + suffix;
			if ( !_data.Aliases.ContainsKey(result) )
			{
				if ( _data.Aliases.ContainsKey(prefix + member) )
					result = _data.Aliases[prefix + member];
			}
			else result = _data.Aliases[result];
			return result;
		}

		protected virtual string VisitUnary(UnaryExpression u, string sign)
		{
			var operand = Visit(u.Operand);
			return sign + operand;
		}

		protected virtual string VisitBinary(BinaryExpression b, string sign)
		{
			if ( b.NodeType == ExpressionType.Coalesce )
				throw new NotSupportedException();

			var left = Visit(b.Left);
			var right = Visit(b.Right);
			if ( b.Type == typeof( int ) ||
			     b.Type == typeof( long ) ||
			     b.Type == typeof( short ) ||
			     b.Type == typeof( byte ) ||
			     b.Type == typeof( uint ) ||
			     b.Type == typeof( ushort ) ||
			     b.Type == typeof( ulong ) ||
			     b.Type == typeof( sbyte ) )
				return $"(parseInt({left}) {sign} parseInt({right}))";

			if ( b.Type == typeof( float ) ||
			     b.Type == typeof( double ) ||
			     b.Type == typeof( decimal ) )
				return $"(parseFloat({left}) {sign} parseFloat({right}))";

			return $"({left} {sign} {right})";
		}

		string VisitArrayIndex(BinaryExpression b) => $"{Visit(b?.Left)}[{Visit(b?.Right)}]";

		protected virtual string VisitConstant(ConstantExpression c)
		{
			var s = c?.Value.ToString();			
				if ( !string.IsNullOrEmpty(s) &&
				     !s.StartsWith("$") )
					return $"'{s}'";
				if ( c?.Value is bool )
					return ( (bool) c.Value ) ? "true" : "false";

			return c?.Value == null ? "null" : c.Value.ToString();
		}

		protected virtual string VisitConditional(ConditionalExpression c)
		{
			return $"{Visit(c.Test)} ? {Visit(c.IfTrue)} : {Visit(c.IfFalse)}";
		}

		protected virtual string VisitParameter(ParameterExpression p) => _lambdaFrom.Contains(p?.Name) ? _data.InstanceNames.SingleOrDefault(i => i == p?.Name) : p?.Name;

		protected virtual string VisitLambda(LambdaExpression lambda)
		{
			foreach ( var parameter in lambda.Parameters )
				_lambdaFrom.Add(parameter.Name);

			return Visit(lambda.Body);
		}

		protected virtual string VisitTypeIs(TypeBinaryExpression b) { throw new NotSupportedException(); }

		protected virtual string VisitMethodCall(MethodCallExpression m)
		{
			if ( m.Arguments.Count == 1 &&
			     m.Method.Name == "get_Item" )
				return $"{this.Visit(m.Object)}[{this.Visit(m.Arguments[0])}]";

			if ( m.Arguments.Count == 0 &&
			     m.Method.Name == "ToString" )
			{
				var o = (ParameterExpression) m.Object;
				return _lambdaFrom.Contains(o?.Name) ? "$data" : Visit(m.Object);
			}

			if ( m.Arguments.Count == 1 &&
			     m.Object == null &&
			     m.Method.Name == "ToBoolean" )
				return Visit(m.Arguments[0]); 

			if ( typeof( Expression ).IsAssignableFrom(m.Method.ReturnType) )
				return VisitMemberAccess(m.Object, m.Method.Name);

			throw new NotSupportedException();
		}

		protected virtual ReadOnlyCollection<string> VisitExpressionList(ReadOnlyCollection<Expression> original)
		{
			throw new NotSupportedException();
		}

		protected virtual string VisitMemberAssignment(MemberAssignment assignment) { throw new NotSupportedException(); }

		protected virtual string VisitMemberMemberBinding(MemberMemberBinding binding) { throw new NotSupportedException(); }

		protected virtual string VisitMemberListBinding(MemberListBinding binding) { throw new NotSupportedException(); }

		protected virtual IEnumerable<string> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
		{
			throw new NotSupportedException();
		}

		protected virtual string VisitNew(NewExpression nex) { throw new NotSupportedException(); }

		protected virtual string VisitMemberInit(MemberInitExpression init) { throw new NotSupportedException(); }

		protected virtual string VisitListInit(ListInitExpression init) { throw new NotSupportedException(); }

		protected virtual string VisitNewArray(NewArrayExpression na) { throw new NotSupportedException(); }

		protected virtual string VisitInvocation(InvocationExpression iv) { throw new NotSupportedException(); }
	}
#endif
}
