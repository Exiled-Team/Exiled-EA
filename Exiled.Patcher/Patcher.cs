// -----------------------------------------------------------------------
// <copyright file="Patcher.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Patcher
{
    using System;
    using System.IO;

    using dnlib.DotNet;
    using dnlib.DotNet.Emit;

    /// <summary>
    /// Takes a file path to your assembly as input, and will patch the assembly with the Bootstrap class.
    /// Original Patcher created by KadeDev.
    /// </summary>
    internal static class Patcher
    {
        private static void Main(string[] args)
        {
            try
            {
                string path;

                if (args.Length != 1)
                {
                    Console.WriteLine("Provide the location of Assembly-CSharp.dll:");

                    path = Console.ReadLine();
                }
                else
                {
                    path = args[0];
                }

                ModuleDefMD module = ModuleDefMD.Load(path);

                if (module is null)
                {
                    Console.WriteLine($"File {path} not found!");
                    return;
                }

                Console.WriteLine($"Loaded {module.Name}");

                Console.WriteLine("Resolving References...");

                module.Context = ModuleDef.CreateModuleContext();

                ((AssemblyResolver)module.Context.AssemblyResolver).AddToCache(module);

                Console.WriteLine("Injecting the Bootstrap Class.");

                ModuleDefMD bootstrap = ModuleDefMD.Load(Path.Combine(Directory.GetCurrentDirectory(), "Exiled.Bootstrap.dll"));

                Console.WriteLine("Loaded " + bootstrap.Name);

                TypeDef modClass = bootstrap.Types[0];

                foreach (TypeDef type in bootstrap.Types)
                {
                    if (type.Name == "Bootstrap")
                    {
                        modClass = type;
                        Console.WriteLine($"Hooked to: \"{type.Namespace}.{type.Name}\"");
                    }
                }

                TypeDef modRefType = modClass;

                bootstrap.Types.Remove(modClass);

                modRefType.DeclaringType = null;

                module.Types.Add(modRefType);

                MethodDef call = FindMethod(modRefType, "Load");

                if (call is null)
                {
                    Console.WriteLine($"Failed to get the \"{call.Name}\" method! Maybe you don't have permission?");
                    return;
                }

                Console.WriteLine("Injected!");
                Console.WriteLine("Injection completed!");
                Console.WriteLine("Patching code...");

                TypeDef typeDef = FindType(module.Assembly, "ServerConsole");

                MethodDef start = FindMethod(typeDef, "Start");

                if (start is null)
                {
                    start = new MethodDefUser("Start", MethodSig.CreateInstance(module.CorLibTypes.Void), MethodImplAttributes.IL | MethodImplAttributes.Managed, MethodAttributes.Private | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
                    typeDef.Methods.Add(start);
                }

                start.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(call));

                module.Write(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(path)), "Assembly-CSharp-Exiled.dll"));

                Console.WriteLine("Patching completed successfully!");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"An error has occurred while patching: {exception}");
            }

            Console.Read();
        }

        private static MethodDef FindMethod(TypeDef type, string methodName)
        {
            if (type is not null)
            {
                foreach (MethodDef method in type.Methods)
                {
                    if (method.Name == methodName)
                        return method;
                }
            }

            return null;
        }

        private static TypeDef FindType(AssemblyDef assembly, string path)
        {
            foreach (ModuleDef module in assembly.Modules)
            {
                foreach (TypeDef type in module.Types)
                {
                    if (type.FullName == path)
                        return type;
                }
            }

            return null;
        }
    }
}