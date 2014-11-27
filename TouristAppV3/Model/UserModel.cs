using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouristAppV3.Model
{
    class UserModel
    {
        private string _userName;
        private string _password;

        public String UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public String Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public override string ToString()
        {
            return UserName.ToString();
        }
    }
}
