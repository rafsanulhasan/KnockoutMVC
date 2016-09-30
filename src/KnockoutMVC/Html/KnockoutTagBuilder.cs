namespace Microsoft.AspNetCore.Html
{
	using Mvc.Knockout;
	using Mvc.Knockout.Binding;
	using Mvc.Rendering;
	using Mvc.ViewFeatures;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public class KnockoutTagBuilder <TModel> : KnockoutBinding<TModel>
	{
		readonly TagBuilder _tagBuilder;

		public KnockoutTagBuilder(
			KnockoutContext<TModel> context,
			string tagName,
			ICollection<string> instanceNames,
			IDictionary<string, string> aliases)
			: base(context, instanceNames, aliases)
		{
			_tagBuilder = new TagBuilder(tagName);
			TagRenderMode = TagRenderMode.Normal;
		}

		public void ApplyAttributes(object htmlAttributes)
		{
			var dictionaryAttributes = htmlAttributes as IDictionary<string, object> ??
			                           HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
			ApplyAttributes(dictionaryAttributes);
		}

		public void ApplyAttributes(IDictionary<string, object> htmlAttributes)
		{
			if ( htmlAttributes == null ) return;

			foreach ( var htmlAttribute in htmlAttributes )
				_tagBuilder.Attributes[htmlAttribute.Key] = htmlAttribute.Value.ToString();
		}

		public string InnerHtml
		{
			get { return _tagBuilder.InnerHtml.ToString(); }
			set
			{
				// ReSharper disable once MustUseReturnValue
				_tagBuilder.InnerHtml.SetHtmlContent(new HtmlString(value));

			}
		}

		public KnockoutTagBuilder<TModel> SetInnerHtml(string innerHtml)
		{
			InnerHtml = innerHtml;
			return this;
		}

		public TagRenderMode TagRenderMode { get; set; }

		public override string ToHtmlString()
		{
			_tagBuilder.Attributes["data-bind"] = BindingAttributeContent();
			return _tagBuilder.ToString();
		}

		public string ToHtmlString( TagRenderMode? tagRenderMode )
		{
			_tagBuilder.TagRenderMode = tagRenderMode ?? TagRenderMode.Normal;
			return ToHtmlString();
		}

		public string ToString(TagRenderMode? tagRenderMode)
		{
			_tagBuilder.TagRenderMode = tagRenderMode ?? TagRenderMode.Normal;
			return _tagBuilder.ToString();
		}
	}

}