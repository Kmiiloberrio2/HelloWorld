﻿using HelloWorld.CrossCutting.Testing;
using HelloWorld.Domain.Contracts;
using HelloWorld.Domain.Contracts.Handlers;
using HelloWorld.Domain.Contracts.Readers;
using Moq;
using Should;
using System;
using Xunit;


namespace HelloWorld.Business.UnitTests
{
    public class ConversationManagerTests : TestsFor<ConversationManager>
    {

        public override void BeforeInstanceGenerated()
        {
            AutoMock.Container.Configure(x => {
                x.For<ILogger>().Use(new Mock<ILogger>().Object);                               
            });

            AutoMock.Container.Inject<IExceptionHandler>(new ExceptionHandler(AutoMock.Get<ILogger>()));            
        }


        [Fact]
        public void RequestGreeting_WhenCalled_ReturnsGreeting()
        {
            // Arrange
            var greeting = Guid.NewGuid().ToString();
            GetMockFor<ITokenReader>().Setup(tr => tr.GetGreeting()).Returns(greeting);
                
            // Act
            var result = Instance.RequestGreeting();

            // Assert
            result.ShouldEqual(greeting);
        }


        [Fact]
        public void RequestGreeting_TokenReaderThrowsException_TheExceptionIsLogged()
        {
            // Arrange            
            var someException = SomeException;
            GetMockFor<ITokenReader>().Setup(reader => reader.GetGreeting()).Throws(someException);

            // Act
            Instance.RequestGreeting();

            // Assert
            GetMockFor<ILogger>().Verify(logger => logger.LogException(someException), Times.Once());
        }


        private Exception SomeException
        {
            get { return new Exception(Guid.NewGuid().ToString()); }
        }
    }
}
