using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using EnumsNET;
using FastEnumUtility;
using NetEscapades.EnumGenerators;
using SlowestEM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Benchmark
{
    [EnumExtensions]
    [Flags]
    public enum Fruits
    {
        Apple = 1,
        Lemon = 2,
        Melon = 4,
        Banana = 8,
    }

    [MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class EnumTest
    {
        public EnumTest()
        {
            Enums<Fruits>.Instance = new FEnums();
        }

        [Benchmark(Baseline = true)]
        public Fruits Parse()
        {
            return Enum.Parse<Fruits>("melon", true);
        }

        [Benchmark]
        public Fruits FastEnumParse()
        {
            return FastEnum.Parse<Fruits>("melon", true);
        }

        [Benchmark]
        public Fruits EnumsParse()
        {
            return Enums.Parse<Fruits>("melon", true);
        }

        [Benchmark]
        public Fruits NetEscapadesEnumGeneratorsParse()
        {
            return FruitsExtensions.Parse("melon", true);
        }

        [Benchmark]
        public Fruits TryParseIgnoreCase()
        {
            TryParseIgnoreCase("melon", out var v);
            return v;
        }

        [Benchmark]
        public Fruits TryParseIgnoreCase2()
        {
            Enums<Fruits>.ParseIgnoreCase("melon", out var v);
            return v;
        }

        private static bool TryParseIgnoreCase(
            string? name,
            out Fruits value)
        {
            switch (StringHashing.NormalizedHash(name))
            {
                case -593928178:
                    value = global::Benchmark.Fruits.Apple;
                    return true;

                case -892798575:
                    value = global::Benchmark.Fruits.Lemon;
                    return true;

                case 2098206805:
                    value = global::Benchmark.Fruits.Melon;
                    return true;

                case 1044320250:
                    value = global::Benchmark.Fruits.Banana;
                    return true;

                case var _ when int.TryParse(name, out var val):
                    value = (global::Benchmark.Fruits)val;
                    return true;

                default:
                    value = default;
                    return false;
            }
        }

        private static bool TryParseIgnoreCase2(
#if NETCOREAPP3_0_OR_GREATER
            [global::System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            string? name,
            out global::Benchmark.Fruits value)
        {
            switch (name)
            {
                case string s when s.Equals(nameof(global::Benchmark.Fruits.Apple), global::System.StringComparison.OrdinalIgnoreCase):
                    value = global::Benchmark.Fruits.Apple;
                    return true;

                case string s when s.Equals(nameof(global::Benchmark.Fruits.Lemon), global::System.StringComparison.OrdinalIgnoreCase):
                    value = global::Benchmark.Fruits.Lemon;
                    return true;

                case string s when s.Equals(nameof(global::Benchmark.Fruits.Melon), global::System.StringComparison.OrdinalIgnoreCase):
                    value = global::Benchmark.Fruits.Melon;
                    return true;

                case string s when s.Equals(nameof(global::Benchmark.Fruits.Banana), global::System.StringComparison.OrdinalIgnoreCase):
                    value = global::Benchmark.Fruits.Banana;
                    return true;

                case string s when int.TryParse(name, out var val):
                    value = (global::Benchmark.Fruits)val;
                    return true;

                default:
                    value = default;
                    return false;
            }
        }
    }

    public interface IEnums<T>
    {
        bool ParseIgnoreCase(string name, out T value);
    }

    public class FEnums : IEnums<Fruits>
    {
        public bool ParseIgnoreCase(string name, out Fruits value)
        {
            switch (name)
            {
                case string s when s.Equals(nameof(global::Benchmark.Fruits.Apple), global::System.StringComparison.OrdinalIgnoreCase):
                    value = global::Benchmark.Fruits.Apple;
                    return true;

                case string s when s.Equals(nameof(global::Benchmark.Fruits.Lemon), global::System.StringComparison.OrdinalIgnoreCase):
                    value = global::Benchmark.Fruits.Lemon;
                    return true;

                case string s when s.Equals(nameof(global::Benchmark.Fruits.Melon), global::System.StringComparison.OrdinalIgnoreCase):
                    value = global::Benchmark.Fruits.Melon;
                    return true;

                case string s when s.Equals(nameof(global::Benchmark.Fruits.Banana), global::System.StringComparison.OrdinalIgnoreCase):
                    value = global::Benchmark.Fruits.Banana;
                    return true;

                case string s when int.TryParse(name, out var val):
                    value = (global::Benchmark.Fruits)val;
                    return true;

                default:
                    value = default;
                    return false;
            }
        }
    }

    public static class Enums<T>
    {
        public static IEnums<T> Instance;

        public static bool ParseIgnoreCase(string name, out T value)
        {
            return Instance.ParseIgnoreCase(name, out value);
        }
    }
}