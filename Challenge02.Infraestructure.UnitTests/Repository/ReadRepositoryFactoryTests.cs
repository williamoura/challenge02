using FluentAssertions;
using NSubstitute;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using Challenge02.Infraestructure.Repository.Factories;

namespace Challenge02.Infraestructure.UnitTests.Repository
{
    public class ReadRepositoryFactoryTests
    {
        public ReadRepositoryFactoryTests()
        {
        }

        [Theory]
        [InlineData(RepositoryType.Mongo)]
        [InlineData(RepositoryType.Sql)]
        public void GetReadRepository_WithSupportedRepositoryType_ReturnsRepository(RepositoryType type)
        {
            var mockRepository = Substitute.For<IReadDevRepository>();
            mockRepository.CanExecute(Arg.Any<RepositoryType>()).Returns(true);
            var repositories = new List<IReadDevRepository> { mockRepository };
            var factory = new ReadRepositoryFactory(repositories);

            var result = factory.GetReadRepository(type);

            result.Should().BeEquivalentTo(mockRepository);
        }

        [Theory]
        [InlineData(RepositoryType.Mongo)]
        [InlineData(RepositoryType.Sql)]
        public void GetReadRepository_WithUnsupportedRepositoryType_ThrowsInvalidOperationException(RepositoryType type)
        {
            var repositories = new List<IReadDevRepository> { Substitute.For<IReadDevRepository>() };
            var factory = new ReadRepositoryFactory(repositories);

            var act = () => factory.GetReadRepository((RepositoryType)2);
            act.Should().Throw<InvalidOperationException>();
        }

    }
}