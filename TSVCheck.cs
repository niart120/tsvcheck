using PokemonStandardLibrary;
using PokemonPRNG.SFMT;
using tsvcheckGUI;

namespace tsvcheck
{
    public class TSVChecker
    {
        private readonly uint g7tid;
        private readonly List<uint> ivs;
        private readonly Nature nature;
        private readonly bool isUSUM;
        private readonly int min;
        private readonly int max;
        private Form1 form;

        public TSVChecker(uint g7TID, List<uint> IVs, Nature Nature, bool IsUSUM, int Min, int Max, Form1 Form)
        {
            g7tid = g7TID;
            ivs = IVs;
            nature = Nature;
            isUSUM = IsUSUM;
            min = Min;
            max = Max;
            form = Form;
        }

        public bool Check(CancellationToken ct)
        {
            const int totalTicks = 4096;
            form.ResetProgress();

            for (uint t = 0; t < totalTicks; t++)
            {
                uint header = t << 20;

                var result = Parallel.For(0, 1<<20, (i,state) => {
                    uint seed = header | (uint)i;
                    if (CheckSeed(seed))
                    {
                        state.Stop();
                    }
                });

                if (!result.IsCompleted)
                {
                    return true;
                }

                if (ct.IsCancellationRequested)
                {
                    form.InvokeLogBoxWriteLine("Search Cancel.");
                    return false;
                }

                form.IncrementProgress();
            }
            form.InvokeLogBoxWriteLine("Could not find TSV.");
            return false;
        }

        private bool CheckSeed(uint seed)
        {
            //init SFMT
            var rng = new SFMT(seed);

            if (isUSUM)
            {
                rng.SkipUSUM();
            }
            else
            {
                rng.SkipSM();
            }

            //calculate g7tid
            uint tmp = (uint)(rng.GetRand64() & 0xFFFFFFFF);

            uint g7tid = tmp % 1_000_000;
            if (g7tid != this.g7tid) return false;

            uint sid = tmp >> 16;
            uint tid = tmp & 0x0000FFFF;

            uint tsv = (sid ^ tid) >> 4;
            string trv = ((sid^ tid) & 0xF).ToString("X");

            int start = this.min;
            int end = this.max;

            while ((int)rng.Index64 < start) rng.GetRand64();

            List<ulong> randPool = new List<ulong>();
            while ((int)rng.Index64 < end+10) randPool.Add(rng.GetRand64());

            for(int i= 0; i < end - start; i++)
            {
                uint h, a, b, c, d, s;
                h = (uint)(randPool[i + 0] & 0x1F);
                a = (uint)(randPool[i + 1] & 0x1F);
                b = (uint)(randPool[i + 2] & 0x1F);
                c = (uint)(randPool[i + 3] & 0x1F);
                d = (uint)(randPool[i + 4] & 0x1F);
                s = (uint)(randPool[i + 5] & 0x1f);

                List<uint> ivs = new List<uint>{h, a, b, c, d, s };
                Nature nature = (Nature)(randPool[i + 7] % 25);
                if (ivs.SequenceEqual(this.ivs) && nature == this.nature)
                {
                    form.InvokeLogBoxWriteLine("Search Successful.");
                    form.InvokeLogBoxWriteLine($"TSV:{tsv}, TRV:{trv}, TID:{tid}, SID:{sid}, SEED:0x{seed.ToString("X8")}, advance:{start+i}");
                    return true;
                }
            }

            return false;
        }
    }
}
