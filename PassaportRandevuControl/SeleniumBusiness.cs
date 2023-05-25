using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Mail;
using System.Net;
using Patagames.Ocr;
using System.Timers;

namespace PassaportServices
{
    public class SeleniumBusiness
    {
        UserDataandUrl db = new UserDataandUrl();
        ServiceLogFile serviceLog = new ServiceLogFile();

        public IWebDriver chrome;
        public string pageUrl = "";
        public string mailSendStatus = "";

        public IWebDriver chromeDrivers()
        {
            ChromeDriverService chromeDriveService = ChromeDriverService.CreateDefaultService();
            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("headless");
            //options.AddArgument("disable-gpu");
            chromeDriveService.HideCommandPromptWindow = true;
            IWebDriver chrome = new ChromeDriver(chromeDriveService, options);
            return chrome;
        }
        public string randevuPageInput()
        {
            if (chrome == null)
            {
                chrome = chromeDrivers();
                chrome.Navigate().GoToUrl(db.randevuUrl);
                Thread.Sleep(2000);

                pageUrl = chrome.Url;
                return pageUrl;
            }

            else
            {
                chrome.Navigate().Refresh();

                chrome.Navigate().GoToUrl(db.randevuUrl);
                Thread.Sleep(2000);

                pageUrl = chrome.Url;
                return pageUrl;
            }
        }
        public string randevuPageInputProcess()
        {
            IWebElement pasaportButton = chrome.FindElement(By.XPath("/html/body/div/div[1]/div/div/div[2]/form/div/a[3]/div"));
            pasaportButton.Click();
            Thread.Sleep(2000);
            IWebElement acceptTheContract = chrome.FindElement(By.XPath("/html/body/div[2]/div[2]/div/div/div/div/div/div/div/div[4]/button[1]"));
            acceptTheContract.Click();
            Thread.Sleep(2000);

            pageUrl = chrome.Url;
            return pageUrl;
        }
        public string userDataInputPage()
        {
            chrome.FindElement(By.XPath("//*[@id=\"pasaport\"]/div/div/div/div[1]/div/button")).Click();
            chrome.FindElement(By.XPath("//*[@id=\"divKisi\"]/div[1]/div[1]/div/input")).SendKeys(db.userName);
            chrome.FindElement(By.XPath("//*[@id=\"divKisi\"]/div[1]/div[2]/div/input")).SendKeys(db.userSurname);
            chrome.FindElement(By.XPath("//*[@id=\"IdentityNo\"]")).SendKeys(db.identificationNumberTC.ToString());
            chrome.FindElement(By.XPath("//*[@id=\"divKisi\"]/div[1]/div[4]/div/div/div[1]/input")).SendKeys(db.dateOfBirthDay.ToString());
            chrome.FindElement(By.XPath("//*[@id=\"divKisi\"]/div[1]/div[4]/div/div/div[2]/input")).SendKeys(db.dateOfBirthMonth.ToString());
            chrome.FindElement(By.XPath("//*[@id=\"divKisi\"]/div[1]/div[4]/div/div/div[3]/input")).SendKeys(db.dateOfBirthYear.ToString());
            chrome.FindElement(By.XPath("//*[@id=\"divKisi\"]/div[1]/div[5]/div/input")).SendKeys(db.telephoneNumber.ToString());
            IWebElement captcha = chrome.FindElement(By.XPath("//*[@id=\"divKisi\"]/div[1]/div[6]/div/div/img"));
            Thread.Sleep(1000);
            chrome.FindElement(By.XPath("//*[@id=\"divKisi\"]/div[1]/div[6]/div/input")).SendKeys(captchaResult(captcha));
            Thread.Sleep(2000);

            pageUrl = chrome.Url;
            return pageUrl;
        }
        public string captchaResult(IWebElement captcha)
        {
            //Login captcha Screnshot
            var titleScreenshot = (captcha as ITakesScreenshot).GetScreenshot();
            titleScreenshot.SaveAsFile(captcha.Text + "_.png", ScreenshotImageFormat.Png);

            //Login captcha Screnshot Text
            var objOcr = OcrApi.Create();
            objOcr.Init(Patagames.Ocr.Enums.Languages.English);
            string captchaText = objOcr.GetTextFromImage(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\_.png");

            return captchaText;
        }
        public void randevuStatusPage()
        {
            chrome.FindElement(By.XPath("/html/body/div/form/section/div/div[2]/div[3]/div/div/a[2]")).Click();

            List<string> list = new List<string>();
            list = randevuStatuEmpty(db.date);
            mailSendStatus = sendMail(list);    

        }
        public List<string> randevuStatuEmpty(string[] tarihler)
        {
            List<string> status = new List<string>();

            foreach (string tarih in tarihler)
            {
                // Tarih olan ilçelerin listesini al
                IList<IWebElement> districts = chrome.FindElements(By.XPath($"//span[contains(text(), '{tarih}')]/ancestor::a"));

                // Her ilçenin nüfus müdürünün ismini yazdır
                foreach (IWebElement district in districts)
                {
                    string districtName = district.FindElement(By.XPath(".//span[1]")).Text;
                    status.Add(districtName + "\n" + tarih + "\n");
                }
            }

            return status;
        }
        public string sendMail(List<string> emailList)
        {
            string result="";
            if (emailList.Count != 0)
            {
                // E-posta gönderimi için gerekli bilgiler
                string to = "mevlut.boztepe@abdiibrahim.com.tr";
                string from = db.mail; // Gönderen e-posta adresi
                string subject = "Pasaport Randevu";
                string body = "Merhaba,\n\nAşağıda listelenen İlçelerde müsaitlik vardır:\n\n";

                foreach (string item in emailList)
                {
                    body += item + "\n";
                }
                // MailMessage sınıfı ile e-posta gönderme işlemini gerçekleştirin
                MailMessage message = new MailMessage(from, to, subject, body);

                // Gmail SMTP sunucusu için gerekli ayarlar
                SmtpClient client = new SmtpClient("smtp.office365.com", 587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(db.mail, db.mailPassword);

                try
                {
                    client.Send(message);
                    return result= "Mail Başarıyla Gönderildi!";

                }
                catch (Exception ex)
                {
                    return result = "Mail Gönderilirken Hata Oluştu!" + ex;
                }
            }
            else
            {
                return result = "Randevu Olmadığı İçin Mail Gönderilmedi!";
            }
        }
        public bool InternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("https://randevu.nvi.gov.tr/"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public void appStart(object sender, ElapsedEventArgs e)
        {
        restart:
            if (InternetConnection())
            {
                try
                {
                    if (randevuPageInput() == db.randevuUrl)
                    {
                        if (randevuPageInputProcess() == db.userDataUrl)
                        {
                            if (userDataInputPage() == db.randevuPageUrl)
                            {
                                randevuStatusPage();
                                serviceLog.logFile(mailSendStatus + DateTime.Now);
                            }
                            else
                            {
                                chrome.Navigate().Refresh();
                                goto restart;
                            }
                        }
                        else
                        {
                            chrome.Navigate().Refresh();
                            goto restart;
                        }
                    }
                    else
                    {
                        chrome.Navigate().Refresh();
                        goto restart;
                    }
                }
                catch (Exception ex)
                {
                    serviceLog.logFile("Serviste Sorun oluştu. Hata Detayı: " + ex + DateTime.Now);
                    chrome.Navigate().Refresh();
                    goto restart;
                }
            }
        }
        public void appStartFrom()
        {
        restart:
            if (InternetConnection()) 
            {
                try
                {
                    if (randevuPageInput() == db.randevuUrl)
                    {
                        if (randevuPageInputProcess() == db.userDataUrl)
                        {
                            if (userDataInputPage() == db.randevuPageUrl)
                            {
                                randevuStatusPage();
                            }
                            else
                            {
                                chrome.Navigate().Refresh();
                                goto restart;
                            }
                        }
                        else
                        {
                            chrome.Navigate().Refresh();
                            goto restart;
                        }
                    }
                    else
                    {
                        chrome.Navigate().Refresh();
                        goto restart;
                    }
                }
                catch (Exception)
                {
                    chrome.Navigate().Refresh();
                    goto restart;
                }
            }
        }
    }
}
