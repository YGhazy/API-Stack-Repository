using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Stack.Repository.Common
{
    public class HelperFunctions
    {
        // Function to compute the SHA256 Hash of a string . 
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   .
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        //Generates a 10 digit random number .
        public static string GenerateRandomNumber()
        {
            Random random = new Random();
            string r = "";
            int i;
            for (i = 1; i < 11; i++)
            {
                r += random.Next(0, 9).ToString();
            }
            return r;

        }

        public static async Task<DateTime> GetEgyptsCurrentLocalTime()
        {
            return await Task.Run(() =>
            {
                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
                DateTimeOffset localServerTime = DateTimeOffset.Now;
                DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, timeZoneInfo);
                return localTime.DateTime;
            });
        }

    }
}
