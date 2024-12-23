using System;
using System.IO;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats.CodeGen
{
    internal class AssemblyProcessorMain : ILPostProcessor
    {
        static readonly Type[] k_AssemblyProcessors =
        {
            typeof(EventPayloadRegistrationAssemblyProcessor),
            typeof(MetricIdTypeRegistrationAssemblyProcessor)
        };

        public sealed override ILPostProcessor GetInstance() => this;

        public sealed override bool WillProcess(ICompiledAssembly compiledAssembly)
            => CodeGenHelpers.AssemblyDependsOnNetStats(compiledAssembly);

        readonly List<DiagnosticMessage> m_Diagnostics = new List<DiagnosticMessage>();

        class Logger : IAssemblyProcessingLogger
        {
            readonly AssemblyProcessorMain m_ProcessorMain;
            readonly string m_ProcessorName;
            readonly string m_AssemblyName;

            public Logger(AssemblyProcessorMain processorMain, string processorName, string assemblyName)
            {
                m_ProcessorMain = processorMain;
                m_ProcessorName = processorName;
                m_AssemblyName = assemblyName;
            }

            public void LogError(string message)
                => m_ProcessorMain.m_Diagnostics.AddError($"[{m_ProcessorName}] [{m_AssemblyName}] {message}");
        }

        enum ProcessAssemblyResult
        {
            HasChangeOrError,
            Skip
        }

        public sealed override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
        {
            m_Diagnostics.Clear();

            var result = ProcessAssembly(compiledAssembly, out var assemblyDefinition);

            if (result == ProcessAssemblyResult.Skip)
            {
                return null;
            }

            return OverwriteAssembly(assemblyDefinition);
        }

        ProcessAssemblyResult ProcessAssembly(ICompiledAssembly compiledAssembly, out AssemblyDefinition assemblyDefinition)
        {
            if (!WillProcess(compiledAssembly))
            {
                assemblyDefinition = default;
                return ProcessAssemblyResult.Skip;
            }

            assemblyDefinition = CodeGenHelpers.AssemblyDefinitionFor(compiledAssembly, out _);
            if (assemblyDefinition == null)
            {
                m_Diagnostics.AddError($"Cannot read assembly definition: {compiledAssembly.Name}");
                return ProcessAssemblyResult.HasChangeOrError;
            }

            var mainModule = assemblyDefinition.MainModule;
            if (mainModule == null)
            {
                m_Diagnostics.AddError($"Failed to find main module from assembly: {compiledAssembly.Name}");
                return ProcessAssemblyResult.HasChangeOrError;
            }

            bool anyChanges = false;

            foreach (var processorType in k_AssemblyProcessors)
            {
                var logger = new Logger(this, processorType.Name, compiledAssembly.Name);

                var processor = Activator.CreateInstance(processorType) as IAssemblyProcessor;
                if (processor == null)
                {
                    logger.LogError("Invalid processor type");
                    return ProcessAssemblyResult.HasChangeOrError;
                }

                try
                {
                    if (!processor.ImportReferences(mainModule, logger))
                    {
                        logger.LogError($"Failed to import references");
                        return ProcessAssemblyResult.HasChangeOrError;
                    }
                }
                catch (Exception e)
                {
                    logger.LogError("Exception during ImportReferences: " + e);
                    return ProcessAssemblyResult.HasChangeOrError;
                }

                try
                {
                    anyChanges |= processor.ProcessAssembly(compiledAssembly, assemblyDefinition, mainModule, logger);
                }
                catch(Exception e)
                {
                    logger.LogError("Exception during ProcessAssembly: " + e);
                    return ProcessAssemblyResult.HasChangeOrError;
                }
            }

            if (!anyChanges)
            {
                return ProcessAssemblyResult.Skip;
            }

            return ProcessAssemblyResult.HasChangeOrError;
        }

        ILPostProcessResult OverwriteAssembly(AssemblyDefinition assemblyDefinition)
        {
            var pe = new MemoryStream();
            var pdb = new MemoryStream();

            var writerParameters = new WriterParameters
            {
                SymbolWriterProvider = new PortablePdbWriterProvider(),
                SymbolStream = pdb,
                WriteSymbols = true
            };

            assemblyDefinition.Write(pe, writerParameters);

            return new ILPostProcessResult(new InMemoryAssembly(pe.ToArray(), pdb.ToArray()), m_Diagnostics);
        }
    }
}
