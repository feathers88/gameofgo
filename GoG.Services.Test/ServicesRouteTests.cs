//using System.Web.Http;
//using GoG.Services.Tests.Common;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Net.Http;

//namespace GoG.Services.Test.Users
//{
//    [TestClass]
//    public class ServicesRouteTests
//    {
//        #region Data
//        readonly HttpConfiguration _config;
//        #endregion Data

//        #region Ctor

//        public ServicesRouteTests()
//        {
//            _config = RouteTester.CreateHttpConfigurationFromGoGServices();
//        }

        
//        #endregion Ctor

//        #region Tests

//        [TestMethod]
//        public void UsersControllerPostIsCorrect()
//        {
//            var request = new HttpRequestMessage(HttpMethod.Post, "http://dummy/users");

//            var routeTester = new RouteTester(_config, request);

//            Assert.AreEqual(typeof(UsersController), routeTester.GetControllerType());
//            var actionName = routeTester.GetActionName();
//            Assert.AreEqual(ReflectionHelpers.GetMethodName((UsersController p) => p.Post(null)), actionName);
//        }

//        //[TestMethod]
//        //public void V2_RPC_UrlControllerPostIsCorrect()
//        //{
//        //    var request = new HttpRequestMessage(HttpMethod.Post, BaseUri + "/v2/url/Add");

//        //    var routeTester = new RouteTester(_config, request);

//        //    Assert.AreEqual(typeof(UsersController), routeTester.GetControllerType());
//        //    Assert.AreEqual(ReflectionHelpers.GetMethodName((UsersController p) => p.Add(new Url())), routeTester.GetActionName());
//        //}
        
//        #endregion Tests

//        #region Private Helpers
        
//        #endregion Private Helpers
//    }

//}
