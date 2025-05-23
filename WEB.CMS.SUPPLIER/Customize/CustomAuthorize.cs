﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using WEB.CMS.SUPPLIER.Models;

namespace WEB.CMS.SUPPLIER.Customize
{
    #region Authorize class none parameter

    public class CustomAuthorize : CustomAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var controllerInfo = context.ActionDescriptor as ControllerActionDescriptor;
            if (context != null)
            {
                if (context.Filters.Any(item => item is IAllowAnonymousFilter))
                {
                    return;
                }
                else
                {
                    var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
                    if (!isAuthenticated)
                    {
                        if (IsAjaxRequest(context.HttpContext.Request))
                        {
                            context.Result = new RedirectResult("/Account/RedirectLogin");
                        }
                        else
                        {
                            //context.Result = new RedirectToRouteResult("/account/login?requestPath=" + Uri.EscapeDataString(context.HttpContext.Request.Path));
                            //context.Result = new RedirectResult("/error/index" + Uri.EscapeDataString(context.HttpContext.Request.Path));
                            context.Result = new RedirectResult("/Account/RedirectLogin");
                        }
                    }
                    else
                    {
                        var IsAllowAccess = true; //context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
                        if (!IsAllowAccess)
                        {
                            context.Result = new ForbidResult();
                        }
                    }
                }
            }
        }
    }

    #endregion

    #region Authorize class with parameter

    public class AuthorizeRole : TypeFilterAttribute
    {
        public AuthorizeRole(string claimType, string claimValue) : base(typeof(AuthorizeRoleFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class AuthorizeRoleFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public AuthorizeRoleFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            if (!hasClaim)
            {
                context.Result = new ForbidResult();
            }
        }
    }

    #endregion

    /// <summary>
    /// Class Attribute check ajax request
    /// </summary>
    public class CustomAttribute : Attribute
    {
        public bool IsAjaxRequest(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";

            return false;
        }
    }
}
