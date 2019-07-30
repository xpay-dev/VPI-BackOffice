using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace RamblerLayer1
{
    class Decrypt
    {
        // private parameter
        Byte[] DK;

        // Buffer to store decrypted result
        String m_Track, m_Track1, m_Track2;

        /// <summary>
        /// Constructor. Load DK.
        /// </summary>
        /// <param name="key">DK</param>
        /// <returns></returns>
        public Decrypt(String key)
        {
            Byte[] bTemp;

            m_Track = "";
            m_Track1 = "";
            m_Track2 = "";
            bTemp = ConvertString2Hex(key);
            DK = new Byte[16];
            if (bTemp.Length >= 16)
                Array.Copy(bTemp, DK, 16);
        }

        /// <summary>
        /// Constructor. Load DK.
        /// </summary>
        /// <param name="key">DK</param>
        /// <returns></returns>
        public Decrypt(Byte[] key)
        {
            m_Track = "";
            m_Track1 = "";
            m_Track2 = "";
            DK = new Byte[16];
            if (key.Length >= 16)
                Array.Copy(key, DK, 16);
        }

        /// <summary>
        /// Decrypt Encrypted Track data
        /// </summary>
        /// <param name="KSN">KSN in Hex String</param>
        /// <param name="EncTrack">Encrypted Track Data in Base64 String</param>
        /// <param name="Track1"></param>
        /// <param name="Track2"></param>
        /// <returns>0-success. other - fail</returns>
        public int Decrypt_Base64(String KSN, String EncTrack, out String Track1, out String Track2)
        {
            Byte[] bTemp;
            int result;

            try { bTemp = System.Convert.FromBase64String(EncTrack); }
            catch
            {
                Track1 = "";
                Track2 = "";
                return 1;
            }
            result = Decrypt_Core(KSN, bTemp);
            if (result == 0)
                result = ExtractTrack();

            if (result == 0)
            {
                Track1 = m_Track1;
                Track2 = m_Track2;
            }
            else
            {
                Track1 = "";
                Track2 = "";
            }

            return result;
        }

        /// <summary>
        /// Decrypt Encrypted Track data
        /// </summary>
        /// <param name="KSN">KSN in Hex String</param>
        /// <param name="EncTrack">Encrypted Track Data in Base64 String</param>
        /// <param name="Track1"></param>
        /// <param name="Track2"></param>
        /// <param name="Track1Len">Track 1 Length, include LRC</param>
        /// <param name="Track2Len">Track 2 Length, include LRC</param>
        /// <returns>0-success. other - fail</returns>
        public int Decrypt_Base64(String KSN, String EncTrack, out String Track1, out String Track2, int Track1Len, int Track2Len)
        {
            Byte[] bTemp;
            int result;

            try { bTemp = System.Convert.FromBase64String(EncTrack); }
            catch
            {
                Track1 = "";
                Track2 = "";
                return 1;
            }
            result = Decrypt_Core(KSN, bTemp);
            if (result == 0)
                result = ExtractTrack(Track1Len, Track2Len);

            if (result == 0)
            {
                Track1 = m_Track1;
                Track2 = m_Track2;
            }
            else
            {
                Track1 = "";
                Track2 = "";
            }

            return result;
        }

        /// <summary>
        /// Decrypt Encrypted Track data
        /// </summary>
        /// <param name="KSN">KSN in Hex String</param>
        /// <param name="EncTrack">Encrypted Track Data in Hex String</param>
        /// <param name="Track1"></param>
        /// <param name="Track2"></param>
        /// <returns>0-success. other - fail</returns>
        public int Decrypt_Hex(String KSN, String EncTrack, out String Track1, out String Track2)
        {
            Byte[] bTemp;
            int result;

            bTemp = ConvertString2Hex(EncTrack);
            result = Decrypt_Core(KSN, bTemp);
            if (result == 0)
                result = ExtractTrack();
            if (result == 0)
            {
                Track1 = m_Track1;
                Track2 = m_Track2;
            }
            else
            {
                Track1 = "";
                Track2 = "";
            }

            return result;
        }

        /// <summary>
        /// Decrypt Encrypted Track data
        /// </summary>
        /// <param name="KSN">KSN in Hex String</param>
        /// <param name="EncTrack">Encrypted Track Data in Hex String</param>
        /// <param name="Track1"></param>
        /// <param name="Track2"></param>
        /// <param name="Track1Len">Track 1 Length, include LRC</param>
        /// <param name="Track2Len">Track 2 Length, include LRC</param>
        /// <returns>0-success. other - fail</returns>
        public int Decrypt_Hex(String KSN, String EncTrack, out String Track1, out String Track2, int Track1Len, int Track2Len)
        {
            Byte[] bTemp;
            int result;

            bTemp = ConvertString2Hex(EncTrack);
            result = Decrypt_Core(KSN, bTemp);
            if (result == 0)
                result = ExtractTrack(Track1Len, Track2Len);
            if (result == 0)
            {
                Track1 = m_Track1;
                Track2 = m_Track2;
            }
            else
            {
                Track1 = "";
                Track2 = "";
            }

            return result;
        }

        /// <summary>
        /// decrypt encrypted track data
        /// </summary>
        /// <param name="KSN"></param>
        /// <param name="encTrack"></param>
        /// <returns>0-success. 1 - fail(KSN length not correct).</returns>
        private int Decrypt_Core(String KSN, Byte[] encTrack)
        {
            Byte[] ksn, DukptKeyVariant, temp, DataKey;

            // convert KSN to byte array
            ksn = ConvertString2Hex(KSN);
            if (ksn.Length != 10)
                return 1;

            // get Data Key
            DukptKeyVariant = DukptServer.GenDataKeyVariant(ksn, DK);
            temp = (Byte[])DukptKeyVariant.Clone();
            DataKey = TDES_Enc_ECB(temp, DukptKeyVariant);

            // decrypt track
            temp = TDES_Dec_CBC(encTrack, DataKey);
            m_Track = "";
            foreach (Byte tmp in temp)
                m_Track += (Char)(tmp);

            return 0;
        }

        /// <summary>
        /// Extract track 1 and track 2
        /// </summary>
        /// <returns>0-success. other - fail</returns>
        private int ExtractTrack()
        {
            int T1_Start, T1_End, T2_Start, T2_End;

            // search for T1 Start char
            T1_End = -1;
            T2_Start = -1;
            T2_End = -1;
            T1_Start = m_Track.IndexOf('%', 0);
            T2_Start = m_Track.IndexOf(';', 0);
            if (T1_Start >= 0)
                T1_End = m_Track.IndexOf('?', T1_Start);
            if (T1_End >= 0)
                T2_Start = m_Track.IndexOf(';', T1_End);
            if (T2_Start >= 0)
                T2_End = m_Track.IndexOf('?', T2_Start);
            if ((T2_End > T2_Start) && (T2_Start > T1_End) && (T1_End > T1_Start)) //card data with track 1 and track 2
            {
                m_Track1 = m_Track.Substring(T1_Start, T1_End - T1_Start + 2);
                m_Track2 = m_Track.Substring(T2_Start, T2_End - T2_Start + 2);

                return 0;
            }
            else if (T1_Start == 0) //for track1 only
            {
                m_Track1 = m_Track.Substring(T1_Start, T1_End - T1_Start + 2);
                m_Track2 = "";

                return 0;
            }
            else if (T2_Start == 0) //for track2 only
            {
                m_Track2 = m_Track.Substring(T2_Start, T2_End - T2_Start + 2);
                m_Track1 = "";

                return 0;
            }
            else
                return 2;
        }

        /// <summary>
        /// Extract track 1 and track 2
        /// </summary>
        /// <param name="Track1Len">track 1 length</param>
        /// <param name="Track2Len">track 2 length</param>
        /// <returns>0-success. other - fail</returns>
        private int ExtractTrack(int Track1Len, int Track2Len)
        {
            if (m_Track.Length > (Track1Len + Track2Len))
            {
                m_Track1 = m_Track.Substring(0, Track1Len);
                m_Track2 = m_Track.Substring(Track1Len, Track2Len);

                return 0;
            }
            else
                return 1;
        }

        /// <summary>
        /// TDES Decryption in CBC mode
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private Byte[] TDES_Dec_CBC(Byte[] input, Byte[] key)
        {
            // use .NET library
            TripleDES tdes = new TripleDESCryptoServiceProvider();
            tdes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            tdes.Padding = PaddingMode.None;
            tdes.Mode = CipherMode.CBC;
            tdes.Key = key;

            return tdes.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
        }

        /// <summary>
        /// TDES Encryption in ECB mode
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private Byte[] TDES_Enc_ECB(Byte[] input, Byte[] key)
        {
            // use .NET libaray
            TripleDES tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = key;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.None;

            return tdes.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
        }

        /// <summary>
        /// Convert String to Byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Byte[] ConvertString2Hex(String input)
        {
            Byte[] baTemp;
            int i, j;
            Char[] caTemp;

            caTemp = input.ToCharArray();
            baTemp = new Byte[caTemp.Length];

            for (i = 0, j = 0; i < caTemp.Length; i++)
            {
                if ((caTemp[i] >= '0') && (caTemp[i] <= '9'))
                    baTemp[j++] = (Byte)(caTemp[i] - '0');
                else if ((caTemp[i] >= 'a') && (caTemp[i] <= 'f'))
                    baTemp[j++] = (Byte)(caTemp[i] - 'a' + 10);
                else if ((caTemp[i] >= 'A') && (caTemp[i] <= 'F'))
                    baTemp[j++] = (Byte)(caTemp[i] - 'A' + 10);
            }
            j /= 2;
            for (i = 0; i < j; i++)
                baTemp[i] = (Byte)(((baTemp[2 * i] << 4) & 0xF0) | (baTemp[2 * i + 1] & 0x0F));
            if (j > 0)
                Array.Resize(ref baTemp, j);
            else
                baTemp = new Byte[0];
            return baTemp;
        }

    }
}

