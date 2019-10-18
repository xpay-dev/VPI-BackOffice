using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WisepadLayer
{
    public class TripleDES
    {
        private static byte[] GetKey(byte[] key)
        {
            byte[] bKey = new byte[24];
            int i;

            if (key.Length == 8)
            {
                for (i = 0; i < 8; i++)
                {
                    bKey[i] = key[i];
                    bKey[i + 8] = key[i];
                    bKey[i + 16] = key[i];
                }
            }
            else if (key.Length == 16)
            {
                for (i = 0; i < 8; i++)
                {
                    bKey[i] = key[i];
                    bKey[i + 8] = key[i + 8];
                    bKey[i + 16] = key[i];
                }
            }
            else if (key.Length == 24)
            {
                for (i = 0; i < 24; i++)
                    bKey[i] = key[i];
            }

            return bKey;
        }

        /**
         * encrypt data in ECB mode
         * @param data
         * @param key
         * @return
         */
        public static byte[] encrypt(byte[] data, byte[] key)
        {
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = key;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.None;


            return tdes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length); ;
        }

        /**
         * decrypt data in ECB mode
         * @param data
         * @param key
         * @return
         */
        public static byte[] decrypt(byte[] data, byte[] key)
        {
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = key;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.None;


            return tdes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length); ;
        }

        /**
         * encrypt data in CBC mode
         * @param data
         * @param key
         * @return
         */
        public static byte[] encrypt_CBC(byte[] data, byte[] key)
        {
            //SecretKey sk = new SecretKeySpec(GetKey(key), "DESede");
            try
            {
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = key;
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.None;


                return tdes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
            }
            catch (Exception)
            {
            }

            return null;
        }

        //Need to find out if this method is needed
        //Encrypt method
        public static byte[] encrypt_CBC(byte[] data, byte[] key, byte[] IV)
        {

            try
            {
                //thus need to be tested
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = key;
                tdes.IV = IV;
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.None;

                byte[] enc = new byte[data.Length];
                byte[] dataTemp1 = new byte[8];
                byte[] dataTemp2 = new byte[8];

                for (int i = 0; i < 8; i++)
                {
                    dataTemp2[i] = IV[i];
                }

                for (int i = 0; i < data.Length; i += 8)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        dataTemp1[j] = (byte)(data[i + j] ^ dataTemp2[j]);
                    }

                    dataTemp2 = tdes.CreateEncryptor().TransformFinalBlock(dataTemp1, 0, dataTemp1.Length);

                    for (int j = 0; j < 8; j++)
                    {
                        enc[i + j] = dataTemp2[j];
                    }

                }
                return enc;
            }
            catch (Exception e)
            {
            }


            return null;
        }

        /**
         * decrypt data in CBC mode
         * @param data
         * @param key
         * @return
         */
        public static byte[] decrypt_CBC(byte[] data, byte[] key)
        {

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = key;
            tdes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            tdes.Mode = CipherMode.CBC;
                 tdes.Padding = PaddingMode.None;

            return tdes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
        }

        /**
         * encrypt data in ECB mode
         * @param data
         * @param key
         * @return
         */
        public static String encrypt(String data, String key)
        {
            byte[] bData, bKey, bOutput;
            String result;

            bData = String2Hex(data);
            bKey = String2Hex(key);
            bOutput = encrypt(bData, bKey);
            result = Hex2String(bOutput);

            return result;
        }

        /**
         * decrypt data in ECB mode
         * @param data
         * @param key
         * @return
         */
        public static String decrypt(String data, String key)
        {
            byte[] bData, bKey, bOutput;
            String result;

            bData = String2Hex(data);
            bKey = String2Hex(key);
            bOutput = decrypt(bData, bKey);
            result = Hex2String(bOutput);

            return result;
        }

        /**
         * encrypt data in CBC mode
         * @param data
         * @param key
         * @return
         */
        public static String encrypt_CBC(String data, String key)
        {
            byte[] bData, bKey, bOutput;
            String result;

            bData = String2Hex(data);
            bKey = String2Hex(key);
            bOutput = encrypt_CBC(bData, bKey);
            result = Hex2String(bOutput);

            return result;
        }

        /**
         * decrypt data in CBC mode
         * @param data
         * @param key
         * @return
         */
        public static String decrypt_CBC(String data, String key)
        {
            byte[] bData, bKey, bOutput;
            String result;

            bData = String2Hex(data);
            bKey = String2Hex(key);
            bOutput = decrypt_CBC(bData, bKey);
            result = Hex2String(bOutput);

            return result;
        }

        /**
         * Convert byte Array to Hex String
         * @param data
         * @return
         */
        public static String Hex2String(byte[] data)
        {
            String result = BitConverter.ToString(data).Replace("-", "");

            return result;
        }

        /**
         * Convert Hex String to byte array
         * @param data
         * @return
         */
        public static byte[] String2Hex(String data)
        {
            byte[] result;

            result = new byte[data.Length / 2];

            for (int i = 0; i < data.Length; i += 2)
            {
                String t1 = data.Substring(i, 2);

                result[i / 2] = Convert.ToByte(data.Substring(i, 2), 16);
            }

            return result;
        }

    }
}
