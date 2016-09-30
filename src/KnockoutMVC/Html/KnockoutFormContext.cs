

namespace Microsoft.AspNetCore.Html
{
	using KnockoutMVC.Extensions;
	using Mvc.Knockout;
	using Mvc.Rendering;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public class KnockoutFormContext <TModel> : KnockoutRegionContext<TModel>
	{
		readonly KnockoutContext<TModel> _context;
		readonly ICollection<string> _instanceNames;
		readonly IDictionary<string, string> _aliases;

		readonly string _actionName;
		readonly string _controllerName;
		readonly object _routeValues;
		readonly object _htmlAttributes;

		public KnockoutFormContext(
			ViewContext viewContext,
			KnockoutContext<TModel> context,
			ICollection<string> instanceNames,
			IDictionary<string, string> aliases,
			string actionName,
			string controllerName,
			object routeValues,
			object htmlAttributes)
			: base(viewContext)
		{
			_context = context;
			_instanceNames = instanceNames;
			_aliases = aliases;
			_actionName = actionName;
			_controllerName = controllerName;
			_routeValues = routeValues;
			_htmlAttributes = htmlAttributes;
			InStack = false;
		}

		public override void WriteStart(TextWriter writer)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(_context, "form", _instanceNames, _aliases);
			tagBuilder.ApplyAttributes(_htmlAttributes);
			tagBuilder.Submit(_actionName, _controllerName, _routeValues);
			tagBuilder.TagRenderMode = TagRenderMode.StartTag;
			writer.WriteLine(tagBuilder.ToHtmlString());
		}

		protected override void WriteEnd(TextWriter writer)
		{
			var tagBuilder = new TagBuilder("form");
			writer.WriteLine(new HtmlString(tagBuilder.ToString(TagRenderMode.EndTag)));
		}
	}

}