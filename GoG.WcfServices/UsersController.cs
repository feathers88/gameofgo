//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Security.Cryptography;
//using System.Text;
//using System.Web.Http;
//using GoG.Infrastructure.Services.Users;
//using GoG.Repository.Users;
//using System.Net.Mail;

//namespace GoG.Services.Controllers
//{
//    public class UsersController : ApiController
//    {
//        #region Data
//        readonly IUsersRepository _usersRepository = null;
//        #endregion Data

//        // Normal ctor.
//        public UsersController()
//        {
//            _usersRepository = new DbUsersRepository();
//        }

//        // Ctor for testing.
//        public UsersController(IUsersRepository usersRepository)
//        {
//            _usersRepository = usersRepository;
//        }
        
//        // POST api/users -- adds a user
//        public HttpResponseMessage Post([FromBody]NewUserRequest newUserRequest)
//        {
//            try
//            {
//                ValidateUserId(newUserRequest.UserId);
//                ValidateEmail(newUserRequest.Email);
//                ValidatePassword(newUserRequest.Password);
                
//                var encryptedPassword = Encrypt(newUserRequest.Password);

//                _usersRepository.AddUser(newUserRequest, encryptedPassword);

//                var response = Request.CreateResponse(HttpStatusCode.Created, new UserResponse());

//                // Create Location header to send back.
//                var uri = Url.Link("DefaultApi", new { controller = "Users", id = newUserRequest.UserId });
//                response.Headers.Location = new Uri(uri);
                
//                return response;
//            }
//            catch (UsersException uex)
//            {
//                // Pass on special code to client.
//                // TODO: Add logging code.
//                return Request.CreateErrorResponse(HttpStatusCode.NoContent, uex.Code.ToString());
//            }
//            catch (Exception ex)
//            {
//                // Let web.config determine what info is passed down to client (CustomErrors on or off).
//                // TODO: Add logging code.
//                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
//                throw;
//            }
//        }


//        //// PUT api/users -- update a user
//        //public HttpResponseMessage Put([FromBody]UpdateUserRequest newUserRequest)
//        //{
//        //    // We should structure all methods like this...
//        //    try
//        //    {
//        //        // Attempt to update the user to the repository.
//        //        var repo = new FakeUserRepository();
//        //        repo.UpdateUser(newUserRequest);
//        //        // Success.
//        //        return Request.CreateResponse(HttpStatusCode.OK, new UserResponse());
//        //    }
//        //    catch (UsersException uex)
//        //    {
//        //        // Pass on special code to client.
//        //        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, uex.Code.ToString());
//        //        throw;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        // Let web.config determine what info is passed down to client (CustomErrors on or off).
//        //        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
//        //        throw;
//        //    }
//        //}

//        ////// GET api/users -- all user
//        //public HttpResponseMessage Get()
//        //{
//        //    // We should structure all methods like this...
//        //    try
//        //    {
//        //        // Attempt to update the user to the repository.
//        //        var repo = new FakeUserRepository();
//        //        List<NewUserRequest> userList = repo.GetAllUser();
//        //        // Success.  
//        //        foreach (var u in userList)
//        //        {
//        //            Console.WriteLine("{UserId}  {Email}", u.UserId, u.Email);
//        //        }
//        //        return Request.CreateResponse(HttpStatusCode.OK, new UserResponse());
//        //    }
//        //    catch (UsersException uex)
//        //    {
//        //        // Pass on special code to client.
//        //        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, uex.Code.ToString());
//        //        throw;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        // Let web.config determine what info is passed down to client (CustomErrors on or off).
//        //        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
//        //        throw;
//        //    }

//        //}


//        ////// DELETE api/users -- delete user by userId
//        //public HttpResponseMessage Delete(string userId)
//        //{
//        //    // We should structure all methods like this...
//        //    try
//        //    {
//        //        // Attempt to delete the user to the repository.
//        //        var repo = new FakeUserRepository();
//        //        repo.DeleteUser(userId);
//        //        // Success.
//        //        return Request.CreateResponse(HttpStatusCode.OK, new UserResponse());
//        //    }
//        //    catch (UsersException uex)
//        //    {
//        //        // Pass on special code to client.
//        //        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, uex.Code.ToString());
//        //        throw;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        // Let web.config determine what info is passed down to client (CustomErrors on or off).
//        //        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
//        //        throw;
//        //    }
//        //}

//        ////// Reset Password 
//        //public HttpResponseMessage PutResetPassword(string oldPassword, string newPassword)
//        //{
//        //    // We should structure all methods like this...
//        //    try
//        //    {
//        //        // Attempt to delete the user to the repository.
//        //        var repo = new FakeUserRepository();
//        //        repo.ResetPassword(oldPassword, newPassword);
//        //        // Success.
//        //        return Request.CreateResponse(HttpStatusCode.OK, new UserResponse());
//        //    }
//        //    catch (UsersException uex)
//        //    {
//        //        // Pass on special code to client.
//        //        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, uex.Code.ToString());
//        //        throw;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        // Let web.config determine what info is passed down to client (CustomErrors on or off).
//        //        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
//        //        throw;
//        //    }
//        //}

//        ////// GET api/users -- Forgot Password
//        //public HttpResponseMessage GetForgotPassword(string email)
//        //{
//        //    // We should structure all methods like this...
//        //    try
//        //    {
//        //        // Attempt to update the user to the repository.
//        //        var repo = new FakeUserRepository();
//        //        var userpassword = repo.GetForgotPass(email);
//        //        // Success.
//        //        // send mail user                  
//        //        SendMailtoUser(userpassword, email);
//        //        return Request.CreateResponse(HttpStatusCode.OK, new UserResponse());
//        //    }
//        //    catch (UsersException uex)
//        //    {
//        //        // Pass on special code to client.
//        //        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, uex.Code.ToString());
//        //        throw;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        // Let web.config determine what info is passed down to client (CustomErrors on or off).
//        //        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
//        //        throw;
//        //    }

//        //}

//        //#region Private Helpers

//        ///// <summary>
//        ///// mail to user
//        ///// </summary>
//        ///// <param name="userpassword"></param>
//        ///// <param name="email"></param>
//        //private void SendMailtoUser(string userpassword, string email)
//        //{
//        //    try
//        //    {
//        //        string strMailBody = "";
//        //        string strToEmailAddress = email;
//        //        string strFromMailAddress = "nikhilsharmaece2011@gmail.com";
//        //        string strBccMail = "rahulmathur06@gmail.com";
//        //        string strSubject = "Forgot password";

//        //        //Creating Mail
//        //        strMailBody = strMailBody + "<html><head><style>.label {";
//        //        strMailBody = strMailBody + "PADDING-LEFT: 2px; FONT-WEIGHT: 400;FONT-SIZE: 10px;";
//        //        strMailBody = strMailBody + "COLOR: #000; FONT-STYLE: normal;";
//        //        strMailBody = strMailBody + "FONT-FAMILY: Verdana, Arial, Helvetica, sans-serif;";
//        //        strMailBody = strMailBody + "TEXT-DECORATION: none; text-align:left;";
//        //        strMailBody = strMailBody + "vertical-align:middle; } </style></head><body>";
//        //        strMailBody = strMailBody + "            <div ><h2 > Forgot password</h2><div ><div> <div>";
//        //        strMailBody = strMailBody + "                             <table cellpadding='5px' cellspacing='0'width='100%' >";
//        //        strMailBody = strMailBody + "                                 <tr>";
//        //        strMailBody = strMailBody + "                                     <td>";
//        //        strMailBody = strMailBody + "                                       <span > Password:</span> ";
//        //        strMailBody = strMailBody + "                                         " + userpassword + "";
//        //        strMailBody = strMailBody + "                                     </td>";
//        //        strMailBody = strMailBody + "                                 </tr>";
//        //        strMailBody = strMailBody + "                            </table>";
//        //        strMailBody = strMailBody + "</body></html>";

//        //        //Mail Send
//        //        SendMail(strFromMailAddress, strToEmailAddress, strSubject, strMailBody, strBccMail);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        throw ex;
//        //    }
//        //}

//        bool EmailIsWellFormed(string email)
//        {
//            //string pattern = @"^[a-z][a-z|0-9|]*([_][a-z|0-9]+)*([.][a-z|" + @"0-9]+([_][a-z|0-9]+)*)?@[a-z][a-z|0-9|]*\.([a-z]" + @"[a-z|0-9]*(\.[a-z][a-z|0-9]*)?)$";
//            //Match match = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
//            //return match.Success;

//            // Needs to handle non Utf-8 character sets so can't just check characters.
//            // TODO: Figure out how to verify Passwords are complex and well formed in an internationalizable way.
//            return true;
//        }

//        bool UserIdIsWellFormed(string userId)
//        {
//            // Needs to handle non Utf-8 character sets so can't just check characters.
//            // TODO: Figure out how to verify UserIds are well formed in an internationalizable way.
//            return true;
//        }

//        bool PasswordIsWellFormed(string pwd)
//        {
//            // Needs to handle non Utf-8 character sets so can't just check characters.
//            // TODO: Figure out how to verify Passwords are complex and well formed in an internationalizable way.
//            return true;
//        }

//        static readonly SHA256 Hasher = SHA256.Create();

//        /// <summary>
//        /// Encrypts a string using SHA256.
//        /// </summary>
//        byte[] Encrypt(string strInput)
//        {
//            var hashedBytes = Hasher.ComputeHash(Encoding.Unicode.GetBytes(strInput));
//            return hashedBytes;
//        }

//        private void ValidatePassword(string password)
//        {
//            if (String.IsNullOrWhiteSpace(password))
//                throw new UsersException(UserErrorCode.PasswordDoesNoteMeetRequirements, "Password was blank.");
//            if (password.Length < 8 || password.Length > 20)
//                throw new UsersException(UserErrorCode.PasswordDoesNoteMeetRequirements,
//                                         "Password length was " + password.Length + '.');
//            if (!PasswordIsWellFormed(password))
//                throw new UsersException(UserErrorCode.PasswordDoesNoteMeetRequirements,
//                                         "Password doesn't meet complexity/security requirements: " + password + '.');
//        }

//        private void ValidateEmail(string email)
//        {
//            if (String.IsNullOrWhiteSpace(email))
//                throw new UsersException(UserErrorCode.EmailAddressIsInvalid, "Email was blank.");
//            if (!EmailIsWellFormed(email))
//                throw new UsersException(UserErrorCode.EmailAddressIsInvalid,
//                                         "Email address was invalid: " + email + '.');
//        }

//        private void ValidateUserId(string userId)
//        {
//            if (String.IsNullOrWhiteSpace(userId))
//                throw new UsersException(UserErrorCode.UserIdDoesNotMeetRequrements, "UserId was blank.");
//            if (userId.Length < 8 || userId.Length > 20)
//                throw new UsersException(UserErrorCode.UserIdDoesNotMeetRequrements,
//                                         "UserId length was " + userId.Length + '.');
//            if (!UserIdIsWellFormed(userId))
//                throw new UsersException(UserErrorCode.UserIdDoesNotMeetRequrements,
//                                         "UserId doesn't meet complexity/security requirements: " + userId + '.');
//        }

//        private static void SendMail(string strFrom, string strTo, string strSubject, string strBody, string strBccMail)
//        {
//            try
//            {
//                SmtpClient objSmtpClient = new SmtpClient();
//                objSmtpClient.Host = "smtp.gmail.com";
//                objSmtpClient.Port = 587;
//                objSmtpClient.EnableSsl = true;
//                objSmtpClient.Credentials = new System.Net.NetworkCredential("nikhilsharmaece2011", "9694584584");
//                // Create instance of mail message class.
//                MailMessage objMailMessage = new MailMessage();
//                MailAddress objstrFrom = new MailAddress(strFrom);
//                objMailMessage.From = objstrFrom;
//                objMailMessage.To.Add(strTo);
//                objMailMessage.Subject = strSubject;
//                objMailMessage.Body = strBody;
//                //Add BCC mail if required.
//                if (strBccMail != "")
//                    objMailMessage.Bcc.Add(strBccMail);
//                //Set the mail format to html by default.
//                objMailMessage.IsBodyHtml = true;
//                //Send mail.
//                objSmtpClient.Send(objMailMessage);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }


//        //// GET api/values/5
//        //public string Get(int id)
//        //{
//        //    return "value";
//        //}




//    }

//}
