using System;
using Mono.Cecil;

namespace TestCecil
{
	/// <summary>
	/// http://stackoverflow.com/questions/13526519/ilspy-failed-to-resolve-assembly-in-astbuilder
	/// </summary>
	public class MyDefaultAssemblyResolver : DefaultAssemblyResolver
	{
		public override AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			try
			{
				return base.Resolve(name);
			}
			catch { }
			return null;            
		}

		public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
		{
			try
			{
				return base.Resolve(name, parameters);
			}
			catch { }
			return null;
		}

		public override AssemblyDefinition Resolve(string fullName)
		{   
			try
			{
				return base.Resolve(fullName);
			}
			catch { }
			return null;
		}

		public override AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
		{
			try
			{
				return base.Resolve(fullName, parameters);
			}
			catch { }
			return null;
		}
	}
}

