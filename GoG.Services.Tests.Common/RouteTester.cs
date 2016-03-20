using System;
using System.Net.Http;


namespace GoG.Services.Tests.Common
{
    //public class RouteTester
    //{
    //    public static HttpConfiguration CreateHttpConfigurationFromGoGServices()
    //    {
    //        var config = new HttpConfiguration();
    //        WebApiConfig.Register(config);
    //        config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

    //        return config;
    //    }

    //    HttpConfiguration config;
    //    HttpRequestMessage request;
    //    IHttpRouteData routeData;
    //    IHttpControllerSelector controllerSelector;
    //    HttpControllerContext controllerContext;

    //    public RouteTester(HttpConfiguration conf, HttpRequestMessage req)
    //    {
    //        config = conf;
    //        request = req;
    //        routeData = config.Routes.GetRouteData(request);
    //        request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
    //        controllerSelector = new DefaultHttpControllerSelector(config);
    //        controllerContext = new HttpControllerContext(config, routeData, request);
    //    }

    //    public string GetActionName()
    //    {
    //        try
    //        {
    //            if (controllerContext.ControllerDescriptor == null)
    //                GetControllerType();

    //            var actionSelector = new ApiControllerActionSelector();
    //            var descriptor = actionSelector.SelectAction(controllerContext);

    //            return descriptor.ActionName;     
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
            
    //    }

    //    public Type GetControllerType()
    //    {
    //        var descriptor = controllerSelector.SelectController(request);
    //        controllerContext.ControllerDescriptor = descriptor;
    //        return descriptor.ControllerType;
    //    }
    //}
}
