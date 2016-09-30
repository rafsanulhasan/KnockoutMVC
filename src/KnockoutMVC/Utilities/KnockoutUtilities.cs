using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.Mvc.Knockout.Utilities
{

	using System.Diagnostics.CodeAnalysis;

	using static KnockoutUtilities;

	[SuppressMessage("ReSharper", "ArrangeStaticMemberQualifier")]
    public static class KnockoutUtilities
    {
        static readonly List<AssemblyName> SystemNames = new List<AssemblyName>
        {
        new AssemblyName ("mscorlib, Version=4.0.0.0, Culture=neutral, " +
                          "PublicKeyToken=b77a5c561934e089"),
        new AssemblyName ("System.Core, Version=4.0.0.0, Culture=neutral, "+
                          "PublicKeyToken=b77a5c561934e089")
        };

	    public static void ConvertData(object data)
	    {
		    var type = data?.GetType();
		    if ( type?.Namespace == null )
			    return;

		    if ( type.GetTypeInfo().IsClass &&
		         type.Namespace.Equals("System.Data.Entity.DynamicProxies") )
			    type = type.GetTypeInfo().BaseType;
		    foreach ( var property in type.GetProperties() )
		    {
			    if ( property.GetCustomAttributes(typeof( Newtonsoft.Json.JsonIgnoreAttribute ), false).Any() )
				    continue;
			    if ( property.GetGetMethod() == null )
				    continue;
			    if ( property.GetGetMethod().GetParameters().Length > 0 )
				    continue;

			    var value = property.GetValue(data, null);
			    if ( value != null )
			    {
				    if ( !IsSystemType(property.PropertyType) )
						ConvertData(value);
			    }
			    else
			    {
				    value = GetActualValue(property.PropertyType, null);
				    if ( value != null &&
				         property.CanWrite )
					    property.SetValue(data, value, null);
			    }
		    }
	    }

	    public static object GetActualValue(Type type, object value)
        {
	        if ( value != null ) return value;

	        if (typeof(string).IsAssignableFrom(type))
		        return "";
	        return typeof(IList).IsAssignableFrom(type) ? type.GetConstructor(new Type[0]).Invoke(null) : null;
        }

        static bool IsSystemType(Type type)
        {
            var objAn = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            return KnockoutUtilities.SystemNames.Any(n => n.Name == objAn.Name &&
                                        n.GetPublicKeyToken().SequenceEqual(objAn.GetPublicKeyToken()));
        }
    }
}
