using RWGame.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace RWGame.Models
{
    class RegistrationPageModel
    {

    }

    public class NewUserProfile : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string LastName { get; set; } = "Default";
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool AcceptTerms { get; set; } = false;
        public bool AcceptPrivacy { get; set; } = false;
        public DateTime Birthday { get; set; } = DateTime.Now;

        private readonly Dictionary<bool, string> CorrectnessImages = 
            new Dictionary<bool, string> { { false, "no.png" }, { true, "yes.png" } };
        private string GetCorrectImage(bool? IsCorrect)
        {
            if (IsCorrect is null) return null;
            else return CorrectnessImages[IsCorrect.Value];
        }
        private readonly ServerWorker serverWorker;

        public string NameError { get; set; } = " ";
        public bool? NameIsCorrect { get; set; } = null;
        public string NameCorrectnessImage 
        {
            get { return GetCorrectImage(NameIsCorrect); } 
        }

        public string EmailError { get; set; } = " ";
        public bool? EmailIsCorrect { get; set; } = null;
        public string EmailCorrectnessImage
        {
            get { return GetCorrectImage(EmailIsCorrect); }
        }

        public string LoginError { get; set; } = " ";
        public bool? LoginIsCorrect { get; set; } = null;
        public string LoginCorrectnessImage
        {
            get { return GetCorrectImage(LoginIsCorrect); }
        }


        public string PasswordError { get; set; } = " ";
        public bool? PasswordIsCorrect { get; set; } = null;
        public string PasswordCorrectnessImage
        {
            get { return GetCorrectImage(PasswordIsCorrect); }
        }

        public string ConfirmPasswordError { get; set; } = " ";
        public bool? ConfirmPasswordIsCorrect { get; set; } = null;
        public string ConfirmPasswordCorrectnessImage
        {
            get { return GetCorrectImage(ConfirmPasswordIsCorrect); }
        }

        public void CheckNameCorrectness()
        {
            if (Name != null)
            {
                Name = Name.Trim();
                if (Name.Length < 1)
                {
                    NameError = "Name should be at least 1 character long";
                    NameIsCorrect = false;
                }
                else if (Name.Length > 255)
                {
                    NameError = "Name should be no longer than 255 characters";
                    NameIsCorrect = false;
                }
                else if (!Regex.IsMatch(Name, @"^[\p{L}]+(([',. -][\p{L} ])?[\p{L}]*)*$", RegexOptions.CultureInvariant))
                {
                    NameError = "Name should be valid person name";
                    NameIsCorrect = false;
                }
                else
                {
                    NameError = " ";
                    NameIsCorrect = true;
                }
            }
            else
            {
                NameError = "Name should be at least 1 character long";
                NameIsCorrect = false;
            }
        }
        public async void CheckEmailCorrectness()
        {
            if (Email != null && Email != "")
            {
                string emailPattern = @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
                if (Email.Length > 255)
                {
                    EmailError = "Email should be no longer than 255 characters";
                    EmailIsCorrect = false;
                }
                else if (!Regex.IsMatch(Email, emailPattern, RegexOptions.CultureInvariant))
                {
                    EmailError = "Enter a valid email address";
                    EmailIsCorrect = false;
                }
                else if (!await serverWorker.TaskCheckEmail(Email))
                {
                    EmailError = "This email is already used";
                    EmailIsCorrect = false;
                }
                else
                {
                    EmailError = " ";
                    EmailIsCorrect = true;
                }
            }
            else
            {
                EmailError = "Please, enter your email";
                EmailIsCorrect = false;
            }
        }
        public async void CheckLoginCorrectness()
        {
            if (Login != null)
            {
                string loginPattern = @"^[a-zA-Z_][a-zA-Z0-9_\-\.]{1,255}$";
                Login = Login.Trim();
                if (Login.Length < 2)
                {
                    LoginError = "Login should be at least 2 charachters long";
                    LoginIsCorrect = false;
                } else if (char.IsDigit(Login[0]))
                {
                    LoginError = "Login can only start with a letter or underscore";
                    LoginIsCorrect = false;
                }
                else if (Login.Length > 255)
                {
                    LoginError = "Login should be no longer than 255 characters";
                    LoginIsCorrect = false;
                }
                else if (!Regex.IsMatch(Login, loginPattern, RegexOptions.CultureInvariant))
                {
                    LoginError = "Login should contain only latin letters, numbers and _ . -";
                    LoginIsCorrect = false;
                }
                else if (!await serverWorker.TaskCheckLogin(Login))
                {
                    LoginError = "This login is already used";
                    LoginIsCorrect = false;
                }
                else
                {
                    LoginError = " ";
                    LoginIsCorrect = true;
                }
            }
            else
            {
                LoginError = "Login cannot be empty";
                LoginIsCorrect = false;
            }
        }
        public void CheckPasswordCorrectness()
        {
            if (Password != null)
            {
                string passwordPattern = @"^[a-zA-Z0-9_\.\-\!\#\$\%\&\'\(\)\*\+\,\.\:\;\<\=\>\?\@\[\^\`\{\|\}\~\–]{6,256}$";
                if (Password.Length < 6)
                {
                    PasswordError = "Password should contain at least 6 characters";
                    PasswordIsCorrect = false;
                }
                else if (Password.Length > 255)
                {
                    PasswordError = "Password should be no longer than 255 characters";
                    PasswordIsCorrect = false;
                }
                else if (!Regex.IsMatch(Password, passwordPattern, RegexOptions.CultureInvariant))
                {
                    var match = Regex.Match(Password, 
                        passwordPattern.Substring(0, passwordPattern.Length - 1), 
                        RegexOptions.CultureInvariant);
                    if (match.Length != Password.Length)
                    {
                        PasswordError = "Password should not contain \"" + Password[match.Length] + "\"";
                    }
                    else
                    {
                        PasswordError = "Password should contain only latin letters, digits and special symbols";
                    }
                    PasswordIsCorrect = false;
                }
                else
                {
                    PasswordError = " ";
                    PasswordIsCorrect = true;
                }
            }
            else
            {
                PasswordError = "Password cannot be empty";
                PasswordIsCorrect = false;
            }
        }
        public void CheckConfirmPasswordCorrectness(bool isNullError = true)
        {
            if (ConfirmPassword != null)
            {
                if (ConfirmPassword != Password)
                {
                    ConfirmPasswordError = "Passwords don't match";
                    ConfirmPasswordIsCorrect = false;
                }
                else
                {
                    ConfirmPasswordError = " ";
                    ConfirmPasswordIsCorrect = true;
                }
            }
            else
            {
                if (isNullError)
                {
                    ConfirmPasswordError = "Passwords don't match";
                    ConfirmPasswordIsCorrect = false;
                }
            }
        }
        public NewUserProfile(ServerWorker serverWorker)
        {
            this.serverWorker = serverWorker;
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
