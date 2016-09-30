namespace Microsoft.AspNetCore.Mvc.Knockout.Binding
{
#if netcoreapp16
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq.Expressions;

	using Microsoft.AspNetCore.Html;

	using Utilities;

	public abstract class KnockoutBindingItem
	{
		HtmlString _htmlString;
		public string Name { get; set; }

		public abstract string GetKnockoutExpression(KnockoutExpressionData data);

		public virtual bool IsValid() => true;
	}

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public class KnockoutBindingItem <TModel, TResult> : KnockoutBindingItem
	{
		public Expression<Func<TModel, TResult>> Expression { get; set; }

		public override string GetKnockoutExpression(KnockoutExpressionData data)
		{
			var value = KnockoutExpressionConverter.Convert(Expression, data);
			return $"{Name} : {value ?? "$data"}";
		}
	}
#endif
}
