using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Web;

namespace Suteki.Shop.Tests
{
    public class HttpContextTestContext
    {
        public Mock<HttpContextBase> ContextMock { get; private set; }
        public Mock<HttpRequestBase> RequestMock { get; private set; }
        public Mock<HttpResponseBase> ResponseMock { get; private set; }
        public Mock<HttpSessionStateBase> SessionMock { get; private set; }
        public Mock<HttpServerUtilityBase> ServerMock { get; private set; }

        public HttpContextBase Context { get { return ContextMock.Object; } }
        public HttpRequestBase Request { get { return RequestMock.Object; } }
        public HttpResponseBase Response { get { return ResponseMock.Object; } }
        public HttpSessionStateBase Session { get { return SessionMock.Object; } }
        public HttpServerUtilityBase Server { get { return ServerMock.Object; } }

        public HttpContextTestContext()
        {
            ContextMock = new Mock<HttpContextBase>();
            RequestMock = new Mock<HttpRequestBase>();
            ResponseMock = new Mock<HttpResponseBase>();
            SessionMock = new Mock<HttpSessionStateBase>();
            ServerMock = new Mock<HttpServerUtilityBase>();

            ContextMock.ExpectGet(c => c.Request).Returns(RequestMock.Object);
            ContextMock.ExpectGet(c => c.Response).Returns(ResponseMock.Object);
            ContextMock.ExpectGet(c => c.Session).Returns(SessionMock.Object);
            ContextMock.ExpectGet(c => c.Server).Returns(ServerMock.Object);
        }
    }
}
