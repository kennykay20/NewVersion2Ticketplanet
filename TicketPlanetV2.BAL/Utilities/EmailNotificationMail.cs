

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TicketPlanetV2.BAL.Utilities
{
    public static class EmailNotificationMail
    {

        public static bool emailIsValid(string email)
        {
            string expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, string.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

       

     
        public static void SendEmailPlain(string to_email_address, string email_subject, string email_message, string to_email_cc, string to_email_bc)
        {
            try
            {


                if (!string.IsNullOrEmpty(to_email_address))
                {
                    if (emailIsValid(to_email_address))
                    {
                        try
                        {
                            //create the mail message 
                            MailMessage mail = new MailMessage();

                            //set the addresses 
                            mail.From = new MailAddress("support@ticketplanet.ng"); //IMPORTANT: This must be same as your smtp authentication address.
                            mail.To.Add(to_email_address);

                            //set the content 
                            // mail.Attachments.Add(new Attachment(new MemoryStream(bytes), Subject));
                            mail.IsBodyHtml = true;
                            mail.Subject = email_subject;
                            mail.Body = email_message;


                            SmtpClient smtp = new SmtpClient("mail.ticketplanet.ng");
                            smtp.Port = 587;
                            if (!string.IsNullOrEmpty(to_email_cc))
                            {
                                mail.CC.Add(new MailAddress(to_email_cc));
                            }
                            if (!string.IsNullOrEmpty(to_email_bc))
                            {
                                mail.Bcc.Add(new MailAddress(to_email_bc));
                            }
                            NetworkCredential Credentials = new NetworkCredential("support@ticketplanet.ng", "12Dilichukwu$");
                            smtp.Credentials = Credentials;
                            smtp.Send(mail);


                        }
                        catch (Exception ex)
                        {

                        }

                    }

                }



            }
            catch (Exception ex)
            {

            }

        }

        public static void SendEmailContact(string email_address_from, string to_email_address, string email_subject, string email_message, string to_email_cc, string to_email_bc)
        {
            try
            {

                if (!string.IsNullOrEmpty(to_email_address))
                {
                    if (emailIsValid(to_email_address) && emailIsValid(email_address_from))
                    {
                        try
                        {
                            //create the mail message 
                            MailMessage mail = new MailMessage();

                            //set the addresses 
                            mail.From = new MailAddress("support@ticketplanet.ng"); //IMPORTANT: This must be same as your smtp authentication address.
                            mail.To.Add(to_email_address);

                            //set the content 
                            // mail.Attachments.Add(new Attachment(new MemoryStream(bytes), Subject));
                            mail.IsBodyHtml = false;
                            mail.Subject = email_subject;
                            mail.Body = email_message;

                            SmtpClient smtp = new SmtpClient("mail.ticketplanet.ng");
                            smtp.Port = 587;
                            if (!string.IsNullOrEmpty(to_email_cc))
                            {
                                mail.CC.Add(new MailAddress(to_email_cc));
                            }
                            if (!string.IsNullOrEmpty(to_email_bc))
                            {
                                mail.Bcc.Add(new MailAddress(to_email_bc));
                            }
                            NetworkCredential Credentials = new NetworkCredential("support@ticketplanet.ng", "12Dilichukwu$");
                            smtp.Credentials = Credentials;
                            smtp.Send(mail);


                        }
                        catch (Exception ex)
                        {

                        }

                    }

                }



            }
            catch (Exception ex)
            {

            }

        }


        public static void SendEmail(string to_email_address, string email_subject, AlternateView email_message, string to_email_cc, string to_email_bc)
        {
            try
            {

                if (!string.IsNullOrEmpty(to_email_address))
                {
                    if (emailIsValid(to_email_address))
                    {
                        try
                        {
                            //create the mail message 
                            MailMessage mail = new MailMessage();

                            //set the addresses 
                            mail.From = new MailAddress("documentnotification@ticketplanet.ng"); //IMPORTANT: This must be same as your smtp authentication address.
                            mail.To.Add(to_email_address);

                            //set the content 
                            // mail.Attachments.Add(new Attachment(new MemoryStream(bytes), Subject));
                            mail.IsBodyHtml = true;
                            mail.Subject = email_subject;
                            // mail.Body = email_message;
                            mail.AlternateViews.Add(email_message);
                            SmtpClient smtp = new SmtpClient("mail.ticketplanet.ng");
                            smtp.Port = 587;
                            if (!string.IsNullOrEmpty(to_email_cc))
                            {
                                mail.CC.Add(new MailAddress(to_email_cc));
                            }
                            if (!string.IsNullOrEmpty(to_email_bc))
                            {
                                mail.Bcc.Add(new MailAddress(to_email_bc));
                            }
                            NetworkCredential Credentials = new NetworkCredential("documentnotification@ticketplanet.ng", "12Ticketplanet$");
                            smtp.Credentials = Credentials;
                            smtp.Send(mail);


                        }
                        catch (Exception ex)
                        {

                        }

                    }

                }



            }
            catch (Exception ex)
            {

            }

        }
    }
}
