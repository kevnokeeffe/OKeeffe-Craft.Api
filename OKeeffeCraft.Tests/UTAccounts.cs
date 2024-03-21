using AutoFixture;
using Moq;
using OKeeffeCraft.Core.Interfaces;

namespace OKeeffeCraft.Tests
{
    public class UTAccounts
    {
        // private IAccountService _userService;
        /*private Mock<IUserRepository> _mockUserRepository;
        private Fixture _fixture;

        public UserServiceTests()
        {
            // fixture for creating test data
            _fixture = new Fixture();

            // mock user repo dependency
            _mockUserRepository = new Mock<IUserRepository>();

            // automapper dependency
            var mapper = new MapperConfiguration(x => x.AddProfile<AutoMapperProfile>()).CreateMapper();

            // service under test
            _userService = new UserService(_mockUserRepository.Object, mapper);
        }

        [Fact]
        public async Task GetAll_ReturnsAllUsers()
        {
            // Arrange
            var usersFixture = _fixture.CreateMany<User>(3);
            _mockUserRepository.Setup(x => x.GetAll()).ReturnsAsync(usersFixture);

            // Act
            var users = await _userService.GetAll();

            // Assert
            Assert.True(users.Count() == 3);
            Assert.Equal(usersFixture.First().FirstName, users.First().FirstName);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsUser()
        {
            // Arrange
            var userFixture = _fixture.Create<User>();
            var id = userFixture.Id;
            _mockUserRepository.Setup(x => x.GetById(id)).ReturnsAsync(userFixture);

            // Act
            var user = await _userService.GetById(id);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(userFixture.FirstName, user.FirstName);
        }

        [Fact]
        public async Task GetById_InvalidId_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GetById(100));
        }*/
    }
}