using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Format6_Decoder
{
    static class MyTDES
    {
        /// <summary>
        /// Perform 2key TDES encryption in ECB mode
        /// </summary>
        /// <param name="Data">Data to be encrypted</param>
        /// <param name="Key">TDES Key. 16 byte</param>
        public static Byte[] Encrypt(Byte[] Data, Byte[] Key)
        {
            Byte[] IV = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Byte[] result;
            TripleDES tdes;
            Byte[] KeyBuffer;
            int i;

            KeyBuffer = new Byte[24];
            if (Key.Length == 16)
            {
                for (i = 0; i < 16; i++)
                    KeyBuffer[i] = Key[i];
                for (i = 0; i < 8; i++)
                    KeyBuffer[16 + i] = Key[i];
            }
            else
            {
                for (i = 0; i < 24; i++)
                    KeyBuffer[i] = Key[i];
            }

            tdes = new TripleDESCryptoServiceProvider();
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.None;
            tdes.KeySize = 192;
            tdes.Key = KeyBuffer;
            tdes.IV = IV;

            result = tdes.CreateEncryptor().TransformFinalBlock(Data, 0, Data.Length);

            return result;
        }

        /// <summary>
        /// Perform 2key TDES encryption in CBC mode
        /// </summary>
        /// <param name="Data">Data to be decrypted</param>
        /// <param name="Key">TDES Key. 16 byte</param>
        public static Byte[] EncryptCBC(Byte[] Data, Byte[] Key)
        {
            Byte[] IV = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Byte[] result;
            TripleDES tdes;
            Byte[] KeyBuffer;
            int i;

            KeyBuffer = new Byte[24];
            if (Key.Length == 16)
            {
                for (i = 0; i < 16; i++)
                    KeyBuffer[i] = Key[i];
                for (i = 0; i < 8; i++)
                    KeyBuffer[16 + i] = Key[i];
            }
            else
            {
                for (i = 0; i < 24; i++)
                    KeyBuffer[i] = Key[i];
            }

            tdes = new TripleDESCryptoServiceProvider();
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.None;
            tdes.KeySize = 192;
            tdes.Key = KeyBuffer;
            tdes.IV = IV;

            result = tdes.CreateEncryptor().TransformFinalBlock(Data, 0, Data.Length);

            return result;
        }

        /// <summary>
        /// Perform 2key TDES decryption in ECB mode
        /// </summary>
        /// <param name="Data">Data to be encrypted</param>
        /// <param name="Key">TDES Key. 16 byte</param>
        public static Byte[] Decrypt(Byte[] Data, Byte[] Key)
        {
            Byte[] IV = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Byte[] result;
            TripleDES tdes;
            Byte[] KeyBuffer;
            int i;

            KeyBuffer = new Byte[24];
            if (Key.Length == 16)
            {
                for (i = 0; i < 16; i++)
                    KeyBuffer[i] = Key[i];
                for (i = 0; i < 8; i++)
                    KeyBuffer[16 + i] = Key[i];
            }
            else
            {
                for (i = 0; i < 24; i++)
                    KeyBuffer[i] = Key[i];
            }

            tdes = new TripleDESCryptoServiceProvider();
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.None;
            tdes.KeySize = 192;
            tdes.Key = KeyBuffer;
            tdes.IV = IV;

            result = tdes.CreateDecryptor().TransformFinalBlock(Data, 0, Data.Length);

            return result;
        }

        /// <summary>
        /// Perform 2key TDES decryption in CBC mode
        /// </summary>
        /// <param name="Data">Data to be decrypted</param>
        /// <param name="Key">TDES Key. 16 byte</param>
        public static Byte[] DecryptCBC(Byte[] Data, Byte[] Key)
        {
            Byte[] IV = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            Byte[] result;
            TripleDES tdes;
            Byte[] KeyBuffer;
            int i;

            KeyBuffer = new Byte[24];
            if (Key.Length == 16)
            {
                for (i = 0; i < 16; i++)
                    KeyBuffer[i] = Key[i];
                for (i = 0; i < 8; i++)
                    KeyBuffer[16 + i] = Key[i];
            }
            else
            {
                for (i = 0; i < 24; i++)
                    KeyBuffer[i] = Key[i];
            }

            tdes = new TripleDESCryptoServiceProvider();
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.None;
            tdes.KeySize = 192;
            tdes.Key = KeyBuffer;
            tdes.IV = IV;

            result = tdes.CreateDecryptor().TransformFinalBlock(Data, 0, Data.Length);

            return result;
        }

    }
}
