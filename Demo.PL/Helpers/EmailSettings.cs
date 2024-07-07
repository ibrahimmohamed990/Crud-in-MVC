using System.Net;
using System.Net.Mail;

namespace Demo.PL.Helpers
{
    public static class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("ibrahimm0067@gmail.com", "abgnzwubwteuysln");
            client.Send("ibrahimm0067@gmail.com", email.To, email.Tittle, email.Body);
        }
    }
}
