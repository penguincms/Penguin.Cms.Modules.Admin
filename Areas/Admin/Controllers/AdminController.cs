using Loxifi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Penguin.Cms.Core.Extensions;
using Penguin.Cms.Entities;
using Penguin.Cms.Modules.Core.Controllers;
using Penguin.Cms.Modules.Core.Models;
using Penguin.Cms.Web.Extensions;
using Penguin.Extensions.Collections;
using Penguin.Persistence.Abstractions;
using Penguin.Persistence.Repositories.Interfaces;
using Penguin.Reflection;
using Penguin.Reflection.Serialization.Constructors;
using Penguin.Security.Abstractions.Constants;
using Penguin.Security.Abstractions.Extensions;
using Penguin.Security.Abstractions.Interfaces;
using Penguin.Web.Security.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Penguin.Cms.Modules.Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RequiresRole(RoleNames.ADMIN_ACCESS)]
    public class AdminController : ModuleController
    {
        public static MetaConstructor Constructor
        {
            get
            {
                MetaConstructor c = new();

                c.Settings.AttributeIncludeSettings = AttributeIncludeSetting.All;

                //Strip off the EF Proxy shell
                c.Settings.AddTypeGetterOverride((type) => type?.Module?.ScopeName == "EntityProxyModule" ? type.BaseType : type);

                return c;
            }
        }

        protected IServiceProvider ServiceProvider { get; set; }

        protected IUserSession UserSession { get; set; }

        public class QueryResults
        {
            public IEnumerable<object>? Results { get; set; }

            public int TotalCount { get; set; }
        }

        public AdminController(IServiceProvider serviceProvider, IUserSession userSession)
        {
            ServiceProvider = serviceProvider;
            UserSession = userSession;
        }

        public PagedListContainer<T> GenerateList<T>(int count = 20, int page = 0, string text = "", Func<object, T>? Converter = null) where T : class
        {
            return GenerateList(typeof(T), count, page, text, Converter);
        }

        public PagedListContainer<T> GenerateList<T>(string type, int count = 20, int page = 0, string text = "", Func<object, T>? Converter = null) where T : class
        {
            return GenerateList(TypeFactory.GetTypeByFullName(type, typeof(Entity), false), count, page, text, Converter);
        }

        public PagedListContainer<T> GenerateList<T>(Type t, int count = 20, int page = 0, string text = "", Func<object, T>? Converter = null) where T : class
        {
            Converter ??= new Func<object, T>((o) => (T)o);

            PropertyInfo? Key = ContextHelper.GetKeyForType(t);

            if (Key is null)
            {
                throw new Exception($"Unable to find Key for type {t}");
            }

            PagedListContainer<T> pagedList = new();

            QueryResults? DBResult;

            if (ServiceProvider.GetRepositoryForType<IKeyedObjectRepository>(t) is IKeyedObjectRepository TypedRepository)
            {
                DBResult = GetType().GetMethod(nameof(AdminController.QueryDatabase)).MakeGenericMethod(t).Invoke(this, new object[] { count, page, text }) as QueryResults;

                DBResult.Results = DBResult.Results.Select(Converter);
            }
            else
            {
                throw new Exception("Can not access data source for objects that dont derive from KeyedObjectType in this version");
            }

            pagedList.Page = page;

            pagedList.Items.AddRange(DBResult.Results.Cast<T>());

            pagedList.Count = count;

            pagedList.TotalCount = DBResult.TotalCount;

            return pagedList;
        }

        public QueryResults QueryDatabase<T>(int count = 20, int page = 0, string text = "") where T : KeyedObject
        {
            IKeyedObjectRepository<T> repository = ServiceProvider.GetService<IKeyedObjectRepository<T>>();

            IQueryable<T> results = repository.OrderByDescending(i => i._Id).Skip(page * count).Take(count);
            int totalCount = 0;

            if (!string.IsNullOrWhiteSpace(text))
            {
                results = results.Where(ExpressionBuilder.AnyPropertyContains<T>(text));
                //totalCount = repository.Where(ExpressionBuilder.AnyPropertyContains<T>(text)).Count();
            }
            else
            {
                _ = repository.Count();
            }

            ISecurityProvider<T> securityProvider = ServiceProvider.GetService<ISecurityProvider<T>>();

            List<T> ResultsList = results.ToList();

            if (securityProvider != null && !UserSession.LoggedInUser.HasRole(RoleNames.SYS_ADMIN))
            {
                ResultsList = ResultsList.Where(r => securityProvider.CheckAccess(r)).ToList();
            }

            return new QueryResults()
            {
                Results = ResultsList,
                TotalCount = totalCount
            };
        }
    }
}