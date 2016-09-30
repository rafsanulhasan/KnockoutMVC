namespace Microsoft.AspNetCore.Mvc.Knockout.Utilities
{

	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public class KnockoutExpressionData
	{
		public ICollection<string> InstanceNames { get; set; }
		public IDictionary<string, string> Aliases { get; set; }
		public bool NeedBracketsForAllMembers { get; set; } = true;

		public KnockoutExpressionData()
		{
			InstanceNames = new [] { "" };
			Aliases = new Dictionary<string, string>();
		}


		public static KnockoutExpressionData CreateConstructorData() =>
			new KnockoutExpressionData
			{
				InstanceNames = new List<string> { "this" },
				NeedBracketsForAllMembers = true
			};

		public KnockoutExpressionData Clone()
		{
			var data = new KnockoutExpressionData { InstanceNames = InstanceNames ?? new List<string>() };
			if ( Aliases != null )
			{
				foreach ( var pair in this.Aliases )
					data.Aliases[pair.Key] = pair.Value;
			}

			data.NeedBracketsForAllMembers = NeedBracketsForAllMembers;
			return data;
		}
	}

}