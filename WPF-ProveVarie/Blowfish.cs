using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ProveVarie
{
    public class Blowfish
    {

        UInt32[] P;
        UInt32[,] S;
        public Blowfish(char[] key)
        {
            Initialize(key);
        }

        private void Initialize(char[] _key)
        {
            P = new UInt32[18];
            S = new UInt32[4, 256];

            UInt32 L = 0;
            UInt32 R = 0;
            try
            {
                for (int i = 0; i < 18; i++)
                {
                    P[i] ^= _key[i % _key.Length];
                }

                for (int i = 0; i < 17; i++)
                {
                    encrypt(ref L, ref R);
                    P[i] = L;
                    P[i + 1] = R;
                }
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 255; j++)
                    {
                        encrypt(ref L, ref R);
                        S[i, j] = L;
                        S[i, j + 1] = R;
                    }
                }
            }
            catch (Exception ex) { throw; }
        }

        UInt32 f(UInt32 x)
        {
            try
            {
                UInt32 h, h1;
                h = S[0, x >> 24] + S[1, x >> 16 & 0xff];
                h1 = h ^ S[2, x >> 8 & 0xff] + S[3, x & 0xff];
                return h1;
            }
            catch (Exception ex) { throw; }
        }

        private void encrypt(ref UInt32 L, ref UInt32 R)
        {
            try
            {
                for (int i = 0; i < 16; i += 2)
                {
                    L ^= P[i];
                    R ^= f(L);
                    R ^= P[i + 1];
                    L ^= f(R);
                }
                L ^= P[16];
                R ^= P[17];
                swap(ref L, ref R);
            }
            catch (Exception ex) { throw; }
        }

        private void decrypt(ref UInt32 L, ref UInt32 R)
        {
            try
            {
                for (int i = 16; i > 0; i -= 2)
                {
                    L ^= P[i + 1];
                    R ^= f(L);
                    R ^= P[i];
                    L ^= f(R);
                }
                L ^= P[1];
                R ^= P[0];
                swap(ref L, ref R);
            }
            catch (Exception ex) { throw; }
        }

        private void swap<T>(ref T t1, ref T t2)
        {
            try
            {
                T t3;
                t3 = t2;
                t2 = t1;
                t1 = t3;
            }
            catch (Exception ex) { throw; }
        }

        #region BY CHARS
        public string EnCryptStringByChars(string inputstring)
        {
            try
            {
                string cryptedString = "";
                foreach (char cc in inputstring.ToCharArray())
                {
                    cryptedString += CryptChar(cc);
                }
                return cryptedString;
            }
            catch (Exception ex) { throw; }
        }

        private string CryptChar(char ch)
        {
            try
            {
                string code = "";
                UInt64 P;
                UInt32 L, R;

                P = (UInt64)ch;
                R = (UInt32)(P & 0xFFFFFFFF);
                L = (UInt32)(P >> 0x20);

                encrypt(ref L, ref R);

                UInt16 LH;
                UInt16 LL;
                UInt16 RH;
                UInt16 RL;
                LH = (UInt16)(L >> 0x10);
                LL = (UInt16)(L & 0xFFFF);
                RH = (UInt16)(R >> 0x10);
                RL = (UInt16)(R & 0xFFFF);

                code = ((char)LH).ToString() + ((char)LL).ToString() + ((char)RH).ToString() + ((char)RL).ToString();

                return code;
            }
            catch (Exception ex) { throw; }
        }

        public string DeCryptStringByChars(string inputstring)
        {
            try
            {
                int splittedcount = inputstring.Length / 4;
                string[] ss = new string[splittedcount];
                string s = inputstring;
                for (int i = 0; i < splittedcount; i++)
                {
                    ss[i] = s.Substring(i * 4, 4);
                }

                string sout = "";
                for (int i = 0; i < splittedcount; i++)
                {
                    sout += DeCryptChar(ss[i]);
                }
                return sout;
            }
            catch (Exception ex) { throw; }
        }
        private string DeCryptChar(string input)
        {
            try
            {
                string decode = "";
                UInt32 L, R;

                char[] chars = input.ToCharArray();

                UInt16 LH = chars[0];
                UInt16 LL = chars[1];
                UInt16 RH = chars[2];
                UInt16 RL = chars[3];

                L = (UInt32)(LH << 0x10) + LL;
                R = (UInt32)(RH << 0x10) + RL;

                decrypt(ref L, ref R);

                UInt64 result = (UInt64)L;
                result = result << 0x20;
                result += (UInt64)R;

                char decodes = (char)result;

                decode += decodes;

                return decode;
            }
            catch (Exception ex) { throw; }
        }
        #endregion

        #region RAW DATA

        public UInt64[] EnCryptString(string inputstring)
        {
            char[] c_rawdata = inputstring.ToCharArray();
            int dimensionP = (c_rawdata.Length / 4);
            int rest = c_rawdata.Length % 4;

            UInt64[] P = new UInt64[dimensionP];
            UInt32[] L = new UInt32[dimensionP];
            UInt32[] R = new UInt32[dimensionP];

            for (int i = 0, j = -1; i < dimensionP; i++)
            {
                UInt16 LH, LL, RH, RL;
                LH = (UInt16)c_rawdata[++j];
                LL = (UInt16)c_rawdata[++j];
                RH = (UInt16)c_rawdata[++j];
                RL = (UInt16)c_rawdata[++j];

                L[i] = (UInt32)(LH << 0x10) + (UInt32)LL;
                R[i] = (UInt32)(RH << 0x10) + (UInt32)RL;
            }
            
            for (int i = 0; i < P.Length; i++)
            {
                encrypt(ref L[i], ref R[i]);
                P[i] = ((UInt64)L[i] << 0x20) + (UInt64)R[i];
            }

            return P;
        }

        public string DeCryptRawData(UInt64[] C)
        {
            string P = "";
            int dimensionC = C.Length;
            UInt32[] L = new UInt32[dimensionC];
            UInt32[] R = new UInt32[dimensionC];

            for (int i = 0; i < dimensionC; i++)
            {
                R[i] = (UInt32)(C[i] & 0xffffffff);
                L[i] = (UInt32)(C[i] >> 0x20);

                decrypt(ref L[i], ref R[i]);
            }


            for (int i = 0, j = -1; i < dimensionC; i++)
            {
                UInt16 LH, LL, RH, RL;
                LL = (UInt16)(L[i] & 0xffff);
                LH = (UInt16)(L[i] >> 0x10);
                RL = (UInt16)(R[i] & 0xffff);
                RH = (UInt16)(R[i] >> 0x10);

                P += ((char)LH).ToString() + ((char)LL).ToString() + ((char)RH).ToString() + ((char)RL).ToString();
            }

            return P;
        }

        #endregion
    }

}
