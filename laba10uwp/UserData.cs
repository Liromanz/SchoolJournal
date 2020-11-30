using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba10uwp
{
    [Serializable]
    public class UserData
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int AccountType { get; set; }
        public string DisciplineType { get; set; }

        public UserData(string login, string password, string name, string surname, int acctype, string disctype)
        {
            this.Login = login;
            this.Password = password;
            this.Name = name;
            this.Surname = surname;
            this.AccountType = acctype;
            this.DisciplineType = disctype;
        }
        public UserData() { }
    }
}
