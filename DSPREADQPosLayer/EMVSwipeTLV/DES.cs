using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace DSPREADQPosLayer.EMVSwipeTLV
{
    public class DES
    {
        /**
         * encrypt data in ECB mode
         * @param data
         * @param key
         * @return
         */
        public static byte[] encrypt(byte[] data, byte[] key)
        {
            try
            {
                DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.None;
                tdes.Key = key;


                return tdes.CreateEncryptor(key, key).TransformFinalBlock(data, 0, data.Length);
            }
            catch (Exception e)
            {
                String tx1 = e.Message.ToString();
                //String t2 = e.InnerException.ToString();
                String t3 = e.InnerException.Message.ToString();

            }


            return null;
        }

        /**
         * decrypt data in ECB mode
         * @param data
         * @param key
         * @return
         */
        public static byte[] decrypt(byte[] data, byte[] key)
        {
            try
            {
                DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
                tdes.Key = key;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.None;


                return tdes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
            }
            catch (Exception)
            {
            }


            return null;
        }

        /**
         * encrypt data in CBC mode
         * @param data
         * @param key
         * @return
         */
        public static byte[] encrypt_CBC(byte[] data, byte[] key)
        {
            try
            {
                DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
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

        /**
         * decrypt data in CBC mode
         * @param data
         * @param key
         * @return
         */
        public static byte[] decrypt_CBC(byte[] data, byte[] key)
        {
            ;
            try
            {
                DESCryptoServiceProvider tdes = new DESCryptoServiceProvider();
                tdes.Key = key;
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.None;


                return tdes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
            }
            catch (Exception)
            {
            }


            return null;
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

    }//end of class
}
