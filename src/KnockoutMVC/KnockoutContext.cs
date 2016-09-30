namespace Microsoft.AspNetCore.Mvc.Knockout
{
	using Binding;
	using Html;
	using Mvc;
	using Newtonsoft.Json;
	using Rendering;
	using Routing;
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq.Expressions;
	using System.Text;
	using Utilities;

	public interface IKnockoutContext
	{
		string GetInstanceName();
		string GetIndex();
	}

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public class KnockoutContext <TModel> : IKnockoutContext
	{
		#region Private Members

		#region Fields

		TModel _model;

		bool _isInitialized;

		readonly ViewContext _viewContext;

		#endregion

		#region Methods

		string GetInitializeData(TModel model, bool needBinding, string wrapperId, bool applyOnDocumentReady)
		{
			if ( _isInitialized )
				return "";

			_isInitialized = true;
			KnockoutUtilities.ConvertData(model);
			_model = model;

			var json = JsonConvert.SerializeObject(model);

			var sb = new StringBuilder();

			sb.AppendLine(@"<script type=""text/javascript""> ");

			if ( applyOnDocumentReady )
				sb.Append("$(document).ready(function() {");
			sb.AppendLine($"var {ViewModelName}Js = {json};");
			var mappingData = KnockoutJsModelBuilder.CreateMappingData<TModel>();
			if ( mappingData == "{}" )
				sb.AppendLine($"var {this.ViewModelName} = ko.mapping.fromJS({this.ViewModelName}Js); ");
			else
			{
				sb.AppendLine($"var {ViewModelName}MappingData = {mappingData};");
				sb.AppendLine($"var {ViewModelName} = ko.mapping.fromJS({ViewModelName}Js, {ViewModelName}MappingData); ");
			}
			sb.Append(KnockoutJsModelBuilder.AddComputedToModel(model, ViewModelName));
			if ( needBinding )
			{
				sb.AppendLine
				(!string.IsNullOrEmpty(wrapperId)
					 ? $"ko.applyBindings({this.ViewModelName}, document.getElementById('{wrapperId}'))"
					 : $"ko.applyBindings({this.ViewModelName});");
			}

			if ( applyOnDocumentReady )
				sb.Append("});");
			sb.AppendLine(@"</script>");
			return sb.ToString();
		}

		int ActiveSubcontextCount => ContextStack.Count - 1 - ContextStack.IndexOf(this);

		string GetContextPrefix()
		{
			var builder = new StringBuilder();
			var count = ActiveSubcontextCount;
			for ( int i = 0; i < count; i++ )
				builder.Append("$parentContext.");

			return builder.ToString();
		}

		#endregion

		#endregion

		#region Public Members

		#region Properties

		// ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
		public TModel Model => _model;
		public string ViewModelName = "viewModel";

		#endregion

		#region Constructors

		public KnockoutContext(ViewContext viewContext) { _viewContext = viewContext; }

		#endregion

		#region Methods

		public HtmlString Initialize(TModel model)
		{
			return new HtmlString(GetInitializeData(model, false, string.Empty, false));
		}

		public HtmlString Apply(TModel model, string wrapperId = "", bool applyOnDocumentReady = false)
		{
			if ( !_isInitialized )
				return new HtmlString(GetInitializeData(model, true, wrapperId, applyOnDocumentReady));

			var sb = new StringBuilder();
			sb.AppendLine(@"<script type=""text/javascript"">");
			if ( applyOnDocumentReady )
				sb.AppendLine("$(document).ready(function() {");
			sb.AppendLine
			(!string.IsNullOrEmpty(wrapperId)
				 ? $"ko.applyBindings({this.ViewModelName}, document.getElementById('{wrapperId}'))"
				 : $"ko.applyBindings({this.ViewModelName});");
			if ( applyOnDocumentReady )
				sb.AppendLine("});");
			sb.AppendLine(@"</script>");
			return new HtmlString(sb.ToString());
		}

		public HtmlString LazyApply(TModel model, string actionName, string controllerName, string wrapperId = "")
		{
			var sb = new StringBuilder();

			sb.AppendLine(@"<script type=""text/javascript""> ");
			sb.AppendLine("$(document).ready(function() {");

			sb.AppendLine
				($"$.ajax({{ url: '{Url().Action(actionName, controllerName)}', type: 'POST', success: function (data) {{");

			var mappingData = KnockoutJsModelBuilder.CreateMappingData<TModel>();
			if ( mappingData == "{}" )
				sb.AppendLine($"var {ViewModelName} = ko.mapping.fromJS(data); ");
			else
			{
				sb.AppendLine($"var {ViewModelName}MappingData = {mappingData};");
				sb.AppendLine($"var {ViewModelName} = ko.mapping.fromJS(data, {ViewModelName}MappingData); ");
			}
			sb.Append(KnockoutJsModelBuilder.AddComputedToModel(model, ViewModelName));
			sb.AppendLine
			(!string.IsNullOrEmpty(wrapperId)
				 ? $"ko.applyBindings({ViewModelName}, document.getElementById('{wrapperId}'))"
				 : $"ko.applyBindings({ViewModelName});");

			sb.AppendLine
				("}, error: function (error) { alert('There was an error posting the data to the server: ' + error.responseText); } });");

			sb.AppendLine("});");
			sb.AppendLine(@"</script>");

			return new HtmlString(sb.ToString());
		}

		public KnockoutForeachContext<TItem> Foreach <TItem>(Expression<Func<TModel, IList<TItem>>> binding)
		{
			var expression = KnockoutExpressionConverter.Convert(binding, CreateData());
			var regionContext = new KnockoutForeachContext<TItem>(_viewContext, expression);
			regionContext.WriteStart(_viewContext.Writer);
			regionContext.ContextStack = ContextStack;
			ContextStack.Add(regionContext);
			return regionContext;
		}

		public KnockoutWithContext<TItem> With <TItem>(Expression<Func<TModel, TItem>> binding)
		{
			var expression = KnockoutExpressionConverter.Convert(binding, CreateData());
			var regionContext = new KnockoutWithContext<TItem>(_viewContext, expression);
			regionContext.WriteStart(_viewContext.Writer);
			regionContext.ContextStack = ContextStack;
			ContextStack.Add(regionContext);
			return regionContext;
		}

		public KnockoutIfContext<TModel> If(Expression<Func<TModel, bool>> binding)
		{
			var regionContext = new KnockoutIfContext<TModel>(_viewContext, KnockoutExpressionConverter.Convert(binding))
			                    {
				                    InStack = false
			                    };
			regionContext.WriteStart(_viewContext.Writer);
			return regionContext;
		}

		public string GetInstanceName()
		{
			switch ( ActiveSubcontextCount )
			{
				case 0:
					return ContextStack.Count > 0 ? "$data" : "";
				case 1:
					return "$parent";
				default:
					return $"$parents[{ActiveSubcontextCount - 1}]";
			}
		}

		public string GetIndex() => this.GetContextPrefix() + "$index()";

		public virtual KnockoutExpressionData CreateData()
			=> new KnockoutExpressionData { InstanceNames = new [] { this.GetInstanceName() } };

		public virtual KnockoutBinding<TModel> Bind
			=> new KnockoutBinding<TModel>(this, CreateData().InstanceNames, CreateData().Aliases);

		public virtual KnockoutHtml<TModel> Html
			=> new KnockoutHtml<TModel>(_viewContext, this, CreateData().InstanceNames, CreateData().Aliases);

		public HtmlString ServerAction(string actionName, string controllerName, object routeValues = null)
		{
			var url = Url().Action(actionName, controllerName, routeValues);
			url = url.Replace("%28", "(");
			url = url.Replace("%29", ")");
			url = url.Replace("%24", "$");
			var exec = $"executeOnServer({ViewModelName}, '{url}')";
			var startIndex = 0;
			const string parentPrefix = "$parentContext.";
			while ( exec.Substring(startIndex).Contains("$index()") )
			{
				var pattern = "$index()";
				var nextPattern = parentPrefix + pattern;
				var index = startIndex + exec.Substring(startIndex).IndexOf(pattern, StringComparison.Ordinal);
				while ( index - parentPrefix.Length >= startIndex &&
				        exec.Substring(index - parentPrefix.Length, nextPattern.Length) == nextPattern )
				{
					index -= parentPrefix.Length;
					pattern = nextPattern;
					nextPattern = parentPrefix + pattern;
				}

				exec = exec.Substring(0, index) + "'+" + pattern + "+'" + exec.Substring(index + pattern.Length);
				startIndex = index + pattern.Length;
			}

			return new HtmlString(exec);
		}

		protected IUrlHelper Url()
			=>
			new UrlHelper
			(new ActionContext
				 (_viewContext.HttpContext, _viewContext.RouteData, _viewContext.ActionDescriptor, _viewContext.ModelState));

		#endregion

		#endregion

		#region Protected Members
		#region Properties
		protected List<IKnockoutContext> ContextStack { get; set; } = new List<IKnockoutContext>();

		#endregion
		#endregion
	}

}
