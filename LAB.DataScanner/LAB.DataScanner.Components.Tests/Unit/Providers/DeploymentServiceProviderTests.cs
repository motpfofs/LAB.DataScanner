namespace LAB.DataScanner.Components.Tests.Unit.Providers
{
    using System.Threading;
    using LAB.DataScanner.Components.Deployers;
    using LAB.DataScanner.Components.Providers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class DeploymentServiceProviderTests
    {
        [TestMethod]
        public void ShouldStartCorrectServiceWithCorrectToken()
        {
            // Assert
            string hostedServiceName = "TestHostedServiceName";
            CancellationToken stopToken = new CancellationToken();

            var deploymentServiceMock = Substitute.For<IDeploymentService>();

            deploymentServiceMock
                .CanExecute(Arg.Is<string>(hostedServiceName))
                .Returns(true);


            var strategies = new[] { deploymentServiceMock };

            var sut = new DeploymentServiceProvider(strategies);

            // Act
            sut.Start(hostedServiceName);

            // Arrange
            deploymentServiceMock
                .Received(1)
                .Start(stopToken);
        }

        [TestMethod]
        public void ShouldNotStartAnyServicesWithWrongName()
        {
            // Assert
            string hostedServiceName = "TestHostedServiceName";
            CancellationToken stopToken = new CancellationToken();

            var deploymentServiceMock = Substitute.For<IDeploymentService>();

            deploymentServiceMock
                .CanExecute(Arg.Is<string>(hostedServiceName))
                .Returns(false);


            var strategies = new[] { deploymentServiceMock };

            var sut = new DeploymentServiceProvider(strategies);

            // Act
            sut.Start("WrongServiceName");

            // Arrange
            deploymentServiceMock
                .DidNotReceive()
                .Start(stopToken);
        }

        [TestMethod]
        public void ShouldNotStopAnyServicesWithWrongName()
        {
            // Assert
            string hostedServiceName = "TestHostedServiceName";
            CancellationToken stopToken = new CancellationToken();

            var deploymentServiceMock = Substitute.For<IDeploymentService>();

            deploymentServiceMock
                .CanExecute(Arg.Is<string>(hostedServiceName))
                .Returns(false);


            var strategies = new[] { deploymentServiceMock };

            var sut = new DeploymentServiceProvider(strategies);

            // Act
            sut.Stop("WrongServiceName");

            // Arrange
            deploymentServiceMock
                .DidNotReceive()
                .Stop();
        }
    }
}