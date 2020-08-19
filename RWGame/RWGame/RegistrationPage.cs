using RWGame.Classes;
using System;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Essentials;
using Xamarin.Forms;
using System.Text.RegularExpressions;


namespace RWGame
{
    public class RegistrationPage : ContentPage
    {
        private readonly ServerWorker localServerWorker;
        private readonly ObservableCollection<bool> isFieldsCorrect = new ObservableCollection<bool>(new bool[9]);
        public RegistrationPage(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            localServerWorker = serverWorker;
            this.BackgroundColor = Color.FromHex("#39bafa");
            NavigationPage.SetHasNavigationBar(this, false);
            StackLayout HeadStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#39bafa"),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.Fill,
                Spacing = 0
            };
            Label HeadLabel = new Label()
            {
                Text = "Registration",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromHex("#39bafa"),
                Margin = new Thickness(15, 0, 15, 0)
            };
            HeadStack.Children.Add(HeadLabel);

            Thickness LabelMargin = new Thickness(15, 0, 10, 0);

            //Визуализация поля имени
            #region Name
            StackLayout nameStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#39bafa"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 0),
                Spacing = 0
            };
            Label nameLabel = new Label()
            {
                Text = "Your name",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = Color.FromHex("#39bafa"),
                Opacity = 0,
            };
            Label nameTipLabel = new Label()
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.Red,
                FontAttributes = FontAttributes.None,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#39bafa"), //TODO fix color
                Margin = new Thickness(15, 0, 0, 0),
                Opacity = 0,
            };
            Image labelRightImage = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            StackLayout nameLabelStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#39bafa"),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = LabelMargin,
                Spacing = 0
            };

            nameLabelStack.Children.Add(nameLabel);
            nameLabelStack.Children.Add(labelRightImage);

            Entry nameEntry = new Entry()
            {
                Placeholder = nameLabel.Text,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#39bafa"),
                Margin = new Thickness(10, 0, 10, 0),
                Keyboard = Keyboard.Text
            };
            nameEntry.TextChanged += delegate
            {
                nameLabel.Opacity = nameEntry.Text == "" ? 0 : 1;
            };
            nameEntry.Unfocused += delegate
            {
                if (nameEntry != null && nameEntry.Text != null)
                {
                    if (Regex.IsMatch(nameEntry.Text, @"^\s*[\p{L}]+(([',. -][\p{L} ])?[\p{L}]*)*\s*$", RegexOptions.CultureInvariant))
                    {
                        isFieldsCorrect[0] = true;
                        labelRightImage.HeightRequest = nameLabel.Height;
                        labelRightImage.Source = "yes.png";
                        nameTipLabel.Opacity = 0;
                        nameEntry.Text = nameEntry.Text.Trim();
                    }
                    else if (nameEntry.Text.Length < 1)
                    {
                        isFieldsCorrect[0] = false;
                        labelRightImage.HeightRequest = nameLabel.Height;
                        labelRightImage.Source = "no.png";
                        nameTipLabel.Text = "Name should be at least 1 character long";
                        nameTipLabel.Opacity = 1;
                    }
                    else if (nameEntry.Text.Length > 256)
                    {
                        isFieldsCorrect[0] = false;
                        labelRightImage.HeightRequest = nameLabel.Height;
                        labelRightImage.Source = "no.png";
                        nameTipLabel.Text = "Name should be at less than 256 characters long";
                        nameTipLabel.Opacity = 1;
                    }
                    else
                    {
                        isFieldsCorrect[0] = false;
                        labelRightImage.HeightRequest = nameLabel.Height;
                        labelRightImage.Source = "no.png";
                        nameTipLabel.Text = "Name should contain only letters";
                        nameTipLabel.Opacity = 1;
                    }
                }
                else
                {
                    isFieldsCorrect[0] = false;
                    labelRightImage.HeightRequest = nameLabel.Height;
                    labelRightImage.Source = "no.png";
                    nameTipLabel.Text = "Name should be at least 1 character long";
                    nameTipLabel.Opacity = 1;
                }
            };
            nameStack.Children.Add(nameLabelStack);
            nameStack.Children.Add(nameEntry);
            nameStack.Children.Add(nameTipLabel);
            #endregion


            //Визуализация поля фамилия
            #region Surname
            StackLayout surnameStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#38b6f5"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 0),
                Spacing = 0,
            };
            Label surnameLabel = new Label()
            {
                Text = "Your surname",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = Color.FromHex("#38b6f5"),
                Opacity = 0,
            };
            Label surnameTipLabel = new Label()
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.Red,
                FontAttributes = FontAttributes.None,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#38b6f5"), //TODO fix color
                Margin = new Thickness(15, 0, 0, 0),
                Opacity = 0,
            };
            Image surnameRightImage = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            StackLayout surnameLabelStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#38b6f5"),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = LabelMargin
            };
            surnameLabelStack.Children.Add(surnameLabel);
            surnameLabelStack.Children.Add(surnameRightImage);
            Entry surnameEntry = new Entry()
            {
                Placeholder = surnameLabel.Text,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#38b6f5"),
                Margin = new Thickness(10, 0, 10, 0),
                Keyboard = Keyboard.Text,
            };
            surnameEntry.TextChanged += delegate
            {
                surnameLabel.Opacity = surnameEntry.Text == "" ? 0 : 1;
            };
            surnameEntry.Unfocused += delegate
            {
                if (surnameEntry != null && surnameEntry.Text != null)
                {
                    if (Regex.IsMatch(surnameEntry.Text, @"^\s*[\p{L}]+(([',. -][\p{L} ])?[\p{L}]*)*\s*$", RegexOptions.CultureInvariant))
                    {
                        isFieldsCorrect[1] = true;
                        surnameRightImage.HeightRequest = surnameLabel.Height;
                        surnameRightImage.Source = "yes.png";
                        surnameTipLabel.Opacity = 0;
                        surnameEntry.Text = surnameEntry.Text.Trim();
                    }
                    else if (surnameEntry.Text.Length < 1)
                    {
                        isFieldsCorrect[1] = false;
                        surnameRightImage.HeightRequest = surnameLabel.Height;
                        surnameRightImage.Source = "no.png";
                        surnameTipLabel.Text = "Surname should be at least 1 character long";
                        surnameTipLabel.Opacity = 1;
                    }
                    else if (surnameEntry.Text.Length > 256)
                    {
                        isFieldsCorrect[1] = false;
                        surnameRightImage.HeightRequest = surnameLabel.Height;
                        surnameRightImage.Source = "no.png";
                        surnameTipLabel.Text = "Surname should be less than 256 characters long";
                        surnameTipLabel.Opacity = 1;
                    }
                    else
                    {
                        isFieldsCorrect[1] = false;
                        surnameRightImage.HeightRequest = surnameLabel.Height;
                        surnameRightImage.Source = "no.png";
                        surnameTipLabel.Text = "Surname should contain only letters";
                        surnameTipLabel.Opacity = 1;
                    }
                }
                else
                {
                    isFieldsCorrect[1] = false;
                    surnameRightImage.HeightRequest = surnameLabel.Height;
                    surnameRightImage.Source = "no.png";
                    surnameTipLabel.Text = "Surname should be at least 1 character long";
                    surnameTipLabel.Opacity = 1;
                }
            };
            surnameStack.Children.Add(surnameLabelStack);
            surnameStack.Children.Add(surnameEntry);
            surnameStack.Children.Add(surnameTipLabel);
            #endregion


            //Визуализация поля login
            #region Login
            StackLayout loginStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#39b1ed"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 0),
                Spacing = 0,
            };
            Label loginLabel = new Label()
            {
                Text = "Login",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = Color.FromHex("#39b1ed"),
                Opacity = 0,
            };
            Label loginTipLabel = new Label()
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.Red,
                FontAttributes = FontAttributes.None,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#39b1ed"), //TODO fix color
                Margin = new Thickness(15, 0, 0, 0),
                Opacity = 0,
            };
            Image loginRightImage = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            StackLayout loginLabelStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#39b1ed"),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = LabelMargin
            };
            loginLabelStack.Children.Add(loginLabel);
            loginLabelStack.Children.Add(loginRightImage);
            Entry loginEntry = new Entry()
            {
                Placeholder = loginLabel.Text,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#39b1ed"),
                Margin = new Thickness(10, 0, 10, 0),
                Keyboard = Keyboard.Text
            };
            loginEntry.TextChanged += delegate
            {
                loginLabel.Opacity = loginEntry.Text == "" ? 0 : 1;
            };
            loginEntry.Unfocused += async delegate
            {
                if (loginEntry != null && loginEntry.Text != null)
                {
                    loginEntry.Text = loginEntry.Text.Trim();
                    if (Regex.IsMatch(loginEntry.Text, @"^[a-zA-Z_][a-zA-Z0-9_\-\.]{1,255}$", RegexOptions.CultureInvariant))
                    {
                        if (await localServerWorker.TaskCheckLogin(loginEntry.Text))
                        {
                            isFieldsCorrect[2] = true;
                            loginRightImage.HeightRequest = loginLabel.Height;
                            loginRightImage.Source = "yes.png";
                            loginTipLabel.Opacity = 0;
                        } 
                        else
                        {
                            isFieldsCorrect[2] = false;
                            loginRightImage.HeightRequest = loginLabel.Height;
                            loginRightImage.Source = "no.png";
                            loginTipLabel.Text = "This login is already used";
                            loginTipLabel.Opacity = 1;
                        }
                    }
                    else if (loginEntry.Text.Length < 2)
                    {
                        isFieldsCorrect[2] = false;
                        loginRightImage.HeightRequest = loginLabel.Height;
                        loginRightImage.Source = "no.png";
                        loginTipLabel.Text = "Login should be at least 2 charachters long";
                        loginTipLabel.Opacity = 1;
                    }
                    else if (loginEntry.Text.Length > 255)
                    {
                        isFieldsCorrect[2] = false;
                        loginRightImage.HeightRequest = loginLabel.Height;
                        loginRightImage.Source = "no.png";
                        loginTipLabel.Text = "Login should be less than 255 charachters long";
                        loginTipLabel.Opacity = 1;
                    }
                    else if (Regex.IsMatch(loginEntry.Text[0].ToString(), @"^[0-9]{1,255}$", RegexOptions.CultureInvariant))
                    {
                        isFieldsCorrect[2] = false;
                        loginRightImage.HeightRequest = loginLabel.Height;
                        loginRightImage.Source = "no.png";
                        loginTipLabel.Text = "Login can't start with a digit";
                        loginTipLabel.Opacity = 1;
                    }
                    else
                    {
                        isFieldsCorrect[2] = false;
                        loginRightImage.HeightRequest = loginLabel.Height;
                        loginRightImage.Source = "no.png";
                        loginTipLabel.Text = "Login should conatin only latin letters, numbers and _ . -";
                        loginTipLabel.Opacity = 1;
                    }
                }
                else
                {
                    isFieldsCorrect[2] = false;
                    loginRightImage.HeightRequest = loginLabel.Height;
                    loginRightImage.Source = "no.png";
                    loginTipLabel.Text = "Login should be at least 2 characters long";
                    loginTipLabel.Opacity = 1;
                }
            };
            loginStack.Children.Add(loginLabelStack);
            loginStack.Children.Add(loginEntry);
            loginStack.Children.Add(loginTipLabel);
            #endregion


            //Визуализация поля password
            #region Password
            StackLayout passwordStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#38ade8"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 0),
                Spacing = 0,
            };
            Label passwordLabel = new Label()
            {
                Text = "Password",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = Color.FromHex("#38ade8"),
                Opacity = 0,
            };
            Label passwordTipLabel = new Label()
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.Red,
                FontAttributes = FontAttributes.None,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#38ade8"), //TODO fix color
                Margin = new Thickness(15, 0, 0, 0),
                Opacity = 0,
            };
            Image passwordRightImage = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            StackLayout passwordLabelStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#38ade8"),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = LabelMargin
            };
            passwordLabelStack.Children.Add(passwordLabel);
            passwordLabelStack.Children.Add(passwordRightImage);
            Entry passwordEntry = new Entry()
            {
                Placeholder = passwordLabel.Text,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#38ade8"),
                IsPassword = true,
                Margin = new Thickness(10, 0, 10, 0)
            };
            Label passwordConfirmLabel = new Label()
            {
                Text = "Confirm your password",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = Color.FromHex("#38ade8"),
                Opacity = 0,
            };
            Label passwordConfirmTipLabel = new Label()
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.Red,
                FontAttributes = FontAttributes.None,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#38ade8"), //TODO fix color
                Margin = new Thickness(15, 0, 0, 0),
                Opacity = 0,
            };
            Image passwordConfirmRightImage = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            StackLayout passwordConfirmLabelStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#38ade8"),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = LabelMargin
            };
            passwordConfirmLabelStack.Children.Add(passwordConfirmLabel);
            passwordConfirmLabelStack.Children.Add(passwordConfirmRightImage);
            Entry passwordConfirmEntry = new Entry()
            {
                Placeholder = passwordConfirmLabel.Text,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#38ade8"),
                IsPassword = true,
                Margin = new Thickness(10, 0, 10, 0)
            };
            passwordEntry.TextChanged += delegate
            {
                passwordLabel.Opacity = passwordEntry.Text == "" ? 0 : 1;
            };
            passwordEntry.Unfocused += delegate
            {
                if (passwordEntry != null && passwordEntry.Text != null)
                {
                    if (Regex.IsMatch(passwordEntry.Text, @"^[a-zA-Z0-9_\.\-\!\#\$\%\&\'\(\)\*\+\,\.\:\;\<\=\>\?\@\[\^\`\{\|\}\~\–]{6,256}$", RegexOptions.CultureInvariant))
                    {
                        isFieldsCorrect[3] = true;
                        passwordRightImage.HeightRequest = passwordLabel.Height;
                        passwordRightImage.Source = "yes.png";
                        passwordTipLabel.Opacity = 0;
                        passwordConfirmTipLabel.Text = "Confirm your password";
                        passwordConfirmTipLabel.Opacity = 1;
                        if (passwordConfirmEntry != null && passwordConfirmEntry.Text != null)
                        {
                            if (passwordConfirmEntry.Text == passwordEntry.Text)
                            {
                                isFieldsCorrect[4] = true;
                                passwordConfirmRightImage.HeightRequest = passwordConfirmLabel.Height;
                                passwordConfirmRightImage.Source = "yes.png";
                                passwordConfirmTipLabel.Opacity = 0;
                                passwordConfirmRightImage.Opacity = 1;
                            }
                            else
                            {
                                isFieldsCorrect[4] = false;
                                passwordConfirmRightImage.HeightRequest = passwordConfirmLabel.Height;
                                passwordConfirmRightImage.Source = "no.png";
                                passwordConfirmTipLabel.Text = "Passwords don't match";
                                passwordConfirmTipLabel.Opacity = 1;
                                passwordConfirmRightImage.Opacity = 1;
                            }
                        }
                        else
                        {
                            isFieldsCorrect[4] = false;
                            passwordConfirmRightImage.HeightRequest = passwordConfirmLabel.Height;
                            passwordConfirmRightImage.Source = "no.png";
                            passwordConfirmTipLabel.Text = "Confirm your password";
                            passwordConfirmTipLabel.Opacity = 1;
                            passwordConfirmRightImage.Opacity = 1;
                        }
                    }
                    else if (passwordEntry.Text.Length < 6)
                    {
                        isFieldsCorrect[3] = false;
                        passwordRightImage.HeightRequest = passwordLabel.Height;
                        passwordRightImage.Source = "no.png";
                        passwordTipLabel.Text = "Password should contain at least 6 characters";
                        passwordTipLabel.Opacity = 1;
                        passwordConfirmTipLabel.Opacity = 0;
                        passwordConfirmRightImage.Opacity = 0;
                    }
                    else if (passwordEntry.Text.Length > 256)
                    {
                        isFieldsCorrect[3] = false;
                        passwordRightImage.HeightRequest = passwordLabel.Height;
                        passwordRightImage.Source = "no.png";
                        passwordTipLabel.Text = "Password should contain at least 6 characters";
                        passwordTipLabel.Opacity = 1;
                        passwordConfirmTipLabel.Opacity = 0;
                        passwordConfirmRightImage.Opacity = 0;
                    }
                    else
                    {
                        isFieldsCorrect[3] = false;
                        passwordRightImage.HeightRequest = passwordLabel.Height;
                        passwordRightImage.Source = "no.png";
                        bool foundMistake = false;

                        for (int i = 0; i < passwordEntry.Text.Length; ++i)
                        {
                            if (!Regex.IsMatch(passwordEntry.Text[i].ToString(), @"^[a-zA-Z0-9_\.\-\!\#\$\%\&\'\(\)\*\+\,\.\:\;\<\=\>\?\@\[\^\`\{\|\}\~\–]$", RegexOptions.CultureInvariant))
                            {
                                passwordTipLabel.Text = "Password should not contain \"" + passwordEntry.Text[i].ToString() + "\"";
                                foundMistake = true;
                                break;
                            }
                        }

                        if (!foundMistake)
                            passwordTipLabel.Text = "Password should contain at only latin letters, digits and special symbols";
                        passwordTipLabel.Opacity = 1;
                        passwordConfirmTipLabel.Opacity = 0;
                        passwordConfirmRightImage.Opacity = 0;
                    }
                }
                else
                {
                    isFieldsCorrect[3] = false;
                    passwordRightImage.HeightRequest = passwordLabel.Height;
                    passwordRightImage.Source = "no.png";
                    passwordTipLabel.Text = "Password should contain at least 6 characters";
                    passwordTipLabel.Opacity = 1;
                    passwordConfirmTipLabel.Opacity = 0;
                    passwordConfirmRightImage.Opacity = 0;
                }
            };
            passwordConfirmEntry.TextChanged += delegate
            {
                passwordConfirmLabel.Opacity = passwordConfirmEntry.Text == "" ? 0 : 1;
            };
            passwordConfirmEntry.Unfocused += delegate
            {
                if (passwordConfirmEntry != null && passwordConfirmEntry.Text != null && passwordEntry != null && passwordEntry.Text != null && isFieldsCorrect[3] == true)
                {
                    if (passwordConfirmEntry.Text == passwordEntry.Text)
                    {
                        isFieldsCorrect[4] = true;
                        passwordConfirmRightImage.HeightRequest = passwordConfirmLabel.Height;
                        passwordConfirmRightImage.Source = "yes.png";
                        passwordConfirmTipLabel.Opacity = 0;
                    }
                    else
                    {
                        isFieldsCorrect[4] = false;
                        passwordConfirmRightImage.HeightRequest = passwordConfirmLabel.Height;
                        passwordConfirmRightImage.Source = "no.png";
                        passwordConfirmTipLabel.Text = "Passwords don't match";
                        passwordConfirmTipLabel.Opacity = 1;
                    }
                }
                else
                {
                    isFieldsCorrect[4] = false;
                    passwordConfirmRightImage.HeightRequest = passwordConfirmLabel.Height;
                    passwordConfirmRightImage.Source = "no.png";
                    passwordConfirmTipLabel.Text = "Confirm your password";
                    passwordConfirmTipLabel.Opacity = 1;
                }
            };

            passwordStack.Children.Add(passwordLabelStack);
            passwordStack.Children.Add(passwordEntry);
            passwordStack.Children.Add(passwordTipLabel);
            passwordStack.Children.Add(passwordConfirmLabelStack);
            passwordStack.Children.Add(passwordConfirmEntry);
            passwordStack.Children.Add(passwordConfirmTipLabel);
            #endregion


            //Визуализация поля date
            #region Birthday
            StackLayout dateStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#39aae3"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 0),
                Spacing = 0,
            };
            Label dateLabel = new Label()
            {
                Text = "Birthday",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = Color.FromHex("#39aae3"),
                Opacity = 1,
            };
            Image dateRightImage = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            StackLayout dateLabelStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#39aae3"),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = LabelMargin
            };
            dateLabelStack.Children.Add(dateLabel);
            dateLabelStack.Children.Add(dateRightImage);
            DatePicker datePicker = new DatePicker
            {
                Format = "d",
                TextColor = Color.White,
                MaximumDate = DateTime.Now,
                Date = DateTime.Now,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(10, 0, 10, 0),
            };
            //datePicker.PropertyChanged += delegate
            //{
            //    dateLabel.Opacity = datePicker.Date == "" ? 0 : 1;
            //};
            datePicker.Unfocused += delegate
            {
                if (datePicker != null)
                {
                    isFieldsCorrect[5] = true;
                    dateRightImage.HeightRequest = dateLabel.Height;
                    dateRightImage.Source = "yes.png";
                }
                else
                {
                    isFieldsCorrect[5] = false;
                    dateRightImage.HeightRequest = dateLabel.Height;
                    dateRightImage.Source = "no.png";
                }
            };
            dateStack.Children.Add(dateLabelStack);
            dateStack.Children.Add(datePicker);
            #endregion


            //Визуализация поля email
            #region Email
            StackLayout emailStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#35a6de"),
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 0),
                Spacing = 0,
            };
            Label emailLabel = new Label()
            {
                Text = "Email",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = Color.FromHex("#35a6de"),
                Opacity = 0,
            };
            Label emailTipLabel = new Label()
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.Red,
                FontAttributes = FontAttributes.None,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.FromHex("#35a6de"), //TODO fix color
                Margin = new Thickness(15, 0, 0, 0),
                Opacity = 0,
            };
            Image emailRightImage = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            StackLayout emailLabelStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#35a6de"),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                Margin = LabelMargin
            };
            emailLabelStack.Children.Add(emailLabel);
            emailLabelStack.Children.Add(emailRightImage);
            Entry emailEntry = new Entry()
            {
                Placeholder = emailLabel.Text,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#35a6de"),
                Margin = new Thickness(10, 0, 10, 0)
            };
            emailEntry.TextChanged += delegate
            {
                emailLabel.Opacity = emailEntry.Text == "" ? 0 : 1;
            };
            emailEntry.Unfocused += async delegate
            {
                if (emailEntry != null && emailEntry.Text != null)
                {
                    string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
                    if (Regex.IsMatch(emailEntry.Text,
                        pattern,
                        RegexOptions.CultureInvariant))
                    {
                        if (await serverWorker.TaskCheckEmail(emailEntry.Text))
                        {
                            isFieldsCorrect[6] = true;
                            emailRightImage.HeightRequest = emailLabel.Height;
                            emailRightImage.Source = "yes.png";
                            emailTipLabel.Opacity = 0;
                        } 
                        else
                        {
                            isFieldsCorrect[6] = false;
                            emailRightImage.HeightRequest = emailLabel.Height;
                            emailRightImage.Source = "no.png";
                            emailTipLabel.Text = "This email is already used";
                            emailTipLabel.Opacity = 1;
                        }
                    }
                    else
                    {
                        isFieldsCorrect[6] = false;
                        emailRightImage.HeightRequest = emailLabel.Height;
                        emailRightImage.Source = "no.png";
                        emailTipLabel.Text = "Enter a valid email address";
                        emailTipLabel.Opacity = 1;
                    }
                }
                else
                {
                    isFieldsCorrect[6] = false;
                    emailRightImage.HeightRequest = emailLabel.Height;
                    emailRightImage.Source = "no.png";
                    emailTipLabel.Text = "Enter a valid email address";
                    emailTipLabel.Opacity = 1;
                }
            };
            emailStack.Children.Add(emailLabelStack);
            emailStack.Children.Add(emailEntry);
            emailStack.Children.Add(emailTipLabel);
            #endregion


            //Визуализация галочки и ссылки на политику
            #region Policy
            StackLayout policyStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#35a6de"),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, 0, 0, -5),
                Spacing = 0,
            };
            Label policyTextLabel = new Label()
            {
                Text = "I have read and agree to the ",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromHex("#35a6de"),
                Margin = new Thickness(5, 15, 0, 15)
            };
            Label policyHyperlinkLabel = new Label()
            {
                Text = "Privacy policy",
                TextDecorations = TextDecorations.Underline,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromHex("#35a6de"),
                Margin = new Thickness(0, 15, 15, 15)
            };

            CheckBox policyCheckBox = new CheckBox
            {
                IsChecked = false,
                BackgroundColor = Color.FromHex("#35a6de")
            };

            var policyLabelTapGestureRecognizer = new TapGestureRecognizer();
            var policyTapGestureRecognizer = new TapGestureRecognizer();

            policyTapGestureRecognizer.Tapped += async (s, e) =>
            {
                Uri uri = new Uri("https://scigames.ru/privacy_policy");
                await Launcher.OpenAsync(uri);
            };

            policyLabelTapGestureRecognizer.Tapped += (s, e) =>
            {
                policyCheckBox.IsChecked = !policyCheckBox.IsChecked;
            };

            policyCheckBox.CheckedChanged += delegate
            {
                isFieldsCorrect[7] = policyCheckBox.IsChecked;
            };

            policyTextLabel.GestureRecognizers.Add(policyLabelTapGestureRecognizer);
            policyHyperlinkLabel.GestureRecognizers.Add(policyTapGestureRecognizer);
            policyStack.Children.Add(policyCheckBox);
            policyStack.Children.Add(policyTextLabel);
            policyStack.Children.Add(policyHyperlinkLabel);

            #endregion


            //Визуализация галочки и ссылки на соглашение
            #region Agreement
            StackLayout agreementStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#35a6de"),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0, -10, 0, 0),
                Spacing = 0,
            };

            Label agreementTextLabel = new Label()
            {
                Text = "I have read and agree to the ",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromHex("#35a6de"),
                Margin = new Thickness(5, 15, 0, 15)
            };
            Label agreementHyperlinkLabel = new Label()
            {
                Text = "Terms and Conditions",
                TextDecorations = TextDecorations.Underline,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.FromHex("#35a6de"),
                Margin = new Thickness(0, 15, 0, 15)
            };

            CheckBox agreementCheckBox = new CheckBox
            {
                IsChecked = false,
                BackgroundColor = Color.FromHex("#35a6de")
            };

            var agreementTapGestureRecognizer = new TapGestureRecognizer();
            var agreementLabelTapGestureRecoginzer = new TapGestureRecognizer();
            agreementTapGestureRecognizer.Tapped += async (s, e) =>
            {
                Uri uri = new Uri("https://scigames.ru/terms");
                await Launcher.OpenAsync(uri);
            };
            agreementLabelTapGestureRecoginzer.Tapped += (s, e) =>
            {
                agreementCheckBox.IsChecked = !agreementCheckBox.IsChecked;
            };

            agreementCheckBox.CheckedChanged += delegate
            {
                isFieldsCorrect[8] = agreementCheckBox.IsChecked;
            };

            agreementTextLabel.GestureRecognizers.Add(agreementLabelTapGestureRecoginzer);
            agreementHyperlinkLabel.GestureRecognizers.Add(agreementTapGestureRecognizer);
            agreementStack.Children.Add(agreementCheckBox);
            agreementStack.Children.Add(agreementTextLabel);
            agreementStack.Children.Add(agreementHyperlinkLabel);



            #endregion


            //Визуализация кнопки регистрации и отмены
            #region Register
            StackLayout registrationStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#35a6de"),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0, 0, 0, 10),
            };
            Button registrateButton = new Button()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                Text = "Sign Up",
                BackgroundColor = Color.FromHex("#7ad3ff"),
                TextColor = Color.White,
                Margin = new Thickness(10, 0, 10, 0),
                IsEnabled = false
            };

            isFieldsCorrect.CollectionChanged += delegate
            {
                registrateButton.IsEnabled = isFieldsCorrect.All(x => x == true);
            };

            registrateButton.Clicked += async delegate
            {
                if (isFieldsCorrect.Contains(false))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "There are wrong entered fields", "OK");
                }
                else if (policyCheckBox.IsChecked == true && agreementCheckBox.IsChecked == true)
                {
                    if (await serverWorker.TaskRegistrateNewPlayer(nameEntry.Text, surnameEntry.Text,
                        loginEntry.Text, passwordEntry.Text, passwordConfirmEntry.Text, String.Format("{0:dd-MM-yyyy}", datePicker.Date), emailEntry.Text))
                    {
                        await SecureStorage.SetAsync("login", loginEntry.Text);
                        await App.Current.MainPage.DisplayAlert("Success", "Registration successful! =)", "OK");
                        await App.Current.MainPage.DisplayAlert("Thank you!", "Your participation in the project means a world to us and we would like to thank you for choosing to help science!", "OK");
                        await Navigation.PushAsync(new Views.LoginPage(systemSettings));
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Registration unsuccessful =( Try again later", "OK");
                    }
                }
                else if (!policyCheckBox.IsChecked && agreementCheckBox.IsChecked)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Agree to the Privacy Policy!", "OK");
                }
                else if (policyCheckBox.IsChecked && !agreementCheckBox.IsChecked)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Agree to the Terms and Conditions!", "OK");
                }
                else if (!policyCheckBox.IsChecked && !agreementCheckBox.IsChecked)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Agree to the Privacy Policy and the Terms and Conditions!", "OK");
                }
            };

            Button cancelButton = new Button()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                Text = "Cancel",
                BackgroundColor = Color.FromHex("#7ad3ff"),
                TextColor = Color.White,
                Margin = new Thickness(10, 0, 10, 0),
            };
            cancelButton.Clicked += async delegate
            {
                await Navigation.PopAsync();
            };

            registrationStack.Children.Add(registrateButton);
            registrationStack.Children.Add(cancelButton);
            #endregion



            HeadStack.Children.Add(nameStack);
            HeadStack.Children.Add(surnameStack);
            HeadStack.Children.Add(loginStack);
            HeadStack.Children.Add(passwordStack);
            HeadStack.Children.Add(dateStack);
            HeadStack.Children.Add(emailStack);
            HeadStack.Children.Add(policyStack);
            HeadStack.Children.Add(agreementStack);
            HeadStack.Children.Add(registrationStack);


            var scroll = new ScrollView
            {
                Content = HeadStack
            };
            Content = HeadStack;

        }
    }
}