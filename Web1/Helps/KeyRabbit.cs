﻿using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Web1.Data;

namespace Web1.Helps
{
    public class KeyRabbit
    {
        // Exchange

        public const string nameExchange = "user_events_exchange";

        // Queue

        public const string FORGOT_PASSWORD_QUEUE = "forgot_password_queue";

        public const string CONFIRM_EMAIL_QUEUE = "confirm_email_queue";

        public const string NOTIFICATION_QUEUE = "notification_queue";

        public const string ADMIN_NOTIFICATION_QUEUE = "admin_notification_queue";

        public const string USER_NOTIFICATION_QUEUE = "user_notification_queue";

        // Routing Key

        public const string FORGOT_PASSWORD_ROUTING = "forgot_password";

        public const string CONFIRM_EMAIL_ROUTING = "confirm_email";

        public static string NOTIFICATION_ROUTING = "notification";

        public const string ADMIN_NOTIFICATION_ROUTING = "admin_notification";

        public const string USER_NOTIFICATION_ROUTING = "user_notification";
    }


}
