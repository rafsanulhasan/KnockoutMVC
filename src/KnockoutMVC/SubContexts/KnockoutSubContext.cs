

namespace Microsoft.AspNetCore.Mvc.Knockout
{
	using System.Collections.Generic;

	using System.Diagnostics.CodeAnalysis;
	using Utilities;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
    public abstract class KnockoutSubContext<TModel>
    {
        protected KnockoutContext<TModel> Context { get; set; }
        protected ICollection<string> InstanceNames { get; set; }
        protected IDictionary<string, string> Aliases { get; set; }

        protected KnockoutSubContext(KnockoutContext<TModel> context, ICollection<string> instanceNames = null, IDictionary<string, string> aliases = null)
        {
            Context = context;
            InstanceNames = instanceNames;
            Aliases = aliases;
        }

        protected KnockoutExpressionData CreateData()
        {
            var data = new KnockoutExpressionData();
            if (InstanceNames != null)
                data.InstanceNames = InstanceNames;
            if (Aliases != null)
                data.Aliases = Aliases;
            return data.Clone();
        }
    }
}
