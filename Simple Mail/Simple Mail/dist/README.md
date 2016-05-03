# Smtp Gun Module

It works with both Desktop and Web applications. It makes it simpler to send email without having all the work of setting it all up.

A small Smtp Email Module for sending emails via website using html templates

Make sure to check the web.config to change basic settings of the module.

Use it to create a simple Smtp module that sends html emails 

[GitHub](https://github.com/ccasalicchio/Smtp-Module) Page

Either use a config file (app.config/web.config)

            <appSettings>
                <!--email provider section-->
                <add key="siteadmin" value="postmaster@simpleemail.com" />
                <add key="email-subject" value="Email from Email Website" />
                <add key="email-from" value="postmaster@puul.ga" />
                <add key="email-from-name" value="Simple Email" />
                <add key="email-require-auth" value="false" />
                <add key="email-username" value="simpleemail" />
                <add key="email-password" value="" />
                <add key="email-port" value="3535" />
                <add key="useSSL" value="false" />
                <add key="email-server" value="localhost" />
                <add key="email-html-share" value="~/Email Templates/share.html" />
                <add key="email-html-default" value="~/Email Templates/default.html" />
                <add key="email-html-forgotten" value="~/Email Templates/default.html" />
                <add key="email-html-confirmation" value="~/Email Templates/default.html" />
                <add key="email-html-welcome" value="~/Email Templates/default.html" />
            </appSettings>
            
Or instanciate the module via code:

                using Simple_Mail;
                namespace Example{
                        public class Test() {
                            Message email = new Message(smtpServer, smtpPort, useAuth, username, password,  useSSL);
                        }
                    }
                }
                

To test emails locally use
[Smtp4Dev](https://smtp4dev.codeplex.com/)
