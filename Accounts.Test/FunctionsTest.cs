using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AutoFixture;
using Moq;
using Accounts.Application;
using Xunit;
using System.Collections.Generic;

namespace Tests
{
    public class Tests
    {
        private Fixture _fixture;
        private Function _function;

        public Tests()
        {
            _fixture = new Fixture();
            _function = new Function();
        }

        [Fact]
        public void CreateUser()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\r\n\"name\":\"First User\",\r\n\"password\": \"babybacon\",\r\n\"country\": \"brazil\",\r\n\"isActive\":false,\r\n\"email\":\"pedro.rrmaia@live.com\",\r\n\"phone\":\"27123456789\",\r\n\"characters\":[{\r\n\"name\":\"Vancartier\",\r\n\"game\":\"wow\",\r\n\"server\":\"nemesis\"\r\n}],\r\n\"idroles\":[1],\r\n\"idclaims\":[0]\r\n}")
                .Create();

            var result = _function.CreateAccount(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void UpdateUser()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\"Id\":5,\"name\":\"Andre Paris\",\"email\":\"andreparis.comp@gmail.com\",\"country\":\"Brazil\",\"phone\":\"12312312313\",\"characters\":[],\"idRoles\":[],\"password\":\"\",\"isActive\":false}")
                .Create();

            var result = _function.UpdateAccount(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void GetUserByFilter()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.QueryStringParameters, new Dictionary<string, string>() 
                {
                    {"startDate","2021-02-01" },
                    {"endDate","2021-04-22" }
                })
                .Create();

            var result = _function.GetAllAccountsByFilters(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void AuthUser()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\r\n\"email\":\"andreparis.comp@gmail.com\",\r\n\"password\":\"andre\"\r\n}")
                .Create();

            var result = _function.AuthenticateAccount(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void AuthUserByFb()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\"facebookLogin\":{\"name\":\"Andr\u00E9 Paris\",\"email\":\"andre.paris@yahoo.com.br\",\"picture\":{\"data\":{\"height\":50,\"is_silhouette\":false,\"url\":\"https:\\/\\/platform-lookaside.fbsbx.com\\/platform\\/profilepic\\/?asid=5205428666196965&height=50&width=50&ext=1619378951&hash=AeR-9nn68D1eARhi7Y4\",\"width\":50}},\"id\":\"5205428666196965\",\"accessToken\":\"EAAFQDiot3jgBAMBuUEcMI64a9VcbpVm3X7Ec1HeQ67YU4X1wl3WZAA1Tum1KBc6hYsfNZCo3CscS3LfDEFFrnM0c9BUc45RsGdt51qGwWCgaDlwKXalLZC6jaEGybuhYn8KBMTx8Pc3QM1e4OFJzdALJhQZC19huJPgpVF1pIiavZBYGVw0VZBd0PiPWCAp76m3okHkIlk2AZDZD\",\"userID\":\"5205428666196965\",\"expiresIn\":5450,\"signedRequest\":\"De6LCmz5fqcGkObgioVPvUn3dhJiQ6LAP5wC_2vqOVM.eyJ1c2VyX2lkIjoiNTIwNTQyODY2NjE5Njk2NSIsImNvZGUiOiJBUURxUGpkcG9wb2szV1lnNDFRS1FpdlBsU0VrMkZULWYtRlZBNWdON1FoV0NxdENxaEhpVnZpQngxdS0wZ2dDUjN4b0MtbVYzelpIcTZHTURWc25DWERVbERUTWhCOGxKS3ZLY05haVQ3d2NDTHZ2aThHb0hRMUlrSS01TG5OOWxIdWgzcVhEVmhYNTVHal9aOWFiOXAwbWdVRFNpR2ZkcmZxeHI1S2VsdXJVak9tSm5ISmZ5aGMwcGVHTmg0UmQzSnVkVlRIU2xqVmdBaHM1cEUxczZMTjdBYkxvNXRrbk8zQXJ4V0NqcF9tNVBzRHZ5R29Dem9qM2VoV3ZTMkJvcE1IUUNWR0hxUzBXQkpPVFhkcU5HQmkwRWlVbHd3WUZQekY5aUdNc1JhNEVGRF9sdlp2NEZDN2xQZ01BMDlxVHJyOFpSTG9VaVBma3hWeF9PYjExSDZXTWtVRVpXcFJZSmhNTnN2YWtodDhmdWxxLUhVcTh1RDdsQ1FONmJDU016Vk0iLCJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZF9hdCI6MTYxNjc4Njk1MH0\",\"graphDomain\":\"facebook\",\"data_access_expiration_time\":1624562950}}")
                .Create();

            var result = _function.AuthenticateAccount(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void CreateClaimOrdersGet()
        {
            var lambdaContext = new Mock<ILambdaContext>(); 
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\r\n\"name\":\"GET_ORDER\",\r\n\"claimType\": \"ORDERS\",\r\n\"claimValue\": \"GET\"\r\n}")
                .Create();

            var result = _function.CreateClaim(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void CreateClaimOrdersPost()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\r\n\"name\":\"POST_ORDER\",\r\n\"claimType\": \"ORDERS\",\r\n\"claimValue\": \"POST\"\r\n}")
                .Create();

            var result = _function.CreateClaim(apiContext, lambdaContext.Object);
        }



        [Fact]
        public void CreateRole()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\r\n\"name\":\"Clients\",\r\n\"Claims\": [0]\r\n}")
                .Create();

            var result = _function.CreateRoles(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void GetAllRoles()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .Create();

            var result = _function.GetAllRoles(apiContext, lambdaContext.Object);

        }

        [Fact]
        public void UserPatchAddTest()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\r\n\"UserId\":5,\r\n\"Patches\":[{\r\n\"op\":\"add\",\r\n\"path\":\"\\/roles\",\r\n\"value\":[1]\r\n},{\r\n\"op\":\"remove\",\r\n\"path\":\"\\/roles\",\r\n\"value\":[2]\r\n}]\r\n}")
                .Create();

            var result = _function.UserPatch(apiContext, lambdaContext.Object);
        }

        [Fact]
        public void RolePatchAddTest()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\r\n\"RoleId\":1,\r\n\"Patches\":[{\r\n\"op\":\"add\",\r\n\"path\":\"\\/claims\",\r\n\"value\":[2]\r\n}]\r\n}")
                .Create();

            var result = _function.RolePatch(apiContext, lambdaContext.Object);
        }

        [Fact]
        public void RolePatchRemoveAddTest()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\r\n\"RoleId\":1,\r\n\"Patches\":[{\r\n\"op\":\"remove\",\r\n\"path\":\"\\/claims\",\r\n\"value\":[2]\r\n},{\r\n\"op\":\"add\",\r\n\"path\":\"\\/claims\",\r\n\"value\":[3]\r\n}]\r\n}")
                .Create();

            var result = _function.RolePatch(apiContext, lambdaContext.Object);
        }

        [Fact]
        public void ConfirmAccountTest()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\"link\":\"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2MTc0Nzc5MzV9.3MzD20Faa1VgRZt1xlPZOHP4ZzpNG6E8IHyLSnhuDyw\"}")
                .Create();

            var result = _function.ConfirmAccount(apiContext, lambdaContext.Object);
        }

        [Fact]
        public void RecoverPasswordEmailTest()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\"email\":\"andre.paris@yahoo.com.br\"}")
                .Create();

            var result = _function.RecoverPasswordEmail(apiContext, lambdaContext.Object);
        }

        [Fact]
        public void RecoverPasswordTest()
        {
            var lambdaContext = new Mock<ILambdaContext>();
            var apiContext = _fixture
                .Build<APIGatewayProxyRequest>()
                .With(x => x.Body, "{\"password\": \"user123\",\"verification\":\"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhbmRyZS5wYXJpc0B5YWhvby5jb20uYnIiLCJqdGkiOiJkNWQ5YmQ2MWQ2Njk0YTcyODRiYmMwZGNlZDE4MDAzNSIsInVuaXF1ZV9uYW1lIjoiYW5kcmUucGFyaXNAeWFob28uY29tLmJyIiwiZXhwIjoxNjE0OTAxMDY0fQ.Uc-D1MzOLnZNLYWH8yeMQ_HLY1qQoTWXS3wPwtIKgdo\"}")
                .Create();

            var result = _function.RecoverPassword(apiContext, lambdaContext.Object);
        }
    }
}