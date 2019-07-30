using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RamblerLayer1
{
    public class Class1
    {
        public Track2Data IDC_Decrypt2(String enctrack, String ksn, String dk)
        {
            Track2Data response = new Track2Data();
            Decrypt decrypt;
            String track1, track2;
            String lrc = string.Empty;

            decrypt = new Decrypt(dk);

            if (decrypt.Decrypt_Hex(ksn, enctrack, out track1, out track2) == 0)
            {
                try
                {
                    string tempTk = track2;
                    int last = track2.LastIndexOf('?');

                    if (track2.Length < 16)
                    {
                        tempTk = string.Empty;
                    }
                    else
                    {
                        if (last != -1)
                        {
                            if (last != track2.Length - 1)
                            {
                                lrc = track2.Substring(last + 1);
                                string newTk = track2.Remove(last + 1);
                                if (newTk.Count() < 39)
                                {
                                    newTk = newTk.Replace("?", "").PadRight(38, '0') + "?";
                                }

                                tempTk = newTk;
                            }
                        }
                    }

                    int endSen = Regex.Matches(tempTk, @"[?]").Count;
                    if (endSen == 2)
                    {
                        tempTk = tempTk.Substring(0, tempTk.Length - 1);
                    }

                    int startSen = Regex.Matches(tempTk, @"[;]").Count;
                    if (startSen == 2)
                    {
                        tempTk = tempTk.Substring(1, tempTk.Length - 1);
                    }

                    response.Track2 = tempTk;
                    response.LRCTrack2 = lrc;
                }
                catch
                {
                    response.Track2 = track2;
                }
            }
            else
            {
                response.Track2 = ""; //Failed To decrypt
            }

            return response;
        }

        public Track1Data IDC_Decrypt_Track1(String enctrack, String ksn, String dk)
        {
            Track1Data response = new Track1Data();
            Decrypt decrypt;
            String track1, track2, lrc = string.Empty;

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
                            lrc = track1.Substring(last + 1);
                            newTrack1 = track1.Remove(last + 1);
                        }
                    }

                    int endSen = Regex.Matches(newTrack1, @"[?]").Count;
                    if (endSen == 2)
                    {
                        newTrack1 = newTrack1.Substring(0, newTrack1.Length - 1);
                    }

                    response.Track1 = newTrack1;
                    response.LRCTrack1 = lrc;
                }
                catch
                {
                    response.Track1 = track1;
                }
            }
            else
            {
                response.Track1 = ""; //Fail to decript track
            }

            return response;
        }
    }
}