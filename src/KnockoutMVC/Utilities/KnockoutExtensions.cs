namespace Microsoft.AspNetCore.Mvc.Knockout.Utilities
{
#if netcoreapp16
	using ViewFeatures;

	public static class KnockoutExtensions
	{
		public static KnockoutContext<TModel> CreateKnockoutContext <TModel>(this HtmlHelper<TModel> helper)
		{
			return new KnockoutContext<TModel>(helper.ViewContext);
		}

		public static KnockoutContext<TModel> CreateKnockoutContext <TModel>(
			this HtmlHelper<TModel> helper,
			string viewModelName)
		{
			var context = helper.CreateKnockoutContext();
			context.ViewModelName = viewModelName;
			return context;
		}
	}
#endif
}
