using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using SDGWebService.Classes;

namespace SDGWebService.Functions
{
    public class CryptorEngine
    {
        public static string Ecrypt(string dataToEcrypt)
        {
            string key = "v3r1TasP@Y";
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;

            rijndaelCipher.KeySize = 0x80;
            rijndaelCipher.BlockSize = 0x80;

            byte[] b = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[0x10];
            int len = b.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }

            Array.Copy(b, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;

            byte[] encrypted = EncryptStringToBytes(dataToEcrypt, rijndaelCipher.Key, rijndaelCipher.IV);

            return HttpUtility.UrlEncode(Convert.ToBase64String(encrypted));
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static CreditCardDetails Decrypt(string dataToDecrypt)
        {
            string key = "v3r1TasP@Y";
            CreditCardDetails card = new CreditCardDetails();

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;

            rijndaelCipher.KeySize = 0x80;
            rijndaelCipher.BlockSize = 0x80;

            string decodedUrl = HttpUtility.UrlDecode(dataToDecrypt);
            byte[] encryptedData = Convert.FromBase64String(decodedUrl);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[0x10];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            string result = encoding.GetString(plainText);

            card = ParseCardData(result);

            return card;
        }

        private static CreditCardDetails ParseCardData(string result)
        {
            CreditCardDetails card = new CreditCardDetails();

            int separator = result.IndexOf('|');
            int separatorLast = result.LastIndexOf('|');

            #region ParseCardData
            string cardnumber = result.Substring(0, separator);
            string expiry = result.Substring(separator + 1, 4);
            string cvv = result.Substring(separatorLast + 1, 3);
            #endregion

            card.CardNumber = cardnumber;
            card.ExpiryMonth = expiry.Substring(0, 2);
            card.ExpiryYear = expiry.Substring(2, 2);
            card.CVV = cvv;

            return card;
        }
    }
}