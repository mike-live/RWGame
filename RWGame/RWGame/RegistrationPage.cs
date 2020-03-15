using RWGame.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace RWGame
{
    public class RegistrationPage : ContentPage
    {
        ServerWorker localServerWorker;
        bool[] RightInformationInField;
        public RegistrationPage(ServerWorker serverWorker, SystemSettings systemSettings)
        {
            RightInformationInField = new bool[7];
            for (int i = 0; i < RightInformationInField.Length; i++)
                RightInformationInField[i] = false;

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
                Margin = new Thickness(15, 15, 15, 15)
            };
            HeadStack.Children.Add(HeadLabel);

            Thickness LabelMargin = new Thickness(10, 0, 10, 0);

            //Визуализация поля имени
            #region
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
            Image labelRightImage = new Image() {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            StackLayout nameLabelStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#39bafa"),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
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
                Margin = new Thickness(10, 0, 10, 0)
            };
            nameEntry.TextChanged += delegate
            {
                nameLabel.Opacity = nameEntry.Text == "" ? 0 : 1;
            };
            nameEntry.Unfocused += delegate
            {
                if (nameEntry != null && nameEntry.Text != null)
                {
                    if (Regex.IsMatch(nameEntry.Text, @"^.{1,256}$", RegexOptions.CultureInvariant))
                    {
                        RightInformationInField[0] = true;
                        labelRightImage.HeightRequest = nameLabel.Height;
                        labelRightImage.Source = "yes.png";
                    }
                    else
                    {
                        RightInformationInField[0] = false;
                        labelRightImage.HeightRequest = nameLabel.Height;
                        labelRightImage.Source = "no.png";
                    }
                }   
                else
                {
                    RightInformationInField[0] = false;
                    labelRightImage.HeightRequest = nameLabel.Height;
                    labelRightImage.Source = "no.png";
                }
            };
            nameStack.Children.Add(nameLabelStack);
            nameStack.Children.Add(nameEntry);
            #endregion


            //Визуализация поля фамилия
            #region
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
                Margin = new Thickness(10, 0, 10, 0)
            };
            surnameEntry.TextChanged += delegate
            {
                surnameLabel.Opacity = surnameEntry.Text == "" ? 0 : 1;
            };
            surnameEntry.Unfocused += delegate
            {
                if (surnameEntry != null && surnameEntry.Text != null)
                {
                    if (Regex.IsMatch(surnameEntry.Text, @"^.{1,256}$", RegexOptions.CultureInvariant))
                    {
                        RightInformationInField[1] = true;
                        surnameRightImage.HeightRequest = surnameLabel.Height;
                        surnameRightImage.Source = "yes.png";
                    }
                    else
                    {
                        RightInformationInField[1] = false;
                        surnameRightImage.HeightRequest = surnameLabel.Height;
                        surnameRightImage.Source = "no.png";
                    }
                }
                else
                {
                    RightInformationInField[1] = false;
                    surnameRightImage.HeightRequest = surnameLabel.Height;
                    surnameRightImage.Source = "no.png";
                }
            };
            surnameStack.Children.Add(surnameLabelStack);
            surnameStack.Children.Add(surnameEntry);
            #endregion


            //Визуализация поля login
            #region
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
                Margin = new Thickness(10, 0, 10, 0)
            };
            loginEntry.TextChanged += delegate
            {
                loginLabel.Opacity = loginEntry.Text == "" ? 0 : 1;
            };
            loginEntry.Unfocused += async delegate
            {
                if (loginEntry != null && loginEntry.Text != null)
                {
                    if (Regex.IsMatch(loginEntry.Text, @"^[a-zA-Z_\-\.][a-zA-Z0-9_\-\.]{2,255}$", RegexOptions.CultureInvariant))
                    {
                        if (await localServerWorker.TaskCheckLogin(loginEntry.Text))
                        {
                            RightInformationInField[2] = true;
                            loginRightImage.HeightRequest = loginLabel.Height;
                            loginRightImage.Source = "yes.png";
                        }
                        else
                        {
                            RightInformationInField[2] = false;
                            loginRightImage.HeightRequest = loginLabel.Height;
                            loginRightImage.Source = "no.png";
                        }
                    }
                    else
                    {
                        RightInformationInField[2] = false;
                        loginRightImage.HeightRequest = loginLabel.Height;
                        loginRightImage.Source = "no.png";
                    }
                }
                else
                {
                    RightInformationInField[2] = false;
                    loginRightImage.HeightRequest = loginLabel.Height;
                    loginRightImage.Source = "no.png";
                }
            };
            loginStack.Children.Add(loginLabelStack);
            loginStack.Children.Add(loginEntry);
            #endregion


            //Визуализация поля password
            #region
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
                        RightInformationInField[3] = true;
                        passwordRightImage.HeightRequest = passwordLabel.Height;
                        passwordRightImage.Source = "yes.png";
                    }
                    else 
                    {
                        RightInformationInField[3] = false;
                        passwordRightImage.HeightRequest = passwordLabel.Height;
                        passwordRightImage.Source = "no.png";
                    }
                }
                else
                {
                    RightInformationInField[3] = false;
                    passwordRightImage.HeightRequest = passwordLabel.Height;
                    passwordRightImage.Source = "no.png";
                }
            };
            Label passwordConfirmLabel = new Label()
            {
                Text = "Confirm password",
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                BackgroundColor = Color.FromHex("#38ade8"),
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
            passwordConfirmEntry.TextChanged += delegate
            {
                passwordConfirmLabel.Opacity = passwordConfirmEntry.Text == "" ? 0 : 1;
            };
            passwordConfirmEntry.Unfocused += delegate
            {
                if (passwordConfirmEntry != null && passwordConfirmEntry.Text != null && passwordEntry != null && passwordEntry.Text != null)
                {
                    if (passwordConfirmEntry.Text == passwordEntry.Text)
                    {
                        RightInformationInField[4] = true;
                        passwordConfirmRightImage.HeightRequest = passwordConfirmLabel.Height;
                        passwordConfirmRightImage.Source = "yes.png";
                    }
                    else
                    {
                        RightInformationInField[4] = false;
                        passwordConfirmRightImage.HeightRequest = passwordConfirmLabel.Height;
                        passwordConfirmRightImage.Source = "no.png";
                    }
                }
                else
                {
                    RightInformationInField[4] = false;
                    passwordConfirmRightImage.HeightRequest = passwordConfirmLabel.Height;
                    passwordConfirmRightImage.Source = "no.png";
                }
            };

            passwordStack.Children.Add(passwordLabelStack);
            passwordStack.Children.Add(passwordEntry);
            passwordStack.Children.Add(passwordConfirmLabelStack);
            passwordStack.Children.Add(passwordConfirmEntry);
            #endregion


            //Визуализация поля date
            #region
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
                Opacity = 0,
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
                Date = new DateTime(1990, 1, 1),
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
                    RightInformationInField[5] = true;
                    dateRightImage.HeightRequest = dateLabel.Height;
                    dateRightImage.Source = "yes.png";
                }
                else
                {
                    RightInformationInField[5] = false;
                    dateRightImage.HeightRequest = dateLabel.Height;
                    dateRightImage.Source = "no.png";
                }
            };
            dateStack.Children.Add(dateLabelStack);
            dateStack.Children.Add(datePicker);
            #endregion


            //Визуализация поля email
            #region
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
                    string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";

                    if (Regex.IsMatch(emailEntry.Text,
                        pattern, 
                        RegexOptions.CultureInvariant) && await serverWorker.TaskCheckEmail(emailEntry.Text))
                    {
                        RightInformationInField[6] = true;
                        emailRightImage.HeightRequest = emailLabel.Height;
                        emailRightImage.Source = "yes.png";
                    }
                    else
                    {
                        RightInformationInField[6] = false;
                        emailRightImage.HeightRequest = emailLabel.Height;
                        emailRightImage.Source = "no.png";
                    }
                }
                else
                {
                    RightInformationInField[6] = false;
                    emailRightImage.HeightRequest = emailLabel.Height;
                    emailRightImage.Source = "no.png";
                }
            };
            emailStack.Children.Add(emailLabelStack);
            emailStack.Children.Add(emailEntry);

            #endregion

            //Визуализация галочки и ссылки на политику
            #region

            StackLayout policyStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#35a6de"),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(10, -10),
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

            var policyTapGestureRecognizer = new TapGestureRecognizer();
            policyTapGestureRecognizer.Tapped += async (s, e) =>
            {
                Uri uri = new Uri("https://scigames.ru/privacy_policy");
                await Xamarin.Essentials.Launcher.OpenAsync(uri);
            };
            policyHyperlinkLabel.GestureRecognizers.Add(policyTapGestureRecognizer);
            policyStack.Children.Add(policyCheckBox);
            policyStack.Children.Add(policyTextLabel);
            policyStack.Children.Add(policyHyperlinkLabel);

            #endregion

            //Визуализация галочки и ссылки на соглашение
            #region

            StackLayout agreementStack = new StackLayout()
            {
                BackgroundColor = Color.FromHex("#35a6de"),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(10, 0),
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
                Margin = new Thickness(0, 15, 15, 15)
            };

            CheckBox agreementCheckBox = new CheckBox
            {
                IsChecked = false,
                BackgroundColor = Color.FromHex("#35a6de")
            };

            var agreementTapGestureRecognizer = new TapGestureRecognizer();
            agreementTapGestureRecognizer.Tapped += async (s, e) =>
            {
                Uri uri = new Uri("https://scigames.ru/terms");
                await Xamarin.Essentials.Launcher.OpenAsync(uri);
            };

            agreementHyperlinkLabel.GestureRecognizers.Add(agreementTapGestureRecognizer);
            agreementStack.Children.Add(agreementCheckBox);
            agreementStack.Children.Add(agreementTextLabel);
            agreementStack.Children.Add(agreementHyperlinkLabel);

            #endregion

            //Визуализация кнопки регистрации и отмены
            #region
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
                Margin = new Thickness(10, 0, 10, 0)
            };
            registrateButton.Clicked += async delegate
            {
                if (RightInformationInField.Contains(false))
                {
                    await DisplayAlert("Error", "There are wrong entered fields", "OK");
                }
                else if (policyCheckBox.IsChecked == true && agreementCheckBox.IsChecked == true)
                {
                    if (await serverWorker.TaskRegistrateNewPlayer(nameEntry.Text, surnameEntry.Text,
                        loginEntry.Text, passwordEntry.Text, passwordConfirmEntry.Text, String.Format("{0:dd-MM-yyyy}", datePicker.Date), emailEntry.Text))
                    {
                        await SecureStorage.SetAsync("login", loginEntry.Text);
                        await DisplayAlert("Success", "Registration successful! =)", "OK");
                        await DisplayAlert("Thank you!", "Your participation in the project means a world to us and we would like to thank you for choosing to help science!", "OK");
                        await Navigation.PushAsync(new LoginPage(systemSettings));
                    }
                    else
                    {
                        await DisplayAlert("Error", "Registration unsuccessful =( Try again later", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "Checks the agreement and privacy policy boxes!", "OK");
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