﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Quartzmin.Models;
using Quartz;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Quartzmin.Controllers
{
    public abstract partial class PageControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver(), // PascalCase as default
        };

        protected Services Services => (Services) Request.HttpContext.Items[typeof(Services)];
        protected string GetRouteData(string key) => RouteData.Values[key].ToString();
        protected IActionResult Json( object content ) => new JsonResult( content, _serializerSettings );


        protected IActionResult NotModified() => new StatusCodeResult(304);

        protected IEnumerable<string> GetHeader(string key)
        {
            var values = Request.Headers[key];
            return values == StringValues.Empty ? (IEnumerable<string>)null : values;
        }
    }


	public abstract partial class PageControllerBase
    {
        protected IScheduler Scheduler => Services.Scheduler;

        protected dynamic ViewBag { get; } = new ExpandoObject();

        internal class Page
        {
            PageControllerBase _controller;

            public string ControllerName => _controller.GetRouteData("controller");

            public string ActionName => _controller.GetRouteData("action");

            public Services Services => _controller.Services;

            public object ViewBag => _controller.ViewBag;

            public object Model { get; set; }

            public Page(PageControllerBase controller, object model = null)
            {
                _controller = controller;
                Model = model;
            }
        }

        protected IActionResult View(object model)
        {
            return View(GetRouteData("action"), model);
        }

        protected IActionResult View(string viewName, object model)
        {
            string content = Services.ViewEngine.Render($"{GetRouteData("controller")}/{viewName}.hbs", new Page(this, model));
            return Html(content);
        }

        protected IActionResult Html(string html)
        {
            return new ContentResult()
            {
                Content = html,
                ContentType = "text/html",
            };
        }

        protected string GetETag()
        {
            IEnumerable<string> values = GetHeader("If-None-Match");
            if (values == null)
                return null;
            else
                return new System.Net.Http.Headers.EntityTagHeaderValue(values.FirstOrDefault()).Tag;
        }

        public IActionResult TextFile(string content, string contentType, DateTime lastModified, string etag)
        {
            return new ContentResult()
            {
                Content = content,
                ContentType = contentType,
            };
        }

        protected JobDataMapItem JobDataMapItemTemplate => new JobDataMapItem()
        {
            SelectedType = Services.Options.DefaultSelectedType,
            SupportedTypes = Services.Options.StandardTypes.Order(),
        };
    }
}
