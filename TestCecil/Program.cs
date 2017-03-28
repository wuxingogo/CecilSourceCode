using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using System.IO;
using System.Collections.Generic;



namespace TestCecil
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string fullCommand = "";
			for (int i = 0; i < args.Length; i++) {
				fullCommand += args [i] + " ";
			}
			Console.WriteLine ("Excute : " + fullCommand);
			MainClass mc = new MainClass ();
			int l = args.Length;
			Console.WriteLine ("l length is : " + l);
			if (l == 0) {
				Console.WriteLine ("Argument has Error!");
				return;
			} else {
				List<string> allDll = new List<string> ();
				for (int i = 0; i < args.Length; i++) {
					switch (args[i]) {
					case "-a":
						{
							for (int j = i + 1; j < args.Length; j++) {
								allDll.Add (args [j]);
							}
							mc.DecompileAllDLL (allDll);
						}
						return;
					case "-s":
						List<string> allclass = new List<string> ();
						for (int j = i + 1; j < args.Length; j++) {
							allclass.Add (args [j]);
						}
						mc.DecompileSingleFile (allDll, allclass);
						return;
					case "-v":
						for (int j = i + 1; j < args.Length; j++) {
							allDll.Add (args [j]);
						}
						mc.DecompileAllAssembly (allDll);
						return;
					default:
						allDll.Add (args[i]);
						break;
					}
				}



			}

		}

		void DecompileSingleFile(List<string> allDll,List<string> allClass)
		{
			for (int i = 0; i < allDll.Count; i++) {
				for (int j = 0; j < allClass.Count; j++) {
					Console.WriteLine ("******* start decompile : " + allDll[i] + "--" + allClass[i] + " *******");
					DecompileFile (allDll [i], allClass [j]);
					Console.WriteLine ("******* finish decompile : " + allDll[i] + "--" + allClass[i] + " *******");
				}
			}
		}

		void DecompileAllDLL(List<string> args)
		{
			Console.WriteLine ("******* Mode : DecompileAllDLL *******");
			for (int i = 0; i < args.Count; i++) {
				Console.WriteLine ("******* start decompile : " + args[i] + " *******");
				this.DecompileDLL (args [i]);
				Console.WriteLine ("******* finish decompile : " + args[i] + " *******");
			}
		}
		void DecompileAllAssembly(List<string> args)
		{
			Console.WriteLine ("******* Mode : DecompileAllAssembly *******");
			for (int i = 0; i < args.Count; i++) {
				Console.WriteLine ("******* start decompile : " + args[i] + " *******");
				this.DecompileAssembly (args [i]);
				Console.WriteLine ("******* finish decompile : " + args[i] + " *******");
			}
		}

		void DecompileFile(string dllName, string className)
		{
			string directoryName = GetDirectoryName (dllName);
			if (!Directory.Exists (directoryName)) {
				Console.WriteLine ("Create Directory assembly : " + directoryName);
				Directory.CreateDirectory (directoryName);
			}
			var resolver = new MyDefaultAssemblyResolver();
			resolver.AddSearchDirectory("FolderOfMyAssembly");
			var parameters = new ReaderParameters
			{
				AssemblyResolver = resolver,
			};

			Mono.Cecil.AssemblyDefinition assemblyDefinition = Mono.Cecil.AssemblyDefinition.ReadAssembly (dllName, parameters);
			AstBuilder astBuilder = null;
			foreach (var typeInAssembly in assemblyDefinition.MainModule.Types) {
				if (typeInAssembly.Name == className) {
					astBuilder = new AstBuilder(new ICSharpCode.Decompiler.DecompilerContext(assemblyDefinition.MainModule) { CurrentType = typeInAssembly } );
					astBuilder.AddType (typeInAssembly);
					StringWriter output = new StringWriter ();
					astBuilder.GenerateCode (new PlainTextOutput (output));
					string result = output.ToString ();
					Console.WriteLine ("======== Decompile Start ========");
					Console.WriteLine (result);
					Console.WriteLine ("======== Decompile Finish ========");
					output.Dispose ();
					using (StreamWriter outputFile = new StreamWriter(directoryName +"/" + typeInAssembly.Name +".cs")) {
						outputFile.Write (result);
					}
				}
			}
		}



		void DecompileDLL(string dllName)
		{
			string directoryName = GetDirectoryName (dllName);
			if (!Directory.Exists (directoryName)) {
				Console.WriteLine ("Create Directory assembly : " + directoryName);
				Directory.CreateDirectory (directoryName);
			}
			var resolver = new MyDefaultAssemblyResolver();
			resolver.AddSearchDirectory("FolderOfMyAssembly");
			var parameters = new ReaderParameters
			{
				AssemblyResolver = resolver,
			};
			
			Mono.Cecil.AssemblyDefinition assemblyDefinition = Mono.Cecil.AssemblyDefinition.ReadAssembly (dllName, parameters);
			AstBuilder astBuilder = null;

			foreach (var typeInAssembly in assemblyDefinition.MainModule.Types) {

				Console.WriteLine ("T:{0}", typeInAssembly.Name);

				try{
					astBuilder = new AstBuilder(new ICSharpCode.Decompiler.DecompilerContext(assemblyDefinition.MainModule) { CurrentType = typeInAssembly } );
					astBuilder.AddType (typeInAssembly);
					StringWriter output = new StringWriter ();
					astBuilder.GenerateCode (new PlainTextOutput (output));
					string result = output.ToString ();
					output.Dispose ();
					using (StreamWriter outputFile = new StreamWriter(directoryName+ "/" + typeInAssembly.Name +".cs")) {
						outputFile.Write (result);
					}
				}catch(AssemblyResolutionException e){
					Console.WriteLine (e.ToString ());
				}


			}
		}
		string GetDirectoryName(string dllName)
		{
			return dllName.Substring(0, dllName.IndexOf('.'));
		}

		void DecompileAssembly(string assembly)
		{
			string directoryName = GetDirectoryName (assembly);
			if (!Directory.Exists (assembly)) {
				Console.WriteLine ("Create Directory assembly : " + assembly);
				Directory.CreateDirectory (assembly);
			}
			var resolver = new MyDefaultAssemblyResolver();
			resolver.AddSearchDirectory("FolderOfMyAssembly");
			var parameters = new ReaderParameters
			{
				AssemblyResolver = resolver,
			};

			Mono.Cecil.AssemblyDefinition assemblyDefinition = Mono.Cecil.AssemblyDefinition.CreateAssembly (new AssemblyNameDefinition (assembly, new Version()), assembly, ModuleKind.Dll);
			AstBuilder astBuilder = null;

			foreach (var typeInAssembly in assemblyDefinition.MainModule.Types) {

				Console.WriteLine ("T:{0}", typeInAssembly.Name);

				try{
					astBuilder = new AstBuilder(new ICSharpCode.Decompiler.DecompilerContext(assemblyDefinition.MainModule) { CurrentType = typeInAssembly } );
					astBuilder.AddType (typeInAssembly);
					StringWriter output = new StringWriter ();
					astBuilder.GenerateCode (new PlainTextOutput (output));
					string result = output.ToString ();
					output.Dispose ();
					using (StreamWriter outputFile = new StreamWriter(directoryName + "/" + typeInAssembly.Name +".cs")) {
						outputFile.Write (result);
					}
				}catch(AssemblyResolutionException e){
					Console.WriteLine (e.ToString ());
				}


			}
		}
	}
}