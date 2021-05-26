namespace LeanCode.ContractsGenerator.Tests
{
    public static class ExampleBasedHelpers
    {
        private static string Wrap(this string code)
        {
            return @$"
using System;
using System.Collections;
using System.Collections.Generic;
using LeanCode.CQRS;
{code}";
        }

        public static Export Compiles(this string code)
        {
            var compiled = ContractsCompiler.CompileCode(code.Wrap());
            return new ContractsGenerator(compiled).Generate(string.Empty);
        }
    }
}
