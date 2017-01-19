using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using System.IO;
using UnityEngine;


namespace TestCecil
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Mono.Cecil.AssemblyDefinition assemblyDefinition = Mono.Cecil.AssemblyDefinition.ReadAssembly ("UnityEngine.dll");
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
					using (StreamWriter outputFile = new StreamWriter("Output/" + typeInAssembly.Name +".txt")) {
						outputFile.Write (result);
					}
				}
			}
		}
	}
}