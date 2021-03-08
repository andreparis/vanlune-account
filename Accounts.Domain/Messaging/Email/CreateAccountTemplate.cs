﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Accounts.Domain.Messaging.Email
{
    public static class CreateAccountTemplate
    {
        public static string GetAcocuntsBody(string link)
        {
            var template = GetTemplateAsync("Accounts.Domain.Messaging.Email.Template.email.html");

            template = template.Replace("#{{action_url}}", link);

            return template;
        }

        private static string GetTemplateAsync(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            var reader = new StreamReader(stream);
            string template = reader.ReadToEndAsync().Result;
            return template;
        }
    }
}
