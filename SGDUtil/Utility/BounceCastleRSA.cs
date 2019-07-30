using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Encodings;

namespace SDGUtil.Utility
{

    class BouncyCastleRSA
    {
        public BouncyCastleRSA()
        {
            rsakey = new RSAKEY();
        }
        /// <summary>
        /// KEY 结构体
        /// </summary>
        public struct RSAKEY
        {
            /// <summary>
            /// 公钥
            /// </summary>
            public string PublicKey
            {
                get;
                set;
            }
            /// <summary>
            /// 私钥
            /// </summary>
            public string PrivateKey
            {
                get;
                set;
            }
        }

        RSAKEY rsakey;

        public RSAKEY GetKey()
        {
            //RSA密钥对的构造器  
            RsaKeyPairGenerator keyGenerator = new RsaKeyPairGenerator();

            //RSA密钥构造器的参数  
            RsaKeyGenerationParameters param = new RsaKeyGenerationParameters(
                Org.BouncyCastle.Math.BigInteger.ValueOf(3),
                new Org.BouncyCastle.Security.SecureRandom(),
                1024,   //密钥长度  
                25);
            //用参数初始化密钥构造器  
            keyGenerator.Init(param);
            //产生密钥对  
            AsymmetricCipherKeyPair keyPair = keyGenerator.GenerateKeyPair();
            //获取公钥和密钥  
            AsymmetricKeyParameter publicKey = keyPair.Public;
            AsymmetricKeyParameter privateKey = keyPair.Private;

            SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);


            Asn1Object asn1ObjectPublic = subjectPublicKeyInfo.ToAsn1Object();

            byte[] publicInfoByte = asn1ObjectPublic.GetEncoded("UTF-8");
            Asn1Object asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
            byte[] privateInfoByte = asn1ObjectPrivate.GetEncoded("UTF-8");

            RSAKEY item = new RSAKEY()
            {
                PublicKey = Convert.ToBase64String(publicInfoByte),
                PrivateKey = Convert.ToBase64String(privateInfoByte)
            };
            return item;
        }

        public string Encrypt(String data)
        {
            return EncryptByPublicKey(data, this.rsakey.PublicKey);
        }
        public string Decrypt(String EncData)
        {
            return DecryptByPublicKey(EncData, this.rsakey.PrivateKey);
        }

        public static string EncryptByPublicKey(String data, String publickey)
        {
            if (null == data || "".Equals(data.Trim()))
            {
                return "";
            }
            byte[] publicInfoByte = Convert.FromBase64String(publickey);
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(publicInfoByte);//这里也可以从流中读取，从本地导入   
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(SubjectPublicKeyInfo.GetInstance(pubKeyObj));
            //IAsymmetricBlockCipher cipher = new RsaEngine();
            IAsymmetricBlockCipher cipher = new Pkcs1Encoding(new RsaEngine());
            cipher.Init(true, pubKey);//true表示加密   
            //加密
            byte[] encryptData = cipher.ProcessBlock(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
            return Convert.ToBase64String(encryptData);
        }

        public static string DecryptByPublicKey(String encryptData, String privatekey)
        {
            if (null == encryptData || "".Equals(encryptData.Trim()))
            {
                return "";
            }
            byte[] privateInfoByte = Convert.FromBase64String(privatekey);
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(privateInfoByte);//这里也可以从流中读取，从本地导入   
            //IAsymmetricBlockCipher cipher = new RsaEngine();
            IAsymmetricBlockCipher cipher = new Pkcs1Encoding(new RsaEngine());
            //解密   
            AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(privateInfoByte);
            cipher.Init(false, priKey);//false表示解密   
            string decryptData = Encoding.UTF8.GetString(cipher.ProcessBlock(Convert.FromBase64String(encryptData), 0, encryptData.Length));
            return decryptData;
        }



        public static void TestMain(string[] args)
        {
            //生成密钥对   
            RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
            RsaKeyGenerationParameters rsaKeyGenerationParameters = new RsaKeyGenerationParameters(BigInteger.ValueOf(3), new Org.BouncyCastle.Security.SecureRandom(), 1024, 25);
            rsaKeyPairGenerator.Init(rsaKeyGenerationParameters);//初始化参数   
            AsymmetricCipherKeyPair keyPair = rsaKeyPairGenerator.GenerateKeyPair();
            AsymmetricKeyParameter publicKey = keyPair.Public;//公钥   
            AsymmetricKeyParameter privateKey = keyPair.Private;//私钥   

            SubjectPublicKeyInfo subjectPublicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);

            Asn1Object asn1ObjectPublic = subjectPublicKeyInfo.ToAsn1Object();
            byte[] publicInfoByte = asn1ObjectPublic.GetEncoded();
            Asn1Object asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
            byte[] privateInfoByte = asn1ObjectPrivate.GetEncoded();

            string pkey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDJ1fKGMV/yOUnY1ysFCk0yPP4bfOolC/nTAyHmoser+1yzeLtyYsfitYonFIsXBKoAYwSAhNE+ZSdXZs4A5zt4EKoU+T3IoByCoKgvpCuOx8rgIAqC3O/95pGb9n6rKHR2sz5EPT0aBUUDAB2FJYjA9Sy+kURxa52EOtRKolSmEwIDAQAB";
            //string pkey = "uKI+0wG6eILUPRNf6ImqRdez/nLxsV0LHGsuvYR0LDVrXz8Y7sYSlpAkn1HpJI8US8Sx5bJzvBibvKv0pAa7UQ==";
            System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
            publicInfoByte = Convert.FromBase64String(pkey);

            //这里可以将密钥对保存到本地   
            Console.WriteLine("PublicKey:\n" + Convert.ToBase64String(publicInfoByte));
            Console.WriteLine("PrivateKey:\n" + Convert.ToBase64String(privateInfoByte));

            //加密、解密   
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(publicInfoByte);//这里也可以从流中读取，从本地导入   
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(SubjectPublicKeyInfo.GetInstance(pubKeyObj));
            //IAsymmetricBlockCipher cipher = new RsaEngine();
            IAsymmetricBlockCipher cipher = new Pkcs1Encoding(new RsaEngine());
            cipher.Init(true, pubKey);//true表示加密   
            //加密   
            string data = "1234567812345678";
            Console.WriteLine("\n明文：" + data);
            byte[] encryptData = cipher.ProcessBlock(Encoding.UTF8.GetBytes(data), 0, Encoding.UTF8.GetBytes(data).Length);
            Console.WriteLine("密文:" + Convert.ToBase64String(encryptData));
            Console.WriteLine("密文2:" + DecryptByPublicKey(data, pkey));
            //解密   
            //AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(privateInfoByte);
            //cipher.Init(false, priKey);//false表示解密   
            //string decryptData = Encoding.UTF8.GetString(cipher.ProcessBlock(encryptData, 0, encryptData.Length));
            //Console.WriteLine("解密后数据：" + decryptData);
            //Console.Read();
        }
    }
}
