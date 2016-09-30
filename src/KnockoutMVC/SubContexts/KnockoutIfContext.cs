namespace Microsoft.AspNetCore.Mvc.Knockout
{

	using Rendering;

	public class KnockoutIfContext<TModel> : KnockoutCommonRegionContext<TModel>
	{
		#region Public Members
		#region Constructors
		public KnockoutIfContext(ViewContext viewContext, string expression)
            : base(viewContext, expression)
        {
        }

		#endregion
		#endregion

		#region Protected Members
		#region Properties
		protected override string Keyword => "if";

		#endregion
		#endregion
	}
}