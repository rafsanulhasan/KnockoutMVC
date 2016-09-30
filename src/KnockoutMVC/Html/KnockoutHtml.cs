﻿

namespace Microsoft.AspNetCore.Html
{

	using Mvc.Knockout;
	using Mvc.Knockout.Utilities;
	using Mvc.Rendering;

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Linq.Expressions;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public class KnockoutHtml <TModel> : KnockoutSubContext<TModel>
	{
		#region Private Members

		#region Fields

		readonly ViewContext _viewContext;
		KnockoutContext<TModel> _knockoutContext;

		#endregion

		#endregion

		public KnockoutHtml(
			ViewContext viewContext,
			KnockoutContext<TModel> context,
			ICollection<string> instanceNames = null,
			IDictionary<string, string> aliases = null)
			: base(context, instanceNames, aliases)
		{
			_viewContext = viewContext;
			_knockoutContext = context;
			InstanceNames = instanceNames;
			Aliases = aliases;
		}

		KnockoutTagBuilder<TModel> Input(Expression<Func<TModel, object>> text, string type, object htmlAttributes = null)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "input", InstanceNames, Aliases);
			tagBuilder.ApplyAttributes(htmlAttributes);
			if ( !string.IsNullOrWhiteSpace(type) )
				tagBuilder.ApplyAttributes(new { type });
			if ( text != null )
			{
				tagBuilder.Value(text);
				var memberExpression = text.Body as MemberExpression;
				if ( memberExpression != null )
					tagBuilder.ApplyAttributes(new { name = memberExpression.Member.Name });
			}
			tagBuilder.TagRenderMode = TagRenderMode.SelfClosing;
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> TextBox(Expression<Func<TModel, object>> text, object htmlAttributes = null)
		{
			return Input(text, "text", htmlAttributes);
		}

		public KnockoutTagBuilder<TModel> Password(Expression<Func<TModel, object>> text, object htmlAttributes = null)
		{
			return Input(text, "password", htmlAttributes);
		}

		public KnockoutTagBuilder<TModel> Hidden(Expression<Func<TModel, object>> text = null, object htmlAttributes = null)
		{
			return Input(text, "hidden", htmlAttributes);
		}

		public KnockoutTagBuilder<TModel> RadioButton(Expression<Func<TModel, object>> @checked, object htmlAttributes = null)
		{
			var tagBuilder = Input(null, "radio", htmlAttributes);
			tagBuilder.Checked(@checked);
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> CheckBox(Expression<Func<TModel, object>> @checked, object htmlAttributes = null)
		{
			var tagBuilder = Input(null, "checkbox", htmlAttributes);
			tagBuilder.Checked(@checked);
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> TextArea(Expression<Func<TModel, object>> text, object htmlAttributes = null)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "textarea", InstanceNames, Aliases);
			tagBuilder.ApplyAttributes(htmlAttributes);
			tagBuilder.Value(text);
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> DropDownList <TItem>(
			Expression<Func<TModel, IList<TItem>>> options,
			object htmlAttributes = null,
			Expression<Func<TItem, object>> optionsText = null)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
			tagBuilder.ApplyAttributes(htmlAttributes);
			if ( options != null )
				tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
			if ( optionsText == null ) return tagBuilder;

			var data = new KnockoutExpressionData { InstanceNames = new [] { "item" } };
			tagBuilder.OptionsText("function(item) { return " + KnockoutExpressionConverter.Convert(optionsText, data) + "; }");
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> DropDownList <TItem>(
			Expression<Func<TModel, IList<TItem>>> options,
			object htmlAttributes = null,
			string optionsTextValue = null,
			string optionsIdValue = null)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
			tagBuilder.ApplyAttributes(htmlAttributes);
			if ( options != null )
				tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
			if ( !string.IsNullOrEmpty(optionsTextValue) )
				tagBuilder.OptionsText(optionsTextValue, true);
			if ( !string.IsNullOrEmpty(optionsIdValue) )
				tagBuilder.OptionsValue(optionsIdValue, true);
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> ListBox <TItem>(
			Expression<Func<TModel, IList<TItem>>> options,
			object htmlAttributes = null,
			Expression<Func<TItem, object>> optionsText = null)
		{
			var tagBuilder = DropDownList(options, htmlAttributes, optionsText);
			tagBuilder.ApplyAttributes(new { multiple = "multiple" });
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> DropDownList <TItem>(
			Expression<Func<TModel, IList<TItem>>> options,
			object htmlAttributes,
			Expression<Func<TModel, TItem, object>> optionsText)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "select", InstanceNames, Aliases);
			tagBuilder.ApplyAttributes(htmlAttributes);
			if ( options != null )
				tagBuilder.Options(Expression.Lambda<Func<TModel, IEnumerable>>(options.Body, options.Parameters));
			if ( optionsText != null )
			{
				var data = CreateData();
				var keys = data.Aliases.Keys.ToList();
				if ( !string.IsNullOrEmpty(Context.GetInstanceName()) )
				{
					foreach ( var key in keys )
					{
						data.Aliases[this.Context.GetInstanceName() + "." + key] = data.Aliases[key];
						data.Aliases.Remove(key);
					}
				}

				data.InstanceNames = new [] { Context.GetInstanceName(), "item" };
				tagBuilder.OptionsText("function(item) { return " + KnockoutExpressionConverter.Convert(optionsText, data) + "; }");
			}

			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> ListBox <TItem>(
			Expression<Func<TModel, IList<TItem>>> options,
			object htmlAttributes,
			Expression<Func<TModel, TItem, object>> optionsText)
		{
			var tagBuilder = DropDownList(options, htmlAttributes, optionsText);
			tagBuilder.ApplyAttributes(new { multiple = "multiple" });
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> Span(Expression<Func<TModel, object>> text, object htmlAttributes = null)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
			tagBuilder.ApplyAttributes(htmlAttributes);
			tagBuilder.Text(text);
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> Span(string text, object htmlAttributes = null)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
			tagBuilder.ApplyAttributes(htmlAttributes);
			tagBuilder.SetInnerHtml(new HtmlString(text).Value);
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> SpanInline(string text, object htmlAttributes = null)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "span", InstanceNames, Aliases);
			tagBuilder.ApplyAttributes(htmlAttributes);
			tagBuilder.Text(text);
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> Button(
			string caption,
			string actionName,
			string controllerName,
			object routeValues = null,
			object htmlAttributes = null)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "button", InstanceNames, Aliases);
			tagBuilder.ApplyAttributes(htmlAttributes);
			tagBuilder.Click(actionName, controllerName, routeValues);
			tagBuilder.SetInnerHtml(new HtmlString(caption).Value);
			return tagBuilder;
		}

		public KnockoutTagBuilder<TModel> HyperlinkButton(
			string caption,
			string actionName,
			string controllerName,
			object routeValues = null,
			object htmlAttributes = null)
		{
			var tagBuilder = new KnockoutTagBuilder<TModel>(Context, "a", InstanceNames, Aliases);
			tagBuilder.ApplyAttributes(htmlAttributes);
			tagBuilder.ApplyAttributes(new { href = "#" });
			tagBuilder.Click(actionName, controllerName, routeValues);
			tagBuilder.SetInnerHtml(new HtmlString(caption).Value);
			return tagBuilder;
		}

		public KnockoutFormContext<TModel> Form(
			string actionName,
			string controllerName,
			object routeValues = null,
			object htmlAttributes = null)
		{
			var formContext = new KnockoutFormContext<TModel>
			                  (
				                  _viewContext,
				                  Context,
				                  InstanceNames,
				                  Aliases,
				                  actionName,
				                  controllerName,
				                  routeValues,
				                  htmlAttributes);
			formContext.WriteStart(_viewContext.Writer);
			return formContext;
		}
	}

}