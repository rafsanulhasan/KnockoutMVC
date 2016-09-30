namespace KnockoutMVC.Extensions
{

	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using System.Diagnostics.CodeAnalysis;

	[SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
	public static class UrlHelperExtensions
	{
		static IHttpContextAccessor _httpContextAccessor;
		public static void Configure(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

		public static string AbsoluteAction(
			this IUrlHelper url,
			string actionName,
			string controllerName,
			object routeValues = null)
		{
			var scheme = _httpContextAccessor.HttpContext.Request.Scheme;
			return url.Action(actionName, controllerName, routeValues, scheme);
		}
	}
}
