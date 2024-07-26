﻿using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowestEM.Generator
{
    [Generator]
    public class ReaderToEntitySourceGenerator : ISourceGenerator
    {
        private Dictionary<string,string> supportReaderFieldType = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) 
        { 
            {"string","ReadToString" },
            {"short","ReadToInt16" },
            {"short?","ReadToInt16Nullable" },
            {"int","ReadToInt32" },
            {"int?","ReadToInt32Nullable" },
            {"long","ReadToInt64" },
            {"long?","ReadToInt64Nullable" },
            {"float","ReadToFloat" },
            {"float?","ReadToFloatNullable" },
            {"double","ReadToDouble" },
            {"double?","ReadToDoubleNullable" },
            {"decimal","ReadToDecimal" },
            {"decimal?","ReadToDecimalNullable" },
            {"bool","ReadToBoolean" },
            {"bool?","ReadToBooleanNullable" },
            {"char","ReadToChar" },
            {"char?","ReadToCharNullable" },
            {"byte","ReadToByte" },
            {"byte?","ReadToByteNullable" },
            {"DateTime","ReadToDateTime" },
            {"DateTime?","ReadToDateTimeNullable" },
        };
        public void Execute(GeneratorExecutionContext context)
        {
            var finder = context.SyntaxContextReceiver as FindClassReceiver;
            var cList = new StringBuilder();
            foreach (var namedType in finder.AllClass(context))
            {
                if (namedType.IsAbstract)
                    continue;
                if (namedType.IsGenericType)
                {
                    GenerateGenericClassMapper(context, cList, namedType);
                }
                else
                {
                    GenerateClassMapper(context, cList, namedType);
                }
            }

            GenerateEnableFunc(context, cList);
        }

        private void GenerateGenericClassMapper(GeneratorExecutionContext context, StringBuilder cList, INamedTypeSymbol namedType)
        {
            //                    var src = $@"
            //// <auto-generated/>
            //#pragma warning disable 8019 //disable 'unnecessary using directive' warning
            //using System;
            //using System.Runtime.CompilerServices;

            //namespace SlowestEM.Generator
            //{{
            //    public static partial class {namedType.Name}_Accessors
            //    {{
            //        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
            //        public static extern {namedType.ToDisplayString()} Ctor();
            //    }}
            //}}
            //";
            //                    context.AddSource($"{namedType.ToDisplayString()}.g.cs", src);
        }


        private void GenerateClassMapper(GeneratorExecutionContext context, StringBuilder cList, INamedTypeSymbol namedType)
        {
            var ps = namedType.GetAllSettableProperties().Where(i => supportReaderFieldType.ContainsKey(i.Type.ToRealTypeDisplayString())).ToList();
            if(ps == null || ps.Count == 0) return;
            var fullName = namedType.ToDisplayString();
            var src = $@"
// <auto-generated/>
#pragma warning disable 8019 //disable 'unnecessary using directive' warning
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace SlowestEM.Generator
{{
    public static partial class {namedType.Name}_Accessors
    {{
        {namedType.DeclaredAccessibility.ToDisplayString()} static IEnumerable<{fullName}> Read(IDataReader reader)
        {{
            var s = new Action<{fullName}>[reader.FieldCount];
            for (int i = 0; i < s.Length; i++)
            {{
                var j = i;
                switch (reader.GetName(j).ToLower())
                {{
                    {string.Join("", ps.Select(i => 
                                         {
                                             return $@"
                    case ""{i.Name.ToLower()}"": 
                    {{
                        // {i.Type.ToDisplayString()}
                        var needConvert = typeof({(i.Type.IsNullable() && i.Type is INamedTypeSymbol pnt ? pnt.TypeArguments[0].ToRealTypeDisplayString() : i.Type.ToRealTypeDisplayString())}) != reader.GetFieldType(i);
                        s[i] = d => d.{i.Name} = DBExtensions.{supportReaderFieldType[i.Type.ToRealTypeDisplayString()]}(reader,j,needConvert); 
                    }}
                    break;";
                                         }))}
                    default:
                        break;
                }}
            }}
            while (reader.Read())
            {{
                var d = new {fullName}();
                foreach (var item in s)
                {{
                    item?.Invoke(d);
                }}
                yield return d;
            }}
        }}
    }}
}}
            ";
            context.AddSource($"{fullName}_Accessors.g.cs", src);
            cList.AppendLine($"DBExtensions.ReaderCache[typeof({fullName})] = {namedType.Name}_Accessors.Read;");
        }

        private static void GenerateEnableFunc(GeneratorExecutionContext context, StringBuilder cList)
        {
            var generatorSrc = $@"
// <auto-generated/>
#pragma warning disable 8019 //disable 'unnecessary using directive' warning
using System;
using SlowestEM;

namespace SlowestEM.Generator
{{
    public static partial class EntitiesGenerator
    {{
        public static void Enable()
        {{
            {cList}
        }}
    }}
}}
";

            context.AddSource($"SlowestEM.Generator.EntitiesGenerator.g.cs", generatorSrc);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new FindClassReceiver());
        }
    }
}
