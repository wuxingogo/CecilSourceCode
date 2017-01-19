using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using System.IO;



namespace TestCecil
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			MainClass mc = new MainClass ();
			for (int i = 0; i < args.Length; i++) {
				mc.Decompile (args [i]);
			}
		}

		void Decompile(string file)
		{
			Mono.Cecil.AssemblyDefinition assemblyDefinition = Mono.Cecil.AssemblyDefinition.ReadAssembly (file);
			AstBuilder astBuilder = null;

			foreach (var typeInAssembly in assemblyDefinition.MainModule.Types) {
				if (typeInAssembly.IsPublic) {
					Console.WriteLine ("T:{0}", typeInAssembly.Name);
					astBuilder = new AstBuilder(new ICSharpCode.Decompiler.DecompilerContext(assemblyDefinition.MainModule) { CurrentType = typeInAssembly } );
					astBuilder.AddType (typeInAssembly);
					StringWriter output = new StringWriter ();
					astBuilder.GenerateCode (new PlainTextOutput (output));
					string result = output.ToString ();
					output.Dispose ();
					using (StreamWriter outputFile = new StreamWriter("Output/" + typeInAssembly.Name +".cs")) {
						outputFile.Write (result);
					}
				}
			}
		}
	}
}