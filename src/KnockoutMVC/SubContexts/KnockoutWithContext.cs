namespace Microsoft.AspNetCore.Mvc.Knockout
{

	using Rendering;

	public class KnockoutWithContext <TModel> : KnockoutCommonRegionContext<TModel>
	{
		#region Public Members

		#region Constructors

		public KnockoutWithContext(ViewContext viewContext, string expression)
			: base(viewContext, expression) { }

		#endregion

		#endregion

		#region Protected Members

		#region Properties

		protected override string Keyword => "with";

		#endregion

		#endregion
	}

}