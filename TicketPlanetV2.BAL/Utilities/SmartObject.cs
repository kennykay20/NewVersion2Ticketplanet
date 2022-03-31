using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace TicketPlanetV2.BAL.Utilities
{
    public class EmailSubject
    {
        public int email_type { get; set; }
        public string email_subject { get; set; }
    }
    public static class SmartObject
    {
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "pemgail9uzpgzl88";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;
        //Encrypt

        public static string EncryptString(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string DecryptString(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);

        }


        public static EmailSubject getEmailHeader(short email_type)
        {
            string line = string.Empty;
            List<EmailSubject> infoObj = new List<EmailSubject>();
            string filePath = Path.Combine(HttpRuntime.AppDomainAppPath, "pdftemplate/EmailSubject.txt");
            StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));
            while ((line = sr.ReadLine()) != null)
            {
                // Initialization.
                EmailSubject infoObj_str = new EmailSubject();
                string[] info = line.Split(',');
                infoObj_str.email_type = Convert.ToInt16(info[0]);
                infoObj_str.email_subject = info[1];
                infoObj.Add(infoObj_str);
            }

            // Closing.
            sr.Dispose();
            sr.Close();
            return infoObj.Where(p => p.email_type == email_type).FirstOrDefault();
        }
        public static string PopulateUserBody(short email_type)
        {
            string body = string.Empty;
            //string contentRootPath = HostingEnvironment.ContentRootPath;
            var path = string.Empty;
            switch (email_type)
            {
                case 0:
                    path = Path.Combine(HttpRuntime.AppDomainAppPath, "EmailTemplates/GenesisMaturionMovieEmail.html");
                    break;
                case 1:
                    path = Path.Combine(HttpRuntime.AppDomainAppPath, "EmailTemplates/FilmHouseEmail.html");
                    break;
                case 2:
                    path = Path.Combine(HttpRuntime.AppDomainAppPath, "EmailTemplates/EventTemplate.html");
                    break;
               
            }
            StreamReader reader = new StreamReader(path);
            body = reader.ReadToEnd();


            return body;
        }

    }
}
