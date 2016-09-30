

namespace Microsoft.AspNetCore.Mvc
{
	using Knockout.Utilities;
	using Newtonsoft.Json;
	using System.Diagnostics.CodeAnalysis;
	using System.Text;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public abstract class KnockoutController : Controller
	{
		#region Private Members

		#region Fields

		readonly JsonSerializerSettings _settings;

		#endregion

		#endregion

		#region Protected Members

		#region Constructors

		protected KnockoutController(JsonSerializerSettings settings)
		{
			_settings = settings ?? new JsonSerializerSettings()
			                        {
				                        NullValueHandling = new NullValueHandling(),
				                        ConstructorHandling = new ConstructorHandling()
			                        };
		}

		#endregion

		#region Methods

		protected JsonResult Json(object data, string contentType, Encoding contentEncoding)
		{
			KnockoutUtilities.ConvertData(data);
			return Json(data, _settings);
		}

		#endregion
		#endregion
	}

}
