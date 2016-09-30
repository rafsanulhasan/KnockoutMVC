namespace Microsoft.AspNetCore.Mvc.Knockout
{

	using Rendering;

	public class KnockoutForeachContext <TModel> : KnockoutCommonRegionContext<TModel>
	{
		#region Public Members

		#region Constructors

		public KnockoutForeachContext(ViewContext viewContext, string expression)
			: base(viewContext, expression) { }

		#endregion

		#endregion

		#region Protected Members
		#region Properties
		protected override string Keyword => "foreach";

		#endregion
		#endregion
	}

}
