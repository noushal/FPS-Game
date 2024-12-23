using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using UnityEngine;

namespace Unity.Multiplayer.Tools.NetStats.CodeGen
{
    sealed class MetricIdTypeRegistrationAssemblyProcessor : IAssemblyProcessor
    {
        TypeReference m_MetricTypeEnumAttributeTypeRef;
        TypeReference m_MetricIdTypeLibraryTypeRef;
        MethodReference m_MetricIdTypeLibraryRegisterTypeRef;

        public bool ImportReferences(
            ModuleDefinition moduleDefinition,
            IAssemblyProcessingLogger logger)
        {
            m_MetricTypeEnumAttributeTypeRef = moduleDefinition.ImportReference(typeof(MetricTypeEnumAttribute));
            m_MetricIdTypeLibraryTypeRef = moduleDefinition.ImportReference(typeof(MetricIdTypeLibrary));
            m_MetricIdTypeLibraryRegisterTypeRef =
                moduleDefinition.ImportReference(
                    typeof(MetricIdTypeLibrary).GetMethod(
                        nameof(MetricIdTypeLibrary.RegisterType),
                        BindingFlags.Static | BindingFlags.Public));
            return true;
        }
        
        public bool ProcessAssembly(
            ICompiledAssembly compiledAssembly, 
            AssemblyDefinition assemblyDefinition, 
            ModuleDefinition mainModule,
            IAssemblyProcessingLogger logger)
        {
            var metricTypeEnums = mainModule.GetTypes()
                .Where(type =>
                    type.IsEnum
                    && type.HasCustomAttributes
                    && type.CustomAttributes.Any(
                        attribute =>
                            attribute.AttributeType.Name.Equals(m_MetricTypeEnumAttributeTypeRef.Name)))
                .OrderBy(e => e.FullName)
                .ToList();

            if (metricTypeEnums.Count == 0)
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

                        foreach (var type in metricTypeEnums)
                        {
                            var importedType = mainModule.ImportReference(type);
                            var genericInstanceMethod = CodeGenHelpers.CreateStaticGenericMethod(
                                mainModule,
                                m_MetricIdTypeLibraryTypeRef,
                                m_MetricIdTypeLibraryRegisterTypeRef,
                                importedType);
                            instructions.Add(processor.Create(OpCodes.Call, genericInstanceMethod));
                        }

                        return instructions;
                    });
            }
            catch (Exception e)
            {
                logger.LogError((e + e.StackTrace).Replace("\n", "|").Replace("\r", "|"));
            }

            return true;
        }
    }
}
