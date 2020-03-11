using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_ProveVarie
{
    public struct BlowfishState
    {
        public UInt32[] P;
        public UInt32[,] S;
    }

    public class Blowfish
    {
        UInt32[] _P;
        UInt32[,] _S;
        public Blowfish(char[] key)
        {
            Initialize(key);
        }

        public BlowfishState GetBlowfishState()
        {
            BlowfishState state = new BlowfishState();
            state.P = _P;
            state.S = _S;
            return state;
        }
        private void Initialize(char[] _key)
        {
            _P = new UInt32[18];
            _S = new UInt32[4, 256];

            UInt32 L = 0;
            UInt32 R = 0;
            try
            {
                for (int i = 0; i < 18; i++)
                {
                    _P[i] ^= _key[i % _key.Length];
                }

                for (int i = 0; i < 17; i++)
                {
                    encrypt(ref L, ref R);
                    _P[i] = L;
                    _P[i + 1] = R;
                }
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 255; j++)
                    {
                        encrypt(ref L, ref R);
                        _S[i, j] = L;
                        _S[i, j + 1] = R;
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
                h = _S[0, x >> 24] + _S[1, x >> 16 & 0xff];
                h1 = h ^ _S[2, x >> 8 & 0xff] + _S[3, x & 0xff];
                return h1;
            }
            catch (Exception ex) { throw; }
        }

        public void encrypt(ref UInt32 L, ref UInt32 R)
        {
            try
            {
                for (int i = 0; i < 16; i += 2)
                {
                    L ^= _P[i];
                    R ^= f(L);
                    R ^= _P[i + 1];
                    L ^= f(R);
                }
                L ^= _P[16];
                R ^= _P[17];
                swap(ref L, ref R);
            }
            catch (Exception ex) { throw; }
        }

        public void decrypt(ref UInt32 L, ref UInt32 R)
        {
            try
            {
                for (int i = 16; i > 0; i -= 2)
                {
                    L ^= _P[i + 1];
                    R ^= f(L);
                    R ^= _P[i];
                    L ^= f(R);
                }
                L ^= _P[1];
                R ^= _P[0];
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


        #region RAW DATA
        
        public string DeCryptRawData(UInt64[] C)
        {
            try
            {
                string _P = "";
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

                    _P += (LH > 0) ? ((char)LH).ToString() : ""; //avoid NULL character
                    _P += (LL > 0) ? ((char)LL).ToString() : ""; //avoid NULL character 
                    _P += (RH > 0) ? ((char)RH).ToString() : ""; //avoid NULL character
                    _P += (RL > 0) ? ((char)RL).ToString() : ""; //avoid NULL character
                }

                return _P;
            }
            catch (Exception ex) { throw; }
        }

        public UInt64[] EnCryptString(string inputstring) //IT WORKS
        {
            try
            {
                char[] c_rawdata = inputstring.ToCharArray();

                int ratio = (c_rawdata.Length / 4);
                int rest = c_rawdata.Length % 4; //rest = numers of chars left
                int dimensionP = (rest > 0) ? ratio + 1 : ratio;

                UInt64[] _P = new UInt64[dimensionP];
                UInt32[] L = new UInt32[dimensionP];
                UInt32[] R = new UInt32[dimensionP];

                for (int i = 0, j = -1; i < ratio; i++)
                {
                    UInt16 LH, LL, RH, RL;
                    LH = (UInt16)c_rawdata[++j];
                    LL = (UInt16)c_rawdata[++j];
                    RH = (UInt16)c_rawdata[++j];
                    RL = (UInt16)c_rawdata[++j];

                    L[i] = (UInt32)(LH << 0x10) + (UInt32)LL;
                    R[i] = (UInt32)(RH << 0x10) + (UInt32)RL;
                }

                /*LAST PACKAGE OF DATA*/
                if (rest > 0)
                {
                    int indexOfLastPack = ratio * 4;
                    UInt32 LLAST = 0;
                    UInt32 RLAST = 0;
                    UInt16 LHLast, LLLast, RHLast, RLLast;
                    LHLast = 0;
                    LLLast = 0;
                    RHLast = 0;
                    RLLast = 0;
                    try
                    {
                        LHLast = (UInt16)c_rawdata[indexOfLastPack];
                        LLLast = (UInt16)c_rawdata[indexOfLastPack + 1];
                        RHLast = (UInt16)c_rawdata[indexOfLastPack + 2];
                        RLLast = (UInt16)c_rawdata[indexOfLastPack + 3];
                    }
                    catch (Exception ex) { }
                    LLAST = ((UInt32)LHLast << 0x10) + (UInt32)LLLast;
                    RLAST = ((UInt32)RHLast << 0x10) + (UInt32)RLLast;
                    L[dimensionP - 1] = LLAST;
                    R[dimensionP - 1] = RLAST;
                }

                for (int i = 0; i < dimensionP; i++)
                {
                    encrypt(ref L[i], ref R[i]);
                    _P[i] = ((UInt64)L[i] << 0x20) + (UInt64)R[i];
                }

                return _P;
            }
            catch (Exception ex) { throw; }
        }

        public UInt64[] EnCryptString(byte[] utf8Encoding)
        {
            try
            {
                return EnCryptString(Encoding.UTF8.GetString(utf8Encoding));
            }
            catch (Exception ex) { throw; }
        }






        #endregion


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
                UInt64 _P;
                UInt32 L, R;

                _P = (UInt64)ch;
                R = (UInt32)(_P & 0xFFFFFFFF);
                L = (UInt32)(_P >> 0x20);

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
                string _S = inputstring;
                for (int i = 0; i < splittedcount; i++)
                {
                    ss[i] = _S.Substring(i * 4, 4);
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

    }

}
