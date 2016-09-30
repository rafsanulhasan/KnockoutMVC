namespace Microsoft.AspNetCore.Mvc.Knockout
{
#if netcoreapp16
	using Rendering;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public abstract class KnockoutCommonRegionContext <TModel> : KnockoutRegionContext<TModel>
	{
#region Protected Members

#region  Protected Properties

		protected string Expression { get; set; }

#endregion

#region Protected Constructors

		protected KnockoutCommonRegionContext(ViewContext viewContext, string expression)
			: base(viewContext) { Expression = expression; }

#endregion

#region Protected Methods


		protected override void WriteEnd(TextWriter writer) { writer.WriteLine(@"<!-- /ko -->"); }

#endregion

#region Protected Abstract Methods


		protected abstract string Keyword { get; }

#endregion

#endregion

#region Public Members
#region Public Methods
		public override void WriteStart(TextWriter writer) { writer?.WriteLine(@"<!-- ko {0}: {1} -->", Keyword, Expression); }
#endregion
#endregion
	}
#endif
}