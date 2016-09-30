namespace Microsoft.AspNetCore.Mvc.Knockout.Utilities
{
	using DelegateDecompiler;
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Text;

	using Newtonsoft.Json;

	[SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
	public static class KnockoutJsModelBuilder
	{
		public static string AddComputedToModel <TModel>(TModel model, string modelName) => AddComputedToModel(typeof( TModel ), model, modelName);

		public static string AddComputedToModel(Type modelType, object model, string modelName)
		{
			var sb = new StringBuilder();
			if ( modelType.IsByRef &&
			     modelType.Namespace.Equals("System.Data.Entity.DynamicProxies") )
				modelType = modelType?.DeclaringType;
			foreach ( var property in modelType.GetProperties() )
			{
				if ( !property.GetCustomAttributes(typeof( ComputedAttribute ), false).Any() ) continue;

				sb.Append(modelName);
				sb.Append('.');
				sb.Append(property.Name);
				sb.Append(" = ");
				sb.Append("ko.computed(function() { try { return ");
				sb.Append
				(KnockoutJsModelBuilder.ExpressionToString
					 (modelType, DecompileExpressionVisitor.Decompile(Expression.Property(Expression.Constant(model), property))));
				sb.Append($"}} catch(e) {{ return null; }}  ;}}, {modelName});");
				sb.AppendLine();
			}

			foreach ( var property in modelType.GetProperties() )
			{
				if ( !property.PropertyType.IsByRef ||
				     typeof( IEnumerable ).IsAssignableFrom(property.PropertyType) ) continue;

				if ( property.GetCustomAttributes(typeof( JsonIgnoreAttribute ), false).Any() )
					continue;

				try
				{
					var value = property.GetValue(model, null);
					sb.Append
						(KnockoutJsModelBuilder.AddComputedToModel(property.PropertyType, value, $"{modelName}.{property.Name}"));
				}

				// ReSharper disable once EmptyGeneralCatchClause
				catch ( Exception ex )
				{
					Debug.WriteLine(ex);
				}
			}

			return sb.ToString();
		}

		public static string CreateMappingData <TModel>()
		{
			Type modelType = typeof( TModel );
			var builder = new StringBuilder();
			builder.AppendLine("{");

			bool first = true;
			foreach ( var property in modelType.GetProperties() )
			{
				if ( typeof( IEnumerable ).IsAssignableFrom(property.PropertyType) &&
				     !typeof( string ).IsAssignableFrom(property.PropertyType) &&
				     property.PropertyType.GetTypeInfo().IsGenericType )
				{
					Type itemType = property.PropertyType.GetGenericArguments()[0];
					var constructor = itemType.GetConstructor(new Type[0]);
					if ( constructor != null )
					{
						var item = constructor.Invoke(null);
						var computed = KnockoutJsModelBuilder.AddComputedToModel(itemType, item, "data");
						if ( string.IsNullOrWhiteSpace(computed) )
							continue;

						if ( first )
							first = false;
						else
							builder.Append(',');
						builder.Append("'");
						builder.Append(property.Name);
						builder.AppendLine("': { create: function(options) {");
						builder.AppendLine("var data = ko.mapping.fromJS(options.data);");

						builder.Append(computed);

						builder.AppendLine("return data;");
						builder.AppendLine("}}");
					}
				}
			}

			builder.Append("}");

			return first ? "{}" : builder.ToString();
		}

		static string ExpressionToString(Type modelType, Expression expression)
		{
			var data = KnockoutExpressionData.CreateConstructorData();
			data.Aliases[modelType.FullName] = "this";
			return KnockoutExpressionConverter.Convert(expression, data);
		}
	}

}
