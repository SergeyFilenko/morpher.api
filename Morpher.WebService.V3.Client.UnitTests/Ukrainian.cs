﻿using Morpher.WebService.V3.Russian;

namespace Morpher.WebService.V3.Client.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Moq;
    using NUnit.Framework;
    using V3.Ukrainian;

    [TestFixture]
    public class Ukrainian
    {
        public string DeclensionResultText { get; } = @"
{
    ""Р"": ""теста"",
    ""Д"": ""тесту"",
    ""З"": ""теста"",
    ""О"": ""тестом"",
    ""М"": ""тесті"",
    ""К"": ""тесте"",
    ""рід"": ""Чоловічий""
}";

        public string SpellText { get; } = @"
{
    ""n"": {
        ""Н"": ""десять"",
        ""Р"": ""десяти"",
        ""Д"": ""десяти"",
        ""З"": ""десять"",
        ""О"": ""десятьма"",
        ""М"": ""десяти"",
        ""К"": ""десять""
    },
    ""unit"": {
        ""Н"": ""рублів"",
        ""Р"": ""рублів"",
        ""Д"": ""рублям"",
        ""З"": ""рублів"",
        ""О"": ""рублями"",
        ""М"": ""рублях"",
        ""К"": ""рублів""
    }
}";

        public string UserDictGetAllText { get; } = @"
[
    {
        ""singular"": {
            ""Н"": ""Тест"",
            ""Р"": ""ТестР"",
            ""Д"": ""ТестД"",
            ""З"": ""ТестЗ"",
            ""О"": ""ТестО"",
            ""М"": ""ТестМ"",
            ""К"": ""ТестК""
        }
    }
]";


        [Test]
        public void Parse_Success()
        {
            var webClient = new Mock<IWebClient>();
            webClient.Setup(client => client.QueryString).Returns(new NameValueCollection());
            webClient.Setup(client => client.DownloadString(It.IsAny<string>())).Returns(DeclensionResultText);
            var morpherClient = new MorpherClient(null, null, webClient.Object);

            DeclensionResult declensionResult = morpherClient.Ukrainian.Parse("тест");
            Assert.IsNotNull(declensionResult);
            Assert.AreEqual("тест", declensionResult.Nominative);
            Assert.AreEqual("теста", declensionResult.Genitive);
            Assert.AreEqual("тесту", declensionResult.Dative);
            Assert.AreEqual("теста", declensionResult.Accusative);
            Assert.AreEqual("тестом", declensionResult.Instrumental);
            Assert.AreEqual("тесті", declensionResult.Prepositional);
            Assert.AreEqual("тесте", declensionResult.Vocative);
            Assert.AreEqual(Gender.Masculine, declensionResult.Gender);
        }

        [Test]
        public void Spell_Success()
        {
            var webClient = new Mock<IWebClient>();
            webClient.Setup(client => client.QueryString).Returns(new NameValueCollection());
            webClient.Setup(client => client.DownloadString(It.IsAny<string>())).Returns(SpellText);
            var morpherClient = new MorpherClient(null, null, webClient.Object);

            NumberSpellingResult declensionResult = morpherClient.Ukrainian.Spell(10, "рубль");
            Assert.IsNotNull(declensionResult);
            
            // number
            Assert.AreEqual("десять", declensionResult.NumberDeclension.Nominative);
            Assert.AreEqual("десяти", declensionResult.NumberDeclension.Genitive);
            Assert.AreEqual("десяти", declensionResult.NumberDeclension.Dative);
            Assert.AreEqual("десять", declensionResult.NumberDeclension.Accusative);
            Assert.AreEqual("десятьма", declensionResult.NumberDeclension.Instrumental);
            Assert.AreEqual("десяти", declensionResult.NumberDeclension.Prepositional);
            Assert.AreEqual("десять", declensionResult.NumberDeclension.Vocative);

            // unit
            Assert.AreEqual("рублів", declensionResult.UnitDeclension.Nominative);
            Assert.AreEqual("рублів", declensionResult.UnitDeclension.Genitive);
            Assert.AreEqual("рублям", declensionResult.UnitDeclension.Dative);
            Assert.AreEqual("рублів", declensionResult.UnitDeclension.Accusative);
            Assert.AreEqual("рублями", declensionResult.UnitDeclension.Instrumental);
            Assert.AreEqual("рублях", declensionResult.UnitDeclension.Prepositional);
            Assert.AreEqual("рублів", declensionResult.UnitDeclension.Vocative);
        }

        [Ignore("ukrainian/declension never returns 495. Will repurpose this test later for 496.")]
        [Test]
        public void Parse_MorpherWebServiceException()
        {
            var webClient = new Mock<IWebClient>();
            webClient.Setup(client => client.QueryString).Returns(new NameValueCollection());
            var exception = new WebException("Exception", null, WebExceptionStatus.ReceiveFailure,
                WebResponseMock.CreateWebResponse((HttpStatusCode)495,
                    new MemoryStream(Encoding.UTF8.GetBytes(ExceptionText.UseSpell))));
            webClient.Setup(client => client.DownloadString(It.IsAny<string>())).Throws(exception);
            var morpherClient = new MorpherClient(null, null, webClient.Object);

            Assert.Throws<NumeralsDeclensionNotSupportedException>(() => morpherClient.Ukrainian.Parse("exception here"));
        }

        [Test]
        public void Spell_MorpherWebServiceException()
        {
            var webClient = new Mock<IWebClient>();
            var morpherClient = new MorpherClient(null, null, webClient.Object);

            Assert.Throws<ArgumentEmptyException>(() => morpherClient.Ukrainian.Parse(""));
        }

        [Test]
        public void UserDictRemove_Success()
        {
            var @params = new NameValueCollection();
            var webClient = new Mock<IWebClient>();
            webClient.Setup(client => client.QueryString).Returns(@params);
            webClient.Setup(client => client.UploadValues(It.IsAny<string>(), "DELETE", It.IsAny<NameValueCollection>()))
                .Returns(Encoding.UTF8.GetBytes("true"));
            var morpherClient = new MorpherClient(null, null, webClient.Object);

            bool found = morpherClient.Ukrainian.UserDict.Remove("тест");

            Assert.IsTrue(found);
            Assert.AreEqual(Uri.EscapeDataString("тест"), @params.Get("s"));
        }

        [Test]
        public void UserDictGetAll_Success()
        {
            var webClient = new Mock<IWebClient>();
            webClient.Setup(client => client.QueryString).Returns(new NameValueCollection());
            webClient.Setup(client => client.DownloadString(It.IsAny<string>())).Returns(UserDictGetAllText);
            var morpherClient = new MorpherClient(null, null, webClient.Object);

            IEnumerable<CorrectionEntry> correctionEntries = morpherClient.Ukrainian.UserDict.GetAll().ToList();

            Assert.IsNotNull(correctionEntries);
            Assert.AreEqual(1, correctionEntries.Count());
            CorrectionEntry entry = correctionEntries.First();

            Assert.AreEqual("Тест", entry.Singular.Nominative);
            Assert.AreEqual("ТестР", entry.Singular.Genitive);
            Assert.AreEqual("ТестД", entry.Singular.Dative);
            Assert.AreEqual("ТестЗ", entry.Singular.Accusative);
            Assert.AreEqual("ТестО", entry.Singular.Instrumental);
            Assert.AreEqual("ТестМ", entry.Singular.Prepositional);
            Assert.AreEqual("ТестК", entry.Singular.Vocative);
        }
    }
}