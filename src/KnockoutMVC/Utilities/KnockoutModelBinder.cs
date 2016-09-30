namespace Microsoft.AspNetCore.Mvc.Knockout.Utilities
{
	using ModelBinding;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Threading.Tasks;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public class KnockoutModelBinder 
		//: Glimpse.Mvc.AlternateType.ModelBinder
	{
		public virtual async Task<object> BindModel(
			ControllerContext controllerContext,
			ModelBindingContext bindingContext)
		{
			await BindModelAsync(bindingContext);
			KnockoutUtilities.ConvertData(result);
			return result;
		}

		/// <inheritdoc />
		public KnockoutModelBinder(Type type)
			: base(type) { }
	}

}