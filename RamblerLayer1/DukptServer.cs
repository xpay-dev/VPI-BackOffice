using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace RamblerLayer1
{
    static class DukptServer
    {
        static public Byte[] GetIPEK(Byte[] KSN, Byte[] DK)
        {
            byte[] KSNtemp = new byte[8];
            byte[] DKtemp1 = new byte[8];
            byte[] DKtemp2 = new byte[8];
            byte[] Result = new byte[16];
            byte[] temp1 = new byte[8];
            byte[] temp2 = new byte[8];

            Array.Copy(KSN, KSNtemp, 8);
            Array.Copy(DK, DKtemp1, 8);
            Array.Copy(DK, 8, DKtemp2, 0, 8);

            KSNtemp[7] &= 0xE0;

            DES des = new DESCryptoServiceProvider();
            des.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            des.Padding = PaddingMode.None;
            des.Mode = CipherMode.ECB;

            des.Key = DKtemp1;
            temp1 = des.CreateEncryptor().TransformFinalBlock(KSNtemp, 0, 8);
            des.Key = DKtemp2;
            temp2 = des.CreateDecryptor().TransformFinalBlock(temp1, 0, 8);
            des.Key = DKtemp1;
            temp1 = des.CreateEncryptor().TransformFinalBlock(temp2, 0, 8);
            Array.Copy(temp1, Result, 8);

            DKtemp1[0] ^= 0xC0;
            DKtemp1[1] ^= 0xC0;
            DKtemp1[2] ^= 0xC0;
            DKtemp1[3] ^= 0xC0;
            DKtemp2[0] ^= 0xC0;
            DKtemp2[1] ^= 0xC0;
            DKtemp2[2] ^= 0xC0;
            DKtemp2[3] ^= 0xC0;

            des.Key = DKtemp1;
            temp1 = des.CreateEncryptor().TransformFinalBlock(KSNtemp, 0, 8);
            des.Key = DKtemp2;
            temp2 = des.CreateDecryptor().TransformFinalBlock(temp1, 0, 8);
            des.Key = DKtemp1;
            temp1 = des.CreateEncryptor().TransformFinalBlock(temp2, 0, 8);
            Array.Copy(temp1, 0, Result, 8, 8);

            return Result;
        }

        static public Byte[] GenTransactionKey(byte[] KSN, byte[] DK)
        {
            int num;
            byte[] IPEK;
            byte[] KSNtemp = new byte[8];
            byte[] counter = new byte[3];
            byte[] counterTemp;

            byte[] DKtemp = new byte[16];
            byte[] key = new byte[16];
            byte[] temp1 = new byte[8];
            byte[] temp2 = new byte[8];

            IPEK = GetIPEK(KSN, DK);

            Array.Copy(KSN, 2, KSNtemp, 0, 8);
            KSNtemp[5] &= 0xE0;
            KSNtemp[6] = 0x00;
            KSNtemp[7] = 0x00;

            Array.Copy(KSN, 7, counter, 0, 3);
            counter[0] &= 0x1F;
            num = CountOne(counter[0]);
            num += CountOne(counter[1]);
            num += CountOne(counter[2]);

            counterTemp = SearchOne(counter);
            procCounter(KSNtemp, ref counter, counterTemp);

            Array.Copy(IPEK, key, 16);
            Array.Copy(KSNtemp, temp1, 8);

            while (num > 0)
            {
                NonRevKeyGen(ref temp1, ref temp2, key);

                Array.Copy(temp1, key, 8);
                Array.Copy(temp2, 0, key, 8, 8);

                counterTemp = SearchOne(counter);
                procCounter(KSNtemp, ref counter, counterTemp);
                Array.Copy(KSNtemp, temp1, 8);

                num--;
            }

            key[7] ^= 0xFF;
            key[15] ^= 0xFF;

            return key;
        }

        static public Byte[] GenDataKeyVariant(byte[] KSN, byte[] DK)
        {
            Byte[] key;

            key = GenTransactionKey(KSN, DK);

            key[5] ^= 0xFF;
            key[7] ^= 0xFF;
            key[13] ^= 0xFF;
            key[15] ^= 0xFF;

            return key;
        }

        static private int CountOne(byte input)
        {
            int temp = 0;
            if ((input & 0x80) == 0x80)
                temp++;
            if ((input & 0x40) == 0x40)
                temp++;
            if ((input & 0x20) == 0x20)
                temp++;
            if ((input & 0x10) == 0x10)
                temp++;
            if ((input & 0x08) == 0x08)
                temp++;
            if ((input & 0x04) == 0x04)
                temp++;
            if ((input & 0x02) == 0x02)
                temp++;
            if ((input & 0x01) == 0x01)
                temp++;
            return temp;
        }

        static private byte[] SearchOne(byte[] counter)
        {
            byte[] temp = new byte[] { 0, 0, 0 };

            if (counter[0] == 0)
            {
                if (counter[1] == 0)
                    temp[2] = SearchOneCore(counter[2]);
                else
                    temp[1] = SearchOneCore(counter[1]);
            }
            else
                temp[0] = SearchOneCore(counter[0]);

            return temp;
        }

        static private byte SearchOneCore(byte input)
        {
            if ((input & 0x80) == 0x80)
                return 0x80;
            if ((input & 0x40) == 0x40)
                return 0x40;
            if ((input & 0x20) == 0x20)
                return 0x20;
            if ((input & 0x10) == 0x10)
                return 0x10;
            if ((input & 0x08) == 0x08)
                return 0x08;
            if ((input & 0x04) == 0x04)
                return 0x04;
            if ((input & 0x02) == 0x02)
                return 0x02;
            if ((input & 0x01) == 0x01)
                return 0x01;
            return 0;
        }

        static private void procCounter(byte[] KSN, ref byte[] counter, byte[] counterTemp)
        {
            KSN[5] |= counterTemp[0];
            counter[0] ^= counterTemp[0];
            KSN[6] |= counterTemp[1];
            counter[1] ^= counterTemp[1];
            KSN[7] |= counterTemp[2];
            counter[2] ^= counterTemp[2];
        }

        static private void NonRevKeyGen(ref byte[] cReg1, ref byte[] cReg2, byte[] key)
        {
            int i;
            Byte[] keytemp;

            DES des = new DESCryptoServiceProvider();
            des.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            des.Padding = PaddingMode.None;
            des.Mode = CipherMode.ECB;

            for (i = 0; i < 8; i++)
                cReg2[i] = (byte)(cReg1[i] ^ key[8 + i]);

            keytemp = new Byte[8];
            Array.Copy(key, keytemp, 8);
            des.Key = keytemp;
            cReg2 = des.CreateEncryptor().TransformFinalBlock(cReg2, 0, 8);

            for (i = 0; i < 8; i++)
                cReg2[i] ^= key[8 + i];

            key[0] ^= 0xC0;
            key[1] ^= 0xC0;
            key[2] ^= 0xC0;
            key[3] ^= 0xC0;
            key[8] ^= 0xC0;
            key[9] ^= 0xC0;
            key[10] ^= 0xC0;
            key[11] ^= 0xC0;

            Array.Copy(key, keytemp, 8);
            des.Key = keytemp;

            for (i = 0; i < 8; i++)
                cReg1[i] ^= key[8 + i];


            cReg1 = des.CreateEncryptor().TransformFinalBlock(cReg1, 0, 8);

            for (i = 0; i < 8; i++)
                cReg1[i] ^= key[8 + i];
        }
    }
}
