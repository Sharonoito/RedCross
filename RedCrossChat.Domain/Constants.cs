using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RedCrossChat.Domain
{
    public class Constants
    {
        // Great Future Solutions Now
        public const string Sitename = "GFSN Credit Rating";
        public const string PoweredBy = "Powered By Pathways Intl'";
        public const string ValidationError = "Validation error!, please check your data.";
        public static readonly string SuperAdministratorId = "bccf3508-7141-4ac4-ae56-aed1a3b58bb3";
        public static readonly string AdministratorId = "f1ca201c-9ea4-4227-9b6c-d448673f7fad";
        public static string DefaultCompanyId = "c5b8841f-15f6-4131-1b4f-08da761e76e2";

       
        //public static Guid DefaultCompanyId = Guid.Parse("c5b8841f-15f6-4131-1b4f-08da761e76e2");

    
        public static readonly string AppRootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        public static readonly string UploadsFolderName = "uploads";


        public static string CompanyName = "GFSN Credit Rating";


        #region ROLES
        /// <summary>
        /// Self Registered clients role
        /// </summary>
        public static string SuperAdministrator = "Super Administrator";
        public static string Administrator = "Administrator";
        public static string ClientRole = "Client";
        public static string SelfRegisteredClientRole = "Client Self";
        public static string PssAgent = "Pss Agent";
        public static string PssManager = "Pss Manager";
        public static string AuditRole = "Audit Roles";
        public static string DefaultSuperAdminEmail = "admin@redcross.com";

        #endregion
    }
}
