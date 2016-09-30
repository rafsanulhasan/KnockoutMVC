namespace KnockoutMVC.Extensions
{

	using Microsoft.AspNetCore.Html;
	using Microsoft.AspNetCore.Mvc.Rendering;

	public static class TagBuilderExtensions
	{
		public static string ToString(this TagBuilder tagBuilder, TagRenderMode? tagRenderMode)
		{
			tagBuilder.TagRenderMode = tagRenderMode ?? TagRenderMode.Normal;
			return tagBuilder.ToString();
		}
		public static HtmlString ToHtmlString( this TagBuilder tagBuilder, TagRenderMode? tagRenderMode )
		{
			tagBuilder.TagRenderMode = tagRenderMode ?? TagRenderMode.Normal;
			return new HtmlString(tagBuilder.ToString());
		}
	}

}