using System;
using System.Text;

namespace SDGUtil
{
    public class MobileAppMethods
    {
        #region Rover Function

        //get the cardnum from track2
        public string MobileAppRover_CardNum(string enccard)
        {
            try
            {
                int iStart = 1;
                int iEnd = 0;
                string deccard;

                for (int counter = 1; counter <= enccard.Length; counter++)
                {
                    if (!char.IsDigit(enccard[counter]))
                    {
                        iEnd = counter - 1;
                        break;
                    }
                }

                deccard = enccard.Substring(iStart, iEnd);

                try
                {
                    decimal tempcard = Convert.ToDecimal(deccard);
                    return deccard;
                }
                catch
                {
                    return "0";
                }
            }
            catch
            {
                return "0";
            }
        }

        #region WisePad

        public string MobileApp_WisePad_CardNum_Track2(string track2)
        {
            try
            {
                string tempnum = "";
                int init;
                int end;
                init = track2.IndexOf(";");
                end = track2.IndexOf("=");

                if (init != -1 && end != -1)
                {
                    tempnum = track2.Substring(init + 1, end - 1);
                }
                else
                {
                    tempnum = "";
                }

                return tempnum;
            }
            catch
            {
                return "";
            }
        }

        public string MobileApp_WisePad_CardNum_Track1(string track1) //for credit
        {
            try
            {
                string tempnum = "";
                int init;
                int end;
                init = track1.IndexOf(";");
                end = track1.IndexOf("=");

                if (init != -1 && end != -1)
                {
                    tempnum = track1.Substring(init + 1, end - 1);
                }
                else
                {
                    tempnum = "";
                }

                return tempnum;
            }
            catch
            {
                return "";
            }
        }

        public string MobileApp_WisePad_CardNum_EmvData(string track) //for credit
        {
            try
            {
                string tempnum = "";
                int init;
                int end;
                init = 0;
                end = track.IndexOf("D");

                if (init != -1 && end != -1)
                {
                    tempnum = track.Substring(init, end);
                }
                else
                {
                    tempnum = "";
                }

                return tempnum;
            }
            catch
            {
                return "";
            }
        }

        #endregion WisePad

        public string MobileApp_Rover_CardNum(string track2)
        {
            try
            {
                string tempnum = "";
                int init;
                int end;
                init = track2.IndexOf(";");
                end = track2.IndexOf("=");

                if (init != -1 && end != -1)
                {
                    tempnum = track2.Substring(init + 1, end - 1);
                }
                else
                {
                    tempnum = "";
                }

                return tempnum;
            }
            catch
            {
                return "";
            }
        }

        //reforming the track2
        public string MobileAppRover_Track2(string inputTrack)
        {
            try
            {
                int x = inputTrack.IndexOf('F');
                string tempT1 = inputTrack.Remove(x);
                string tempT2 = tempT1.Replace('B', ';');
                string tempT3 = tempT2.Replace('D', '=');
                string track2 = tempT3.Insert(tempT3.Length, "?");

                return track2;
            }
            catch
            {
                return "";
            }
        }

        public string MobileAppRover_ReformatTrack2(string inputTrack)
        {
            int x = inputTrack.IndexOf('F');
            string tempT1 = inputTrack.Remove(x);
            string tempT2 = tempT1.Replace('B', ' ');
            string tempT3 = tempT2.Replace('D', '=');
            string track2 = tempT3.Trim();

            return track2;
        }

        //reforming track2 to magData
        public string MobileAppRover_MagData(string inputTrack)
        {
            string MagData = "";

            int x = inputTrack.IndexOf('F');
            string tempT1 = inputTrack.Remove(x);
            string tempT2 = tempT1.Replace("B", "");
            MagData = tempT2.Replace('D', '=');

            return MagData;
        }

        public string MobileAppRover_DecrypteTrack2(string Track2, string TrackKSN)
        {
            try
            {
                string newTrack2 = "";
                string tempTrack2 = "";
                //Use Guy's ConversionTool to decypte track2

                ConversionTool.ConversionToolSoapClient Convert = new ConversionTool.ConversionToolSoapClient();

                tempTrack2 = Convert.DecryptTrack2Rover(Track2, TrackKSN);

                try
                {
                    int x = tempTrack2.IndexOf('F');
                    string tempT1 = tempTrack2.Remove(x);
                    string tempT2 = tempT1.Replace('B', ';');
                    string tempT3 = tempT2.Replace('D', '=');
                    newTrack2 = tempT3.Trim() + "?";
                    return newTrack2;
                }
                catch
                {
                    return tempTrack2;
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public string MobileAppRover_DecrypteTrack1(string Track1, string TrackKSN)
        {
            try
            {
                //Use Guy's ConversionTool to decypte track1
                ConversionTool.ConversionToolSoapClient Convert = new ConversionTool.ConversionToolSoapClient();
                return Convert.DecryptTrack1Rover(Track1, TrackKSN);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public string MobileAppRover_Track2MagData(string Track2)
        {
            try
            {
                string temp1 = Track2.Replace("?", "");
                string MagData = temp1.Replace(";", "");
                return MagData;
            }
            catch
            {
                return "";
            }
        }

        //Function to convert PIN for Evertec debit transaction
        public string MobileAppRover_EvertecDebitKey(string inputPIN)
        {
            try
            {
                string ResultKey;

                ConversionTool.ConversionToolSoapClient Convert = new ConversionTool.ConversionToolSoapClient();

                ResultKey = Convert.EvertecKeyConversion(inputPIN);

                return ResultKey;
            }
            catch
            {
                return "";
            }
        }

        #endregion Rover Function

        #region Rambler and WisePad

        public string MobileApp_Parse_CardNum_Track2(string track2)
        {
            try
            {
                string tempnum = "";
                int init;
                int end;
                init = track2.IndexOf(";");
                end = track2.IndexOf("=");

                if (init != -1 && end != -1)
                {
                    tempnum = track2.Substring(init + 1, end - 1);
                }
                else
                {
                    tempnum = "";
                }

                return tempnum.Replace("^", "");
            }
            catch
            {
                return "";
            }
        }

        public string MobileApp_Parse_CardNum_Track1(string track1)
        {
            try
            {
                string tempnum = "";
                int init;
                int end;
                init = track1.IndexOf("B");
                end = track1.IndexOf("^");

                if (init != -1 && end != -1)
                {
                    tempnum = track1.Substring(init + 1, end - 2);
                }
                else
                {
                    tempnum = "";
                }

                return tempnum.Replace("^", "");
            }
            catch
            {
                return "";
            }
        }

        #endregion Rambler and WisePad

        #region Motorola

        public string MobileApp_Motorola_Track1(string inputTrack3)
        {
            string track1;

            int x = inputTrack3.IndexOf(';');

            try
            {
                string temptrack = inputTrack3.Substring(0, x);
                int y = temptrack.IndexOf('^');
                string temptk1 = temptrack.Substring(y);

                string temptrack2 = inputTrack3.Substring(x);
                int z = temptrack2.IndexOf('=');
                string temptk2 = temptrack2.Substring(1, z - 1);

                string newtk = temptk2 + temptk1;

                track1 = "%B" + newtk;
            }
            catch
            {
                track1 = "";
            }

            return track1;
        }

        public string MobileApp_Motorola_Track2(string inputTrack3)
        {
            string track2;

            int x = inputTrack3.IndexOf(';');

            try
            {
                string tempTK = inputTrack3.Substring(x);

                int count = 0;
                for (int i = 0; i < tempTK.Length; i++)
                {
                    if (tempTK[i] == '?')
                    {
                        count++;
                    }
                }

                if (count > 1)
                {
                    int position = tempTK.IndexOf('?');
                    track2 = tempTK.Remove(position + 1);
                }
                else
                {
                    track2 = tempTK;
                }
            }
            catch
            {
                track2 = "";
            }

            return track2;
        }

        public string MobileApp_Motorola_CardNum(string inputTrack2)
        {
            string CardNum;

            int x = inputTrack2.IndexOf('=');
            try
            {
                CardNum = inputTrack2.Substring(1, x - 1);
            }
            catch
            {
                CardNum = "";
            }

            return CardNum;
        }

        public string MobileApp_Motorola_Name(string inputTrack1)
        {
            string Name;

            int x = inputTrack1.IndexOf('^');
            int y = inputTrack1.IndexOf('^', x + 1);

            try
            {
                Name = inputTrack1.Substring(x + 1, y - x - 1);
            }
            catch
            {
                Name = "";
            }

            return Name;
        }

        public string MobileApp_Motorola_ExpDate(string inputTrack2)
        {
            string ExpDate;

            int x = inputTrack2.IndexOf('=');

            try
            {
                ExpDate = inputTrack2.Substring(x + 1, 4);
            }
            catch
            {
                ExpDate = "";
            }

            return ExpDate;
        }

        public string MobileApp_Motorola_ExpMonth(string ExpDate)
        {
            string ExpMonth;

            try
            {
                ExpMonth = ExpDate.Substring(2, 2);
            }
            catch
            {
                ExpMonth = "";
            }

            return ExpMonth;
        }

        public string MobileApp_Motorola_ExpYear(string ExpDate)
        {
            string ExpYear;

            try
            {
                ExpYear = ExpDate.Substring(0, 2);
            }
            catch
            {
                ExpYear = "";
            }

            return ExpYear;
        }

        #endregion Motorola

        #region ZebraRW220

        public string MobileApp_ZebraRW220_CardNum(string inputTrack2)
        {
            string CardNum;

            int x = inputTrack2.IndexOf('=');
            try
            {
                CardNum = inputTrack2.Substring(0, x);
            }
            catch
            {
                CardNum = "";
            }

            return CardNum;
        }

        public string MobileApp_ZebraRW220_Name(string inputTrack1)
        {
            string Name;

            int x = inputTrack1.IndexOf('^');
            int y = inputTrack1.IndexOf('^', x + 1);

            try
            {
                Name = inputTrack1.Substring(x + 1, y - x - 1);
            }
            catch
            {
                Name = "";
            }

            return Name;
        }

        public string MobileApp_ZebraRW220_ExpDate(string inputTrack2)
        {
            string ExpDate;

            int x = inputTrack2.IndexOf('=');

            try
            {
                ExpDate = inputTrack2.Substring(x + 1, 4);
            }
            catch
            {
                ExpDate = "";
            }

            return ExpDate;
        }

        public string MobileApp_ZebraRW220_ExpMonth(string ExpDate)
        {
            string ExpMonth;

            try
            {
                ExpMonth = ExpDate.Substring(2, 2);
            }
            catch
            {
                ExpMonth = "";
            }

            return ExpMonth;
        }

        public string MobileApp_ZebraRW220_ExpYear(string ExpDate)
        {
            string ExpYear;

            try
            {
                ExpYear = ExpDate.Substring(0, 2);
            }
            catch
            {
                ExpYear = "";
            }

            return ExpYear;
        }

        #endregion ZebraRW220

        #region Evertec

        public string Evertec_PinConversion(string pinInput)
        {
            string convertedPIN;
            try
            {
                //string newInput;
                if (pinInput.Length > 16)
                {
                    string tempPin = pinInput.Substring(0, 16);
                    convertedPIN = tempPin;//tempPin.Substring(1);
                }
                else
                {
                    convertedPIN = pinInput;
                }

                //ConversionTool.ConversionToolSoapClient Convert = new ConversionTool.ConversionToolSoapClient();

                //convertedPIN = Convert.EvertecKeyConversion(newInput);
            }
            catch
            {
                convertedPIN = "";
            }

            return convertedPIN;
        }

        #endregion Evertec

        #region Infinea Tab

        public string MobileApp_InfineaTab_Track1(string track1, string track2)
        {
            string newTrack1;

            try
            {
                int x = track1.IndexOf('^');
                string tempTK1 = track1.Substring(x);

                int y = track2.IndexOf('=');
                string card = track2.Substring(1, y - 1);

                newTrack1 = "%B" + card + tempTK1;
            }
            catch
            {
                newTrack1 = track1;
            }

            return newTrack1;
        }

        public string MobileApp_InfineaTab_CardNumber(string inputTrack2)
        {
            string CardNum;

            int x = inputTrack2.IndexOf('=');
            try
            {
                string tempCard = inputTrack2.Substring(0, x);

                try
                {
                    CardNum = tempCard.Replace(";", "");
                }
                catch
                {
                    CardNum = tempCard;
                }
            }
            catch
            {
                CardNum = "";
            }

            return CardNum;
        }

        #endregion Infinea Tab

        #region Koamtac

        public string MobileApp_Koamtac_Track1(string inputTrack)
        {
            string track1;

            int x = inputTrack.IndexOf(';');

            try
            {
                string temptrack = inputTrack.Substring(0, x);
                int y = temptrack.IndexOf('^');
                string temptk1 = temptrack.Substring(y);

                string temptrack2 = inputTrack.Substring(x);
                int z = temptrack2.IndexOf('=');
                string temptk2 = temptrack2.Substring(1, z - 1);

                string newtk = temptk2 + temptk1;

                track1 = "%B" + newtk;
                //track1 = temptrack;
            }
            catch
            {
                track1 = "";
            }

            return track1;
        }

        public string MobileApp_Koamtac_Track2(string inputTrack)
        {
            string track2;

            int x = inputTrack.IndexOf(';');

            try
            {
                string tempTK = inputTrack.Substring(x);

                track2 = tempTK;
            }
            catch
            {
                track2 = "";
            }

            return track2;
        }

        public string MobileApp_Koamtac_CardNum(string inputTrack2)
        {
            string CardNum;

            int x = inputTrack2.IndexOf('=');
            try
            {
                CardNum = inputTrack2.Substring(1, x - 1);
            }
            catch
            {
                CardNum = "";
            }

            return CardNum;
        }

        public string MobileApp_Koamtac_Name(string inputTrack1)
        {
            string Name;

            int x = inputTrack1.IndexOf('^');
            int y = inputTrack1.IndexOf('^', x + 1);

            try
            {
                Name = inputTrack1.Substring(x + 1, y - x - 1);
            }
            catch
            {
                Name = "";
            }

            return Name;
        }

        public string MobileApp_Koamtac_ExpDate(string inputTrack2)
        {
            string ExpDate;

            int x = inputTrack2.IndexOf('=');

            try
            {
                ExpDate = inputTrack2.Substring(x + 1, 4);
            }
            catch
            {
                ExpDate = "";
            }

            return ExpDate;
        }

        public string MobileApp_Koamtac_ExpMonth(string ExpDate)
        {
            string ExpMonth;

            try
            {
                ExpMonth = ExpDate.Substring(2, 2);
            }
            catch
            {
                ExpMonth = "";
            }

            return ExpMonth;
        }

        public string MobileApp_Koamtac_ExpYear(string ExpDate)
        {
            string ExpYear;

            try
            {
                ExpYear = ExpDate.Substring(0, 2);
            }
            catch
            {
                ExpYear = "";
            }

            return ExpYear;
        }

        #endregion Koamtac

        #region Private Function

        //

        #region Generate Activation Code

        private string GenerateActivationCode(int LengOfString)
        {
            string possibleChar = "QAZWSXEDCRFVTGBYHNUJMIKLP123456789";
            Random random = new Random();
            decimal randNum;
            StringBuilder builder = new StringBuilder();

            for (var I = 1; I <= LengOfString; I++)
            {
                randNum = random.Next(1, possibleChar.Length);
                string ch = possibleChar.Substring(System.Convert.ToInt32(randNum), 1);
                builder.Append(ch);
            }

            return builder.ToString();
        }

        #endregion Generate Activation Code

        //

        #endregion Private Function
    }
}