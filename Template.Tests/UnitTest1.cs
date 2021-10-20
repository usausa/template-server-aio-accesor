namespace Template
{
    using Template.Models.Entity;

    using Xunit;

    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // TODO
            var entity = new AccountEntity { Id = "00000000", Name = "管理者", IsAdmin = true, PasswordHash = string.Empty };
            Assert.Equal("00000000", entity.Id);
        }
    }
}
