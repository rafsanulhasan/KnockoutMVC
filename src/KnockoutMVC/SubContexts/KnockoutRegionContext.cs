namespace Microsoft.AspNetCore.Mvc.Knockout
{

	using Rendering;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.IO;

	[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
	public abstract class KnockoutRegionContext <TModel> : KnockoutContext<TModel>, IDisposable
	{
		#region Protected Constructors

		protected KnockoutRegionContext(ViewContext viewContext)
			: base(viewContext)
		{
			if ( viewContext == null )
				throw new ArgumentNullException(nameof(viewContext));

			_writer = viewContext.Writer;
			InStack = true;
		}

		#endregion

		#region Public Properties

		public bool InStack { get; set; }

		#endregion

		#region Private Fields

		bool _disposed;
		readonly TextWriter _writer;

		#endregion

		#region Public Methods

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region Protected Methods

		protected void Dispose(bool disposing)
		{
			if ( _disposed )
				return;

			_disposed = true;

			WriteEnd(_writer);
			if ( InStack )
				ContextStack.RemoveAt(ContextStack.Count - 1);
		}

		#endregion

		#region Abstract Methods

		public abstract void WriteStart(TextWriter writer);
		protected abstract void WriteEnd(TextWriter writer);

		#endregion
	}

}
