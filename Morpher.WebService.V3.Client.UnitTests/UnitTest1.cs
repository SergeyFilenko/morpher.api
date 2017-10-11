﻿namespace Morpher.WebService.V3.Client.UnitTests
{
    using System.Collections.Specialized;
    using System.Net;
    using Exceptions;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void InvalidUrlThrows()
        {
            Assert.Throws<WebException>(() => new MorpherClient(null, "http://dns-error-fjeqweWe3cu.com").Russian.Parse("кошка"));
        }

        [Test]
        public void GarbageInResponseBody()
        {
            var webClient = new Mock<IWebClient>();
            webClient.Setup(client => client.QueryString).Returns(new NameValueCollection());
            webClient.Setup(client => client.DownloadString(It.IsAny<string>())).Returns("< !DOCTYPE html>");
            var morpherClient = MockClientHelpers.NewMorpherClientInject(webClient.Object);
            Assert.Throws<InvalidServerResponseException>(() => morpherClient.Russian.Parse("кошка"));
        }
    }
}
