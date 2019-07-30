using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDGUtil
{
    public class CardValidator
    {
        #region Luhn class license
        /* 
         * Luhn - Version 1.0
         * Copyright (c) 2012 - Petros Kyladitis
         * All rights reserved.
         * 
         * This class provide validation checking for Luhn formula based numbers
         * 
         * Redistribution and use in source and binary forms, with or without
         * modification, are permitted provided that the following conditions are met: 
         * 
         * 1. Redistributions of source code must retain the above copyright notice, this
         *    list of conditions and the following disclaimer. 
         * 2. Redistributions in binary form must reproduce the above copyright notice,
         *    this list of conditions and the following disclaimer in the documentation
         *    and/or other materials provided with the distribution. 
         * 
         * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
         * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
         * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
         * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
         * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
         * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
         * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
         * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
         * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
         * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
         */
        #endregion

        /// <summary>
        /// This class provide static methods for validation checking of Luhn formula numbers.
        /// </summary>
        public static class Luhn
        {
            /// <summary>
            /// The greek social id, uses the Luhn formula.<br />
            /// The last digit is the validation digit using the Luhn check digit algorithm.<ul><li>
            ///  1 - Counting from the check digit, which is the rightmost, and moving left, double the value of every second digit.</li><li>
            ///  2 - Sum the digits of the products (e.g., 10: 1 + 0 = 1, 14: 1 + 4 = 5) together with the undoubled digits from the original number.</li><li>
            ///  3 - If the total modulo 10 is equal to 0 (if the total ends in zero) then the number is valid according to the Luhn formula; else it is not valid.</li></ul>
            /// </summary>
            /// <param name="sidNum">The social id number in string</param>
            /// <returns>True if pass the Luhn validation, else false</returns>
            public static bool IsValid(string id)
            {
                int idLength = id.Length;
                int currentDigit;
                int idSum = 0;
                int currentProcNum = 0; //the current process number (to calc odd/even proc)

                for (int i = idLength - 1; i >= 0; i--)
                {
                    //get the current rightmost digit from the string
                    string idCurrentRightmostDigit = id.Substring(i, 1);

                    //parse to int the current rightmost digit, if fail return false (not-valid id)
                    if (!int.TryParse(idCurrentRightmostDigit, out currentDigit))
                        return false;

                    //double value of every 2nd rightmost digit (odd)
                    //if value 2 digits (can be 18 at the current case),
                    //then sumarize the digits (made it easy the by remove 9)
                    if (currentProcNum % 2 != 0)
                    {
                        if ((currentDigit *= 2) > 9)
                            currentDigit -= 9;
                    }
                    currentProcNum++; //increase the proc number

                    //summarize the processed digits
                    idSum += currentDigit;
                }

                //if digits sum is exactly divisible by 10, return true (valid), else false (not-valid)
                return (idSum % 10 == 0);
            }
        }
    }
}
