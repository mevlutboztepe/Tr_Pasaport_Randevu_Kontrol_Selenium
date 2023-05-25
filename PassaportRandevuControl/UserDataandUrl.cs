using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassaportServices
{
    internal class UserDataandUrl
    {
        //Page URL
        public string randevuUrl = "https://randevu.nvi.gov.tr/";
        public string userDataUrl = "https://randevu.nvi.gov.tr/default/index?type=3";
        public string randevuPageUrl = "https://randevu.nvi.gov.tr/default/step2";
        //User İnfo
        public string userName = "Name";
        public string userSurname = "Surname";
        public long identificationNumberTC = 111111111;
        public int dateOfBirthDay = 12;
        public int dateOfBirthMonth = 12;
        public int dateOfBirthYear = 1990;
        public long telephoneNumber = 5385666666;
        public string mail = "mailadress@hotmail.com";
        public string mailPassword = "mailpassword";
        public string[] date = { "17.04.2023", "18.04.2023", "19.04.2023", "25.05.2023" };
    }
}
