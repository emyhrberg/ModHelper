using Basic.Reference.Assemblies;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using ModHelper.Helpers;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace ModHelper.Publicizier
{
    public class CompileSystem : ModSystem
    {
        // This is the delegate that will be used to hook into the RoslynCompile method
        public delegate Diagnostic[] RoslynCompileDelegate(
            string name,
            List<string> references,
            string[] files,
            string[] preprocessorSymbols,
            bool allowUnsafe,
            out byte[] code,
            out byte[] pdb);

        private Hook? RoslynCompileHook;

        // In load, we hook into the RoslynCompile method and replace it with our own implementation
        // that uses Publicizer to modify the assembly before compilation.
        public override void Load()
        {
            /* No longer needed! (it checks csproj file anyway)
             
            if (Conf.C != null && Conf.C.Compiler == "Publicizer")
            {
                Log.Info("Publicizer is selected as compiler!");
            }
            else
            {
                Log.Info("Default compiler is selected!");
                return;
            }
            */
            Assembly tModLoaderAssembly = typeof(Main).Assembly;
            Type modCompileType = tModLoaderAssembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo roslynCompileMethod = modCompileType.GetMethod("RoslynCompile", BindingFlags.NonPublic | BindingFlags.Static);
            /*
            RoslynCompileHook = new Hook(roslynCompileMethod,
                (RoslynCompileDelegate orig, string name, List<string> references, string[] files, string[] preprocessorSymbols, bool allowUnsafe, out byte[] code, out byte[] pdb) =>
                {
                    Log.Info("RoslynCompileHook called");
                    try
                    {
                        // Find the csproj file
                        string csprojFile = CompilerUtilities.FindCsprojFile(name, files);
                        if (!System.IO.File.Exists(csprojFile))
                        {
                            throw new Exception($"No csproj found in {csprojFile}");
                        }
                        else
                        {
                            Log.Info($"Found csproj file: {csprojFile}");
                        }

                        // Load the csproj file and find the Publicize references and their context
                        var doc = XDocument.Load(csprojFile);
                        var assemblyContexts = CompilerUtilities.GetPublicizerAssemblyContexts(doc);
                        var referencesToPublicize = assemblyContexts.Keys.ToList();

                        if (referencesToPublicize.Count == 0)
                        {
                            throw new Exception($"No Publicized mod references found in {Path.GetFileName(csprojFile)}");
                        }
                        else
                        {
                            Log.Info($"Publicize mod references found in {Path.GetFileName(csprojFile)}: {string.Join(", ", referencesToPublicize)}");
                        }

                        // Finding the dlls to publicize
                        Dictionary<string, string> dllPathsToPublicize = CompilerUtilities.FindReferencePaths(references, referencesToPublicize);

                        // Publicizing (or reading from files) the dlls
                        var publicizedModReferences = new List<PortableExecutableReference>();
                        foreach (var r in referencesToPublicize)
                        {
                            // Check if the dll path is valid
                            if (!dllPathsToPublicize.ContainsKey(r))
                            {
                                Log.Error($"Failed to find {r} in references!");
                                continue;
                            }

                            // Get the assembly context and dll path
                            var assemblyContext = assemblyContexts[r];
                            var dllPath = dllPathsToPublicize[r];

                            // Compute the hash and filename of the publicized dll path
                            var hash = CompilerUtilities.ComputeHash(dllPath, assemblyContext);
                            var filePath = CompilerUtilities.GetPRFolderPath($"{r}.{hash}.dll");

                            // Check if the publicized dll already exists
                            if (System.IO.File.Exists(filePath))
                            {
                                Log.Info($"Publicized mod reference {r} already exists, loading from {Path.GetFileName(filePath)}");

                                // Loading the publicized dll
                                var publicizedModReference = MetadataReference.CreateFromFile(filePath);
                                publicizedModReferences.Add(publicizedModReference);
                            }
                            else
                            {
                                Log.Info($"Publicizing mod reference {r} to {Path.GetFileName(filePath)}");

                                // Creating a module
                                using ModuleDef module = ModuleDefMD.Load(dllPathsToPublicize[r]);

                                // Publicizing the module
                                bool moduleChanged = PublicizeAssemblies.PublicizeAssembly(module, assemblyContext);
                                if (moduleChanged)
                                {
                                    Log.Info($"Module {r} is changed!");
                                }
                                else
                                {
                                    Log.Info($"Module {r} isn't changed!");
                                    continue;
                                }

                                // Writing the publicized dll to a file
                                var writerOptions = new ModuleWriterOptions(module)
                                {
                                    MetadataOptions = new MetadataOptions(MetadataFlags.KeepOldMaxStack),
                                    Logger = DummyLogger.NoThrowInstance
                                };
                                Utilities.LockingFile(filePath, (reader, writer) =>
                                {
                                    module.Write(writer.BaseStream, writerOptions);
                                });

                                // Loading the publicized dll
                                var publicizedModReference = MetadataReference.CreateFromFile(filePath);
                                publicizedModReferences.Add(publicizedModReference);
                            }
                        }

                        // Normal RoslynCompiler method
                        var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default,
                optimizationLevel: preprocessorSymbols.Contains("DEBUG") ? OptimizationLevel.Debug : OptimizationLevel.Release,
                allowUnsafe: true);

                        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview, preprocessorSymbols: preprocessorSymbols);

                        var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb);

                        var refs = references.Select(s => MetadataReference.CreateFromFile(s));
                        refs = refs.Concat(Net80.References.All);

                        // Adding references to the publicized mod
                        refs = refs.Concat(publicizedModReferences);

                        var src = files.Select(f => SyntaxFactory.ParseSyntaxTree(System.IO.File.ReadAllText(f), parseOptions, f, Encoding.UTF8));

                        // IACT 1
                        var asmAttrs = string.Join(Environment.NewLine,
                        referencesToPublicize.Select(name =>
                            $@"[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo(""{name}"")]"));

                        // IACT 2
                        const string attrDecl = @"
                        namespace System.Runtime.CompilerServices {
                            [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                            internal sealed class IgnoresAccessChecksToAttribute : System.Attribute {
                                public IgnoresAccessChecksToAttribute(string assemblyName) {}
                            }
                        }";

                        // adding IACT to the top of the file
                        src = src
                            .Prepend(SyntaxFactory.ParseSyntaxTree(asmAttrs, parseOptions))
                            .Append(SyntaxFactory.ParseSyntaxTree(attrDecl, parseOptions));

                        var comp = CSharpCompilation.Create(name, src, refs, options);

                        using var peStream = new MemoryStream();
                        using var pdbStream = new MemoryStream();
                        var results = comp.Emit(peStream, pdbStream, options: emitOptions);

                        code = peStream.ToArray();
                        pdb = pdbStream.ToArray();

                        return results.Diagnostics.Where(d => d.Severity >= DiagnosticSeverity.Warning).ToArray();
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"RoslynCompileHook error: {ex.Message}");

                        // Return the original method
                        return orig(name, references, files, preprocessorSymbols, allowUnsafe, out code, out pdb);
                    }
                });*/
            RoslynCompileHook = new Hook(roslynCompileMethod,
                (RoslynCompileDelegate orig, string name, List<string> references, string[] files, string[] preprocessorSymbols, bool allowUnsafe, out byte[] code, out byte[] pdb) =>
                {
                    Log.Info($"[ModHelper] Intercepted RoslynCompile for mod: {name}");

                    code = Array.Empty<byte>();
                    pdb = Array.Empty<byte>();

                    string csprojPath = CompilerUtilities.FindCsprojFile(name, files);
                    if (csprojPath == null)
                    {
                        Log.Warn($"[ModHelper] .csproj not found for mod: {name}, falling back to default compiler.");
                        return orig(name, references, files, preprocessorSymbols, allowUnsafe, out code, out pdb);
                    }

                    string objDir = Path.Combine(Path.GetDirectoryName(csprojPath), "obj");
                    string assetsFile = Path.Combine(objDir, "project.assets.json");
                    if (File.Exists(assetsFile))
                    {
                        try
                        {
                            File.Delete(assetsFile);
                            Log.Info("[ModHelper] Deleted old project.assets.json to avoid decode error.");
                        }
                        catch (Exception ex)
                        {
                            Log.Warn($"[ModHelper] Could not delete assets cache: {ex}");
                        }
                    }

                    Log.Info($"[ModHelper] Found .csproj: {csprojPath}");
                    string modDir = Path.GetDirectoryName(csprojPath);

                    try
                    {
                        var globalProperties = new Dictionary<string, string>
                        {
                            ["Configuration"] = "Debug",
                            ["Platform"] = "AnyCPU"
                        };

                        var buildRequest = new BuildRequestData(csprojPath, globalProperties, null, new[] { "Restore", "Build" }, null);
                        var buildParameters = new BuildParameters(ProjectCollection.GlobalProjectCollection)
                        {
                            Loggers = new List<Microsoft.Build.Framework.ILogger> { new ConsoleLogger(LoggerVerbosity.Detailed, msg => Log.Info(msg.TrimEnd()), color => { }, null) }
                        };

                        var result = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

                        if (result.OverallResult != BuildResultCode.Success)
                        {
                            Log.Warn("[ModHelper] MSBuild compilation failed. Falling back to default compiler.");
                            return orig(name, references, files, preprocessorSymbols, allowUnsafe, out code, out pdb);
                        }

                        Log.Info("[ModHelper] Mod compiled successfully via MSBuild.");

                        string outputDll = Directory.GetFiles(modDir, "*.dll", SearchOption.AllDirectories)
                            .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Equals(name, StringComparison.OrdinalIgnoreCase));
                        string outputPdb = Directory.GetFiles(modDir, "*.pdb", SearchOption.AllDirectories)
                            .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Equals(name, StringComparison.OrdinalIgnoreCase));

                        if (outputDll == null)
                        {
                            Log.Warn("[ModHelper] Compiled DLL not found after MSBuild. Falling back to default compiler.");
                            return orig(name, references, files, preprocessorSymbols, allowUnsafe, out code, out pdb);
                        }

                        code = File.ReadAllBytes(outputDll);
                        pdb = outputPdb != null ? File.ReadAllBytes(outputPdb) : Array.Empty<byte>();

                        Log.Info("[ModHelper] Returning compiled assembly from MSBuild.");
                        return Array.Empty<Diagnostic>();
                    }
                    catch (Exception ex)
                    {
                        Log.Warn($"[ModHelper] Exception during MSBuild compilation: {ex}");
                        return orig(name, references, files, preprocessorSymbols, allowUnsafe, out code, out pdb);
                    }
                    finally
                    {
                        RoslynCompileHook?.Dispose();
                    }
                }
            );

            GC.SuppressFinalize(RoslynCompileHook);

            LocalMod[] l = [];
        }

        public override void Unload()
        {
            RoslynCompileHook?.Dispose();
        }
    }
}