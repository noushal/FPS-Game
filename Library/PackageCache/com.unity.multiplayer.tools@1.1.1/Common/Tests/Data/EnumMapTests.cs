using System;
using System.Collections.Generic;
using NUnit.Framework;


namespace Unity.Multiplayer.Tools.Common.Tests
{
    using static Unity.Multiplayer.Tools.Common.Tests.ContinuousEnum;

    enum EmptyEnum { }

    enum SingleValueEnum
    {
        A = 0,
    }
    enum ContinuousEnum
    {
        A, B, C, D, E, F, G, H,
    }
    enum ContinuousEnum_U8 : Byte
    {
        A, B, C, D, E, F, G, H,
    }
    enum ContinuousEnum_I16 : Int16
    {
        A, B, C, D, E, F, G, H,
    }
    enum ContinuousEnum_U32 : UInt32
    {
        A, B, C, D, E, F, G, H,
    }
    enum ContinuousEnum_I64 : Int64
    {
        A, B, C, D, E, F, G, H,
    }
    enum ContinuousEnumShiftedUp
    {
        A = 7, B, C, D, E, F, G, H,
    }
    enum ContinuousEnumShiftedDown
    {
        A = -5, B, C, D, E, F, G, H,
    }
    enum EnumWithDuplicates
    {
        A = B, B = 0,
        C = D, D = 1,
        E = F, F = 2,
        G = H, H = 3,
    }
    enum EnumWithHoles
    {
        A = 0,
        C = 2,
        E = 4,
        G = 6
    }
    enum EnumWithHolesAndDuplicates
    {
        A = B, B = 0,
        C = D, D = 2,
        E = F, F = 4,
        G = H, H = 6
    }
    enum EnumWithFlags
    {
        None = 0,
        A = 1 << 0,
        B = 1 << 1,
        C = 1 << 2,
        D = 1 << 3,
        E = 1 << 4,
        F = 1 << 5,
        G = 1 << 6,
        H = 1 << 7,
    }

    [TestFixture]
    class EnumMapTests
    {
        [TestCase((EmptyEnum)0                 , int.MaxValue, int.MinValue, 0)]
        [TestCase(SingleValueEnum.A            ,  0,   0,  1)]
        [TestCase(ContinuousEnum.A             ,  0,   7,  8)]
        [TestCase(ContinuousEnumShiftedUp.A    ,  7,  14,  8)]
        [TestCase(ContinuousEnumShiftedDown.A  , -5,   2,  8)]
        [TestCase(EnumWithDuplicates.A         ,  0,   3,  4)]
        [TestCase(EnumWithHoles.A              ,  0,   6,  4)]
        [TestCase(EnumWithHolesAndDuplicates.A ,  0,   6,  4)]
        [TestCase(EnumWithFlags.A              ,  0, 128,  9)]
        public void TestEnumCounts<TEnum>(TEnum _, int min, int max, int unique)
            where TEnum : Enum
        {
            var (resultMin, resultMax, resultUnique) = EnumContinuity.GetMinMaxAndUniqueValueCount<TEnum>();
            Assert.AreEqual(min, resultMin);
            Assert.AreEqual(max, resultMax);
            Assert.AreEqual(unique, resultUnique);
        }

        public enum ExpectedExceptionType
        {
            None,
            UnhandledBackingType,
            EmptyEnum,
            NonZeroEnumMinimumValue,
            DiscontinuousEnum,
        }

        [TestCase((EmptyEnum)0                 , ExpectedExceptionType.EmptyEnum)]
        [TestCase(SingleValueEnum.A            , ExpectedExceptionType.None)]
        [TestCase(ContinuousEnum.A             , ExpectedExceptionType.None)]
        [TestCase(ContinuousEnum_U8.A          , ExpectedExceptionType.UnhandledBackingType)]
        [TestCase(ContinuousEnum_I16.A         , ExpectedExceptionType.UnhandledBackingType)]
        [TestCase(ContinuousEnum_U32.A         , ExpectedExceptionType.UnhandledBackingType)]
        [TestCase(ContinuousEnum_I64.A         , ExpectedExceptionType.UnhandledBackingType)]
        [TestCase(ContinuousEnumShiftedUp.A    , ExpectedExceptionType.NonZeroEnumMinimumValue)]
        [TestCase(ContinuousEnumShiftedDown.A  , ExpectedExceptionType.NonZeroEnumMinimumValue)]
        [TestCase(EnumWithDuplicates.A         , ExpectedExceptionType.None)]
        [TestCase(EnumWithHoles.A              , ExpectedExceptionType.DiscontinuousEnum)]
        [TestCase(EnumWithHolesAndDuplicates.A , ExpectedExceptionType.DiscontinuousEnum)]
        [TestCase(EnumWithFlags.A              , ExpectedExceptionType.DiscontinuousEnum)]
        public void TestEnumContinuity<TEnum>(TEnum _, ExpectedExceptionType expectedExceptionType)
            where TEnum : unmanaged, Enum
        {
            switch (expectedExceptionType)
            {
            case ExpectedExceptionType.UnhandledBackingType:
            {
                Assert.Throws<UnhandledEnumBackingTypeException<TEnum, int>>(() =>
                {
                    EnumContinuity.ValidateEnumForEnumMap<TEnum, int>();
                });
                break;
            }
            case ExpectedExceptionType.EmptyEnum:
            {
                Assert.Throws<EmptyEnumException<TEnum, int>>(() =>
                {
                    EnumContinuity.ValidateEnumForEnumMap<TEnum, int>();
                });
                break;
            }
            case ExpectedExceptionType.NonZeroEnumMinimumValue:
            {
                Assert.Throws<NonZeroEnumMinimumValueException<TEnum, int>>(() =>
                {
                    EnumContinuity.ValidateEnumForEnumMap<TEnum, int>();
                });
                break;
            }
            case ExpectedExceptionType.DiscontinuousEnum:
            {
                Assert.Throws<DiscontinuousEnumException<TEnum, int>>(() =>
                {
                    EnumContinuity.ValidateEnumForEnumMap<TEnum, int>();
                });
                break;
            }
            default:
                Assert.DoesNotThrow(() =>
                {
                    EnumContinuity.ValidateEnumForEnumMap<TEnum, int>();
                });
                break;
            }
        }

        [Test]
        public void EnumMapUsageTest()
        {
            var map = new EnumMap<ContinuousEnum, int>();
            Assert.Zero(map[A]);
            Assert.Zero(map[B]);
            Assert.Zero(map[C]);
            Assert.Zero(map[D]);
            Assert.Zero(map[E]);
            Assert.Zero(map[F]);
            Assert.Zero(map[G]);
            Assert.Zero(map[H]);

            map[A] = 1;
            map[B] = 2;
            map[C] = map[A] + map[B];
            map[D] = map[B] + map[C];
            map[E] = map[C] + map[D];
            map[F] = map[D] + map[E];
            map[G] = map[E] + map[F];
            map[H] = map[F] + map[G];

            Assert.AreEqual( 1, map[A]);
            Assert.AreEqual( 2, map[B]);
            Assert.AreEqual( 3, map[C]);
            Assert.AreEqual( 5, map[D]);
            Assert.AreEqual( 8, map[E]);
            Assert.AreEqual(13, map[F]);
            Assert.AreEqual(21, map[G]);
            Assert.AreEqual(34, map[H]);
        }

        [Test]
        public void CollectionLiteralTest()
        {
            var map = new EnumMap<ContinuousEnum, int>
            {
                { A, 1 },
                { B, 2 },
                { C, 3 },
                { D, 5 },
                { E, 8 },
                { F, 13 },
                { G, 21 },
                { H, 34 },
            };
            Assert.AreEqual( 1, map[A]);
            Assert.AreEqual( 2, map[B]);
            Assert.AreEqual( 3, map[C]);
            Assert.AreEqual( 5, map[D]);
            Assert.AreEqual( 8, map[E]);
            Assert.AreEqual(13, map[F]);
            Assert.AreEqual(21, map[G]);
            Assert.AreEqual(34, map[H]);
        }

        [Test]
        public void IteratorTest()
        {
            var map = new EnumMap<ContinuousEnum, int>();
            map[A] = 1;
            map[B] = 2;
            map[C] = 3;
            map[D] = 5;
            map[E] = 8;
            map[F] = 13;
            map[G] = 21;
            map[H] = 34;
            Assert.AreEqual( 1, map[A]);
            Assert.AreEqual( 2, map[B]);
            Assert.AreEqual( 3, map[C]);
            Assert.AreEqual( 5, map[D]);
            Assert.AreEqual( 8, map[E]);
            Assert.AreEqual(13, map[F]);
            Assert.AreEqual(21, map[G]);
            Assert.AreEqual(34, map[H]);
            CollectionAssert.AreEquivalent(new KeyValuePair<ContinuousEnum, int>[]
            {
                // In C# >= 9 this can be much more concise with Target-typed new,
                // but until we drop support for Unity 2020.3 this must remain this way
                new KeyValuePair<ContinuousEnum, int>(A, 1),
                new KeyValuePair<ContinuousEnum, int>(B, 2),
                new KeyValuePair<ContinuousEnum, int>(C, 3),
                new KeyValuePair<ContinuousEnum, int>(D, 5),
                new KeyValuePair<ContinuousEnum, int>(E, 8),
                new KeyValuePair<ContinuousEnum, int>(F, 13),
                new KeyValuePair<ContinuousEnum, int>(G, 21),
                new KeyValuePair<ContinuousEnum, int>(H, 34),
            }, map);
        }
    }
}