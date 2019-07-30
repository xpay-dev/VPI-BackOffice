using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SDGAdmin
{
    public class CustomAttributes
    {
        public class GreaterThanAttribute : ValidationAttribute
        {
            public string OtherProperty { get; set; }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);

                if (otherPropertyInfo == null)
                {
                    return new ValidationResult(String.Format("Could not find a property named {0}.", OtherProperty));
                }

                object otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

                decimal d = Convert.ToDecimal(value);
                if (d > Convert.ToDecimal(otherPropertyValue))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Make sure this is greater than " + OtherProperty);
            }
        }

        public class SessionExpireFilterAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = HttpContext.Current;

                var controller = ctx.Request.RequestContext.RouteData.Values["controller"].ToString();
                var action = ctx.Request.RequestContext.RouteData.Values["action"].ToString();

                if (!(controller.ToLower().Contains("home") && action.ToLower().Contains("index")) && !(controller.ToLower().Contains("home") && action.ToLower().Contains("forgotpassword")) && !(controller.ToLower().Contains("home") && action.ToLower().Contains("changepassword")))
                {
                    if (ctx.Session["DisplayName"] == null)
                    {
                        // check if a new session id was generated
                        filterContext.Result = new RedirectResult("~/Home");
                        ctx.Session["SessionExpired"] = "Session has expired. Please relogin.";
                        return;
                    }
                }

                base.OnActionExecuting(filterContext);
            }
        }

    }
}