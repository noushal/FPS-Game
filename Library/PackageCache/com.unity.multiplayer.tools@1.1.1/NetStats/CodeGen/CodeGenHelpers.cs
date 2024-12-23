using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats.CodeGen
{
    static class CodeGenHelpers
    {
        const string k_NetStatsAssemblyName = "Unity.Multiplayer.Tools.NetStats";
        
        public static bool AssemblyDependsOnNetStats(ICompiledAssembly compiledAssembly) =>
            compiledAssembly.References.Any(reference => reference.Contains(k_NetStatsAssemblyName));
        
        public static void AddError(this List<DiagnosticMessage> diagnostics, string message)
        {
            diagnostics.AddError((SequencePoint)null, message);
        }

        public static void AddError(this List<DiagnosticMessage> diagnostics, MethodDefinition methodDefinition, string message)
        {
            diagnostics.AddError(methodDefinition.DebugInformation.SequencePoints.FirstOrDefault(), message);
        }

        static void AddError(this List<DiagnosticMessage> diagnostics, SequencePoint sequencePoint, string message)
        {
            diagnostics.Add(new DiagnosticMessage
            {
                DiagnosticType = DiagnosticType.Error,
                File = sequencePoint?.Document.Url.Replace($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}", ""),
                Line = sequencePoint?.StartLine ?? 0,
                Column = sequencePoint?.StartColumn ?? 0,
                MessageData = $" - {message}"
            });
        }
        
        public static AssemblyDefinition AssemblyDefinitionFor(ICompiledAssembly compiledAssembly, out PostProcessorAssemblyResolver assemblyResolver)
        {
            assemblyResolver = new PostProcessorAssemblyResolver(compiledAssembly);
            var readerParameters = new ReaderParameters
            {
                SymbolStream = new MemoryStream(compiledAssembly.InMemoryAssembly.PdbData),
                SymbolReaderProvider = new PortablePdbReaderProvider(),
                AssemblyResolver = assemblyResolver,
                ReflectionImporterProvider = new PostProcessorReflectionImporterProvider(),
                ReadingMode = ReadingMode.Immediate
            };

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(new MemoryStream(compiledAssembly.InMemoryAssembly.PeData), readerParameters);
            assemblyResolver.AddAssemblyDefinitionBeingOperatedOn(assemblyDefinition);

            return assemblyDefinition;
        }
        
        public static void InjectTypeRegistration(
            AssemblyDefinition assembly,
            ModuleDefinition module,
            Func<ILProcessor, List<Instruction>> instructionsFactory)
        {
            var type = module.Types.FirstOrDefault(t => t.FullName == TypeRegistration.k_ClassName);
            
            MethodDefinition method;
            if (type != null)
            {
                method = type.Methods.First(m => m.Name == TypeRegistration.k_MethodName);
            }
            else
            {
                type = new TypeDefinition(
                    string.Empty,
                    TypeRegistration.k_ClassName,
                    TypeAttributes.Class | TypeAttributes.Public,
                    module.TypeSystem.Object);

                method = new MethodDefinition(
                    TypeRegistration.k_MethodName,
                    MethodAttributes.Static,
                    module.TypeSystem.Void);
                method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                
                type.Methods.Add(method);
                module.Types.Add(type);
                
                var assemblyContainsNetStatsTypesAttribute = module.ImportReference(
                    typeof(AssemblyRequiresTypeRegistrationAttribute)
                        .GetConstructor(Type.EmptyTypes));
                assembly.CustomAttributes.Add(
                    new CustomAttribute(
                        assemblyContainsNetStatsTypesAttribute));
            }
            
            var processor = method.Body.GetILProcessor();
            var instructions = instructionsFactory.Invoke(processor);
            instructions.ForEach(instruction => processor.Body.Instructions.Insert(processor.Body.Instructions.Count - 1, instruction));
        }

        public static GenericInstanceMethod CreateStaticGenericMethod(
            ModuleDefinition module, 
            TypeReference staticType, 
            MethodReference staticGenericMethod,
            TypeReference genericTypeArgument)
        {
            var method = new MethodReference(staticGenericMethod.Name, module.TypeSystem.Void, staticType);
            var genericParameter = new GenericParameter(method);
            method.GenericParameters.Add(genericParameter);
            method.HasThis = false;
            var genericInstanceMethod = new GenericInstanceMethod(method);
            genericInstanceMethod.GenericArguments.Add(genericTypeArgument);

            return genericInstanceMethod;
        }
    }
}
