using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MailSenderTEST
{
    class Program : ImageCreator
    {
        static void Main(string[] args)
        {
            
            DatabaseReader();
			Thread.Sleep(60000);
            SendMail();
        }
       
        private static void DatabaseReader()
        {
            try
            {
                var connectionString = "server=IP;uid=UID;pwd=PWD;database=DATABASE";
                var conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                Console.WriteLine($"Connection status: {conn.State}");
                string sql = "SELECT * FROM sales_order WHERE status = 'complete' ORDER BY `created_at` DESC ";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Console.WriteLine(rdr[96] + " " + rdr[121]);
                }
                rdr.Close();

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private static void SendMail()
        {
            var maile = File.ReadAllLines(@"C:\Users\Użytkownik\Desktop\mail.txt");
            foreach(var _mail in maile)
            {
                using (LinkedResource image = new LinkedResource(@"C:\test.jpg", "image/jpeg") { ContentId = "myimage" })
                using (MailMessage mail = new MailMessage())
                using (SmtpClient smtpClient = new SmtpClient())
                {
                    smtpClient.Host = "HOST";
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("MAIL", "PASSWORD");
                    smtpClient.EnableSsl = true;
                    String body = @"
                                   <html>
                                   <head></head>
                                   <body>    
                                   <a href=""HTTP ODNOŚNIK"">
                                <img src=""cid:myimage"" />   
                                    </a>
                                   </body>
                                   </html>
                               ";
                    AlternateView view = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                    view.LinkedResources.Add(image);
                    mail.IsBodyHtml = true;
                    mail.AlternateViews.Add(view);
                    mail.From = new MailAddress("MAIL", "TYTUŁ");
                    mail.To.Add(_mail);
                    mail.Subject = "Wystaw opinię i otrzymaj rabat na kolejne zakupy!";
                    smtpClient.Send(mail);
                    Console.WriteLine("Mail sent: " + DateTime.Now);
                    var list = new List<string>();
                    list.Add(_mail);
                    //zapis ostatniego maila
                    File.AppendAllLines(@"C:\Users\Użytkownik\Desktop\wyslane.txt", list) ;
                    Thread.Sleep(180000);
                }
            }
        }
    }
    public class ImageCreator
    {
        private static void CreateImgWithCodeAndSend()
        {
            var couponcodes = File.ReadAllLines(@"C:\Users\Użytkownik\Desktop\coupons.txt");
            foreach (var code in couponcodes)
            {
                var path = @"C:\Users\Użytkownik\Desktop\powystawieniu.jpg";
                Image image;
                using (Stream s = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    image = Image.FromStream(s);
                }
                using (Image newImage = new Bitmap(image))
                {
                    Graphics graphic = Graphics.FromImage(newImage);
                    var font = new Font("Playfair Display", 82);
                    graphic.DrawString(code, font, Brushes.GreenYellow, 405, 775);
                    newImage.Save(@"C:\Users\Użytkownik\Desktop\couponcode.jpg", ImageFormat.Jpeg);
                }
                image.Dispose();
            }
        }
    }
}
