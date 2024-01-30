using FluentAssertions;
using Graduation.Notification.Domain.Exceptions;

namespace Graduation.Notification.UnitTests.Domain.Exceptions
{
    public class BusinessExceptionTests
    {
        [Fact]
        public void ThrowIfNull_WithNonNullObject_DoesNotThrow()
        {
            // Arrange
            var obj = new object();

            // Act & Assert
            obj.Invoking(o => BusinessException.ThrowIfNull(o)).Should().NotThrow();
        }

        [Fact]
        public void ThrowIfNull_WithNullObject_ThrowsBusinessException()
        {
            // Arrange
            object obj = null;

            // Act & Assert
            Action act = () => BusinessException.ThrowIfNull(obj);
            act.Should().Throw<BusinessException>();
        }
    }
}
