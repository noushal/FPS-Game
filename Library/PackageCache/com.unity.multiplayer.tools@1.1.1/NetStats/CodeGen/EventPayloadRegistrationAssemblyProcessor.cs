using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace Unity.Multiplayer.Tools.NetStats.CodeGen
{
    sealed class EventPayloadRegistrationAssemblyProcessor : IAssemblyProcessor
    {
        TypeReference m_EventTypeTypeRef;
        TypeReference m_EventMetricFactoryTypeRef;
        MethodReference m_EventMetricFactoryRegisterMethodRef;

        public bool ImportReferences(
            ModuleDefinition moduleDefinition,
            IAssemblyProcessingLogger logger)
        {
            m_EventTypeTypeRef = moduleDefinition.ImportReference(typeof(EventMetric<>));
            m_EventMetricFactoryTypeRef = moduleDefinition.ImportReference(typeof(EventMetricFactory));
            m_EventMetricFactoryRegisterMethodRef =
                moduleDefinition.ImportReference(
                    typeof(EventMetricFactory).GetMethod(
                        nameof(EventMetricFactory.RegisterType),
                        BindingFlags.Static | BindingFlags.NonPublic));
            
            return true;
        }
        
        public bool ProcessAssembly(
            ICompiledAssembly compiledAssembly, 
            AssemblyDefinition assemblyDefinition, 
            ModuleDefinition mainModule,
            IAssemblyProcessingLogger logger)
        {
            var fieldTypes = mainModule.GetTypes()
                .SelectMany(t => t.Fields)
                .Where(f => IsEventType(f.FieldType))
                .Select(f => ((GenericInstanceType)f.FieldType).GenericArguments[0].Resolve());

            var variableTypes = mainModule.GetTypes()
                .SelectMany(t => t.Methods)
                .Where(m => m.HasBody)
                .SelectMany(m => m.Body.Variables)
                .Where(v => IsEventType(v.VariableType))
                .Select(v => ((GenericInstanceType)v.VariableType).GenericArguments[0].Resolve());

            var metricTypes = new List<TypeDefinition>();
            metricTypes.AddRange(fieldTypes);
            metricTypes.AddRange(variableTypes);

            metricTypes = metricTypes.Distinct().Where(n => n != null).ToList();

            if (metricTypes.Count == 0)
            {
                return false;
            }

            try
            {
                CodeGenHelpers.InjectTypeRegistration(
                    assemblyDefinition,
                    mainModule, 
                    processor =>
                    {
                        var instructions = new List<Instruction>();

                        foreach (var type in metricTypes)
                        {
                            var importedType = mainModule.ImportReference(type);
                            var genericInstanceMethod = CodeGenHelpers.CreateStaticGenericMethod(
                                mainModule,
                                m_EventMetricFactoryTypeRef,
                                m_EventMetricFactoryRegisterMethodRef,
                                importedType);
                            instructions.Add(processor.Create(OpCodes.Call, genericInstanceMethod));
                        }

                        return instructions;
                    });
            }
            catch (Exception e)
            {
                logger.LogError((e + e.StackTrace.ToString()).Replace("\n", "|").Replace("\r", "|"));
            }

            return true;
        }

        bool IsEventType(TypeReference type)
        {
            return type.IsGenericInstance && type.Resolve() == m_EventTypeTypeRef.Resolve();
        }
    }
}
