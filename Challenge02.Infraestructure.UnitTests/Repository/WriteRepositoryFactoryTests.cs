using FluentAssertions;
using NSubstitute;
using Challenge02.Domain.Interfaces;
using Challenge02.Domain.Models;
using Challenge02.Infraestructure.Repository.Factories;

namespace Challenge02.Infraestructure.UnitTests.Repository
{
    public class WriteRepositoryFactoryTests
    {
        public WriteRepositoryFactoryTests()
        {
        }

        [Theory]
        [InlineData(RepositoryType.Mongo)]
        [InlineData(RepositoryType.Sql)]
        public void GetWriteRepository_WithSupportedRepositoryType_ReturnsRepository(RepositoryType type)
        {
            var mockRepository = Substitute.For<IWriteDevRepository>();
            mockRepository.CanExecute(Arg.Any<RepositoryType>()).Returns(true);
            var repositories = new List<IWriteDevRepository> { mockRepository };
            var factory = new WriteRepositoryFactory(repositories);

            var result = factory.GetWriteRepository(type);

            result.Should().BeEquivalentTo(mockRepository);
        }

        [Theory]
        [InlineData(RepositoryType.Mongo)]
        [InlineData(RepositoryType.Sql)]
        public void GetWriteRepository_WithUnsupportedRepositoryType_ThrowsInvalidOperationException(RepositoryType type)
        {
            var repositories = new List<IWriteDevRepository> { Substitute.For<IWriteDevRepository>() };
            var factory = new WriteRepositoryFactory(repositories);

            var act = () => factory.GetWriteRepository((RepositoryType)2);
            act.Should().Throw<InvalidOperationException>();
        }

    }
}