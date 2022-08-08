using System.Linq;
using static System.Runtime.Intrinsics.X86.Sse2;
using System.Runtime.Intrinsics;

namespace PokemonPRNG.SFMT

/// <summary>
/// modification of PokemonPRNG(https://github.com/yatsuna827/PokemonPRNG)
/// original:@sub_827
/// modifier:@21i10r29
/// </summary>
{
    /// <summary>
    /// SFMTの擬似乱数ジェネレータークラス。
    /// </summary>
    public class SFMT
    {
        /// <summary>
        /// 周期を表す指数。
        /// </summary>
        public const int MEXP = 19937;
        public const int POS1 = 122;
        public const int SL1 = 18;
        public const int SL2 = 1;
        public const int SR1 = 11;
        public const int SR2 = 1;
        public const uint MSK1 = 0xdfffffef;
        public const uint MSK2 = 0xddfecb7f;
        public const uint MSK3 = 0xbffaffff;
        public const uint MSK4 = 0xbffffff6;
        public const uint PARITY1 = 0x00000001;
        public const uint PARITY2 = 0x00000000;
        public const uint PARITY3 = 0x00000000;
        public const uint PARITY4 = 0x13c9e684;

        /// <summary>
        /// 各要素を128bitとしたときの内部状態ベクトルの個数。
        /// </summary>
        public const int N = MEXP / 128 + 1;
        /// <summary>
        /// 各要素を32bitとしたときの内部状態ベクトルの個数。
        /// </summary>
        public const int N32 = N * 4;

        private int randIndex;
        private readonly uint[] stateVector;

        public ulong Index32 { get; private set; }
        public ulong Index64 { get => Index32 / 2; }
        
        private SFMT(SFMT parent) { this.Index32 = parent.Index32; randIndex = parent.randIndex; stateVector = parent.stateVector.ToArray(); }
        public SFMT(uint seed)
        {
            //内部状態配列確保
            stateVector = new uint[N32];

            //内部状態配列初期化
            stateVector[0] = seed;
            for (int i = 1; i < stateVector.Length; i++) stateVector[i] = (uint)(1812433253 * (stateVector[i - 1] ^ (stateVector[i - 1] >> 30)) + i);

            //確認
            PeriodCertification();

            //初期位置設定
            randIndex = N32;

            Index32 = 0;
        }

        public SFMT Clone() => new SFMT(this);

        /// <summary>
        /// 符号なし32bitの擬似乱数を取得します。
        /// </summary>
        public uint GetRand32()
        {
            if (randIndex >= N32)
            {
                GenerateRandAll();
                randIndex = 0;
            }

            Index32++;
            return stateVector[randIndex++];
        }
        public uint GetRand32(uint m) => GetRand32() % m;

        public ulong GetRand64() => GetRand32() | ((ulong)GetRand32() << 32);
        public ulong GetRand64(uint m) => GetRand64() % m;

        /// <summary>
        /// 内部状態ベクトルが適切か確認し、必要であれば調節します。
        /// </summary>
        private void PeriodCertification()
        {
            uint[] PARITY = new uint[] { PARITY1, PARITY2, PARITY3, PARITY4 };

            uint inner = 0;
            for (int i = 0; i < 4; i++) inner ^= stateVector[i] & PARITY[i];
            for (int i = 16; i > 0; i >>= 1) inner ^= inner >> i;

            // check OK
            if ((inner & 1) == 1) return;

            // check NG, and modification
            for (int i = 0; i < 4; i++)
            {
                uint work = 1;
                for (int j = 0; j < 32; j++, work <<= 1)
                {
                    if ((work & PARITY[i]) != 0)
                    {
                        stateVector[i] ^= work;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// GenerateRandAll()を外から呼んで悪いことをする。
        /// </summary>
        public void SkipSM()
        {
            GenerateRandAll();
            GenerateRandAll();
            GenerateRandAll();
            GenerateRandAll();

            randIndex = 152;
            Index32 = 2024;
        }

        /// <summary>
        /// GenerateRandAll()を外から呼んで悪いことをする。
        /// </summary>
        public void SkipUSUM()
        {
            GenerateRandAll();
            GenerateRandAll();
            GenerateRandAll();
            GenerateRandAll();

            randIndex = 392;
            Index32 = 2264;
        }

        /// <summary>
        /// gen_rand_allの(2^19937-1)周期用。
        /// </summary>
        private unsafe void GenerateRandAll()
        {
            fixed (uint* pPtr = this.stateVector)
            {
                var r1 = LoadVector128(pPtr+4*(N-2));
                var r2 = LoadVector128(pPtr+4*(N-1));
                int i=0;
                for (; i < N - POS1; i++)
                {
                    Store(pPtr + 4*i, mm_recursion(LoadVector128(pPtr + 4 * i), LoadVector128(pPtr + 4 * (i + POS1)), r1, r2));
                    r1 = r2;
                    r2 = LoadVector128(pPtr + 4*i);
                }
                for (; i < N; i++)
                {
                    Store(pPtr + 4*i, mm_recursion(LoadVector128(pPtr + 4 * i), LoadVector128(pPtr + 4*(i + POS1 - N)), r1, r2));
                    r1 = r2;
                    r2 = LoadVector128(pPtr + 4*i);
                }
            } ;
            ;

        }

        public readonly uint[] MSK = new uint[] { MSK1, MSK2, MSK3, MSK4 };
        private unsafe Vector128<uint> mm_recursion(Vector128<uint> a, Vector128<uint> b, Vector128<uint> c, Vector128<uint> d)
        {
            Vector128<uint> v, x, y, z, r;

            fixed (uint* pMSK = MSK)
            {
                var MASK = LoadVector128(pMSK);

                y = ShiftRightLogical(b, SR1);
                z = ShiftRightLogical128BitLane(c, SR2);
                v = ShiftLeftLogical(d, SL1);
                z = Xor(z, a);
                z = Xor(z, v);
                x = ShiftLeftLogical128BitLane(a, SL2);
                y = And(y, MASK);
                z = Xor(z, x);
                r = Xor(z, y);
                return r;
            }
        }
        

    }
}
