using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Globalization;


namespace FinPlanWeb.Models
{
    public class User
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember this computer")]
        public bool RememberMe { get; set; }


        /// This part checks if the user with password is existing within the database 
        /// Taking the input of email address and password
        /// return if the data is existed or not
        
        
     public bool isValid(string _username, string _password)

        ///Establishing a connection db
      {
        using (var connection = new SqlConnection(@"Data Source=(LocalDB)\v11.0;" + 
                                @"AttachDbFilename=C:\Users\Pomme\Documents\GitHub\" +
                                @"FinPlanWeb\FinPlanWeb\App_Data\Database1.mdf;Integrated Security=True"))


            {
                string _sql = @"SELECT [Username] FROM [dbo].[users] WHERE [Username] = @u AND [Password] = @p";
                var cmd = new SqlCommand(_sql, connection);
                cmd.Parameters
                        .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                        .Value = _username;
                cmd.Parameters
                        .Add(new SqlParameter("@p", SqlDbType.NVarChar))
                        .Value = Helpers.SHA1.Encode(_password);
                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }

            }
        }

    }

    /// <summary>
    /// Model for registration process
    /// </summary>
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set;}

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        [RegularExpression(@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$", ErrorMessage = "Invalid Email")]
        public string EmailAddress { get; set; }

        [Required (ErrorMessage="The {0} field is required!")]
        [StringLength(100, ErrorMessage="The {0} must be at least {2} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "RetypePassword")]
        [Compare("Password", ErrorMessage="The both password do not matced.")]
        public string ConfirmPassword { get; set;}

        /// <summary>
        /// Calling the connection configuration
        /// </summary>
        /// <returns>The conection string</returns>
        public string getConnection()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void ExecuteInsert(string Username, string Password, string Email)
        {
            SqlConnection conn = new SqlConnection(getConnection());
            string _sql = "INSERT INTO dbo.users (Username, Password, Email) VALUES (@Username, @Password, @Email)";
                
                try{
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(_sql, conn);
                    cmd.Parameters
                       .Add(new SqlParameter("@Username", SqlDbType.NVarChar))
                       .Value = Username;
                    cmd.Parameters
                       .Add(new SqlParameter("@Password", SqlDbType.NVarChar))
                       .Value = Helpers.SHA1.Encode(Password); ;
                    cmd.Parameters
                       .Add(new SqlParameter("@Email", SqlDbType.NVarChar))
                       .Value = Email;

                    cmd.ExecuteNonQuery();
                }
                catch(System.Data.SqlClient.SqlException ex)
                {
                    string msg = "Insert errors";
                    msg += ex.Message;
                    throw new Exception(msg);

                }
                finally
                {
                    conn.Close();
                }
        }

        protected void btn_CreateUser(object sender, EventArgs e)
        {
            if (TxtPassword.Text == txtRePassword.Text)
            {
                ExecuteInsert(TxtUsername.Text, TxtPassword.Text, TxtEmail.Text);
            }
            else
            {
                TxtPassword.Focus();
            }
        }
        
    }
 }

