﻿using System.Web.Mvc;

namespace OpenIddict.Sandbox.AspNet.Server;

public class FilterConfig
{
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        filters.Add(new HandleErrorAttribute());
    }
}
