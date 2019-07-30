using Crypto.CryptoManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace SDGDAL
{
    public class Utility
    {
        public static Dictionary<string, object> GenerateParams(object obj, string[] specifics, bool excludeSpecifics = true, string prefix = "")
        {
            Dictionary<string, object> param = new Dictionary<string, object>();

            PropertyInfo[] propInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in propInfos)
            {
                if (p.PropertyType == typeof(string)
                    || p.PropertyType == typeof(int)
                    || p.PropertyType == typeof(decimal)
                    || p.PropertyType == typeof(double)
                    || p.PropertyType == typeof(char)
                    || p.PropertyType == typeof(bool)
                    || p.PropertyType == typeof(DateTime))
                {
                    if (excludeSpecifics)
                    {
                        if (!specifics.Contains(p.Name))
                        {
                            var paramValue = p.GetValue(obj) == null ? "" : p.GetValue(obj).ToString();
                            param.Add("@" + prefix + p.Name, paramValue);
                        }
                    }
                    else
                    {
                        if (specifics.Contains(p.Name))
                        {
                            var paramValue = p.GetValue(obj) == null ? "" : p.GetValue(obj).ToString();
                            param.Add("@" + prefix + p.Name, paramValue);
                        }
                    }
                }
            }

            return param;
        }

        private static string EncryptionKey = "R8LP5V812CCU73";

        private static string Key = "HybR!dPaYT3ch";

        private static string Data = "HYbR!d@yTech!@#$%";

        public static string Encrypt(string t)
        {
            #region Old Encrption

            //byte[] clearBytes = Encoding.Unicode.GetBytes(t);

            //using (Aes encryptor = Aes.Create())
            //{
            //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            //    encryptor.Key = pdb.GetBytes(32);
            //    encryptor.IV = pdb.GetBytes(16);

            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            //        {
            //            cs.Write(clearBytes, 0, clearBytes.Length);
            //            cs.Close();
            //        }

            //        t = Convert.ToBase64String(ms.ToArray());
            //    }
            //}

            #endregion Old Encrption

            CryptoProvider crypto = new CryptoProvider(Key, Data);
            t = crypto.Encrypt(t.Trim());

            return t;
        }

        public static string Decrypt(string t)
        {
            #region Old Decrypt

            //byte[] cipherBytes = Convert.FromBase64String(t);
            //using (Aes encryptor = Aes.Create())
            //{
            //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            //    encryptor.Key = pdb.GetBytes(32);
            //    encryptor.IV = pdb.GetBytes(16);
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
            //        {
            //            cs.Write(cipherBytes, 0, cipherBytes.Length);
            //            cs.Close();
            //        }

            //        t = Encoding.Unicode.GetString(ms.ToArray());
            //    }
            //}

            #endregion Old Decrypt

            try
            {
                CryptoProvider crypto = new CryptoProvider(Key, Data);
                t = crypto.Decrypt(t.Trim());
            }
            catch (Exception ex)
            {
                CryptoProvider crypto = new CryptoProvider("B0CP#o3n!X", "B@nCoMm3Rc3!@#$%"); //catch old encrypt data
                t = crypto.Decrypt(t.Trim());
            }

            return t;
        }

        public static string GenerateSymmetricKeyAndEcryptData(string data, out byte[] key, out byte[] IV)
        {
            try
            {
                byte[] desKey;
                byte[] desIV;
                MemoryStream output = new MemoryStream();
                byte[] byteData = new UnicodeEncoding().GetBytes(data);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                CryptoStream encrypt = new CryptoStream(output, des.CreateEncryptor(des.Key, des.IV), CryptoStreamMode.Write);
                desKey = des.Key;
                desIV = des.IV;
                encrypt.Write(byteData, 0, byteData.Length);
                encrypt.Close();
                output.Close();

                string encData = Convert.ToBase64String(output.ToArray());

                key = desKey;// Convert.ToBase64String(desKey);
                IV = desIV;// Convert.ToBase64String(desIV);

                return encData;
            }
            catch (Exception e)
            {
                throw new Exception("Encryption failed.");
            }
        }

        //Encrypte input with existing Key and IV
        public static string EncryptDataWithExistingKey(string data, byte[] desKey, byte[] desIV)
        {
            MemoryStream output = new MemoryStream();
            byte[] byteData = new UnicodeEncoding().GetBytes(data);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            CryptoStream encrypt = new CryptoStream(output, des.CreateEncryptor(desKey, desIV), CryptoStreamMode.Write);

            desKey = des.Key;
            desIV = des.IV;
            encrypt.Write(byteData, 0, byteData.Length);
            encrypt.Close();
            output.Close();

            string encData = Convert.ToBase64String(output.ToArray());

            return encData;
        }

        //Decrypte encData with Key and IV
        public string DecryptEncData(String encData, byte[] byteKey, byte[] byteIV)
        {
            TripleDESCryptoServiceProvider tddes = new TripleDESCryptoServiceProvider();
            MemoryStream output = new MemoryStream();

            byte[] byteData = Convert.FromBase64String(encData);

            CryptoStream decrypt = new CryptoStream(output, tddes.CreateDecryptor(byteKey, byteIV), CryptoStreamMode.Write);

            //Decryps data with Symmetric Algorithm
            decrypt.Write(byteData, 0, byteData.Length);
            decrypt.Close();
            output.Close();

            return new UnicodeEncoding().GetString(output.ToArray());
            //return Convert.ToBase64String(output.ToArray());
        }

        //Decrypte encData by retrieve Key and IV from KeyID
        public static string DecryptEncDataWithKeyAndIV(String encData, string key, string IV)
        {
            try
            {
                byte[] byteKey = Convert.FromBase64String(key);
                byte[] byteIV = Convert.FromBase64String(IV);

                //decrypte
                TripleDESCryptoServiceProvider tddes = new TripleDESCryptoServiceProvider();
                MemoryStream output = new MemoryStream();

                byte[] byteData = Convert.FromBase64String(encData);

                CryptoStream decrypt = new CryptoStream(output, tddes.CreateDecryptor(byteKey, byteIV), CryptoStreamMode.Write);

                //Decryps data with Symmetric Algorithm
                decrypt.Write(byteData, 0, byteData.Length);
                decrypt.Close();
                output.Close();

                return new UnicodeEncoding().GetString(output.ToArray());
            }
            catch (Exception e)
            {
                throw new Exception("Unable to decrypt data");
            }
        }

        public static int GenerateSystemTraceAudit(int lastvalue)
        {
            int ctr = 1;
            DateTime dt = DateTime.Now;
            if (dt.Hour == 0 && dt.Minute == 0 && dt.Second == 0)
                lastvalue = 1;
            else
            {
                lastvalue = lastvalue + ctr;
            }

            return lastvalue;
        }
    }
}