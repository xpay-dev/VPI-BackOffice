using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDGDAL
{
    public class PasswordGenerator
    {
        Random random;

        public PasswordGenerator()
        {
            random = new Random();
        }

        public string GeneratePassword(int LenOfString)
        {
            string strPwdchar = "abcdefghijklmnopqrstuvwxyz0123456789#+@&$!@#$%&?ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string strPwd = "";
            Random rnd = new Random();
            for (int i = 0; i <= LenOfString; i++)
            {
                int iRandom = rnd.Next(0, strPwdchar.Length - 1);
                strPwd += strPwdchar.Substring(iRandom, 1);
            }

            return strPwd;
        }
    }
}
