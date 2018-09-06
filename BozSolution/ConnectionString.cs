using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using PropertyAttributes = System.Data.PropertyAttributes;

namespace BizSolution
{
    public static class  ConnectionString
    {
//        private static string 

        public static string GetStringConnection => "Data Source = BABOO;" +
                                              "Initial Catalog=person;" +
                                              "User id=sa;" +
                                              "Password=123456;";
    }
}