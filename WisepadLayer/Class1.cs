using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisepadLayer
{
    public class WisePad
    {
        private DecryptedData decryptTrack(String encTracks, String ksn, String bdk, String nameField = "")
        {
            try
            {
                String key = "A4223344556677889900AABBCCDDEEFFA422334455667788";
                //String key = DUKPTServer.GetDataKeyVar(ksn, bdk);
                String cardholderName = nameField.IndexOf("^") < 0 ? nameField : nameField.Substring(0, nameField.IndexOf("^"));

                String tracks = TripleDES.decrypt_CBC(encTracks, key);
                String track1 = "";
                String track2 = "";
                String track3 = "";
                int i = tracks.Length;
                if (tracks.StartsWith("25"))
                {
                    track1 = tracks.Substring(0, 160);
                    encTracks = encTracks.Substring(160);
                    tracks = TripleDES.decrypt_CBC(encTracks, key);
                }

                if (tracks.Length == 80 || tracks.Length == 304)
                {
                    track2 = tracks.Substring(0, 80);
                    encTracks = encTracks.Substring(80);
                    tracks = TripleDES.decrypt_CBC(encTracks, key);
                }

                if (tracks.Length == 160)
                {
                    track1 = tracks.Substring(0, 80);
                    encTracks = encTracks.Substring(80);
                    tracks = TripleDES.decrypt_CBC(encTracks, key);
                }

                if (tracks.Length == 224)
                {
                    track3 = tracks;
                }

                if (!track1.Equals(""))
                {
                    //not sure
                    track1 = Encoding.UTF8.GetString(hexStringTobytes(track1));
                }

                track2 = decodeTrack2or3(track2);
                track3 = decodeTrack2or3(track3);

                if (track1.StartsWith("%B"))
                {
                    int endIndex = 0;
                    endIndex = track1.IndexOf('?');
                    if (endIndex < 0)
                    {
                        track1 = "";
                    }
                    else
                    {
                        try
                        {
                            track1 = track1.Substring(0, endIndex + 1);
                        }
                        catch (Exception e)
                        {
                            track1 = "";
                        }
                    }
                }
                else
                {
                    track1 = "";
                }

                if (track2.StartsWith(";"))
                {
                    int endIndex = 0;
                    endIndex = track2.IndexOf('?');
                    if (endIndex < 0)
                    {
                        track2 = "";
                    }
                    else
                    {
                        try
                        {
                            track2 = track2.Substring(0, endIndex + 1);
                        }
                        catch (Exception e)
                        {
                            track2 = "";
                        }
                    }
                }
                else
                {
                    track2 = "";
                }

                if (track3.StartsWith(";"))
                {
                    int endIndex = 0;
                    endIndex = track3.IndexOf('?');
                    if (endIndex < 0)
                    {
                        track3 = "";
                    }
                    else
                    {
                        try
                        {
                            track3 = track3.Substring(0, endIndex + 1);
                            if (track3.Length < 13)
                            {
                                track3 = "";
                            }
                        }
                        catch (Exception e)
                        {
                            track3 = "";
                        }
                    }
                }
                else
                {
                    track3 = "";
                }

                return new DecryptedData(cardholderName, track1, track2, track3);
            }
            catch (Exception ex)
            {
 
            }

            return new DecryptedData("", "", "", "");
        }

        public String DecryptTrackData(String encTrack)
        {
            string data = "";

            DecryptedData decryptedData = decryptTrack(encTrack, "", "");

            if (decryptedData.track1 != "")
            {
                data = decryptedData.track1 + "," + decryptedData.cardholderName;
            }
            else if (decryptedData.track2 != "")
            {
                data = decryptedData.track2 + "," + decryptedData.cardholderName;
            }
            else
            {
                data = decryptedData.track3 + "," + decryptedData.cardholderName;
            }

            return data;
        }

        private static byte[] hexStringTobytes(String input)
        {

            int charLength = input.Length;
            byte[] bytes = new byte[charLength / 2];

            for (int i = 0; i < charLength; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(input.Substring(i, 2), 16);
            }

            return bytes;
        }

        private static String decodeTrack2or3(String track2or3)
        {
            byte[] temp;
            int index;

            bool isASCII = false;
            if (track2or3.ToString().ToLower().StartsWith("3b"))
            {
                isASCII = true;
            }

            if (isASCII)
            {
                temp = hexStringTobytes(track2or3);
            }
            else
            {
                temp = new byte[track2or3.Length];
                for (int i = 0; i < track2or3.Length; ++i)
                {
                    String str1 = track2or3[i].ToString();
                    int numVal = 0x30 + Convert.ToInt32(str1, 16);
                    byte tByte = Convert.ToByte(numVal.ToString(), 16);

                    temp[i] = tByte; //Convert.ToByte(track2or3[i].ToString(), 16);//(byte)(Integer.parseInt("" + track2or3.charAt(i), 16) + 0x30);
                }


            }
            track2or3 = Encoding.UTF8.GetString(temp);

            index = track2or3.IndexOf("?");
            if (index < 0)
            {
                return "";
            }
            track2or3 = track2or3.Substring(0, index + 1);

            if (!track2or3.StartsWith(";"))
            {
                return "";
            }

            return track2or3;
        }
    }
}
