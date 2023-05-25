using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PassaportServices
{
    public partial class Service1 : ServiceBase
    {
        SeleniumBusiness sb = new SeleniumBusiness();
        ServiceLogFile serviceLog= new ServiceLogFile();
 
        Timer _timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            serviceLog.logFile("Servis Başlatıldı: " + DateTime.Now);
            _timer.Elapsed += new ElapsedEventHandler(sb.appStart);
            _timer.Interval = 1000; // yarım saat (30 dakika) aralıkta
            _timer.Enabled = true;
        }

        protected override void OnStop()
        {
            serviceLog.logFile("Servis Durdu!: " + DateTime.Now );
            _timer.Enabled = false;
            _timer.Dispose();
        }
         
    }
}
