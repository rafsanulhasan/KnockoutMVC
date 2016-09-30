namespace Microsoft.AspNetCore.Mvc.Knockout.Binding
{
#if netcoreapp16
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Text;
	using Utilities;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public class KnockoutBingindComplexItem : KnockoutBindingItem
    {
        readonly List<KnockoutBindingItem> _subItems = new List<KnockoutBindingItem>();

        public void Add(KnockoutBindingItem subItem)
        {
            _subItems?.Add(subItem);
        }

        public override bool IsValid()
        {
            return _subItems?.Count > 0;
        }

        public override string GetKnockoutExpression(KnockoutExpressionData data)
        {
            var builder = new StringBuilder();

            builder.Append(Name);
            builder.Append(" : {");
            for (var i = 0; i < _subItems?.Count; i++)
            {
                if (i != 0)
                    builder.Append(",");
                builder.Append(_subItems[i].GetKnockoutExpression(data));
            }
            builder.Append('}');

            return builder.ToString();
        }
    }
#endif
}