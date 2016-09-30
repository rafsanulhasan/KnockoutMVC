namespace Microsoft.AspNetCore.Mvc.Knockout.Utilities
{
#if netcoreapp16
	using ModelBinding;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Threading.Tasks;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public class KnockoutModelBinder : DefaultModelBinder
	{
		public virtual async Task<object> BindModel(
			ControllerContext controllerContext,
			ModelBindingContext bindingContext)
		{
			var result = base.BindModel(controllerContext, bindingContext);
			KnockoutUtilities.ConvertData(result);
			return result;
		}

		/// <inheritdoc />
		public KnockoutModelBinder(Type type)
			: base(type) { }
	}
#endif
}