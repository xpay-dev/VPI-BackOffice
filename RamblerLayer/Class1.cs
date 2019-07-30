using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Format6_Decoder;

namespace RamblerLayer
{
    public class Class1
    {
        public String IDC_Decrypt(String enctrack, String ksn, String dk)
        {
            Decrypt decrypt;
            String track1, track2;

            decrypt = new Decrypt(dk);

            if (decrypt.Decrypt_Hex(ksn, enctrack, out track1, out track2) == 0)
            {
                try
                {
                    string newTrack1 = track1;
                    int last = track1.LastIndexOf('?');

                    if (last != -1)
                    {
                        if (last != track1.Length - 1)
                        {
                            newTrack1 = track1.Remove(last + 1);
                        }
                    }
                    return newTrack1;
                }
                catch
                {
                    return track1;
                }
            }
            else
            {
                return "Fail to decript track";
            }
        }

        public String IDC_Decrypt_Track2(String enctrack, String ksn, String dk)
        {
            Decryptor decrypt;
            int iResult;

            decrypt = new Decryptor(dk);

            iResult = decrypt.Decrypt(ksn, enctrack);

            if (iResult == 0)
            {
                String decodedtrack = decrypt.GetTrack2();

                return decodedtrack;
            }
            else
            {
                return "Fail to decrypt";
            }
        }

    }
}
