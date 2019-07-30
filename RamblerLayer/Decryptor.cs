using RamblerLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Format6_Decoder
{
    public class Decryptor
    {
        // private parameter
        Byte[] DK;                 // Key to devire encryption key of each device

        // Buffer to store decrypted result
        String Track2;

        /// <summary>
        /// Constructor. Load DK. Initialize parameter
        /// </summary>
        /// <param name="key">
        /// Derivation Key. Use for calculate device key
        /// </param>
        public Decryptor(String key)
        {
            Byte[] bTemp;
            int i;

            bTemp = ConvertString2Byte(key);

            Track2 = "";

            DK = new Byte[16];
            if (bTemp.Length >= 16)
            {
                for (i = 0; i < 16; i++)
                    DK[i] = bTemp[i];
            }
        }

        /// <summary>
        /// Constructor. Load DK. Initialize parameter
        /// </summary>
        /// <param name="key">Derivation Key. Use for calculate device key</param>
        public Decryptor(Byte[] key)
        {
            int i;

            // copy key to DK, for 
            DK = new Byte[16];
            if (key.Length >= 16)
            {
                for (i = 0; i < 16; i++)
                    DK[i] = key[i];
            }

            Track2 = "";
        }

        /// <summary>
        /// Decrypt CS data.
        /// </summary>
        /// <remarks>
        /// If data is obtained from mobile phone, it may encoded in BASE 64 format when transmit to server.
        /// </remarks>
        /// <param name="KSN">Key serial number</param>
        /// <param name="Track2_B64">Encrypted Track 2 data in Base 64</param>
        public int Decrypt_Base64(String KSN, String Track2_B64)
        {
            Byte[] bTemp;
            String sTemp;

            bTemp = System.Convert.FromBase64String(Track2_B64);

            sTemp = "";
            foreach (Byte tmp in bTemp)
                sTemp += tmp.ToString("X2");

            return DecryptCore(KSN,bTemp);
        }

        /// <summary>
        /// Decrypt CS data.
        /// </summary>
        /// <param name="KSN">Key serial number</param>
        /// <param name="Track2">Encrypted Track 2 data in Hex String</param>
        public int Decrypt(String KSN, String Track2)
        {
            Byte[] bTemp;

            bTemp = ConvertString2Byte(Track2);

            return DecryptCore(KSN, bTemp);
        }

        /// <summary>
        /// decrypt CS data.
        /// Output: 0 - success
        ///         other - fail
        /// </summary>
        /// <param name="KSN">Key serial number</param>
        /// <param name="encTrack2">Encrypted Track 2</param>
        /// <returns>0 - success. otherwise - fail</returns>
        int DecryptCore(String KSN, Byte[] encTrack2)
        {
            int i;
            Byte[] bKSN;
            Byte[] bKey;
            Byte[] bTemp;
            String sTemp;
            
            Char cTemp;

            // get KSN
            bKSN = ConvertString2Byte(KSN);
            if (bKSN.Length != 10)
                return 1;

            // check encrypted Track 2 length
            if (encTrack2.Length < 40)
                return 3;
            else
                Array.Resize(ref encTrack2, 40);

            // generate key
            bKey = DukptServer.GenTransactionKey(bKSN, DK);

            // decrypt Track 2
            bTemp = MyTDES.Decrypt(encTrack2, bKey);

            // search for ending
            Track2 = "";
            sTemp = "";

            if (bTemp[0] != 0x3B)
                return 4;

            for (i = 0; i < 40; i++)
            {
                if ((bTemp[i] >= 0x30) && (bTemp[i] <= 0x3F))
                {
                    cTemp = (Char)bTemp[i];
                    sTemp += cTemp;
                    if (bTemp[i] == 0x3F)
                    {
                        Track2 = sTemp;
                        return 0;
                    }
                }
                else
                    return 4;
            }

            return 5;
        }

        /// <summary>
        /// Get decrypted Track 2
        /// </summary>
        /// <returns>decrypted Track 1</returns>
        /// <remarks>It should call "Decrypt" first</remarks>
        public String GetTrack2() { return Track2; }

        /// <summary>
        /// Convert String to Byte
        /// </summary>
        /// <param name="input">String</param>
        /// <returns>Byte array</returns>
        Byte[] ConvertString2Byte(String input)
        {
            Byte[] bTemp;
            Char[] cTemp;
            Byte[] bResult;
            int i;

            cTemp = input.ToCharArray();
            bTemp = new Byte[cTemp.Length];
            bResult = new Byte[cTemp.Length / 2];

            i = 0;
            foreach (Char temp in cTemp)
                bTemp[i++] = System.Convert.ToByte("" + temp, 16);

            for (i = 0; i < bResult.Length; i++)
                bResult[i] = (Byte)((bTemp[i * 2] << 4) + bTemp[i * 2 + 1]);

            return bResult;
        }
    }
}
